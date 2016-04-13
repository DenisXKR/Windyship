using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Windyship.Entities
{
	public class Shipment : IEntity<int>
	{
		public int Id { get; set; }

		public int UserId { get; set; }

		public int? CarrierId { get; set; }

		public string Title { get; set; }

		public ShipmentStatus ShipmentStatus { get; set; }

		[JsonIgnore]
		public string PinCode { get; set; }

		public DateTime PostDate { get; set; }

		public DateTime? DeleveryDate { get; set; }

		public byte[] Image1 { get; set; }

		public byte[] Image2 { get; set; }

		public byte[] Image3 { get; set; }

		public int CategoryId { get; set; }

		public string Description { get; set; }

		public decimal Budget { get; set; }

		public string Currency { get; set; }

		public string Size { get; set; } // s, m, l

		public decimal Weight { get; set; } // number (1...10)

		public string RecipiantName { get; set; }

		public string RecipiantMobile { get; set; }

		public string RecipiantSecoundary_name { get; set; }// Optional

		public string RecipiantSecoundary_mobile { get; set; }// Optional

		public virtual Category Category { get; set; }

		public virtual User User { get; set; }

		public virtual User Carrier { get; set; }

		public virtual ICollection<LocationFrom> From { get; set; }  // location

		public virtual ICollection<LocationTo> To { get; set; }
	}
}
