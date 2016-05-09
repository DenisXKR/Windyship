using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Windyship.Api.Model.Shipments
{
	public class CarriersForShipmentReqiest
	{
		public CarriersForShipmentReqiest()
		{
			this.Language = "en";
			this.Sort = "nearest";
		}

		public string Language { get; set; }

		public int Shipment_id { get; set; }

		public string Sort { get; set; } //: ”nearest” / “soonest” / “rate” (nearest by default)
	}
}