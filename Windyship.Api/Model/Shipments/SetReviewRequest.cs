using System.ComponentModel.DataAnnotations;

namespace Windyship.Api.Model.Shipments
{
	public class SetReviewRequest
	{
		public int Shipment_id { get; set; }

		[MaxLength(5000)]
		public string Comment { get; set; }

		public int Rate { get; set; } // 1 to 5
	}
}