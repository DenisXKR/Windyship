using System;
using System.Linq.Expressions;
using Windyship.Entities;

namespace Windyship.Identity
{
	public class IdentityUser
	{
		public int Id { get; set; }
		public string UserName { get; set; }
		public string Email { get; set; }
		public string PasswordHash { get; set; }
		public string SecurityStamp { get; set; }
		public string EmailCode { get; set; }
		public bool EmailConfirmed { get; set; }
		public string FirstName { get; set; }
		public string SecondName { get; set; }
		public string MiddleName { get; set; }
		public UserRole Role { get; set; }
		public string TwitterId { get; set; }
		public string FacebookId { get; set; }
        public bool PhoneChecked { get; set; }
		public string PhoneCode { get; set; }
		public DateTime? CodeLastSentTime { get; set; }
        public byte[] Avatar { get; set; }
        public string AvatarMimeType { get; set; }
        public bool IsActive { get; set; }
		public DateTime? AvatarAddedUtc { get; set; }

		public User ToEntityUser()
		{
			return new User
			{
				Id = Id,
				Email = Email,
				PasswordHash = PasswordHash,
				SecurityStamp = SecurityStamp,
				EmailCode = EmailCode,
				EmailChecked = EmailConfirmed,
				FirstName = FirstName,
				SecondName = SecondName,
				MiddleName = MiddleName,
				Role = Role,
				FacebookId = FacebookId,
				TwitterId = TwitterId,
                Phone = UserName,
                PhoneChecked = PhoneChecked,
				PhoneCode = PhoneCode,
				CodeLastSentTime = CodeLastSentTime,
                Avatar = Avatar,
                AvatarMimeType = AvatarMimeType,
                IsActive = IsActive,
				AvatarAddedUtc = AvatarAddedUtc
			};
		}

		public static Expression<Func<User, T>> FromEntityUser<T>() where T : IdentityUser, new()
		{
			return user => new T
			{
				Id = user.Id,
				Email = user.Email,
				PasswordHash = user.PasswordHash,
				SecurityStamp = user.SecurityStamp,
				EmailCode = user.EmailCode,
				EmailConfirmed = user.EmailChecked,
				FirstName = user.FirstName,
				SecondName = user.SecondName,
				MiddleName = user.MiddleName,
				Role = user.Role,
				FacebookId = user.FacebookId,
				TwitterId = user.TwitterId,
                UserName = user.Phone,
                PhoneChecked = user.PhoneChecked,
				PhoneCode = user.PhoneCode,
				CodeLastSentTime = user.CodeLastSentTime,
                Avatar = user.Avatar,
                AvatarMimeType = user.AvatarMimeType,
                IsActive = user.IsActive,
				AvatarAddedUtc = user.AvatarAddedUtc
			};
		}
	}
}