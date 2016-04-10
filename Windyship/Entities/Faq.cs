using System.ComponentModel.DataAnnotations;

namespace Windyship.Entities
{
	public class Faq : IEntity<int>
	{
		[Key]
		public int Id { get; set; }

		public Language Language { get; set; }

		public string Question { get; set; }

		public string Answer { get; set; }
	}
}
