using System.ComponentModel.DataAnnotations.Schema;
namespace Windyship.Entities
{
	public class TravelTo : IEntity<int>
	{
		public int Id { get; set; }

		[ForeignKey("CarryTravel")]
		public int CarryTraveId { get; set; }

		public decimal Lat { get; set; }

		public decimal Long { get; set; }

		public string Country { get; set; }

		public string City { get; set; }

		public string Address { get; set; }

		public virtual CarryTravel CarryTravel { get; set; }
	}
}
