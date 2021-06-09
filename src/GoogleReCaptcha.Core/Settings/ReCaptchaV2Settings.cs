using System;
using System.Collections.Generic;
using System.Text;

namespace GoogleReCaptcha.Core.Settings
{
	public class ReCaptchaV2Settings : IReCaptchaV2Settings
	{
		public bool Enabled { get; set; } = true;
		public string LibUrl { get; set; }
		public string ApiUrl { get; set; }
		public string SiteKey { get; set; }
		public string SecretKey { get; set; }
		public V2Theme? Theme { get; set; }
		public V2Size? Size { get; set; }
	}
}
