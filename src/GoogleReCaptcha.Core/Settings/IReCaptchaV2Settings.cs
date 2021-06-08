using System;
using System.Collections.Generic;
using System.Text;

namespace GoogleReCaptcha.Core.Settings
{
	public interface IReCaptchaV2Settings : IReCaptchaSettings
	{
		V2Theme Theme { get; }
		V2Size Size { get; }
	}
}
