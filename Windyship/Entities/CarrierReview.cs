using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Windyship.Entities
{
	public class CarrierReview : IEntity<int>
	{
		public int Id { get; set; }

		public int ShipmentId { get; set; }

		public int Rate { get; set; }

		[MaxLength(5000)]
		public string Comment { get; set; }

		public virtual Shipment Shipment { get; set; }
	}
}
