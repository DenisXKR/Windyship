using Windyship.Core;
using Windyship.Dal.Core;
using Windyship.Entities;

namespace Windyship.Repositories
{
	public sealed class ArhivedShipmentRepository : RepositoryBase<ArhivedShipment, int>, IArhivedShipmentRepository
	{
		public ArhivedShipmentRepository(IDataContext<ArhivedShipment, int> context)
			: base(context)
		{
		}
	}
}
