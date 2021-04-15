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
	}
}
