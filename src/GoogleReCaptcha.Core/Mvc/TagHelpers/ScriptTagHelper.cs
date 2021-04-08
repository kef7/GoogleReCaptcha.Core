using GoogleReCaptcha3.Core.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleReCaptcha3.Core.Mvc.TagHelpers
{
	/// <summary>
	/// Tag helper to assist in getting Google Lib/JS url into script tag
	/// </summary>
	[HtmlTargetElement(TAG)]
	[HtmlTargetElement("script", Attributes = ATTR_FROMSETTINGS)]
	public class ScriptTagHelper : TagHelper
	{
		#region Static &| Consts

		public const string TAG = TagHelperConstants.TAG_PREFIX + "-script";

		public const string ATTR_FROMSETTINGS = TagHelperConstants.ATTRIBUTE_PREFIX + "-from-settings";
		public const string ATTR_LIBURL = TagHelperConstants.ATTRIBUTE_PREFIX + "-liburl";

		#endregion

		#region Fields

		#endregion

		#region Properties

		/// <summary>
		/// Gets the recaptcha settings to use for this tag's output
		/// </summary>
		protected IReCaptchaV3Settings Settings { get; }

		protected IUrlHelper UrlHelper { get; }

		/// <summary>
		/// From settings attribute; flag and target key; will override src if ture
		/// </summary>
		[HtmlAttributeName(ATTR_FROMSETTINGS)]
		public bool FromSettings { get; set; }

		/// <summary>
		/// Library url attribute
		/// </summary>
		[HtmlAttributeName(ATTR_LIBURL)]
		public string LibUrl { get; set; }

		#endregion

		#region Constructor

		public ScriptTagHelper(IReCaptchaV3Settings settings, IUrlHelper urlHelper)
		{
			Settings = settings;
			UrlHelper = urlHelper;
		}

		#endregion

		#region TagHelper Overridden Methods

		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			if (context == null)
			{
				throw new ArgumentNullException(nameof(context));
			}

			if (output == null)
			{
				throw new ArgumentNullException(nameof(output));
			}

			// Apply settings to props
			if (FromSettings &&
				Settings != null)
			{
				LibUrl = Settings.LibUrl;
			}

			// Apply defaults to props
			if (string.IsNullOrWhiteSpace(LibUrl))
			{
				// Set to null
				LibUrl = null;

				// Pull from src
				if (!FromSettings)
				{
					var src = context.AllAttributes.Where(a => a.Name.ToLower().Equals("src")).LastOrDefault();
					if (src != null)
					{
						LibUrl = src.Value?.ToString();
					}
				}

				// If null set to current Google JS lib URI
				if (LibUrl == null)
				{
					LibUrl = "https://www.google.com/recaptcha/api.js";
				}
			}

			// Resolve relative
			if (LibUrl.StartsWith("~"))
			{
				LibUrl = UrlHelper.Content(LibUrl);
			}

			output.TagName = "script";
			output.Attributes.SetAttribute("type", "text/javascript");
			output.Attributes.SetAttribute("src", LibUrl);
		}

		#endregion

		#region Methods

		#endregion
	}
}
