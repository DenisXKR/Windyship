using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Windyship.Entities
{
	public class LocationFrom : IEntity<int>
	{
		public int Id { get; set; }

		public int ShipmentId { get; set; }

		public decimal Lat { get; set; }

		public decimal Long { get; set; }

		public string Country { get; set; }

		public string Address { get; set; }
	}
}
