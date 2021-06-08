using System;
using System.Collections.Generic;
using System.Text;

namespace GoogleReCaptcha.Core.Settings
{
	public class ReCaptchaSettings : IReCaptchaSettings
	{
		public bool Enabled { get; set; } = true;
		public string LibUrl { get; set; }
		public string ApiUrl { get; set; }
		public string SiteKey { get; set; }
		public string SecretKey { get; set; }
	}
}
