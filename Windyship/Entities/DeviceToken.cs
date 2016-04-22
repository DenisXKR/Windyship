using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windyship.Entities
{
	public class DeviceToken : IEntity<int>
	{
		public int Id { get; set; }

		public int UserId { get; set; }

		public string Token { get; set; }

		public DeviceType DeviceType { get; set; }

		public User User { get; set; }
	}
}
