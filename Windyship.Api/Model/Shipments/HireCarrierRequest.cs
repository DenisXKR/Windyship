﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Windyship.Api.Model.Shipments
{
	public class HireCarrierRequest
	{
		public int Shipment_id {get; set;}

		public int Carrier_id { get; set; }
	}
}