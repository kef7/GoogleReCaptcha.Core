using GoogleReCaptcha3.Core.Services.Models;
using GoogleReCaptcha3.Core.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GoogleReCaptcha3.Core.Services
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

		/// <summary>
		/// Get ReCaptcha V3 settings
		/// </summary>
		public IReCaptchaV3Settings Settings { get; }

		/// <summary>
		/// Get current action context
		/// </summary>
		public ActionContext ActionContext { get; }

		/// <summary>
		/// Get http client factory to spawn http clients
		/// </summary>
		protected IHttpClientFactory HttpClientFactory { get; }

		#endregion

		#region Constructor 

		/// <summary>
		/// ReCaptcha V3 service requires V3 settings and to be tied into the current action context to get the 
		/// recaptcha token Google puts in the request
		/// </summary>
		/// <param name="settings">ReCaptcah V3 settings</param>
		/// <param name="actionContextAccessor">Current action context accessor</param>
		/// <param name="httpClientFactory">HTTP Client factory to send assist in sending verify request to Google's ReCaptcha API</param>
		public ReCaptchaV3Service(IReCaptchaV3Settings settings, IActionContextAccessor actionContextAccessor, IHttpClientFactory httpClientFactory)
		{
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
			// TODO: How to handle Verify() call if Settings, ActionContext, or HttpClientFactory are null? Always return true?

			var token = GetToken();
			if (token != "")
			{
				var req = BuildRequestData(token);
				var awaiter = GetVerifyResponseAsync(req).GetAwaiter();
				var res = awaiter.GetResult();
				if (res != null)
				{
					if (res.Success == true)
					{
						return true;
					}
					if (res.ErrorCodes.Length > 0)
					{
						throw new ReCaptchaVerifyException(res);
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Verify the token recieved
		/// </summary>
		/// <returns>True if token was verified; false otherwise</returns>
		public async Task<bool> VerifyAsync()
		{
			// TODO: How to handle Verify() call if Settings, ActionContext, or HttpClientFactory are null? Always return true?

			var token = GetToken();
			if (token != "")
			{
				var req = BuildRequestData(token);
				var res = await GetVerifyResponseAsync(req);
				if (res != null)
				{
					if (res.Success == true)
					{
						return true;
					}
					if (res.ErrorCodes.Length > 0)
					{
						throw new ReCaptchaVerifyException(res);
					}
				}
			}
			return false;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Get user IP or remote IP address from current action context
		/// </summary>
		/// <returns>User's IP or empty string if not found</returns>
		private string GetUserIp()
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
		private VerifyRequest BuildRequestData(string token)
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
		private async Task<VerifyResponse> GetVerifyResponseAsync(VerifyRequest verifyRequestData)
		{
			if (verifyRequestData == null)
			{
				throw new ArgumentNullException(nameof(verifyRequestData));
			}

			// Build http reqeust message
			var apiUrl = Settings.ApiUrl + "/siteverify";

			// Build post content
			var formContent = new FormUrlEncodedContent(new[]
			{
				new KeyValuePair<string, string>("secret", verifyRequestData.Secret),
				new KeyValuePair<string, string>("response", verifyRequestData.Response),
				new KeyValuePair<string, string>("remoteip", verifyRequestData.RemoteIp)
			});

			// Create new http client
			var httpClient = HttpClientFactory.CreateClient(); // TODO: Should we do a named http client here and in ServiceCollectionExtensions.cs that have all the basic of endpoint URI, and defautl headers etc..?
			httpClient.DefaultRequestHeaders.Add("Accepts", "application/json");

			// Get response
			using var response = await httpClient.PostAsync(apiUrl, formContent);

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
					throw new ReCaptchaServiceException("Could not parse verify response object. See inner exception for details.", ex);
				}
			}

			return null;
		}

		#endregion
	}
}
