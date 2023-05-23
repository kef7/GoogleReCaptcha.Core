namespace GoogleReCaptcha.Core.Mvc
{
    using Microsoft.AspNetCore.Html;
    using Microsoft.AspNetCore.Mvc.Rendering;

    /// <summary>
    /// HtmlHelper extensions for ReCaptcha settings
    /// </summary>
    /// <remarks>
    /// Requires use of middlware <see cref="ReCaptchaSettingsHttpContextItemsInjectionMiddleware"/>
    /// </remarks>
    public static class HtmlHelperExtensions
    {
        public static IHtmlContent ReCaptchaV2SiteKey<TModel>(this IHtmlHelper<TModel> @this)
        {
            var siteKey = @this.ViewContext.HttpContext.Items[Constants.HTTPCTX_KEY_V2_SITEKEY]?.ToString() ?? "";
            return new HtmlString(siteKey);
        }
        public static IHtmlContent ReCaptchaV2Theme<TModel>(this IHtmlHelper<TModel> @this)
        {
            var siteKey = @this.ViewContext.HttpContext.Items[Constants.HTTPCTX_KEY_V2_THEME]?.ToString().ToLower() ?? "";
            return new HtmlString(siteKey);
        }
        public static IHtmlContent ReCaptchaV2Size<TModel>(this IHtmlHelper<TModel> @this)
        {
            var siteKey = @this.ViewContext.HttpContext.Items[Constants.HTTPCTX_KEY_V2_SIZE]?.ToString().ToLower() ?? "";
            return new HtmlString(siteKey);
        }

        public static IHtmlContent ReCaptchaV3SiteKey<TModel>(this IHtmlHelper<TModel> @this)
        {
            var siteKey = @this.ViewContext.HttpContext.Items[Constants.HTTPCTX_KEY_V3_SITEKEY]?.ToString() ?? "";
            return new HtmlString(siteKey);
        }
    }
}
