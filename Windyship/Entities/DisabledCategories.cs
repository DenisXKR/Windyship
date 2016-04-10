namespace Windyship.Entities
{
	public class DisabledCategories : IEntity<int>
	{
		public int Id { get; set; }

		public int UserId { get; set; }
		
		public int CategoryId { get; set; }
	}
}
