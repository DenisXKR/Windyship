using Windyship.Core;
using Windyship.Dal.Core;
using Windyship.Entities;

namespace Windyship.Repositories
{
	public class UserRequestRepository : RepositoryBase<UserRequest, int>, IUserRequestRepository
	{
		public UserRequestRepository(IDataContext<UserRequest, int> context)
			: base(context)
		{
		}
	}
}
