using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Windyship.Api.Model.Common;

namespace Windyship.Api.Model.Shipments
{
	public class CarryTravelViewModel : CarryTravelRequest
	{
		public SmallUserViewModel Carrier { get; set; }

		public IEnumerable<string> not_allowed_categories_str { get; set; }
	}
}