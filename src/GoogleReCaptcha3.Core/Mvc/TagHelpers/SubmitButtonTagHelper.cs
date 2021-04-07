using GoogleReCaptcha3.Core.Settings;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Threading.Tasks;

namespace GoogleReCaptcha3.Core.Mvc.TagHelpers
{
	/// <summary>
	/// Tag helper to assist in adding Google ReCaptcah v3 data attributes from settings configured in appsettings.json
	/// </summary>
	[HtmlTargetElement(TAG, TagStructure = TagStructure.NormalOrSelfClosing)]
	public class SubmitButtonTagHelper : TagHelper
	{
		#region Static &| Consts

		public const string TAG = TagHelperConstants.TAG_PREFIX + "-submit-button";

		public const string ATTR_ACTION = TagHelperConstants.ATTRIBUTE_PREFIX + "-action";
		public const string ATTR_CALLBACK = TagHelperConstants.ATTRIBUTE_PREFIX + "-callback";
		public const string ATTR_SITEKEY = TagHelperConstants.ATTRIBUTE_PREFIX + "-sitekey";

		#endregion

		#region Fields

		#endregion

		#region Properties

		/// <summary>
		/// Gets the recaptcha settings to use for this tag's output
		/// </summary>
		protected IReCaptchaV3Settings Settings { get; }

		/// <summary>
		/// Gets the <see cref="ViewContext"/> of the current rendering view
		/// </summary>
		[HtmlAttributeNotBound]
		[ViewContext]
		public ViewContext ViewContext { get; set; }

		/// <summary>
		/// Action type attribute; "submit"
		/// </summary>
		[HtmlAttributeName(ATTR_ACTION)]
		public string Action { get; set; }

		/// <summary>
		/// Call back func sig attribute
		/// </summary>
		[HtmlAttributeName(ATTR_CALLBACK)]
		public string CallBack { get; set; }

		/// <summary>
		/// Site key attribute
		/// </summary>
		[HtmlAttributeName(ATTR_SITEKEY)]
		public string SiteKey { get; set; }

		#endregion

		#region Constructor

		public SubmitButtonTagHelper(IReCaptchaV3Settings settings)
		{
			Settings = settings;
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
			if (Settings != null)
			{
				SiteKey = Settings.SiteKey ?? "?";
			}

			// Apply default to props
			if (string.IsNullOrWhiteSpace(Action))
			{
				Action = "submit";
			}
			if (string.IsNullOrWhiteSpace(CallBack))
			{
				CallBack = "onGReCaptchaV3Submit";
			}

			output.TagName = "button";
			output.Attributes.SetAttribute("data-action", Action);
			output.Attributes.SetAttribute("data-callback", CallBack);
			output.Attributes.SetAttribute("data-sitekey", SiteKey);
		}

		#endregion

		#region Methods

		#endregion
	}
}
