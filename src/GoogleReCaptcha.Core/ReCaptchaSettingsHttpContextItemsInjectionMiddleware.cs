namespace GoogleReCaptcha.Core
{
    using GoogleReCaptcha.Core.Settings;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Inject limited ReCaptcha settings data into <see cref="HttpContext.Items"/>
    /// </summary>
    public class ReCaptchaSettingsHttpContextItemsInjectionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILoggerFactory _loggerFactory;

        public ReCaptchaSettingsHttpContextItemsInjectionMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _loggerFactory = loggerFactory;
        }

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
        /// Inject V2 settings into conetxt
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
                    logger.LogDebug("Attempting to inject limited ReCaptcha V2 settings into HttpContext Items collection.");
                    context.Items[Constants.HTTPCTX_KEY_V2_SITEKEY] = settings.SiteKey;
                    context.Items[Constants.HTTPCTX_KEY_V2_THEME] = settings.Theme;
                    context.Items[Constants.HTTPCTX_KEY_V2_SIZE] = settings.Size;
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error in attempt to inject limited ReCaptcha V2 settings to HttpContext Items collection.");
            }
        }

        /// <summary>
        /// Inject V3 settings into conetxt
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
                    logger.LogDebug("Attempting to inject limited ReCaptcha V3 settings into HttpContext Items collection.");
                    context.Items[Constants.HTTPCTX_KEY_V3_SITEKEY] = settings.SiteKey;
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error in attempt to inject limited ReCaptcah V3 settings to HttpContext Items collection.");
            }
        }
    }
}
