using Windyship.Core;
using Windyship.Dal.Core;
using Windyship.Entities;

namespace Windyship.Repositories
{
	public sealed class DeviceTokenRepository : RepositoryBase<DeviceToken, int>, IDeviceTokenRepository
	{
		public DeviceTokenRepository(IDataContext<DeviceToken, int> context)
			: base(context)
		{
		}
	}
}
