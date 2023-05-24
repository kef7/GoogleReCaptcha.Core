namespace GoogleReCaptcha.Core.Settings
{
    /// <summary>
    /// reCAPTCHA v3 settings
    /// </summary>
    public class ReCaptchaV3Settings : IReCaptchaV3Settings
    {
        /// <inheritdoc />
        public bool Enabled { get; set; } = true;

        /// <inheritdoc />
        public string LibUrl { get; set; } = null!;

        /// <inheritdoc />
        public string ApiUrl { get; set; } = null!;

        /// <inheritdoc />
        public string SiteKey { get; set; } = null!;

        /// <inheritdoc />
        public string SecretKey { get; set; } = null!;

        /// <inheritdoc />
        public float DefaultPassingScore { get; set; } = Constants.DEFAULT_V3_PASSING_SCORE;
	}
}
