using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Windyship.Api.Model.Shipments
{
	public class InterestRequest
	{
		public int shipment_id { get; set; }

		public bool interest { get; set; }
	}
}