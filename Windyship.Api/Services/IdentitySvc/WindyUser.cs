using Microsoft.AspNet.Identity;
using Windyship.Identity;

namespace Windyship.Api.Services.IdentitySvc
{
	public sealed class WindyUser : IdentityUser, IUser<int>
	{
	}
}