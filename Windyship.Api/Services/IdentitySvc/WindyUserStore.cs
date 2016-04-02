using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Configuration;
using Microsoft.AspNet.Identity;

namespace Windyship.Api.Services.IdentitySvc
{
    using System.Linq;
	using Windyship.Repositories;
	using Windyship.Core;

    public class WindyUserStore :
		IUserLoginStore<WindyUser, int>,
		IUserLockoutStore<WindyUser, int>,
		IUserPasswordStore<WindyUser, int>,
		IUserTwoFactorStore<WindyUser, int>,
		IUserEmailStore<WindyUser, int>,
		IUserSecurityStampStore<WindyUser, int>,
		IUserRoleStore<WindyUser, int>
	{
		private readonly IUserRepository _userRepository;
		private readonly IUnitOfWork _unitOfWork;

		public WindyUserStore(IUserRepository userRepository, IUnitOfWork unitOfWork)
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

		public Task CreateAsync(WindyUser user)
		{
			var dbUser = user.ToEntityUser();
			_userRepository.Add( dbUser );
			_unitOfWork.SaveChanges();
			user.Id = dbUser.Id;
			return Task.FromResult( user.Id );
		}

		public async Task UpdateAsync(WindyUser user)
		{
			_userRepository.Update(user.ToEntityUser());
			await _unitOfWork.SaveChangesAsync();
		}

		public Task DeleteAsync(WindyUser user)
		{
			throw new NotImplementedException();
		}

		public Task<WindyUser> FindByIdAsync(int userId)
		{
			return _userRepository.GetIdentityUserByIdAsync<WindyUser>(userId);
		}

		public Task<WindyUser> FindByNameAsync(string userName)
		{
			return _userRepository.GetIdentityUserByNameAsync<WindyUser>(userName);
		}

		#region IUserLockoutStore

		public Task<DateTimeOffset> GetLockoutEndDateAsync(WindyUser user)
		{
			throw new NotImplementedException();
		}

		public Task SetLockoutEndDateAsync(WindyUser user, DateTimeOffset lockoutEnd)
		{
			throw new NotImplementedException();
		}

		public Task<int> IncrementAccessFailedCountAsync(WindyUser user)
		{
			throw new NotImplementedException();
		}

		public Task ResetAccessFailedCountAsync(WindyUser user)
		{
			throw new NotImplementedException();
		}

		public Task<int> GetAccessFailedCountAsync(WindyUser user)
		{
			throw new NotImplementedException();
		}

		public Task<bool> GetLockoutEnabledAsync(WindyUser user)
		{
			return Task.FromResult(false);
		}

		public Task SetLockoutEnabledAsync(WindyUser user, bool enabled)
		{
			throw new NotImplementedException();
		}

		#endregion IUserLockoutStore

		#region IUserPasswordStore

		public Task SetPasswordHashAsync(WindyUser user, string passwordHash)
		{
			user.PasswordHash = passwordHash;
			return Task.FromResult(0);
		}

		public Task<string> GetPasswordHashAsync(WindyUser user)
		{
			return Task.FromResult(user.PasswordHash);
		}

		/// <summary>
		///     Returns true if the user has a password
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		public Task<bool> HasPasswordAsync(WindyUser user)
		{
			return Task.FromResult(user.PasswordHash != null);
		}

		#endregion IUserPasswordStore

		#region IUserTwoFactorStore

		/// <summary>
		/// Sets whether two factor authentication is enabled for the user
		/// </summary>
		/// <param name="user"/><param name="enabled"/>
		/// <returns/>
		public Task SetTwoFactorEnabledAsync(WindyUser user, bool enabled)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Returns whether two factor authentication is enabled for the user
		/// </summary>
		/// <param name="user"/>
		/// <returns/>
		public Task<bool> GetTwoFactorEnabledAsync(WindyUser user)
		{
			return Task.FromResult(false);
		}

		#endregion

		#region IUserLoginStore

		/// <summary>
		/// Adds a user login with the specified provider and key
		/// </summary>
		/// <param name="user"/><param name="login"/>
		/// <returns/>
		public Task AddLoginAsync(WindyUser user, UserLoginInfo login)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Removes the user login with the specified combination if it exists
		/// </summary>
		/// <param name="user"/><param name="login"/>
		/// <returns/>
		public Task RemoveLoginAsync(WindyUser user, UserLoginInfo login)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Returns the linked accounts for this user
		/// </summary>
		/// <param name="user"/>
		/// <returns/>
		public Task<IList<UserLoginInfo>> GetLoginsAsync(WindyUser user)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Returns the user associated with this login
		/// </summary>
		/// <returns/>
		public async Task<WindyUser> FindAsync(UserLoginInfo login)
		{
			return await _userRepository.GetIdentityUserByProviderAsync<WindyUser>( login.LoginProvider, login.ProviderKey );
		}

		#endregion IUserLoginStore

		public void Dispose()
		{
		}

		/// <summary>
		/// Set the user email
		/// </summary>
		/// <param name="user"/><param name="email"/>
		/// <returns/>
		public Task SetEmailAsync(WindyUser user, string email)
		{
			user.UserName = email;
			return Task.FromResult(0);
		}

		/// <summary>
		/// Get the user email
		/// </summary>
		/// <param name="user"/>
		/// <returns/>
		public Task<string> GetEmailAsync(WindyUser user)
		{
			return Task.FromResult(user.UserName);
		}

		/// <summary>
		/// Returns true if the user email is confirmed
		/// </summary>
		/// <param name="user"/>
		/// <returns/>
		public Task<bool> GetEmailConfirmedAsync(WindyUser user)
		{
			return Task.FromResult(user.EmailConfirmed);
		}

		/// <summary>
		/// Sets whether the user email is confirmed
		/// </summary>
		/// <param name="user"/><param name="confirmed"/>
		/// <returns/>
		public Task SetEmailConfirmedAsync(WindyUser user, bool confirmed)
		{
			user.EmailConfirmed = confirmed;
			return Task.FromResult(0);
		}

		/// <summary>
		/// Returns the user associated with this email
		/// </summary>
		/// <param name="email"/>
		/// <returns/>
		public Task<WindyUser> FindByEmailAsync(string email)
		{
			return _userRepository.GetIdentityUserByNameAsync<WindyUser>(email);
		}

		/// <summary>
		/// Set the security stamp for the user
		/// </summary>
		/// <param name="user"/><param name="stamp"/>
		/// <returns/>
		public Task SetSecurityStampAsync(WindyUser user, string stamp)
		{
			user.SecurityStamp = stamp;
			return Task.FromResult(0);
		}

		/// <summary>
		/// Get the user security stamp
		/// </summary>
		/// <param name="user"/>
		/// <returns/>
		public Task<string> GetSecurityStampAsync(WindyUser user)
		{
			return Task.FromResult(user.SecurityStamp);
		}

	    public Task AddToRoleAsync(WindyUser user, string roleName)
	    {
	        throw new NotSupportedException("Включение пользователя в роль не доступно.");
	    }

	    public Task RemoveFromRoleAsync(WindyUser user, string roleName)
	    {
            throw new NotSupportedException("Исключение пользователя из роли не доступно.");
	    }

	    public Task<IList<string>> GetRolesAsync(WindyUser user)
	    {
            IList<string> roles = new List<string> { user.Role.ToString() };
	        return Task.FromResult(roles);
	    }

	    public Task<bool> IsInRoleAsync(WindyUser user, string roleName)
	    {
	        var isInRole = user.Role.ToString().Equals(roleName, StringComparison.InvariantCultureIgnoreCase);
	        return Task.FromResult(isInRole);
	    }
	}
}