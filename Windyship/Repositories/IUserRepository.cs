using System.Threading.Tasks;
using Windyship.Core;
using Windyship.Entities;
using Windyship.Identity;

namespace Windyship.Repositories
{
	public interface IUserRepository : IRepositoryBase<User, int>
	{
		Task<T> GetIdentityUserByIdAsync<T>(int id) where T : IdentityUser, new();
		Task<T> GetIdentityUserByNameAsync<T>(string userName) where T : IdentityUser, new();
		Task<T> GetIdentityUserByProviderAsync<T>(string providerName, string providerKey) where T : IdentityUser, new();
	}
}