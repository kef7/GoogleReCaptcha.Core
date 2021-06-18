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
	/// Tag helper to assist in adding Google ReCaptcah v2/v3 data attributes from settings configured in appsettings.json into a submit button
	/// </summary>
	/// <remarks>
	/// Use this with invisible captcha for v2 or v3.
	/// </remarks>
	[HtmlTargetElement(TAG, TagStructure = TagStructure.NormalOrSelfClosing)]
	[HtmlTargetElement("button", Attributes = ATTR_FROMSETTINGS, TagStructure = TagStructure.NormalOrSelfClosing)] // Just to get it to call
	[HtmlTargetElement("button", Attributes = ATTR_ACTION + "," + ATTR_CALLBACK + "," + ATTR_SITEKEY, TagStructure = TagStructure.NormalOrSelfClosing)]
	public class SubmitButtonTagHelper : TagHelperBase
	{
		#region Static &| Consts

		public const string TAG = TagHelperConstants.TAG_PREFIX + "-submit-button";

		public const string ATTR_FROMSETTINGS = TagHelperConstants.ATTRIBUTE_PREFIX + "-from-settings";
		public const string ATTR_ACTION = TagHelperConstants.ATTRIBUTE_PREFIX + "-action";
		public const string ATTR_CALLBACK = TagHelperConstants.ATTRIBUTE_PREFIX + "-callback";
		public const string ATTR_SITEKEY = TagHelperConstants.ATTRIBUTE_PREFIX + "-sitekey";

		public const string DEFAULT_CLASSES = "g-recaptcha";

		#endregion

		#region Fields

		#endregion

		#region Properties

		/// <summary>
		/// Gets the recaptcha settings to use for this tag's output
		/// </summary>
		protected IReCaptchaSettings Settings { get; }

		/// <summary>
		/// Gets the <see cref="ViewContext"/> of the current rendering view
		/// </summary>
		[HtmlAttributeNotBound]
		[ViewContext]
		public ViewContext ViewContext { get; set; }

		/// <summary>
		/// From settings attribute; flag and target key; will override src if ture
		/// </summary>
		[HtmlAttributeName(ATTR_FROMSETTINGS)]
		public bool FromSettings { get; set; }

		/// <summary>
		/// Action type attribute; "submit"
		/// </summary>
		/// <remarks>
		/// Custom named action for analysis. See Google's documentation on <see href="https://developers.google.com/recaptcha/docs/v3#actions">actions</see>.
		/// </remarks>
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

		/// <summary>
		/// Button type attribute
		/// </summary>
		[HtmlAttributeName("type")]
		public SubmitButtonType Type { get; set; } = SubmitButtonType.Button;

		#endregion

		#region Constructor

		public SubmitButtonTagHelper(ILogger<SubmitButtonTagHelper> logger, IReCaptchaSettings settings)
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
				// Setup tag as non-recaptcha supported button
				output.TagName = "button";
				Logger.LogTrace("Suppress reCAPTCHA version of button tag");

				// Leave
				return;
			}

			Logger.LogTrace("Prepare output for reCAPTCHA button tag");

			// Apply settings to props
			if (FromSettings ||
				string.IsNullOrWhiteSpace(SiteKey))
			{
				Logger.LogTrace("Set SiteKey from settings");
				SiteKey = Settings.SiteKey;
			}

			// Apply default action
			if (string.IsNullOrWhiteSpace(Action))
			{
				Logger.LogTrace("Set default Action");
				Action = "submit";
			}

			// Apply default callback
			if (string.IsNullOrWhiteSpace(CallBack))
			{
				Logger.LogTrace("Set default Callback");
				CallBack = "onGReCaptchaV3Submit";
			}

			// Setup tag and base google attributes
			Logger.LogDebug("Set button tag to use {Action}, {Callback}, and {SiteKey}", Action, CallBack, SiteKey);
			output.TagName = "button";
			output.Attributes.SetAttribute("data-action", Action);
			output.Attributes.SetAttribute("data-callback", CallBack);
			output.Attributes.SetAttribute("data-sitekey", SiteKey);

			// Merge class attributes with defaults and apply
			var classes = GetMergedClassAttributes(context, DEFAULT_CLASSES);
			output.Attributes.SetAttribute("class", classes);

			// Set type if not defined
			Logger.LogTrace("Set button type if not defined");
			var btnTypeAttr = context.AllAttributes["type"];
			if (btnTypeAttr == null)
			{
				var btnType = Type.ToString().ToLower();
				Logger.LogDebug("Button type not defined, setting to {ButtonType}", btnType);
				output.Attributes.SetAttribute("type", btnType);
			}
		}

		#endregion

		#region Methods

		#endregion
	}
}
