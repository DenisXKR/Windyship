using Windyship.Core;
using Windyship.Dal.Core;
using Windyship.Entities;

namespace Windyship.Repositories
{
	public class LocationToRepository : RepositoryBase<LocationTo, int>, ILocationToRepository
	{
		public LocationToRepository(IDataContext<LocationTo, int> context)
			: base(context)
		{
		}
	}
}
