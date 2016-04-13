using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Windyship.Api.Model.Shipments
{
	public class DeliveredToRecipientRequest
	{
		public int Shipment_id { get; set; }

		public string Pin_code { get; set; } // only if carrier
	}
}