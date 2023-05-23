using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Logging;

namespace GoogleReCaptcha.Core.Mvc.TagHelpers
{
    public abstract class TagHelperBase : TagHelper
	{
		#region Properties

		/// <summary>
		/// Gets the ILogger
		/// </summary>
		protected virtual ILogger Logger { get; }

		#endregion

		#region Constructor

		public TagHelperBase(ILogger logger)
		{
			if (logger == null)
			{
				logger = new Microsoft.Extensions.Logging.Abstractions.NullLogger<TagHelperBase>();
			}
			Logger = logger;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Get and merge class attributes presnt with default class attributes required for Google ReCaptcha
		/// </summary>
		/// <param name="context">Tag helper context of the tag being processed</param>
		/// <returns>String of all classes from current tag context and default</returns>
		protected string GetMergedClassAttributes(TagHelperContext context, string defaultClassAttrs)
		{
			Logger.LogTrace("Merging default and current context class attributes.");
			var classes = defaultClassAttrs ?? "";
			var classTagHelperAttr = context.AllAttributes["class"];
			if (classTagHelperAttr != null)
			{
				var tmp = classTagHelperAttr.Value?.ToString();
				if (!string.IsNullOrWhiteSpace(tmp))
				{
					classes = classes + " " + tmp;
				}
			}
			return classes;
		}

		#endregion
	}
}
