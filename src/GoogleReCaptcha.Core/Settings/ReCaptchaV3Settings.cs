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
        public string LibUrl { get; set; }

        /// <inheritdoc />
        public string ApiUrl { get; set; }

        /// <inheritdoc />
        public string SiteKey { get; set; }

        /// <inheritdoc />
        public string SecretKey { get; set; }

        /// <inheritdoc />
        public float DefaultPassingScore { get; set; } = Constants.DEFAULT_V3_PASSING_SCORE;
	}
}
