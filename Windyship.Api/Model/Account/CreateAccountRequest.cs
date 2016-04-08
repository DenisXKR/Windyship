using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Windyship.Api.Model.Account
{
	public class CreateAccountRequest
	{
		[Required]
		public string Name { get; set; }

		[Required, EmailAddress]
		public string Email { get; set; }

		[Required]
		public string Mobile { get; set; }
	}
}