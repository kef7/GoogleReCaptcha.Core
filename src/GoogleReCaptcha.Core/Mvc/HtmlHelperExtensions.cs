namespace GoogleReCaptcha.Core.Mvc
{
    using Microsoft.AspNetCore.Html;
    using Microsoft.AspNetCore.Mvc.Rendering;

    /// <summary>
    /// HtmlHelper extensions for ReCaptcha settings
    /// </summary>
    /// <remarks>
    /// Requires use of middleware <see cref="ReCaptchaSettingsHttpContextItemsInjectionMiddleware"/>
    /// </remarks>
    public static class HtmlHelperExtensions
    {
        /// <summary>
        /// Get reCAPTCHA v2 site key
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="htmlHelper">Reference to <paramref name="htmlHelper"/></param>
        /// <returns>HTML content</returns>
        public static IHtmlContent ReCaptchaV2SiteKey<TModel>(this IHtmlHelper<TModel> htmlHelper)
        {
            var siteKey = htmlHelper.ViewContext.HttpContext.Items[Constants.HTTPCTX_KEY_V2_SITEKEY]?.ToString() ?? "";
            return new HtmlString(siteKey);
        }

        /// <summary>
        /// Get reCAPTCHA v2 theme
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="htmlHelper">Reference to <paramref name="htmlHelper"/></param>
        /// <returns>HTML content</returns>
        public static IHtmlContent ReCaptchaV2Theme<TModel>(this IHtmlHelper<TModel> htmlHelper)
        {
            var siteKey = htmlHelper.ViewContext.HttpContext.Items[Constants.HTTPCTX_KEY_V2_THEME]?.ToString().ToLower() ?? "";
            return new HtmlString(siteKey);
        }

        /// <summary>
        /// Get reCAPTCHA v2 size
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="htmlHelper">Reference to <paramref name="htmlHelper"/></param>
        /// <returns>HTML content</returns>
        public static IHtmlContent ReCaptchaV2Size<TModel>(this IHtmlHelper<TModel> htmlHelper)
        {
            var siteKey = htmlHelper.ViewContext.HttpContext.Items[Constants.HTTPCTX_KEY_V2_SIZE]?.ToString().ToLower() ?? "";
            return new HtmlString(siteKey);
        }

        /// <summary>
        /// Get reCAPTCHA v3 site key
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="htmlHelper">Reference to <paramref name="htmlHelper"/></param>
        /// <returns>HTML content</returns>
        public static IHtmlContent ReCaptchaV3SiteKey<TModel>(this IHtmlHelper<TModel> @this)
        {
            var siteKey = @this.ViewContext.HttpContext.Items[Constants.HTTPCTX_KEY_V3_SITEKEY]?.ToString() ?? "";
            return new HtmlString(siteKey);
        }
    }
}
