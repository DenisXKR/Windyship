using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Windyship.Api.Model.Shipments
{
	public class ShipmentsRequest
	{
		public ShipmentsRequest()
		{
			this.language = "en";
		}

		public string language { get; set; }

		public int shipment_id { get; set; }
	}
}