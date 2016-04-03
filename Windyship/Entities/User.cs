using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Windyship.Entities
{
	public class User : IEntity<int>
	{
		[Key]
		public int Id { get; set; }

		[Required, MaxLength(DbConsts.UserNameFirstMaxLen)]
		public string FirstName { get; set; }

		[MaxLength(DbConsts.UserNameSecondMaxLen)]
		public string SecondName { get; set; }

		[MaxLength(DbConsts.UserNameMiddleMaxLen)]
		public string MiddleName { get; set; }

		[NotMapped]
		public string FullName
		{
			get
			{
				return string.Format("{0} {1} {2}", FirstName, SecondName, MiddleName).Trim();
			}
		}

		[MaxLength(100)]
		public string FacebookId { get; set; }

		[MaxLength(100)]
		public string TwitterId { get; set; }

		[MaxLength(DbConsts.UserEmailLenMax)]
		public string Email { get; set; }

		[Required]
		public bool EmailChecked { get; set; }

		[JsonIgnore]
		public string PasswordHash { get; set; }

		[JsonIgnore]
		public string SecurityStamp { get; set; }

		[JsonIgnore]
		public byte[] Avatar { get; set; }

		[JsonIgnore]
		public string AvatarMimeType { get; set; }

		[JsonIgnore]
		public DateTime? AvatarAddedUtc { get; set; }

		[MaxLength(DbConsts.UserPhoneMax)]
		public string Phone { get; set; }

		public bool PhoneChecked { get; set; }

		public DateTime? CodeLastSentTime { get; set; }

		[JsonIgnore]
		public UserRole Role { get; set; }

		[JsonIgnore]
		[MaxLength(10)]
		public string PhoneCode { get; set; }

		[JsonIgnore]
		[MaxLength(10)]
		public string EmailCode { get; set; }

		public bool IsActive { get; set; }
	}
}