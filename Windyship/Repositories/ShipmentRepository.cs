using Windyship.Core;
using Windyship.Dal.Core;
using Windyship.Entities;

namespace Windyship.Repositories
{
	public class ShipmentRepository : RepositoryBase<Shipment, int>, IShipmentRepository
	{
		public ShipmentRepository(IDataContext<Shipment, int> context)
			: base(context)
		{
		}
	}
}
