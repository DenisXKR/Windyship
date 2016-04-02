using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Windyship.Entities;

namespace Windyship.Api.Models.Api.v1.UserModels
{
	public class UserModel: BaseUserModel
	{
		[MaxLength(DbConsts.UserEmailLenMax)]
		public string Email { get; set; }

		[MaxLength(DbConsts.UserPhoneMax)]
		public string Phone { get; set; }
	}
}