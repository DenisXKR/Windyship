using Windyship.Core;
using Windyship.Dal.Core;
using Windyship.Entities;

namespace Windyship.Repositories
{
	public class InterestedShipmentRepository : RepositoryBase<InterestedShipment, int>, IInterestedShipmentRepository
	{
		public InterestedShipmentRepository(IDataContext<InterestedShipment, int> context)
			: base(context)
		{
		}
	}
}
