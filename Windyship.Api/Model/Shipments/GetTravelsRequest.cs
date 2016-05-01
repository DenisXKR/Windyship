using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Windyship.Api.Model.Shipments
{
	public class GetTravelsRequest
	{
		public GetTravelsRequest()
		{
			this.language = "en";
			this.Page = 1;
		}

		public string language { get; set; }

		public int Page { get; set; } // page number 1+
	}
}