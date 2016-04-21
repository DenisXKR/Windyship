using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Windyship.Api.Model.Common;
using Windyship.Entities;

namespace Windyship.Api.Model.Shipments
{
	public class ShipmentRequest
	{
		public ShipmentRequest()
		{

		}

		public ShipmentRequest(Shipment entity)
		{
			this.Id = entity.Id;
			this.UserId = entity.UserId;
			this.Title = entity.Title;
			this.Delevery_date = entity.DeleveryDate;
			this.From = entity.From.Select(l => new LocationViewModel { Long = l.Long, Lat = l.Lat, Country = l.Country, Address = l.Address });
			this.To = entity.To.Select(l => new LocationViewModel { Long = l.Long, Lat = l.Lat, Country = l.Country, Address = l.Address });
			this.Category_id = entity.CategoryId;
			this.Description = entity.Description;
			this.Budget = entity.Budget;
			this.Currency = entity.Currency;
			this.Size = entity.Size;
			this.Weight = entity.Weight;
			this.Recipiant_name = entity.RecipiantName;
			this.Recipiant_mobile = entity.RecipiantMobile;
			this.Recipiant_secoundary_name = entity.RecipiantSecoundary_name;
			this.Recipiant_secoundary_mobile = entity.RecipiantSecoundary_mobile;
		}

		public int? Id { get; set; }

		public int? UserId { get; set; }

		[Required]
		public string Title { get; set; }

		public DateTime? Delevery_date { get; set; }

		public IEnumerable<LocationViewModel> From { get; set; }  // location

		public IEnumerable<LocationViewModel> To { get; set; }

		[Required]
		public int Category_id { get; set; }

		public string Description { get; set; }

		public decimal Budget { get; set; }

		public string Currency { get; set; }

		public string Size { get; set; } // s, m, l

		public decimal Weight { get; set; } // number (1...10)

		public string Recipiant_name { get; set; }

		public string Recipiant_mobile { get; set; }

		public string Recipiant_secoundary_name { get; set; }// Optional

		public string Recipiant_secoundary_mobile { get; set; }// Optional
	
	}
}