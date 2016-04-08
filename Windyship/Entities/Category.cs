using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windyship.Entities
{
	public class Category : IEntity<int>
	{
		public int Id { get; set; }

		public int CategoryId { get; set; }

		public Language Language { get; set; }

		public string Name { get; set; }
	}
}
