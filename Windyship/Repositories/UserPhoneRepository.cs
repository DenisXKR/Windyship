using Windyship.Core;
using Windyship.Dal.Core;
using Windyship.Entities;

namespace Windyship.Repositories
{
	public class UserPhoneRepository : RepositoryBase<UserPhone, int>, IUserPhoneRepository
	{
		public UserPhoneRepository(IDataContext<UserPhone, int> context)
			: base(context)
		{
		}
	}
}
