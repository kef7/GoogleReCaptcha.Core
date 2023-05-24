namespace GoogleReCaptcha.Core
{
    using Microsoft.AspNetCore.Builder;

    /// <summary>
    /// <see cref="IApplicationBuilder"/> extensions
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Static flag to indicate if we called base middleware use method
        /// </summary>
        private static bool s_basePrevCalled = false;

        /// <summary>
        /// Base middleware to use for most 
        /// </summary>
        /// <param name="applicationBuilder"></param>
        private static void BaseMiddleware(this IApplicationBuilder applicationBuilder)
        {
            if (!s_basePrevCalled)
            {
                // Add middleware needed for helper extensions to work
                applicationBuilder.UseMiddleware<ReCaptchaSettingsHttpContextItemsInjectionMiddleware>();

                s_basePrevCalled = true;
            }
        }

        /// <summary>
        /// Use <seealso cref="GoogleReCaptcha"/>  settings HTTP context injection middleware
        /// </summary>
        /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/> reference</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="applicationBuilder"/> is null</exception>
        public static void UseGoogleReCaptchaSettingsHttpContextInjection(this IApplicationBuilder applicationBuilder)
        {
            if (applicationBuilder is null)
            {
                throw new ArgumentNullException(nameof(applicationBuilder));
            }

            BaseMiddleware(applicationBuilder);
        }


        /// <summary>
        /// Use <seealso cref="GoogleReCaptcha"/> HTML helper support middleware
        /// </summary>
        /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/> reference</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="applicationBuilder"/> is null</exception>
        public static void UseGoogleReCaptchaHtmlHelperSupport(this IApplicationBuilder applicationBuilder)
        {
            if (applicationBuilder is null)
            {
                throw new ArgumentNullException(nameof(applicationBuilder));
            }

            BaseMiddleware(applicationBuilder);
        }
    }
}
