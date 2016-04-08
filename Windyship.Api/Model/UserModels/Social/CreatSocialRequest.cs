using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Windyship.Api.Model.UserModels.Social
{
	public class CreatSocialRequest
	{
		/// <summary>
		/// facebook or twitter
		/// </summary>
		public string Type { get; set; }

		public string User_name { get; set; }

		public string Avatar { get; set; }

		public string Email { get; set; }

		[Required]
		public string Mobile { get; set; }

		public string Social_id { get; set; }

		/// <summary>
		///  this access token from FB /twitter. It’s not unique - this code auto generate every time user make login
		/// </summary>
		public string Access_token { get; set; }
	}
}