namespace GoogleReCaptcha.Core.Settings
{
    /// <summary>
    /// reCAPTCHA v2 settings
    /// </summary>
    public class ReCaptchaV2Settings : IReCaptchaV2Settings
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
        public V2Theme? Theme { get; set; }

        /// <inheritdoc />
        public V2Size? Size { get; set; }
	}
}
