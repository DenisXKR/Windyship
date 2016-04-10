using System;
using System.ComponentModel.DataAnnotations;

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
