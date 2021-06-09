using System;
using System.Collections.Generic;
using System.Text;

namespace GoogleReCaptcha.Core
{
	public static class Constants
	{
		public const string DEFAULT_HTTP_CLIENT_NAME = @"GoogleReCaptcha.Core_HttpClient";

		public const string DEFAULT_V3_LIBURL = @"https://www.google.com/recaptcha/api.js";
		public const string DEFAULT_V3_APIURL = @"https://www.google.com/recaptcha/api/";
		public const float DEFAULT_V3_PASSING_SCORE = 0.6f;

		public const string DEFAULT_V2_LIBURL = DEFAULT_V3_LIBURL;
		public const string DEFAULT_V2_APIURL = DEFAULT_V3_APIURL;

		public const string HTTPCTX_KEY_V2_SITEKEY = "GoogleReCaptcha.Core_V2_SiteKey";
		public const string HTTPCTX_KEY_V2_THEME = "GoogleReCaptcha.Core_V2_Theme";
		public const string HTTPCTX_KEY_V2_SIZE = "GoogleReCaptcha.Core_V2_Size";

		public const string HTTPCTX_KEY_V3_SITEKEY = "GoogleReCaptcha.Core_V3_SiteKey";
	}
}
