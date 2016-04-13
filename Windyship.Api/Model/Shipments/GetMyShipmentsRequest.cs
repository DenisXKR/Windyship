using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Windyship.Api.Model.Shipments
{
	public class GetMyShipmentsRequest
	{
		public GetMyShipmentsRequest()
		{
			this.language = "en";
		}

		public string language { get; set; }

		public string View_as { get; set; } //  carrier/sender

		public string Type { get; set; } // interested (carrier only) / in-progress / history / requested (sender only)

		public int Page { get; set; } // page number 1+
	}
}