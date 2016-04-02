using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Windyship.Api.Services.IdentitySvc;

namespace Windyship.Api.Site.Services.IdentitySvc
{
	public interface IWindyUserManager
	{
		Task<IdentityResult> CreateAsync(WindyUser user, string password);
		Task<IdentityResult> ConfirmPhoneAsync(string phone, string token);
		Task SendCheckPhoneCode(string phone);
		Task SendEmailAsync(int userId, string subject, string body);
		Task<IdentityResult> SetEmailAsync(int userId, string email);
		Task<WindyUser> FindByNameAsync(string userName);
		Task<bool> IsAccountConfirmedAsync(string phone);
		Task<WindyUser> GetUserById(int userId);
	}
}