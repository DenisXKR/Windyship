using LoginModule;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Threading.Tasks;
using Windyship.Api.Site.Services.IdentitySvc;

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

			var dataProtectorProvider = OwinStart.DataProtectionProvider;
			var dataProtector = dataProtectorProvider.Create("Windyship Identity");
			this.UserTokenProvider = new DataProtectorTokenProvider<WindyUser, int>(dataProtector)
			{
				TokenLifespan = TimeSpan.FromDays(365),
			};
		}

		public async Task<IdentityResult> ConfirmPhoneAsync(string phone, string token)
		{
			var user = await Store.FindByNameAsync(phone);
			if (user == null)
			{
				return IdentityResult.Failed("Error_UserNotFound");
			}

			if (token != null && string.Compare(user.PhoneCode, token) == 0)
			{
				user.PhoneCode = null;
				user.PhoneChecked = true;
				user.IsActive = true;
				user.CodeLastSentTime = null;
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
			user.IsActive = false;
			user.CodeLastSentTime = DateTime.Now;

			await Store.UpdateAsync(user);

			var smsText = string.Format("Code: {0}", smsCode);
			WindySmsService.SendMessage(smsText, phone);
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

		public async Task UpdateUser(WindyUser user)
		{
			await Store.UpdateAsync(user);
		}

		public override async Task<IdentityResult> CreateAsync(WindyUser user)
		{
			var result = await base.CreateAsync(user);

			if (result.Succeeded)
			{
				user.Token = await this.GenerateUserTokenAsync("login", user.Id);
				await Store.UpdateAsync(user);
			}

			return result;
		}
	}
}