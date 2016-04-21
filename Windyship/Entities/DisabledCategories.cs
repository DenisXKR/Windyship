namespace Windyship.Entities
{
	public class DisabledCategories : IEntity<int>
	{
		public int Id { get; set; }

		public int CarryTravelId { get; set; }
		
		public int CategoryId { get; set; }

		public virtual Category Category { get; set; }
	}
}
