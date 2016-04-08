using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
