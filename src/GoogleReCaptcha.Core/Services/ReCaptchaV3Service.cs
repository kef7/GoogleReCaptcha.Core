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
	public class ReCaptchaV3Service : IReCaptchaV3Service
	{
		#region Fields

		/// <summary>
		/// Google ReCaptcha post key; this is where google places the verify token
		/// </summary>
		public const string KEY_GOOGLE_RECAPTCHA_POST = "g-recaptcha-response";

		#endregion

		#region Properties

		/// <summary>
		/// Gets the ILogger
		/// </summary>
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
		/// Verify reCAPTCHA with the default passing score from settings that is equal to or greater then the response score value
		/// </summary>
		/// <returns>True if passing value is equal to or greater than the response score</returns>
		public bool Verify()
		{
			// Verify with passing score from settings
			var passingScore = Settings.DefaultPassingScore;
			return Verify(passingScore);
		}

		/// <summary>
		/// Verify reCAPTCHA with the default passing score from settings that is equal to or greater then the response score value
		/// </summary>
		/// <returns>True if passing value is equal to or greater than the response score</returns>
		public async Task<bool> VerifyAsync()
		{
			// Verify with passing score from settings
			var passingScore = Settings.DefaultPassingScore;
			return await VerifyAsync(passingScore);
		}

		/// <summary>
		/// Verify reCAPTCHA with a passing score that is equal to or greater then the response score value
		/// </summary>
		/// <param name="passing">Passing value in which the response score should be equal to or greater than</param>
		/// <returns>True if passing value is equal to or greater than the response score</returns>
		public bool Verify(float passing)
		{
			// Just call async version and get awaiter
			var awaiter = VerifyAsync(passing).GetAwaiter();

			// Get result of awaiter and return it
			var verify = awaiter.GetResult();
			return verify;
		}

		/// <summary>
		/// Verify reCAPTCHA with a passing score that is equal to or greater then the response score value
		/// </summary>
		/// <param name="passing">Passing value in which the response score should be equal to or greater than</param>
		/// <returns>True if passing value is equal to or greater than the response score</returns>
		public async Task<bool> VerifyAsync(float passing)
		{
			// Enabled?
			if (!Settings.Enabled)
			{
				Logger.LogInformation("Skip attempt to verify reCAPTCHA because it is disabled via settings");
				return true;
			}

			Logger.LogInformation("Attempt to verify reCAPTCHA");
			passing = GetProperPassingScore(passing);

			var token = GetToken();
			if (token != "")
			{
				// Get reqeust data
				var req = BuildRequestData(token);
				Logger.LogDebug("Verify reCAPTCHA request data: {req}", req);

				// Get verify response
				var res = await GetVerifyResponseAsync(req);
				Logger.LogDebug("Verify reCAPTCHA response data: {res}", res);

				// Process response
				if (res != null)
				{
					if (res.Success == true)
					{
						if (res.Score >= passing)
						{
							Logger.LogDebug("Verify reCAPTCHA successful, and passed with {Score} vs {PassingScore}", res.Score, passing);
							return true;
						}
						else
						{
							Logger.LogDebug("Verify reCAPTCHA successful, but did NOT pass with {Score} vs {PassingScore}", res.Score, passing);
							return false;
						}
					}
					else
					{
						Logger.LogDebug("Verify reCAPTCHA unsuccessful");
						return false;
					}
					if (res.ErrorCodes.Length > 0)
					{
						Logger.LogDebug("Verify reCAPTCHA contains response errors");
						throw new ReCaptchaVerifyException(res);
					}
				}
			}

			Logger.LogWarning("Verify reCAPTCHA attempted failed");
			return false;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Get passing score or default set by settings or library if beyond lower bounds of passing score
		/// </summary>
		/// <param name="passingScore">Current passing score value</param>
		/// <returns>Current passing score value or default set by settings or library if beyond lower bounds of passing score</returns>
		protected virtual float GetProperPassingScore(float passingScore)
		{
			if (passingScore < 0.0f)
			{
				passingScore = Settings.DefaultPassingScore;
				if (passingScore < 0.0f)
				{
					passingScore = Constants.DEFAULT_V3_PASSING_SCORE;
				}
			}
			return passingScore;
		}

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
