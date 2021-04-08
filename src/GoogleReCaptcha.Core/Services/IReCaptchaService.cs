using System.Threading.Tasks;

namespace GoogleReCaptcha3.Core.Services
{
	public interface IReCaptchaService
	{
		string GetToken();
		bool Verify();
		Task<bool> VerifyAsync();
	}
}
