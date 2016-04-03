using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Windyship.Api.Site.Services.IdentitySvc;
using Windyship.Entities;

namespace Windyship.Api.Services.IdentitySvc
{
	public sealed class WindyUserManager : UserManager<WindyUser, int>, IWindyUserManager
	{
		public WindyUserManager(IUserStore<WindyUser, int> store)
			: base(store)
		{

			// Configure validation logic for usernames
			UserValidator = new GarGetUserValidator(this)
							{
								AllowOnlyAlphanumericUserNames = false
							};

			// Configure validation logic for passwords
			PasswordValidator = new PasswordValidator
			{
				RequiredLength = 1,
				RequireNonLetterOrDigit = false,
				RequireDigit = false,
				RequireLowercase = false,
				RequireUppercase = false,
			};

			// Configure user lockout defaults
			UserLockoutEnabledByDefault = false;
			DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
			MaxFailedAccessAttemptsBeforeLockout = 5;

			// Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
			// You can write your own provider and plug it in here.
			//			RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<CarGetUser, int>
			//			{
			//				MessageFormat = "Your security code is {0}"
			//			});
			//RegisterTwoFactorProvider("Email Code", emailTokenProvider);

			//			SmsService = new SmsService();
		}

		public async Task<IdentityResult> ConfirmPhoneAsync(string phone, string token)
		{
			var user = await Store.FindByNameAsync(phone);
			if (user == null)
			{
				return IdentityResult.Failed("Error_UserNotFound");
			}

			if (string.Compare(user.PhoneCode, token) == 0)
			{ 
				user.PhoneCode = null;
				user.PhoneChecked = true;
				user.IsActive = true;
				await Store.UpdateAsync(user);

				return IdentityResult.Success;
			}

			return IdentityResult.Failed();
		}

		public async Task<IdentityResult> SetNewPassword(int userId, string password)
		{
			var user = await Store.FindByIdAsync(userId);
			if (user == null) return IdentityResult.Failed("Error_UserNotFound");

			await RemovePasswordAsync(userId);

			var hash = PasswordHasher.HashPassword(password);
			user.PasswordHash = hash;
			return await UpdateAsync(user);
		}

		public async Task<WindyUser> GetUserById(int userId)
		{
			return await Store.FindByIdAsync(userId);
		}

		public async Task SendCheckPhoneCode(string phone)
		{
			var user = await Store.FindByNameAsync(phone);
			if (user == null) return;

			var smsCode = TokenGenerator.GetUniqueDigits(6);
			var isPhoneChecked = user.PhoneChecked;

			user.PhoneCode = smsCode;
			user.PhoneChecked = false;
			user.IsActive = true;

			await Store.UpdateAsync(user);

#warning SendSMS

			/*
			if (isPhoneChecked)
			{
				var smsText = string.Format("Code: {0}", smsCode);
				var sended = await Sms24x7.Send(phone, smsText);
			}*/
		}

		public async Task<bool> IsAccountConfirmedAsync(string phone)
		{
			var user = await Store.FindByNameAsync(phone);
			if (user == null)
			{
				return false;
			}

			return await base.IsEmailConfirmedAsync(user.Id);
		}
	}
}