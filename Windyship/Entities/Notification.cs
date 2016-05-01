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

		public int Id { get; set; }

		public int UserId { get; set; }

		public string Aps { get; set; }

		public string Data { get; set; }

		public bool Sent { get; set; }

		public virtual User User { get; set; }

		public void SetData(Shipment shipment, int type, User fromUser)
		{
			Data = JsonConvert.SerializeObject(new
			{
				id = Id,
				type = type,
				shipment_id = shipment.Id,
				user_id = shipment.UserId,
				carrier_id = shipment.CarrierId,
				fromWhomId = fromUser.Id,
				fromWhomName = fromUser.FirstName,
				fromWhomImage = fromUser.Avatar != null ? string.Format("Image/Avatar?id={0}", fromUser.Id) : null
			});
		}
	}
}
