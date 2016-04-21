namespace Windyship.Entities
{
	public class UserPhone : IEntity<int>
	{
		public int Id { get; set; }

		public int UserId { get; set; }

		public string Phone { get; set; }

		public User User { get; set; }
	}
}
