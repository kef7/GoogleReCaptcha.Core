namespace GoogleReCaptcha.Core.Services
{
    using GoogleReCaptcha.Core.Settings;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Service for Google ReCaptcha V3; can verify user token with Google if configured correctly
    /// </summary>
    public class ReCaptchaV3Service : ReCaptchaServiceBase, IReCaptchaV3Service
    {
        #region Properties

        /// <summary>
        /// Get ReCaptcha V3 settings
        /// </summary>
        protected virtual IReCaptchaV3Settings Settings { get; }

        #endregion

        #region Constructor 

        /// <summary>
        /// ReCaptcha V3 service requires V3 settings and to be tied into the current action context to get the 
        /// recaptcha token Google puts in the request
        /// </summary>
        /// <param name="settings">ReCaptcah V3 settings</param>
        /// <param name="actionContextAccessor">Current action context accessor</param>
        /// <param name="httpClientFactory">HTTP Client factory to send assist in sending verify request to Google's ReCaptcha API</param>
        /// <param name="settings">V3 settings</param>
        public ReCaptchaV3Service(ILogger<ReCaptchaV3Service> logger, IActionContextAccessor actionContextAccessor, IHttpClientFactory httpClientFactory, IReCaptchaV3Settings settings)
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
                }
            }

            Logger.LogWarning("Attempt to verify reCAPTCHA failed");
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

        #endregion
    }
}
