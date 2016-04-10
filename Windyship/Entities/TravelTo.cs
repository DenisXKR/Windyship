namespace Windyship.Entities
{
	public class TravelTo : IEntity<int>
	{
		public int Id { get; set; }

		public int TravelId { get; set; }

		public decimal Lat { get; set; }

		public decimal Long { get; set; }
	}
}
