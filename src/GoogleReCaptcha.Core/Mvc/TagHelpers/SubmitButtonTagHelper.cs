﻿using GoogleReCaptcha.Core.Settings;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Threading.Tasks;

namespace GoogleReCaptcha.Core.Mvc.TagHelpers
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

		public const string DEFAULT_CLASS_ATTRS = "g-recaptcha";

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
				throw new ArgumentNullException(nameof(context));
			}

			if (output == null)
			{
				throw new ArgumentNullException(nameof(output));
			}

			// Enabled?
			if (!Settings.Enabled)
			{
				// Setup tag as non-recaptcha supported button
				output.TagName = "button";

				// Leave
				return;
			}

			// Apply settings to props
			SiteKey = Settings.SiteKey ?? "?";

			// Apply default action
			if (string.IsNullOrWhiteSpace(Action))
			{
				Action = "submit";
			}

			// Apply default callback
			if (string.IsNullOrWhiteSpace(CallBack))
			{
				CallBack = "onGReCaptchaV3Submit";
			}

			// Setup tag and base google attributes
			output.TagName = "button";
			output.Attributes.SetAttribute("data-action", Action);
			output.Attributes.SetAttribute("data-callback", CallBack);
			output.Attributes.SetAttribute("data-sitekey", SiteKey);

			// Merge class attributes with defaults and apply
			var classes = GetMergedClassAttributes(context);
			output.Attributes.SetAttribute("class", classes);
		}

		#endregion

		#region Methods

		/// <summary>
		/// Get and merge class attributes presnt with default class attributes required for Google ReCaptcha
		/// </summary>
		/// <param name="context">Tag helper context of the tag being processed</param>
		/// <returns>String of all classes from current tag context and default</returns>
		protected string GetMergedClassAttributes(TagHelperContext context)
		{
			var classes = DEFAULT_CLASS_ATTRS;
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
