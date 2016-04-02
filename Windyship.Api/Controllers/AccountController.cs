using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Windyship.Api.Services.IdentitySvc;
using Windyship.Api.Site.Services.IdentitySvc;
using Windyship.Api.Model.Account;
using Windyship.Entities;
using Windyship.Api.Models.Api.v1.UserModels.Social;
using Microsoft.AspNet.Identity.Owin;
using Windyship.Repositories;

namespace Windyship.Api.Controllers
{
	[RoutePrefix("api"), Authorize]
	public class AccountController : BaseApiController
	{
		private IWindySignInManager _signInManager;
		private IWindyUserManager _userManager;
		private IUserRepository _userRepository;

		public AccountController(IWindySignInManager signInManager, IWindyUserManager userManager, IUserRepository userRepository)
		{
			if (signInManager == null)
			{
				throw new ArgumentNullException("signInManager");
			}
			if (userManager == null)
			{
				throw new ArgumentNullException("userManager");
			}
			if (userRepository == null)
			{
				throw new ArgumentNullException("userRepository");
			}

			_signInManager = signInManager;
			_userManager = userManager;
			_userRepository = userRepository;
		}

		[Route("createAccount")]
		[AllowAnonymous]
		[HttpPost]
		public async Task<IHttpActionResult> CreateAccount(CreateAccountRequest model)
		{
			if (User.Identity.IsAuthenticated)
			{
				var userId = User.Identity.GetUserId<int>();

				var user = await _userManager.GetUserById(userId);
				if (string.Compare(model.Email, user.UserName) == 0) return ApiResult(false);//Not changed
			}
			else
			{
				var user = await _userManager.FindByNameAsync(model.Mobile);

				if (user == null)
				{
					user = new WindyUser 
					{ 
						Role = UserRole.User,
						UserName = model.Mobile,
						Email = model.Email,
						FirstName = model.Name
					};

					var result = await _userManager.CreateAsync(user, "*");
					if (result.Succeeded)
					{
						await _userManager.SendCheckPhoneCode(user.UserName);
						return ApiResult(true);
					}
				}
			}

			return ApiResult(false);
		}

		[Route("login")]
		[AllowAnonymous]
		[HttpPost]
		public async Task<IHttpActionResult> Login(LoginRequest model)
		{
			var user = _userManager.FindByNameAsync(model.Mobile);

			if (user == null)
			{
				return Ok(ApiResult(false));
			}

			await _userManager.SendCheckPhoneCode(model.Mobile);
			return ApiResult(true);
		}

		[HttpPost]
		[AllowAnonymous]
		[Route("checkSocial")]
		public async Task<IHttpActionResult> CheckSocial(SocialTokenRequest request)
		{
			var user = await _userRepository.GetFirstOrDefaultAsync(u => u.FacebookId == request.social_id || u.TwitterId == request.social_id);

			if (user != null)
			{
				return ApiResult(true, new 
				{
					user_id = user.Id,
					email = user.Email,
					user_name = user.FirstName,
					mobile = user.Email,
					image = "",
 					access_token = user.FacebookId ?? user.TwitterId
				});
			}

			return ApiResult(false);
		}

		[HttpPost]
		[AllowAnonymous]
		[Route("creatBySocial")]
		public async Task<IHttpActionResult> SocialLoginCallback(SocialTokenRequest request)
		{
			SignInStatus result = SignInStatus.Failure;

			switch (request.type)
			{
				case "facebook": result = await _signInManager.FbLoginAsync(request.social_id, true); break;
				case "twitter": result = await _signInManager.TwLoginAsync(request.social_id, true); break;
			}

			if (result == SignInStatus.Success)
			{

				var userId = User.Identity.GetUserId<int>();
				var user = await _userManager.GetUserById(userId);

				return ApiResult(true);
			}

			return ApiResult(false);
		}

		[Route("logout")]
		[HttpPost]
		public IHttpActionResult Logout()
		{
			_signInManager.SignOut();
			return ApiResult(true);
		}

		[Route("resendVerificationCode")]
		[HttpPost, AllowAnonymous]
		public async Task<IHttpActionResult> SendVerificationCode(LoginRequest model)
		{
			var user = await _userManager.FindByNameAsync(model.Mobile);

			if (user == null)
			{
				return Ok(ApiResult(false));
			}

			await _userManager.SendCheckPhoneCode(model.Mobile);
			return ApiResult(true);
		}

		[Route("checkVerificationCode")]
		[AllowAnonymous]
		[HttpPost]
		public async Task<IHttpActionResult> CheckVerificationCode(ConfirmPhoneRequest model)
		{
			var result = await _userManager.ConfirmPhoneAsync(model.Mobile, model.Code);

			if (result.Succeeded)
			{
				var user = await _userManager.FindByNameAsync(model.Mobile);
				await _signInManager.SignInAsync(user, true, true);

				return ApiResult(true, new
				{
					user_id = user.Id,
					email = user.Email,
					user_name = user.FirstName,
					mobile = user.Email,
					image = "",
					access_token = user.FacebookId ?? user.TwitterId
				});
			}

			return ApiResult(false);
		}
    }
}
