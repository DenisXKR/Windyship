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
using Windyship.Api.Model.Shipments;
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
		private IUserPhoneRepository _userPhoneRepository;
		private IDeviceTokenRepository _deviceTokenRepository;

		public AccountController(IWindySignInManager signInManager, IWindyUserManager userManager, IUserRepository userRepository,
			IUnitOfWork unitOfWork, IUserPhoneRepository userPhoneRepository, IDeviceTokenRepository deviceTokenRepository)
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
			_userPhoneRepository = userPhoneRepository;
			_deviceTokenRepository = deviceTokenRepository;
		}

		[Route("createAccount")]
		[AllowAnonymous]
		[HttpPost]
		public async Task<IHttpActionResult> CreateAccount([FromBody] CreateAccountRequest model)
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
					image = user.Avatar == null ? null : string.Format("Image/Avatar?id={0}", user.Id),
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
				if (!idResult.Succeeded)
				{
					return ApiResult(false, idResult.Errors);
				}
			}
			else
			{
				/*
				user.Email = request.Email;
				user.FirstName = request.User_name;*/
				user.TwitterId = request.Type == "twitter" ? request.Social_id : user.TwitterId;
				user.FacebookId = request.Type == "facebook" ? request.Social_id : user.FacebookId;

				await _userManager.UpdateUser(user);
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
					image = user.Avatar == null ? null : string.Format("Image/Avatar?id={0}", user.Id),
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
		public async Task<IHttpActionResult> UpdateProfile([FromBody]string username)
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

		[Route("addNewMobile"), HttpPost]
		public async Task<IHttpActionResult> AddNewMobile(UserMobileRequest request)
		{
			var id = User.Identity.GetUserId<int>();

			var exisits = await _userPhoneRepository.GetFirstOrDefaultAsync(m => m.UserId == id && m.Phone == request.Mobile);

			if (exisits != null) return ApiResult(false);

			_userPhoneRepository.Add(new UserPhone { Phone = request.Mobile, UserId = id });
			await _unitOfWork.SaveChangesAsync();

			return ApiResult(true);
		}

		[Route("delMobile"), HttpPost]
		public async Task<IHttpActionResult> DelMobile(UserMobileRequest request)
		{
			var id = User.Identity.GetUserId<int>();

			_userPhoneRepository.RemoveRange(m => m.UserId == id && m.Phone == request.Mobile);
			await _unitOfWork.SaveChangesAsync();

			return ApiResult(true);
		}

		[Route("getMobile"), HttpGet]
		public async Task<IHttpActionResult> GetMobiles()
		{
			var id = User.Identity.GetUserId<int>();
			var result = await _userPhoneRepository.GetAllAsync(m => m.UserId == id);

			return ApiResult(true, result.Select(m => new { Mobile = m.Phone }));
		}

		[Route("addToken"), HttpPost]
		public async Task<IHttpActionResult> AddToken(AddTokenRequest request)
		{
			var id = User.Identity.GetUserId<int>();

			var result = await _deviceTokenRepository.GetFirstOrDefaultAsync(t => t.UserId == id && (int)t.DeviceType == request.device_type);

			if (result == null)
			{
				var dt = new DeviceToken
				{
					DeviceType = (DeviceType)request.device_type,
					UserId = id,
					Token = request.device_token
				};

				_deviceTokenRepository.Add(dt);
			}
			else
			{
				result.Token = request.device_token;
			}

			await _unitOfWork.SaveChangesAsync();
			return ApiResult(true);
		}

		[Route("logout"), HttpPost]
		public async Task<IHttpActionResult> Logout(AddTokenRequest request)
		{
			var id = User.Identity.GetUserId<int>();
			_deviceTokenRepository.RemoveRange(d => d.UserId == id && d.Token == request.device_token);
			await _unitOfWork.SaveChangesAsync();

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
			_userRepository.RemoveRange(u => u.Email != "denis_krs@mail.ru");
			await _unitOfWork.SaveChangesAsync();
			return ApiResult(true);
		}

		#endregion
	}
}
