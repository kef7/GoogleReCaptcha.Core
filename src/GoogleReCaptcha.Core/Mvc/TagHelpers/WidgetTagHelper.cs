using GoogleReCaptcha.Core.Settings;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace GoogleReCaptcha.Core.Mvc.TagHelpers
{
	/// <summary>
	/// Tag helper to assist in adding Google ReCaptcah v2 data attributes from settings configured in appsettings.json into the v2 widget
	/// </summary>
	/// <remarks>
	/// Used in v2 implementation where the widget is required; not used in invisible variant.
	/// Use <see cref="SubmitdivTagHelper"/> for v2 invisible variant.
	/// </remarks>
	[HtmlTargetElement(TAG, TagStructure = TagStructure.NormalOrSelfClosing)]
	public class WidgetTagHelper : TagHelperBase
	{
		#region Static &| Consts

		public const string TAG = TagHelperConstants.TAG_PREFIX + "-widget";

		public const string ATTR_SITEKEY = TagHelperConstants.ATTRIBUTE_PREFIX + "-sitekey";

		public const string DEFAULT_CLASS_ATTRS = "g-recaptcha";

		#endregion

		#region Fields

		#endregion

		#region Properties

		/// <summary>
		/// Gets the recaptcha settings to use for this tag's output
		/// </summary>
		protected IReCaptchaV2Settings Settings { get; }

		/// <summary>
		/// Gets the <see cref="ViewContext"/> of the current rendering view
		/// </summary>
		[HtmlAttributeNotBound]
		[ViewContext]
		public ViewContext ViewContext { get; set; }

		/// <summary>
		/// Site key attribute
		/// </summary>
		[HtmlAttributeName(ATTR_SITEKEY)]
		public string SiteKey { get; set; }

		#endregion

		#region Constructor

		public WidgetTagHelper(ILogger<WidgetTagHelper> logger, IReCaptchaV2Settings settings)
			: base(logger)
		{
			if (settings == null)
			{
				throw new ArgumentNullException(nameof(settings));
			}

			Settings = settings;
		}

		#endregion

		#region TagHelper Overridden Methods

		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			if (context == null)
			{
				Logger.LogTrace("TagHelperContext obj null for reCAPTCHA script tag");
				throw new ArgumentNullException(nameof(context));
			}

			if (output == null)
			{
				Logger.LogTrace("TagHelperOutput obj null for reCAPTCHA script tag");
				throw new ArgumentNullException(nameof(output));
			}

			// Enabled?
			if (!Settings.Enabled)
			{
				// Setup tag as non-recaptcha supported div
				output.TagName = "div";
				Logger.LogTrace("Suppress reCAPTCHA version of widget div tag");

				// Leave
				return;
			}

			Logger.LogTrace("Prepare output for reCAPTCHA widget div tag");

			// Apply settings to props
			if (!string.IsNullOrWhiteSpace(Settings.SiteKey))
			{
				Logger.LogTrace("Get SiteKey from settings");
				SiteKey = Settings.SiteKey;
			}
			else
			{
				Logger.LogTrace("Set default SiteKey");
				SiteKey = "?";
			}

			// Setup tag and base google attributes
			Logger.LogDebug("Set widget div tag to use {SiteKey}", SiteKey);
			output.TagName = "div";
			output.Attributes.SetAttribute("data-sitekey", SiteKey);

			// Merge class attributes with defaults and apply
			Logger.LogTrace("Merge and set class attributes");
			var classes = GetMergedClassAttributes(context, DEFAULT_CLASS_ATTRS);
			output.Attributes.SetAttribute("class", classes);
		}

		#endregion

		#region Methods

		#endregion
	}
}
