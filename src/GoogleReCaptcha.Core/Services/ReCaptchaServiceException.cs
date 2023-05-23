namespace GoogleReCaptcha.Core.Services
{
    public class ReCaptchaServiceException : Exception
	{
		#region Fields

		private const string DEFAULT_MESSAGE = "Error in attempt to use configured Google ReCaptcha services from client.";

		#endregion

		#region Constuctors

		public ReCaptchaServiceException()
			: base(DEFAULT_MESSAGE)
		{
		}

		public ReCaptchaServiceException(string message)
			: base(message)
		{
		}

		public ReCaptchaServiceException(Exception innerException)
			: base(DEFAULT_MESSAGE, innerException)
		{
		}

		public ReCaptchaServiceException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		#endregion
	}
}
