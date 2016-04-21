using System;
using System.Collections;
using System.Collections.Generic;
using Windyship.Api.Model.Common;

namespace Windyship.Api.Model.Shipments
{
	public class CarrierViewModel
	{
		public int TravelId { get; set; }

		public int CarrierId { get; set; }

		public string Name { get; set; }

		public decimal Rating { get; set; }

		public LocationViewModel From { get; set; }

		public LocationViewModel To { get; set; }

		public DateTime Delivery_date { get; set; }

		public string Mobile { get; set; }

		public string Image { get; set; }

		public IEnumerable<string> CarrierRestrictions {get; set;}

		/*
		public string Currency {get ;set;}

		public string Budget { get; set; }*/
	}
}