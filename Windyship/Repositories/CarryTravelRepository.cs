using Windyship.Core;
using Windyship.Dal.Core;
using Windyship.Entities;

namespace Windyship.Repositories
{
	public class CarryTravelRepository : RepositoryBase<CarryTravel, int>, ICarryTravelRepository
	{
		public CarryTravelRepository(IDataContext<CarryTravel, int> context)
			: base(context)
		{
		}
	}
}
