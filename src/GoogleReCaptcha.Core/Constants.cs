namespace GoogleReCaptcha.Core
{
	/// <summary>
	/// Lib constants
	/// </summary>
    public static class Constants
	{
        #region General constants

        /// <summary>
        /// Default HTTP client name
        /// </summary>
        public const string DEFAULT_HTTP_CLIENT_NAME = @"GoogleReCaptcha.Core_HttpClient";

        #endregion

        #region reCAPTCHA v2 constants

        /// <summary>
        /// Default reCAPTCHA v2 library URL
        /// </summary>
        public const string DEFAULT_V2_LIBURL = DEFAULT_V3_LIBURL;

        /// <summary>
        /// Default reCAPTCHA v2 API URL
        /// </summary>
		public const string DEFAULT_V2_APIURL = DEFAULT_V3_APIURL;

        /// <summary>
        /// HTTP context key for reCAPTCHA v2 site key
        /// </summary>
        public const string HTTPCTX_KEY_V2_SITEKEY = "GoogleReCaptcha.Core_V2_SiteKey";

        /// <summary>
        /// HTTP context key for reCAPTCHA v2 theme
        /// </summary>
		public const string HTTPCTX_KEY_V2_THEME = "GoogleReCaptcha.Core_V2_Theme";

        /// <summary>
        /// HTTP context key for reCAPTCHA v2 size
        /// </summary>
		public const string HTTPCTX_KEY_V2_SIZE = "GoogleReCaptcha.Core_V2_Size";

        #endregion

        #region reCAPTCHA v3 constants

        /// <summary>
        /// Default reCAPTCHA v3 library URL
        /// </summary>
        public const string DEFAULT_V3_LIBURL = @"https://www.google.com/recaptcha/api.js";

        /// <summary>
        /// Default reCAPTCHA v3 API URL
        /// </summary>
        public const string DEFAULT_V3_APIURL = @"https://www.google.com/recaptcha/api/";

        /// <summary>
        /// Default reCAPTCHA v3 passing score
        /// </summary>
        public const float DEFAULT_V3_PASSING_SCORE = 0.6f;

        /// <summary>
        /// HTTP context key for reCAPTCHA v3 site key
        /// </summary>
		public const string HTTPCTX_KEY_V3_SITEKEY = "GoogleReCaptcha.Core_V3_SiteKey";

        #endregion
    }
}
