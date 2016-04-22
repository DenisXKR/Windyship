using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windyship.Entities
{
	public class Notification : IEntity<int>
	{
		public Notification()
		{
			this.Aps = "{ \"content-available\" : false, \"alert\" : \"Your message here.\", \"sound\" : \"default\", \"badge\" : 1 }";
			this.Sent = false;
		}

		public Notification(Shipment shipment)
			: this()
		{
			var notic = new Notification
			{
				Data = JsonConvert.SerializeObject(new
				{
					type = (int)shipment.ShipmentStatus,
					shipment_id = shipment.Id,
					user_id = shipment.UserId,
					carrier_id = shipment.CarrierId,
				})
			};
		}

		public int Id { get; set; }

		public int UserId { get; set; }

		public string Aps { get; set; }

		public string Data { get; set; }

		public bool Sent { get; set; }

		public virtual User User { get; set; }
	}
}
