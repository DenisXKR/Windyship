using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windyship.Entities
{
	public class LocationFrom : IEntity<int>
	{
		public int Id { get; set; }

		public int ShipmentId { get; set; }

		public decimal Lat { get; set; }

		public decimal Long { get; set; }
	}
}
