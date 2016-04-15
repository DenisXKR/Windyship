using System.ComponentModel.DataAnnotations.Schema;
namespace Windyship.Entities
{
	public class TravelFrom : IEntity<int>
	{
		public int Id { get; set; }

		[ForeignKey("CarryTravel")]
		public int CarryTraveId { get; set; }

		public decimal Lat { get; set; }

		public decimal Long { get; set; }

		public virtual CarryTravel CarryTravel { get; set; }
	}
}
