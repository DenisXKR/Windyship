using System;
using Newtonsoft.Json;

namespace Windyship.Api.Models.Api.v1.UserModels
{
	public class BaseUserModel
	{
		public int Id { get; set; }

		public string FirstName { get; set; }

		public string SecondName { get; set; }

		public string MiddleName { get; set; }

		public string AvatarUrl { get; set; }

		[JsonIgnore]
		public DateTime? AvatarAddedUtc { get; set; }

		public string FullName
		{
			get
			{
				return string.Format( "{1} {0} {2}", FirstName, SecondName, MiddleName ).Trim();
			}
		}
	}
}