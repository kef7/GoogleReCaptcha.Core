﻿namespace GoogleReCaptcha.Core.Mvc.TagHelpers
{
    using GoogleReCaptcha.Core.Settings;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Razor.TagHelpers;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Tag helper to assist in getting Google reCAPTCHA Lib/JS url into script tag
    /// </summary>
    [HtmlTargetElement(TAG)]
    [HtmlTargetElement("script", Attributes = ATTR_FROMSETTINGS)]
    [HtmlTargetElement("script", Attributes = ATTR_LIBURL + "," + ATTR_USE_EXPLICIT_DEFAULT + "," + ATTR_SET_EXPLICIT_CALLBACK)]
    public class ScriptTagHelper : TagHelperBase
    {
        #region Static &| Consts

        /// <summary>
        /// Script tag
        /// </summary>
        public const string TAG = TagHelperConstants.TAG_PREFIX + "-script";

        /// <summary>
        /// From settings attribute name
        /// </summary>
        public const string ATTR_FROMSETTINGS = TagHelperConstants.ATTRIBUTE_PREFIX + "-from-settings";

        /// <summary>
        /// Lib URL attribute name
        /// </summary>
        public const string ATTR_LIBURL = TagHelperConstants.ATTRIBUTE_PREFIX + "-liburl";

        /// <summary>
        /// Explicit attribute name
        /// </summary>
        public const string ATTR_USE_EXPLICIT_DEFAULT = TagHelperConstants.ATTRIBUTE_PREFIX + "-explicit";

        /// <summary>
        /// Explicit callback attribute name
        /// </summary>
        public const string ATTR_SET_EXPLICIT_CALLBACK = TagHelperConstants.ATTRIBUTE_PREFIX + "-explicit-cb";

        #endregion

        #region Fields

        #endregion

        #region Properties

        /// <summary>
        /// Gets the reCAPTCHA settings to use for this tag's output
        /// </summary>
        protected IReCaptchaSettings Settings { get; }

        /// <summary>
        /// Gets the UrlHelper attached to the current pipeline
        /// </summary>
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
        public string? LibUrl { get; set; }

        /// <summary>
        /// Get or set flag informing scrpit to use explicit flag with default call-back javascript function name
        /// </summary>
        [HtmlAttributeName(ATTR_USE_EXPLICIT_DEFAULT)]
        public bool UseExplicitDefault { get; set; } = false;

        /// <summary>
        /// Get or set explicit call-back javascript function name; will also set explicit flag in url
        /// </summary>
        [HtmlAttributeName(ATTR_SET_EXPLICIT_CALLBACK)]
        public string? ExplicitCallBack { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// reCAPTCHA script tag helper
        /// </summary>
        /// <param name="logger">Generic logger</param>
        /// <param name="settings">Settings object</param>
        /// <param name="urlHelper">URL helper reference</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="settings"/> or <paramref name="urlHelper"/> is null</exception>
        public ScriptTagHelper(ILogger<ScriptTagHelper> logger, IReCaptchaSettings settings, IUrlHelper urlHelper)
            : base(logger)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));;
            UrlHelper = urlHelper ?? throw new ArgumentNullException(nameof(urlHelper));;
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
                // Kill the output of this tag
                output.SuppressOutput();
                Logger.LogTrace("Suppress output for reCAPTCHA script tag");

                // Leave
                return;
            }

            Logger.LogTrace("Prepare output for reCAPTCHA script tag");

            // Apply settings to props
            if (FromSettings)
            {
                Logger.LogTrace("Set lib url from settings");
                LibUrl = Settings.LibUrl;
            }

            // Apply defaults to props
            if (string.IsNullOrWhiteSpace(LibUrl))
            {
                // Set to null
                LibUrl = null;

                // Pull from src
                Logger.LogTrace("Get lib url already present in tag src");
                var src = context.AllAttributes.Where(a => a.Name.ToLower().Equals("src")).LastOrDefault();
                if (src != null)
                {
                    Logger.LogTrace("Set lib url from src");
                    LibUrl = src.Value as string ?? "";
                }

                // If null set to default known Google JS lib URL
                if (LibUrl == null)
                {
                    Logger.LogTrace("Get default lib url");
                    LibUrl = Constants.DEFAULT_V3_LIBURL;
                }
            }

            // Resolve relative
            if (LibUrl.StartsWith("~"))
            {
                Logger.LogTrace("Resolve lib url relative path");
                LibUrl = UrlHelper.Content(LibUrl);
            }

            // Set explicit call-back script function in URL
            if (!string.IsNullOrWhiteSpace(ExplicitCallBack))
            {
                Logger.LogTrace("Set explicit callback using provided");
                LibUrl = RebuildUrlUsingExplicitCallBackQuery(LibUrl, ExplicitCallBack);
            }
            else if (UseExplicitDefault)
            {
                Logger.LogTrace("Set explicit callback using default");
                LibUrl = RebuildUrlUsingExplicitCallBackQuery(LibUrl, "onloadCallback");
            }

            // Add script tag and set attributes
            Logger.LogDebug("Set script tag to use {LibUrl}", LibUrl);
            output.TagName = "script";
            output.Attributes.SetAttribute("type", "text/javascript");
            output.Attributes.SetAttribute("src", LibUrl);

            // Add async attributes
            var asyncTagAttr = new TagHelperAttribute("async", "", HtmlAttributeValueStyle.Minimized);
            output.Attributes.SetAttribute(asyncTagAttr);

            // Add defer attributes
            var deferTagAttr = new TagHelperAttribute("defer", "", HtmlAttributeValueStyle.Minimized);
            output.Attributes.SetAttribute(deferTagAttr);
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

        /// <summary>
        /// Rebuild URL using explicit callback query
        /// </summary>
        /// <param name="url">Script URL</param>
        /// <param name="callBackFnName">Callback function name to call after load</param>
        /// <returns>Script URL with callback if provided</returns>
        /// <exception cref="ArgumentException"></exception>
        private string RebuildUrlUsingExplicitCallBackQuery(string url, string callBackFnName)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentException($"'{nameof(url)}' cannot be null or whitespace.", nameof(url));
            }

            if (string.IsNullOrWhiteSpace(callBackFnName))
            {
                throw new ArgumentException($"'{nameof(callBackFnName)}' cannot be null or whitespace.", nameof(callBackFnName));
            }

            // Rebuild url using explicit call-back in query string if needed
            if (url.Contains("?"))
            {// Split URL in two where query string is second part
                var theSplits = url.Split("?", 2);
                if (theSplits.Length == 2)
                {
                    // Split query string into items
                    var queryItems = theSplits[1].Split("&");
                    if (queryItems.Length > 0)
                    {
                        // Define needed query mappings
                        var neededQueryMappings = new Dictionary<string, string>()
                            {
                                { "onload", callBackFnName },
                                { "render", "explicit" }
                            };

                        // Iterate over items and update what is needed
                        for (var i = 0; i < queryItems.Length; i++)
                        {
                            // Split item into key and value elements
                            var qi = queryItems[i].Split("=", 2);
                            if (qi.Length == 2)
                            {
                                // Match query item in mapping
                                var qiKey = qi[0];
                                foreach (var kvpMappings in neededQueryMappings)
                                {
                                    if (kvpMappings.Key.Equals(qi[0], StringComparison.OrdinalIgnoreCase))
                                    {
                                        // Match found, rebuild query item and set to current item
                                        queryItems[i] = string.Format("{0}={1}", kvpMappings.Key, kvpMappings.Value);
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    // Rebuild URL
                    var newQueryString = string.Join("&", queryItems);
                    if (newQueryString != "")
                    {
                        newQueryString = "?" + newQueryString;
                    }
                    url = theSplits[0] + newQueryString;
                }
            }
            else
            {
                url = string.Format("{0}?onload={1}&render=explicit", url, callBackFnName);
            }

            return url;
        }

        #endregion
    }
}
