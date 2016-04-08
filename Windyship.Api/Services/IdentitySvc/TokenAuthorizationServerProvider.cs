using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.OAuth;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Windyship.Api.Services.IdentitySvc
{
	public class TokenAuthorizationServerProvider : OAuthAuthorizationServerProvider
	{
		public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
		{
			context.Validated();
		}

		public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
		{
			var userManager = context.OwinContext.GetUserManager<WindyUserManager>();
			WindyUser user;
			try
			{
				user = await userManager.FindByNameAsync(context.UserName);

				if (user != null && user.IsActive && user.Token == context.Password)
				{
					ClaimsIdentity identity = await userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ExternalBearer);
					context.Validated(identity);
				}
				else
				{
					context.SetError("invalid_client", "Bad temp token or user not exists");
					context.Rejected();
				}
			}
			catch
			{
				context.SetError("server_error");
				context.Rejected();
				return;
			}
		}
	}
}