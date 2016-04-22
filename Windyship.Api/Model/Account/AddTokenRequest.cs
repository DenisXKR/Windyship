using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Windyship.Api.Model.Account
{
	public class AddTokenRequest
	{
		public string device_token {get; set;}

		public int device_type { get; set; } // ios or android
	}
}