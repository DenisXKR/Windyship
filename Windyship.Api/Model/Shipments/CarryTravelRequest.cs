using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Windyship.Entities;

namespace Windyship.Api.Model.Shipments
{
	public class CarryTravelRequest
	{
		public int Id { get; set; }

		[Required]
		public DateTime traveling_date { get; set; }

		[Required]
		public DateTime arrival_before { get; set; }

		public IEnumerable<TravelFrom> From { get; set; }

		public IEnumerable<TravelFrom> To { get; set; }

		public IEnumerable<string> repeat_days { get; set; }

		public IEnumerable<int> not_allowed_categories { get; set; }

		public string max_size { get; set; } // s, m, l

		public decimal max_weight { get; set; } // number (1...10)
	}
}