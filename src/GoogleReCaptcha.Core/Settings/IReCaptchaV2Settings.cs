namespace GoogleReCaptcha.Core.Settings
{
    public interface IReCaptchaV2Settings : IReCaptchaSettings
	{
		V2Theme? Theme { get; }
		V2Size? Size { get; }
	}
}
