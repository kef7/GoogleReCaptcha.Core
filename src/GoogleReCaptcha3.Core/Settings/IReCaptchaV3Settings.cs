﻿using System;
using System.Collections.Generic;
using System.Text;

namespace GoogleReCaptcha3.Core.Settings
{
	public interface IReCaptchaV3Settings
	{
		bool Enabled { get; }
		string LibUrl { get; }
		string ApiUrl { get; }
		string SiteKey { get; }
		string SecretKey { get; }
	}
}
