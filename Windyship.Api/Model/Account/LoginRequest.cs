using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Windyship.Api.Model.Account
{
	public class LoginRequest
	{
		[Required]
		public string Mobile { get; set; }
	}
}