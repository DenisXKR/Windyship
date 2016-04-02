using System.Threading.Tasks;
using System.Data.Entity;
using System.Linq;
using Windyship.Dal.Core;
using Windyship.Identity;
using Windyship.Entities;
using Windyship.Core;

namespace Windyship.Repositories
{
	public sealed class UserRepository : RepositoryBase<User, int>, IUserRepository
	{
		public UserRepository(IDataContext<User, int> context)
			: base(context)
		{
		}

		public Task<T> GetIdentityUserByIdAsync<T>(int id) where T : IdentityUser, new()
		{
			return GetFirstOrDefaultAsync(u => u.Id == id, IdentityUser.FromEntityUser<T>());
		}

		public Task<T> GetIdentityUserByNameAsync<T>(string userName) where T : IdentityUser, new()
		{
			return GetFirstOrDefaultAsync(u => u.Phone == userName, IdentityUser.FromEntityUser<T>());
		}

		public Task<T> GetIdentityUserByProviderAsync<T>(string providerName, string providerKey) where T : IdentityUser, new()
		{
			switch (providerName)
			{
				case ExternalProviderName.Facebook:
					return GetFirstOrDefaultAsync(u => u.FacebookId == providerKey, IdentityUser.FromEntityUser<T>());

				case ExternalProviderName.Twitter:
					return GetFirstOrDefaultAsync(u => u.TwitterId == providerKey, IdentityUser.FromEntityUser<T>());

				default:
					return null;
			}
		}
	}
}