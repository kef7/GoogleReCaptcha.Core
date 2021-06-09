namespace GoogleReCaptcha.Core.Settings
{
	public interface IReCaptchaV3Settings : IReCaptchaSettings
	{
		float DefaultPassingScore { get; }
	}
}
