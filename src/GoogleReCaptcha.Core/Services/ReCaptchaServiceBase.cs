namespace GoogleReCaptcha.Core.Services
{
    using GoogleReCaptcha.Core.Services.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.Extensions.Logging;
    using System.Text.Json;

    public abstract class ReCaptchaServiceBase
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
        protected virtual ILogger Logger { get; }

        /// <summary>
        /// Get current action context
        /// </summary>
        protected virtual ActionContext ActionContext { get; set; }

        /// <summary>
        /// Get http client factory to spawn http clients
        /// </summary>
        protected virtual IHttpClientFactory HttpClientFactory { get; set; }

        #endregion

        #region Constructor

        public ReCaptchaServiceBase(ILogger<ReCaptchaServiceBase> logger, IActionContextAccessor actionContextAccessor, IHttpClientFactory httpClientFactory)
        {
            if (actionContextAccessor == null)
            {
                throw new ArgumentNullException(nameof(actionContextAccessor));
            }

            if (httpClientFactory == null)
            {
                throw new ArgumentNullException(nameof(httpClientFactory));
            }

            if (logger == null)
            {
                logger = new Microsoft.Extensions.Logging.Abstractions.NullLogger<ReCaptchaServiceBase>();
            }
            Logger = logger;

            ActionContext = actionContextAccessor?.ActionContext;
            HttpClientFactory = httpClientFactory;
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
        /// Get Google ReCaptcha V2 verify token
        /// </summary>
        /// <returns>The verify token or an empty string if not found</returns>
        public string GetToken()
        {
            if (ActionContext != null)
            {
                // Get token from request in form as Google states it should be if script is on site
                var token = ActionContext?.HttpContext?.Request?.Form[KEY_GOOGLE_RECAPTCHA_POST];
                return token?.ToString() ?? "";
            }
            return "";
        }

        /// <summary>
        /// Buid request object with current settings and context
        /// </summary>
        /// <param name="token">User's verify token for request</param>
        /// <returns>New request object with data from current settings and context</returns>
        protected virtual VerifyRequest BuildRequestData(string secretKey, string token)
        {
            return new VerifyRequest
            {
                Secret = secretKey,
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
