using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Windyship.Api.Model.Account
{
	public class ConfirmPhoneRequest
	{
		[Required]
		public string Mobile { get; set; }

		[Required]
		public string Code { get; set; }
	}
}