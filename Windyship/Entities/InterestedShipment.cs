using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windyship.Entities
{
	public class InterestedShipment : IEntity<int>
	{
		public int Id { get; set; }

		public int ShipmentId { get; set; }

		public int UserId { get; set; }

		public virtual Shipment Shipment { get; set; }

		public virtual User User { get; set; }
	}
}
