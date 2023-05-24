namespace GoogleReCaptcha.Core.Mvc.TagHelpers
{
    using Microsoft.AspNetCore.Razor.TagHelpers;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Tag helper base
    /// </summary>
    public abstract class TagHelperBase : TagHelper
    {
        #region Properties

        /// <summary>
        /// Gets the ILogger
        /// </summary>
        protected virtual ILogger Logger { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Tag helper base
        /// </summary>
        /// <param name="logger">Logger</param>
        public TagHelperBase(ILogger logger)
        {
            Logger = logger ?? new Microsoft.Extensions.Logging.Abstractions.NullLogger<TagHelperBase>(); ;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get and merge CSS class attributes present with default class attributes required for Google ReCaptcha
        /// </summary>
        /// <param name="context">Tag helper context of the tag being processed</param>
        /// <param name="defaultClassAttrs">Default set of CSS classes separated by spaces</param>
        /// <returns>String of all classes from current tag context and default</returns>
        protected string GetMergedClassAttributes(TagHelperContext context, string defaultClassAttrs)
        {
            Logger.LogTrace("Merging default and current context class attributes.");

            // Define classes
            var classes = defaultClassAttrs ?? "";

            // Get classes from current tag context
            var classTagHelperAttr = context.AllAttributes["class"];
            if (classTagHelperAttr != null)
            {
                // Concat classes
                var tmp = classTagHelperAttr.Value?.ToString();
                if (!string.IsNullOrWhiteSpace(tmp))
                {
                    classes = classes + " " + tmp;
                }
            }

            // Return classes
            return classes;
        }

        #endregion
    }
}
