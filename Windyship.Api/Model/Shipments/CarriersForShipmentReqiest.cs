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
		}

		public string Language { get; set; }

		public int Shipment_id { get; set; }

	}
}