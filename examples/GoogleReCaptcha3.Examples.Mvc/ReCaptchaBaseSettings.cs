using System;
using System.Collections.Generic;
using System.Text;

namespace GoogleReCaptcha3.Core.Settings
{
	public abstract class ReCaptchaBaseSettings
	{
		public string LibUrl { get; set; }
		public string SiteKey { get; set; }
	}
}
