using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;

namespace Windyship.Api.Services.IdentitySvc
{
	public interface IWindySignInManager
	{
		Task<ExternalLoginInfo> GetExternalLoginInfoAsync();
		Task<SignInStatus> PasswordSignInAsync(string userName, string password, bool isPersistent);
		Task<SignInStatus> ExternalLoginAsync(ExternalLoginInfo loginInfo, bool isPersistent);
		Task<SignInStatus> TwLoginAsync(string code, bool isPersistent);
		Task<SignInStatus> FbLoginAsync(string code, bool isPersistent);
		Task SignInAsync(WindyUser user, bool isPersistent, bool rememberBrowser);
		void SignOut();
	}
}