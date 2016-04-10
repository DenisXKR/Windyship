using Windyship.Core;
using Windyship.Dal.Core;
using Windyship.Entities;

namespace Windyship.Repositories
{
	public class TravelFromRepository : RepositoryBase<TravelFrom, int>, ITravelFromRepository
	{
		public TravelFromRepository(IDataContext<TravelFrom, int> context)
			: base(context)
		{
		}
	}
}
