namespace GoogleReCaptcha.Core.Mvc.TagHelpers
{
    using GoogleReCaptcha.Core.Settings;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Microsoft.AspNetCore.Razor.TagHelpers;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Tag helper to assist in adding Google reCAPTCHA v2 data attributes from settings configured in appsettings.json into the v2 widget
    /// </summary>
    /// <remarks>
    /// Used in v2 implementation where the widget is required; not used in invisible variant.
    /// Use <see cref="SubmitButtonTagHelper"/> for v2 invisible variant.
    /// </remarks>
    [HtmlTargetElement(TAG, TagStructure = TagStructure.NormalOrSelfClosing)]
    public class WidgetTagHelper : TagHelperBase
    {
        #region Static &| Consts

        /// <summary>
        /// Widget tag
        /// </summary>
        public const string TAG = TagHelperConstants.TAG_PREFIX + "-widget";

        /// <summary>
        /// Sitekey attribute name
        /// </summary>
        public const string ATTR_SITEKEY = TagHelperConstants.ATTRIBUTE_PREFIX + "-sitekey";

        /// <summary>
        /// Theme attribute name
        /// </summary>
        public const string ATTR_THEME = TagHelperConstants.ATTRIBUTE_PREFIX + "-theme";

        /// <summary>
        /// Size attribute name
        /// </summary>
        public const string ATTR_SIZE = TagHelperConstants.ATTRIBUTE_PREFIX + "-size";

        /// <summary>
        /// Tab index attribute name
        /// </summary>
        public const string ATTR_TAB_INDEX = TagHelperConstants.ATTRIBUTE_PREFIX + "-tabindex";

        /// <summary>
        /// Callback attribute name
        /// </summary>
        public const string ATTR_CALLBACK = TagHelperConstants.ATTRIBUTE_PREFIX + "-cb";

        /// <summary>
        /// Exp callback attribute name
        /// </summary>
        public const string ATTR_EXP_CALLBACK = TagHelperConstants.ATTRIBUTE_PREFIX + "-exp-cb";

        /// <summary>
        /// Error callback attribute name
        /// </summary>
        public const string ATTR_ERR_CALLBACK = TagHelperConstants.ATTRIBUTE_PREFIX + "-err-cb";

        /// <summary>
        /// Default reCAPTCHA classes
        /// </summary>
        public const string DEFAULT_CLASS_ATTRS = "g-recaptcha";

        #endregion

        #region Fields

        #endregion

        #region Properties

        /// <summary>
        /// Gets the reCAPTCHA settings to use for this tag's output
        /// </summary>
        protected IReCaptchaV2Settings Settings { get; }

        /// <summary>
        /// Gets the <see cref="ViewContext"/> of the current rendering view
        /// </summary>
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext? ViewContext { get; set; }

        /// <summary>
        /// Site key attribute; will attempt to use from configured settings if not defined
        /// </summary>
        [HtmlAttributeName(ATTR_SITEKEY)]
        public string? SiteKey { get; set; }

        /// <summary>
        /// V2 theme attribute; will attempt to use from configured settings if not defined
        /// </summary>
        [HtmlAttributeName(ATTR_THEME)]
        public V2Theme? Theme { get; set; }

        /// <summary>
        /// V2 theme attribute; will attempt to use from configured settings if not defined
        /// </summary>
        [HtmlAttributeName(ATTR_THEME)]
        public V2Size? Size { get; set; }

        /// <summary>
        /// Tab index attribute
        /// </summary>
        [HtmlAttributeName(ATTR_TAB_INDEX)]
        public int? TabIndex { get; set; }

        /// <summary>
        /// Call-back function; executed when the use submits a successful response (token passed into it)
        /// </summary>
        [HtmlAttributeName(ATTR_CALLBACK)]
        public string? CallBack { get; set; }

        /// <summary>
        /// Expired call-back function; executed when reCAPTCHA response expires and the user needs to re-verify
        /// </summary>
        [HtmlAttributeName(ATTR_EXP_CALLBACK)]
        public string? ExpiredCallBack { get; set; }

        /// <summary>
        /// Error call-back function; executed when script encounters an error
        /// </summary>
        [HtmlAttributeName(ATTR_ERR_CALLBACK)]
        public string? ErrorCallBack { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Widget tag helper
        /// </summary>
        /// <param name="logger">Generic logger</param>
        /// <param name="settings">Settings object</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="settings"/> is null</exception>
        public WidgetTagHelper(ILogger<WidgetTagHelper> logger, IReCaptchaV2Settings settings)
            : base(logger)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));;
        }

        #endregion

        #region TagHelper Overridden Methods

        /// <summary>
        /// Process tag
        /// </summary>
        /// <param name="context">Tag helper context</param>
        /// <param name="output">Tag helper output object</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="context"/> or <paramref name="output"/> is null</exception>
        public override void Process(TagHelperContext context, TagHelperOutput output)
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
                // Setup tag as non-reCAPTCHA supported div
                output.TagName = "div";
                Logger.LogTrace("Suppress reCAPTCHA version of widget div tag");

                // Leave
                return;
            }

            Logger.LogTrace("Prepare output for reCAPTCHA widget div tag");

            // Apply sitekey setting to props
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

            // Apply theme setting to prop
            if (!Theme.HasValue &&
                Settings.Theme.HasValue)
            {
                Logger.LogTrace("Get Theme from settings");
                Theme = Settings.Theme.Value;
            }

            // Apply theme setting to prop
            if (!Size.HasValue &&
                Settings.Size.HasValue)
            {
                Logger.LogTrace("Get Size from settings");
                Size = Settings.Size.Value;
            }

            // Setup tag and base google attributes
            Logger.LogDebug("Set widget div tag to use {SiteKey}", SiteKey);
            output.TagName = "div";
            output.Attributes.SetAttribute("data-sitekey", SiteKey);

            // Setup theme attribute
            if (Theme.HasValue)
            {
                Logger.LogDebug("Apply widget theme attribute as {Theme}", Theme.Value);
                output.Attributes.SetAttribute("data-theme", Theme.Value.ToString().ToLower());
            }

            // Setup size attribute
            if (Size.HasValue)
            {
                Logger.LogDebug("Apply widget size attribute as {Size}", Size.Value);
                output.Attributes.SetAttribute("data-size", Size.Value.ToString().ToLower());
            }

            // Setup tabindex attribute
            if (TabIndex.HasValue)
            {
                Logger.LogDebug("Apply widget tabindex attribute as {TabIndex}", TabIndex.Value);
                output.Attributes.SetAttribute("data-tabindex", TabIndex.Value.ToString());
            }

            // Setup call-back attribute
            if (!string.IsNullOrWhiteSpace(CallBack))
            {
                Logger.LogDebug("Apply widget call-back attribute as {CallBack}", CallBack);
                output.Attributes.SetAttribute("data-callback", CallBack);
            }

            // Setup expired call-back attribute
            if (!string.IsNullOrWhiteSpace(ExpiredCallBack))
            {
                Logger.LogDebug("Apply widget expired call-back attribute as {ExpiredCallBack}", ExpiredCallBack);
                output.Attributes.SetAttribute("data-expired-callback", ExpiredCallBack);
            }

            // Setup error call-back attribute
            if (!string.IsNullOrWhiteSpace(ErrorCallBack))
            {
                Logger.LogDebug("Apply widget error call-back attribute as {ErrorCallBack}", ErrorCallBack);
                output.Attributes.SetAttribute("data-error-callback", ErrorCallBack);
            }

            // Merge class attributes with defaults and apply
            Logger.LogTrace("Merge and set class attributes");
            var classes = GetMergedClassAttributes(context, DEFAULT_CLASS_ATTRS);
            output.Attributes.SetAttribute("class", classes);
        }

        /// <summary>
        /// Process tag
        /// </summary>
        /// <param name="context">Tag helper context</param>
        /// <param name="output">Tag helper output object</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="context"/> or <paramref name="output"/> is null</exception>
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            await Task.Run(() => Process(context, output));
        }

        #endregion

        #region Methods

        #endregion
    }
}
