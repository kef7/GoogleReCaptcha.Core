namespace GoogleReCaptcha.Core.Services
{
    using System.ComponentModel;

    /// <summary>
    /// Verify error codes that can be returned by Google's verify API endpoint
    /// </summary>
    public enum VerifyErrorCode
    {
        [Description("missing-input-secret")]
        MissingInputSecret,

        [Description("invalid-input-secret")]
        InvalidInputSecret,

        [Description("missing-input-response")]
        MissingInputResponse,

        [Description("invalid-input-response")]
        InvalidInputResponse,

        [Description("bad-request")]
        BadRequest,

        [Description("timeout-or-duplicate")]
        TimeoutOrDuplicate
    }
}
