using System.ComponentModel.DataAnnotations;

namespace Windyship.Entities
{
	public class Content : IEntity<int>
	{
		[Key]
		public int Id { get; set; }

		public Language Language { get; set; }
		
		public ContentPart ContentPart { get; set; }

		public string Text { get; set; }
	}
}
