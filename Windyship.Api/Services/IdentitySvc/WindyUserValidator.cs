using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Windyship.Api.Services.IdentitySvc;

namespace Windyship.Api.Site.Services.IdentitySvc
{
	public class GarGetUserValidator : UserValidator<WindyUser, int>
	{
		private readonly UserManager<WindyUser, int> _manager;

		private const string EmailPattern =
			@"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";

		public GarGetUserValidator(UserManager<WindyUser, int> manager)
			: base(manager)
		{
			_manager = manager;
		}


		public override async Task<IdentityResult> ValidateAsync(WindyUser item)
		{
			var errors = new List<string>();

			if (_manager != null)
			{
				if (string.IsNullOrEmpty(item.Email) || !Regex.IsMatch(item.Email, EmailPattern, RegexOptions.IgnoreCase))
				{
					if (!(string.IsNullOrEmpty(item.FacebookId) || string.IsNullOrEmpty(item.TwitterId)))
					{
						errors.Add("Error_InvalidEmail");
					}
				}
				else
				{
					var user = await _manager.FindByNameAsync(item.UserName);

					if (user != null && user.Id != item.Id && user.EmailConfirmed)
					{
						errors.Add(string.Format("Error_EmailAlreadyInUse", item.UserName));
					}
					else
					{
						var identityResult = await base.ValidateAsync(item);
						return identityResult;
					}
				}
			}

			return errors.Any()
				? IdentityResult.Failed(errors.ToArray())
				: IdentityResult.Success;
		}
	}
}