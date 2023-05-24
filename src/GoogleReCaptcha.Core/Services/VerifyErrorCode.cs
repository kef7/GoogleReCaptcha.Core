namespace GoogleReCaptcha.Core.Services
{
    using System.ComponentModel;

    /// <summary>
    /// Verify error codes that can be returned by Google's reCAPTCHA verify API endpoint
    /// </summary>
    public enum VerifyErrorCode
    {
        /// <summary>
        /// Missing input secret error
        /// </summary>
        [Description("missing-input-secret")]
        MissingInputSecret,

        /// <summary>
        /// Invalid input secret error
        /// </summary>
        [Description("invalid-input-secret")]
        InvalidInputSecret,

        /// <summary>
        /// Missing input response error
        /// </summary>
        [Description("missing-input-response")]
        MissingInputResponse,

        /// <summary>
        /// Invalid input response error
        /// </summary>
        [Description("invalid-input-response")]
        InvalidInputResponse,

        /// <summary>
        /// Bad request error
        /// </summary>
        [Description("bad-request")]
        BadRequest,

        /// <summary>
        /// Timeout or duplicate submission error
        /// </summary>
        [Description("timeout-or-duplicate")]
        TimeoutOrDuplicate
    }
}
