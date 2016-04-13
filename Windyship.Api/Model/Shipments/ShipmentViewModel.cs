using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Windyship.Api.Model.Common;
using Windyship.Entities;

namespace Windyship.Api.Model.Shipments
{
	public class ShipmentViewModel
	{
		public int shipment_id { get; set; }

		public string title { get; set; }

		public DateTime post_date { get; set; }

		public DateTime? delevery_date { get; set; }

		public IEnumerable<LocationViewModel> from { get; set; }

		public IEnumerable<LocationViewModel> to { get; set; }

		public int category_id { get; set; }

		public string image1 { get; set; }

		public string image2 { get; set; }

		public string image3 { get; set; }

		public string description { get; set; }

		public decimal budget { get; set; }

		public string currency { get; set; }

		public string size { get; set; } // s, m, l

		public decimal weight { get; set; } // number (1...10)

		public string recipiant_name { get; set; }

		public string recipiant_mobile { get; set; }

		public string recipiant_secoundary_name { get; set; }

		public string recipiant_secoundary_mobile { get; set; }

		public int shipment_status { get; set; }

		public SmallUserViewModel sender { get; set; }

		public SmallUserViewModel receiver { get; set; }

		public string category { get; set; }
	}
}