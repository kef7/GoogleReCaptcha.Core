using System;
using System.Collections.Generic;
using System.Text;

namespace GoogleReCaptcha3.Core.Settings
{
	public class ReCaptchaV3Settings : IReCaptchaV3Settings
	{
		public bool Enabled { get; set; } // TODO: Make this enabled flag work in settings, tag helpers, and services.
		public string LibUrl { get; set; }
		public string ApiUrl { get; set; }
		public string SiteKey { get; set; }
		public string SecretKey { get; set; }
	}
}
