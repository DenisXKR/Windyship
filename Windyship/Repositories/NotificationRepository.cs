using Windyship.Core;
using Windyship.Dal.Core;
using Windyship.Entities;

namespace Windyship.Repositories
{
	public class NotificationRepository : RepositoryBase<Notification, int>, INotificationRepository
	{
		public NotificationRepository(IDataContext<Notification, int> context)
			: base(context)
		{
		}
	}
}
