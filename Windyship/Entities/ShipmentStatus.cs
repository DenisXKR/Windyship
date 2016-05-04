using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windyship.Entities
{
	public enum ShipmentStatus
	{
		PostShipmentRequest = 1,
		HireCarrier = 2, // (Still can edit, or cancel)
		AcceptShipmentRequest = 3,
		DeliveredToCarrier = 4,
		DeliveredToReceipt = 5,
		Review = 6,
		Cancelled = 7,
		NotDelivered = 8,
		Arhived = 9
	}
}
