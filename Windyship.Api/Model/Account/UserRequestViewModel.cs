using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Windyship.Api.Model.Account
{
	public class UserRequestViewModel
	{
		public string Name { get; set; }
		public string Email { get; set; }
		public string Message { get; set; }
	}
}