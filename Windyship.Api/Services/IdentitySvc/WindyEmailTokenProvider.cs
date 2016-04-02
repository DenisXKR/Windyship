using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Windyship.Repositories;
using Windyship.Core;

namespace Windyship.Api.Services.IdentitySvc
{
	public sealed class WindyEmailTokenProvider : IUserTokenProvider<WindyUser, int>
	{
		private readonly IUserRepository _userRepository;
		private readonly IUnitOfWork _unitOfWork;

		public WindyEmailTokenProvider(IUserRepository userRepository, IUnitOfWork unitOfWork)
		{
			if (userRepository == null)
			{
				throw new ArgumentNullException("userRepository");
			}

			if (unitOfWork == null)
			{
				throw new ArgumentNullException( "unitOfWork" );
			}

			_userRepository = userRepository;
			_unitOfWork = unitOfWork;
		}

		/// <summary>
		/// Generate a token for a user with a specific purpose
		/// </summary>
		/// <param name="purpose"/><param name="manager"/><param name="user"/>
		/// <returns/>
		public async Task<string> GenerateAsync(string purpose, UserManager<WindyUser, int> manager, WindyUser user)
		{
			var token = TokenGenerator.GetUnique(6);

			user.EmailCode = token;
            user.EmailConfirmed = false;
			_userRepository.Update(user.ToEntityUser());
			await _unitOfWork.SaveChangesAsync();

			return token;
		}

		/// <summary>
		/// Validate a token for a user with a specific purpose
		/// </summary>
		/// <param name="purpose"/><param name="token"/><param name="manager"/><param name="user"/>
		/// <returns/>
		public async Task<bool> ValidateAsync(string purpose, string token, UserManager<WindyUser, int> manager, WindyUser user)
		{
			if (string.IsNullOrEmpty(user.EmailCode))
			{
				return false;
			}

			if (purpose == "ResetPassword")
			{
				// при сбросе пароля генерируется сразу новый пароль, а не токен
				return true;
			}

			var result = await _userRepository.AnyAsync(u => u.Id == user.Id && u.EmailCode == token);
            user.IsActive = result;

            await manager.UpdateAsync( user );

            return result;
		}

		/// <summary>
		/// Notifies the user that a token has been generated, for example an email or sms could be sent, or 
		///                 this can be a no-op
		/// </summary>
		/// <param name="token"/><param name="manager"/><param name="user"/>
		/// <returns/>
		public Task NotifyAsync(string token, UserManager<WindyUser, int> manager, WindyUser user)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Returns true if provider can be used for this user, i.e. could require a user to have an email
		/// </summary>
		/// <param name="manager"/><param name="user"/>
		/// <returns/>
		public Task<bool> IsValidProviderForUserAsync(UserManager<WindyUser, int> manager, WindyUser user)
		{
			return Task.FromResult(true);
		}
	}
}