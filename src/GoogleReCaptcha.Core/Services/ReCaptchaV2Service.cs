namespace GoogleReCaptcha.Core.Services
{
    using GoogleReCaptcha.Core.Settings;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Service for Google ReCaptcha V2; can verify user token with Google if configured correctly
    /// </summary>
    public class ReCaptchaV2Service : ReCaptchaServiceBase, IReCaptchaV2Service
    {
        #region Properties

        /// <summary>
        /// Get ReCaptcha V2 settings
        /// </summary>
        protected virtual IReCaptchaV2Settings Settings { get; }

        #endregion

        #region Constructor 

        /// <summary>
        /// ReCaptcha V2 service requires V2 settings and to be tied into the current action context to get the 
        /// recaptcha token Google puts in the request
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="actionContextAccessor">Current action context accessor</param>
        /// <param name="httpClientFactory">HTTP Client factory to send assist in sending verify request to Google's ReCaptcha API</param>
        /// <param name="settings">V2 settings</param>
        public ReCaptchaV2Service(ILogger<ReCaptchaV2Service> logger, IActionContextAccessor actionContextAccessor, IHttpClientFactory httpClientFactory, IReCaptchaV2Settings settings)
            : base(logger, actionContextAccessor, httpClientFactory)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            Settings = settings;
        }

        #endregion

        #region IReCaptchaService Methods

        /// <summary>
        /// Verify reCAPTCHA against Google
        /// </summary>
        /// <returns>True if passing; false otherwise</returns>
        public bool Verify()
        {
            // Just call async version and get awaiter
            var awaiter = VerifyAsync().GetAwaiter();

            // Get result of awaiter and return it
            var verify = awaiter.GetResult();
            return verify;
        }

        /// <summary>
        /// Verify reCAPTCHA against Google
        /// </summary>
        /// <returns>True if passing; false otherwise</returns>
        public async Task<bool> VerifyAsync()
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
                var req = BuildRequestData(Settings.SecretKey, token);
                Logger.LogDebug("Verify reCAPTCHA request data: {req}", req);

                // Get verify response
                var res = await GetVerifyResponseAsync(req);
                Logger.LogDebug("Verify reCAPTCHA response data: {res}", res);

                // Process response
                if (res != null)
                {
                    if (res.ErrorCodes?.Length > 0)
                    {
                        Logger.LogDebug("Verify reCAPTCHA contains response errors");
                        throw new ReCaptchaVerifyException(res);
                    }

                    if (res.Success == true)
                    {
                        Logger.LogDebug("Verify reCAPTCHA successful");
                        return true;
                    }
                    else
                    {
                        Logger.LogDebug("Verify reCAPTCHA unsuccessful");
                        return false;
                    }
                }
            }

            Logger.LogWarning("Attempt to verify reCAPTCHA failed");
            return false;
        }

        #endregion
    }
}
