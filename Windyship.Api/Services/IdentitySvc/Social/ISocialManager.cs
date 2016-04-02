using Microsoft.AspNet.Identity.Owin;
using Windyship.Api.Models.Api.v1.UserModels.Social;

namespace Windyship.Api.Services.IdentitySvc.Social
{
	public interface ISocialManager
	{
		ExternalLoginInfo GetLoginInfo(string code);

		ISocialUser GetUser(ExternalLoginInfo loginInfo);

		string ProviderName { get; }
	}
}