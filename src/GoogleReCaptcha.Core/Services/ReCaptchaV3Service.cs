using GoogleReCaptcha.Core.Services.Models;
using GoogleReCaptcha.Core.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace GoogleReCaptcha.Core.Services
{
	/// <summary>
	/// Service for Google ReCaptcha V3; can verify user token with Google if configured correctly
	/// </summary>
	public class ReCaptchaV3Service : IReCaptchaService
	{
		#region Fields

		/// <summary>
		/// Google ReCaptcha post key; this is where google places the verify token
		/// </summary>
		public const string KEY_GOOGLE_RECAPTCHA_POST = "g-recaptcha-response";

		#endregion

		#region Properties

		protected virtual ILogger<ReCaptchaV3Service> Logger { get; }

		/// <summary>
		/// Get ReCaptcha V3 settings
		/// </summary>
		protected virtual IReCaptchaV3Settings Settings { get; }

		/// <summary>
		/// Get current action context
		/// </summary>
		protected virtual ActionContext ActionContext { get; }

		/// <summary>
		/// Get http client factory to spawn http clients
		/// </summary>
		protected virtual IHttpClientFactory HttpClientFactory { get; }

		#endregion

		#region Constructor 

		/// <summary>
		/// ReCaptcha V3 service requires V3 settings and to be tied into the current action context to get the 
		/// recaptcha token Google puts in the request
		/// </summary>
		/// <param name="settings">ReCaptcah V3 settings</param>
		/// <param name="actionContextAccessor">Current action context accessor</param>
		/// <param name="httpClientFactory">HTTP Client factory to send assist in sending verify request to Google's ReCaptcha API</param>
		public ReCaptchaV3Service(ILogger<ReCaptchaV3Service> logger, IReCaptchaV3Settings settings, IActionContextAccessor actionContextAccessor, IHttpClientFactory httpClientFactory)
		{
			if (logger == null)
			{
				logger = new Microsoft.Extensions.Logging.Abstractions.NullLogger<ReCaptchaV3Service>();
			}
			else
			{
				Logger = logger;
			}

			if (settings == null)
			{
				throw new ArgumentNullException(nameof(settings));
			}

			if (actionContextAccessor == null)
			{
				throw new ArgumentNullException(nameof(actionContextAccessor));
			}

			if (httpClientFactory == null)
			{
				throw new ArgumentNullException(nameof(httpClientFactory));
			}

			Settings = settings;
			ActionContext = actionContextAccessor?.ActionContext;
			HttpClientFactory = httpClientFactory;
		}

		#endregion

		#region IReCaptchaService Methods

		/// <summary>
		/// Get Google ReCaptcha V3 verify token
		/// </summary>
		/// <returns>The verify token or an empty string if not found</returns>
		public string GetToken()
		{
			if (ActionContext != null)
			{
				// Get token from request in form as Google states it should be if script is on site
				var token = ActionContext?.HttpContext?.Request?.Form[KEY_GOOGLE_RECAPTCHA_POST].ToString();
				return token ?? "";
			}
			return "";
		}

		/// <summary>
		/// Verify the token recieved
		/// </summary>
		/// <returns>True if token was verified; false otherwise</returns>
		public bool Verify()
		{
			// Enabled?
			if (!Settings.Enabled)
			{
				Logger.LogInformation("Skip attempt to verify reCAPTCHA because it is disabled via settings");
				return true;
			}

			Logger.LogInformation("Attempt to verify reCAPTCHA");

			var token = GetToken();
			if (token != "")
			{
				// Get reqeust data
				var req = BuildRequestData(token);
				Logger.LogDebug("Verify reCAPTCHA request data: {req}", req);

				// Get verify response
				var awaiter = GetVerifyResponseAsync(req).GetAwaiter();
				var res = awaiter.GetResult();
				Logger.LogDebug("Verify reCAPTCHA response data: {res}", res);

				// Process response
				if (res != null)
				{
					if (res.Success == true)
					{
						Logger.LogInformation("Verify reCAPTCHA resolved to true");
						return true;
					}
					if (res.ErrorCodes.Length > 0)
					{
						Logger.LogInformation("Verify reCAPTCHA contains response errors");
						throw new ReCaptchaVerifyException(res);
					}
				}
			}

			Logger.LogInformation("Verify reCAPTCHA resolved to false");
			return false;
		}

		/// <summary>
		/// Verify the token recieved
		/// </summary>
		/// <returns>True if token was verified; false otherwise</returns>
		public async Task<bool> VerifyAsync()
		{
			// Enabled?
			if (!Settings.Enabled)
			{
				Logger.LogInformation("Skip attempt to verify reCAPTCHA (async) because it is disabled via settings");
				return true;
			}

			Logger.LogInformation("Attempt to verify reCAPTCHA (async)");

			var token = GetToken();
			if (token != "")
			{
				// Get reqeust data
				var req = BuildRequestData(token);
				Logger.LogDebug("Verify reCAPTCHA request data: {req}", req);

				// Get verify response
				var res = await GetVerifyResponseAsync(req);
				Logger.LogDebug("Verify reCAPTCHA (async) response data: {res}", res);

				// Process response
				if (res != null)
				{
					if (res.Success == true)
					{
						Logger.LogInformation("Verify reCAPTCHA resolved to true");
						return true;
					}
					if (res.ErrorCodes.Length > 0)
					{
						Logger.LogInformation("Verify reCAPTCHA contains response errors");
						throw new ReCaptchaVerifyException(res);
					}
				}
			}

			Logger.LogInformation("Verify reCAPTCHA resolved to false");
			return false;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Get user IP or remote IP address from current action context
		/// </summary>
		/// <returns>User's IP or empty string if not found</returns>
		protected virtual string GetUserIp()
		{
			if (ActionContext != null)
			{
				var userIp = ActionContext?.HttpContext?.Connection?.RemoteIpAddress?.ToString();
				return userIp ?? "";
			}
			return "";
		}

		/// <summary>
		/// Buid request object with current settings and context
		/// </summary>
		/// <param name="token">User's verify token for request</param>
		/// <returns>New request object with data from current settings and context</returns>
		protected virtual VerifyRequest BuildRequestData(string token)
		{
			return new VerifyRequest
			{
				Secret = Settings.SecretKey,
				Response = token,
				RemoteIp = GetUserIp()
			};
		}

		/// <summary>
		/// Get verify response from API endpoint
		/// </summary>
		/// <param name="verifyRequestData">The verify response data to send to API endpoint</param>
		/// <returns>Verify response object or null if nothing returned</returns>
		protected virtual async Task<VerifyResponse> GetVerifyResponseAsync(VerifyRequest verifyRequestData)
		{
			if (verifyRequestData == null)
			{
				throw new ArgumentNullException(nameof(verifyRequestData));
			}

			// Build post content
			var formContent = new FormUrlEncodedContent(new[]
			{
				new KeyValuePair<string, string>("secret", verifyRequestData.Secret),
				new KeyValuePair<string, string>("response", verifyRequestData.Response),
				new KeyValuePair<string, string>("remoteip", verifyRequestData.RemoteIp)
			});

			// Create new http client
			var httpClient = HttpClientFactory.CreateClient(Constants.DEFAULT_HTTP_CLIENT_NAME);

			// Get response
			using var response = await httpClient.PostAsync("siteverify", formContent);

			// Parse response for data
			if (response.IsSuccessStatusCode)
			{
				try
				{
					var jsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
					{
						PropertyNameCaseInsensitive = false,
						ReadCommentHandling = JsonCommentHandling.Skip,
						WriteIndented = false
					};
					using var responseStream = await response.Content.ReadAsStreamAsync();
					var verifyResponseData = await JsonSerializer.DeserializeAsync<VerifyResponse>(responseStream, jsonSerializerOptions);
					return verifyResponseData;
				}
				catch (Exception ex)
				{
					Logger.LogError(ex, "Error in attempt to parse verify response from reCAPTCHA");
					throw new ReCaptchaServiceException("Could not parse verify response object. See inner exception for details.", ex);
				}
			}

			return null;
		}

		#endregion
	}
}
