using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Windyship.Api.Services.IdentitySvc.Social;
using Windyship.Identity;
using Windyship.Entities;
using Windyship.Api.Models.Api.v1.UserModels.Social;
using LoginModule.Services.IdentitySvc.Social;


namespace Windyship.Api.Services.IdentitySvc
{
	public sealed class WindySignInManager : SignInManager<WindyUser, int>, IWindySignInManager
	{
		public WindySignInManager(UserManager<WindyUser, int> userManager, IAuthenticationManager authenticationManager)
			: base(userManager, authenticationManager)
		{
		}

		public Task<ExternalLoginInfo> GetExternalLoginInfoAsync()
		{
			return AuthenticationManager.GetExternalLoginInfoAsync();
		}

		public Task<SignInStatus> PasswordSignInAsync(string userName, string password, bool isPersistent)
		{
			return PasswordSignInAsync(userName, password, isPersistent, false);
		}

		public void SignOut()
		{
			AuthenticationManager.SignOut();
		}

		private ISocialUser GetSocialUser(ExternalLoginInfo loginInfo)
		{
			var manager = loginInfo.Login.LoginProvider == ExternalProviderName.Twitter
				? (ISocialManager)new TwitterManager()
				: new FbManager();

			return manager.GetUser(loginInfo);
		}

		public async Task<SignInStatus> ExternalLoginAsync(ExternalLoginInfo loginInfo, bool isPersistent)
		{
			var result = await ExternalSignInAsync(loginInfo, isPersistent);

			if (result != SignInStatus.Failure)
			{
				return result;
			}

			var socialUser = GetSocialUser(loginInfo);

			var existUser = await UserManager.FindByEmailAsync(socialUser.Email);
			if (existUser == null)
			{
				existUser = new WindyUser
							{
								UserName = socialUser.Email,
								EmailConfirmed = true,
								FirstName = socialUser.FirstName,
								SecondName = socialUser.LastName,
								Avatar = socialUser.Avatar,
								AvatarMimeType = "image/jpeg",
								Role = UserRole.User,
								AvatarAddedUtc = DateTime.Now,
								IsActive = true
							};

				SetSocialUserId(loginInfo.Login.LoginProvider, loginInfo.Login.ProviderKey, existUser);
				var identity = await UserManager.CreateAsync(existUser);
				if (identity == null || !identity.Succeeded)
				{
					return SignInStatus.Failure;
				}
			}
			else
			{
				SetSocialUserId(loginInfo.Login.LoginProvider, loginInfo.Login.ProviderKey, existUser);
				await UserManager.UpdateAsync(existUser);
			}

			return await ExternalSignInAsync(loginInfo, isPersistent);
		}

		public async Task<SignInStatus> TwLoginAsync(string code, bool isPersistent)
		{
			return await SocialLogin(code, isPersistent, new TwitterManager());
		}

		public async Task<SignInStatus> FbLoginAsync(string code, bool isPersistent)
		{
			return await SocialLogin(code, isPersistent, new FbManager());
		}

		private async Task<SignInStatus> SocialLogin(string code, bool isPersistent, ISocialManager manager)
		{
			var loginInfo = manager.GetLoginInfo(code);
			return await ExternalLoginAsync(loginInfo, isPersistent);
		}

		private static void SetSocialUserId(string provideName, string userId, IdentityUser user)
		{
			switch (provideName)
			{
				case ExternalProviderName.Facebook:
					user.FacebookId = userId;
					break;
				case ExternalProviderName.Twitter:
					user.TwitterId = userId;
					break;
			}
		}
	}
}