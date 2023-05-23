using GoogleReCaptcha.Core.Services.Models;

namespace GoogleReCaptcha.Core.Services
{
    public class ReCaptchaVerifyException : Exception
	{
		#region Fields

		private const string DEFAULT_MESSAGE = "Google ReCaptcha errored or the response contains errors.";

		#endregion

		#region Properties

		public VerifyResponse Response { get; private set; } = null;

		#endregion

		#region Constuctors

		public ReCaptchaVerifyException()
			: base(DEFAULT_MESSAGE)
		{
		}

		public ReCaptchaVerifyException(VerifyResponse response)
			: base(DEFAULT_MESSAGE)
		{
			Response = response;
		}

		public ReCaptchaVerifyException(string message, VerifyResponse response)
			: base(message)
		{
			Response = response;
		}

		public ReCaptchaVerifyException(string message, Exception innerException, VerifyResponse response)
			: base(message, innerException)
		{
			Response = response;
		}

		public ReCaptchaVerifyException(Exception innerException, VerifyResponse response)
			: base(DEFAULT_MESSAGE, innerException)
		{
			Response = response;
		}

		#endregion

		#region Methods

		#endregion
	}
}
