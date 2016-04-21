using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Windyship.Api.Model.Common
{
	public class LocationViewModel
	{
		public decimal Long { get; set; }

		public decimal Lat { get; set; }

		public string Country { get; set; }

		public string Address { get; set; }
	}
}