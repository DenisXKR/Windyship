using Windyship.Core;
using Windyship.Dal.Core;
using Windyship.Entities;

namespace Windyship.Repositories
{
	public class TravelToRepository : RepositoryBase<TravelTo, int>, ITravelToRepository
	{
		public TravelToRepository(IDataContext<TravelTo, int> context)
			: base(context)
		{
		}
	}
}
