namespace GoogleReCaptcha.Core
{
    using GoogleReCaptcha.Core.Settings;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Inject limited reCAPTCHA settings data into <see cref="HttpContext.Items"/>
    /// </summary>
    public class ReCaptchaSettingsHttpContextItemsInjectionMiddleware
    {
        /// <summary>
        /// Request delegate to call next
        /// </summary>
        private readonly RequestDelegate _next;

        /// <summary>
        /// Logger factory
        /// </summary>
        private readonly ILoggerFactory _loggerFactory;

        /// <summary>
        /// Inject limited reCAPTCHA settings data into <see cref="HttpContext.Items"/>
        /// </summary>
        /// <param name="next">Next request delegate to call</param>
        /// <param name="loggerFactory">Logger factory</param>
        public ReCaptchaSettingsHttpContextItemsInjectionMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _loggerFactory = loggerFactory;
        }

        /// <summary>
        /// Invoke middleware
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <param name="reCaptchaV2Settings">reCAPTCHA v2 settings object</param>
        /// <param name="reCaptchaV3Settings">reCAPTCHA v3 settings object</param>
        /// <returns></returns>
        public async Task InvokeAsync(
            HttpContext context,
            IReCaptchaV2Settings reCaptchaV2Settings,
            IReCaptchaV3Settings reCaptchaV3Settings
        )
        {
            var logger = _loggerFactory.CreateLogger<ReCaptchaSettingsHttpContextItemsInjectionMiddleware>();

            InjectV2(logger, context, reCaptchaV2Settings);

            InjectV3(logger, context, reCaptchaV3Settings);

            await _next(context);
        }

        /// <summary>
        /// Inject reCAPTCHA v2 settings into context
        /// </summary>
        /// <param name="logger">ILogger to log progress/issues</param>
        /// <param name="context">HttpContext to inject settings into</param>
        /// <param name="settings">Settings object to get settings from</param>
        private void InjectV2(ILogger logger, HttpContext context, IReCaptchaV2Settings settings)
        {
            try
            {
                if (settings != null)
                {
                    logger.LogDebug("Attempting to inject limited reCAPTCHA v2 settings into HttpContext Items collection.");
                    context.Items[Constants.HTTPCTX_KEY_V2_SITEKEY] = settings.SiteKey;
                    context.Items[Constants.HTTPCTX_KEY_V2_THEME] = settings.Theme;
                    context.Items[Constants.HTTPCTX_KEY_V2_SIZE] = settings.Size;
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error in attempt to inject limited reCAPTCHA v2 settings to HttpContext Items collection.");
            }
        }

        /// <summary>
        /// Inject reCAPTCHA v3 settings into context
        /// </summary>
        /// <param name="logger">ILogger to log progress/issues</param>
        /// <param name="context">HttpContext to inject settings into</param>
        /// <param name="settings">Settings object to get settings from</param>
        private void InjectV3(ILogger logger, HttpContext context, IReCaptchaV3Settings settings)
        {
            try
            {
                if (settings != null)
                {
                    logger.LogDebug("Attempting to inject limited reCAPTCHA v3 settings into HttpContext Items collection.");
                    context.Items[Constants.HTTPCTX_KEY_V3_SITEKEY] = settings.SiteKey;
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error in attempt to inject limited reCAPTCHA v3 settings to HttpContext Items collection.");
            }
        }
    }
}
