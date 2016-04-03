using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
