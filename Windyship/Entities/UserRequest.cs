using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windyship.Entities
{
	public class UserRequest : IEntity<int>
	{
		[Key]
		public int Id { get; set; }

		public String Name { get; set; }

		public DateTime Date { get; set; }

		public string Email { get; set; }

		public String Request { get; set; }

		public bool Resolved { get; set; }
	}
}
