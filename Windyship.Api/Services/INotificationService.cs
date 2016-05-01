using System.Threading.Tasks;
using Windyship.Entities;

namespace Windyship.Api.Services
{
	public interface INotificationService
	{
		Task<string> SendNotice(int fromUserId, int toUserId, Shipment shipment, int type);
	}
}