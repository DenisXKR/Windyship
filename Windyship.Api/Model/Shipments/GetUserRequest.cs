using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Windyship.Api.Model.Shipments
{
	public class GetUserRequest
	{
		public GetUserRequest()
		{
			this.language = "en";
		}

		public string language { get; set; }

		public int User_id { get; set; }
	}
}