using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Windyship.Api.Model.Account;
using Windyship.Api.Model.UserModels.Social;
using Windyship.Api.Models.Api.v1.UserModels.Social;
using Windyship.Api.Services.IdentitySvc;
using Windyship.Api.Site.Services.IdentitySvc;
using Windyship.Core;
using Windyship.Entities;
using Windyship.Repositories;

namespace Windyship.Api.Controllers
{
	[RoutePrefix("api"), Authorize]
	public class AccountController : BaseApiController
	{
		private IWindySignInManager _signInManager;
		private IWindyUserManager _userManager;
		private IUserRepository _userRepository;
		private IUnitOfWork _unitOfWork;

		public AccountController(IWindySignInManager signInManager, IWindyUserManager userManager, IUserRepository userRepository, IUnitOfWork unitOfWork)
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
			if (unitOfWork == null)
			{
				throw new ArgumentNullException("unitOfWork");
			}

			_signInManager = signInManager;
			_userManager = userManager;
			_userRepository = userRepository;
			_unitOfWork = unitOfWork;
		}

		[Route("createAccount")]
		[AllowAnonymous]
		[HttpPost]
		public async Task<IHttpActionResult> CreateAccount(CreateAccountRequest model)
		{
			WindyUser user = await _userManager.FindByNameAsync(model.Mobile.Trim());

			if (user == null)
			{
				user = new WindyUser
				{
					Role = UserRole.User,
					UserName = model.Mobile.Trim(),
					Email = model.Email,
					FirstName = model.Name,
					IsActive = false,
				};

				var result = await _userManager.CreateAsync(user, "*");
				if (result.Succeeded)
				{
					await _userManager.SendCheckPhoneCode(user.UserName);
					return ApiResult(true, user.SecurityStamp);
				}

				else ApiResult(false, result.Errors);
			}
			else if (!user.IsActive)
			{
				user.Role = UserRole.User;
				user.UserName = model.Mobile.Trim();
				user.Email = model.Email;
				user.FirstName = model.Name;
				user.IsActive = false;

				await _userManager.UpdateUser(user);
				await _userManager.SendCheckPhoneCode(user.UserName);
				return ApiResult(true, user.SecurityStamp);
			}

			return ApiResult(false);
		}

		private async Task GetAvatar(WindyUser user, string fieldName)
		{
			if (!Request.Content.IsMimeMultipartContent())
			{
				var provider = new MultipartMemoryStreamProvider();
				await Request.Content.ReadAsMultipartAsync(provider);

				foreach (var file in provider.Contents)
				{
					if (string.Compare(file.Headers.ContentDisposition.Name, fieldName, true) == 0) continue;
					var data = await file.ReadAsByteArrayAsync();
					user.Avatar = data;
					user.AvatarAddedUtc = DateTime.Now;
				}
			}
		}

		[Route("login")]
		[AllowAnonymous]
		[HttpPost]
		public async Task<IHttpActionResult> Login(LoginRequest model)
		{
			var user = await _userManager.FindByNameAsync(model.Mobile);

			if (user == null)
			{
				return ApiResult(false);
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
					mobile = user.Phone,
					image = user.Avatar == null ? null : string.Format("/Image/Avatar?id={0}", user.Id),
					access_token = user.TwitterId ?? user.FacebookId
				});
			}

			return ApiResult(false);
		}

		[HttpPost]
		[AllowAnonymous]
		[Route("creatBySocial")]
		public async Task<IHttpActionResult> CreatBySocial(CreatSocialRequest request)
		{
			var user = await _userManager.FindByNameAsync(request.Mobile);

			if (user == null)
			{
				user = new WindyUser
				{
					Role = UserRole.User,
					UserName = request.Mobile.Trim(),
					Email = request.Email,
					FirstName = request.User_name,
					IsActive = false,
					TwitterId = request.Type == "twitter" ? request.Social_id : null,
					FacebookId = request.Type == "facebook" ? request.Social_id : null
				};

				var idResult = await _userManager.CreateAsync(user, "*");
				if (!idResult.Succeeded) return ApiResult(false, idResult.Errors);
			}
			else
			{
				user.Email = request.Email;
				user.FirstName = request.User_name;
				user.TwitterId = request.Type == "twitter" ? request.Social_id : null;
				user.FacebookId = request.Type == "facebook" ? request.Social_id : null;

				await _userManager.UpdateUser(user);
				return ApiResult(true);
			}

			await _userManager.SendCheckPhoneCode(request.Mobile);
			return ApiResult(true, user.SecurityStamp);
		}

		[HttpPost, AllowAnonymous]
		[Route("UploadAvatar")]
		public async Task<IHttpActionResult> UploadAvatar([FromUri] string token)
		{
			var user = await _userRepository.GetFirstOrDefaultAsync(u => u.SecurityStamp == token);

			if (user == null) return ApiResult(false);

			if (Request.Content.IsMimeMultipartContent())
			{
				var provider = new MultipartMemoryStreamProvider();
				await Request.Content.ReadAsMultipartAsync(provider);

				foreach (var file in provider.Contents)
				{
					if (string.Compare(file.Headers.ContentDisposition.Name, "avatar", true) == 0) continue;
					var data = await file.ReadAsByteArrayAsync();
					user.Avatar = data;
					user.AvatarAddedUtc = DateTime.Now;
				}

				await _unitOfWork.SaveChangesAsync();
				return ApiResult(true);
			}

			return ApiResult(false);
		}

		[HttpPost]
		[Route("UploadAvatar")]
		public async Task<IHttpActionResult> UploadAvatar()
		{
			var id = User.Identity.GetUserId<int>();
			var user = await _userRepository.GetFirstOrDefaultAsync(u => u.Id == id);

			if (user == null) return ApiResult(false);

			if (Request.Content.IsMimeMultipartContent())
			{
				var provider = new MultipartMemoryStreamProvider();
				await Request.Content.ReadAsMultipartAsync(provider);

				foreach (var file in provider.Contents)
				{
					if (string.Compare(file.Headers.ContentDisposition.Name, "avatar", true) == 0) continue;
					var data = await file.ReadAsByteArrayAsync();
					user.Avatar = data;
					user.AvatarAddedUtc = DateTime.Now;
				}

				await _unitOfWork.SaveChangesAsync();
				return ApiResult(true);
			}

			return ApiResult(false);
		}

		[HttpPost]
		[AllowAnonymous]
		[Route("FbLoginCallback")]
		public async Task<IHttpActionResult> FblLoginCallback(SocialTokenRequest request)
		{
			SignInStatus result = result = await _signInManager.FbLoginAsync(request.social_id, true);

			if (result == SignInStatus.Success)
			{
				var userId = User.Identity.GetUserId<int>();
				var user = await _userManager.GetUserById(userId);

				return ApiResult(true);
			}

			return ApiResult(false);
		}

		[HttpPost]
		[AllowAnonymous]
		[Route("TwLoginCallback")]
		public async Task<IHttpActionResult> TwlLoginCallback(SocialTokenRequest request)
		{
			SignInStatus result = await _signInManager.TwLoginAsync(request.social_id, true);

			if (result == SignInStatus.Success)
			{
				var userId = User.Identity.GetUserId<int>();
				var user = await _userManager.GetUserById(userId);

				return ApiResult(true);
			}

			return ApiResult(false);
		}

		[Route("TestMethod")]
		[HttpGet]
		public IHttpActionResult Logout()
		{
			var userId = User.Identity.GetUserId<int>();
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
				return ApiResult(false);
			}

			if (DateTime.Now - user.CodeLastSentTime < TimeSpan.FromMinutes(1)) return ApiResult(false);

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

				return ApiResult(true, new
				{
					user_id = user.Id,
					email = user.Email,
					user_name = user.FirstName,
					mobile = user.UserName,
					image = user.Avatar == null ? null : string.Format("/Image/Avatar?id={0}", user.Id),
					access_token = user.Token
				});
			}

			return ApiResult(false);
		}

		[Route("addMobile"), HttpPost]
		public async Task<IHttpActionResult> AddMobile(string mobile)
		{
			try
			{
				var id = User.Identity.GetUserId<int>();
				var user = await _userRepository.GetFirstOrDefaultAsync(u => u.Id == id);
				user.Phone = mobile;
				await _unitOfWork.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				return ApiResult(false, ex.Message);
			}

			return ApiResult(true);
		}

		[Route("updateProfile"), HttpPost]
		public async Task<IHttpActionResult> UpdateProfile(string username)
		{
			try
			{
				var id = User.Identity.GetUserId<int>();
				var user = await _userRepository.GetFirstOrDefaultAsync(u => u.Id == id);
				user.FirstName = username;
				await _unitOfWork.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				return ApiResult(false, ex.Message);
			}

			return ApiResult(true);
		}

		#region System

		[Route("getusers")]
		[AllowAnonymous]
		[HttpGet]
		public IHttpActionResult GetUsers()
		{
			var result = _userRepository.All();
			return ApiResult(true, result);
		}

		[Route("DelAllUsers")]
		[AllowAnonymous]
		[HttpGet]
		public async Task<IHttpActionResult> DelAllUsers()
		{
			_userRepository.RemoveRange(u => true);
			await _unitOfWork.SaveChangesAsync();
			return ApiResult(true);
		}

		#endregion
	}
}
