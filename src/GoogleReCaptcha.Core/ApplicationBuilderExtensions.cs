namespace GoogleReCaptcha.Core
{
    using Microsoft.AspNetCore.Builder;

    public static class ApplicationBuilderExtensions
    {
        private static bool s_basePrevCalled = false;

        /// <summary>
        /// Base middleware to use for most ReCaptcah middleware
        /// </summary>
        /// <param name="this"></param>
        private static void BaseMiddlware(this IApplicationBuilder @this)
        {
            if (!s_basePrevCalled)
            {
                // Add middleware needed for helper extensions to work
                @this.UseMiddleware<ReCaptchaSettingsHttpContextItemsInjectionMiddleware>();

                s_basePrevCalled = true;
            }
        }

        public static void UseGoogleReCaptchaSettingsHttpContextInjection(this IApplicationBuilder @this)
        {
            if (@this is null)
            {
                throw new ArgumentNullException(nameof(@this));
            }

            BaseMiddlware(@this);
        }

        public static void UseGoogleReCaptchaHtmlHelperSupport(this IApplicationBuilder @this)
        {
            if (@this is null)
            {
                throw new ArgumentNullException(nameof(@this));
            }

            BaseMiddlware(@this);
        }
    }
}
