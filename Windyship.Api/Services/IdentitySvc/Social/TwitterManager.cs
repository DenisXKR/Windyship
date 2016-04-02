using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using Windyship.Api.Models.Api.v1.UserModels.Social;
using Windyship.Api.Services.IdentitySvc.Social;
using Windyship.Identity;

namespace LoginModule.Services.IdentitySvc.Social
{
	public class TwitterManager : ISocialManager
	{
		public ExternalLoginInfo GetLoginInfo(string code)
		{
			if (string.IsNullOrEmpty(code))
			{
				return null;
			}

			var client = new RestClient();
			var request = new RestRequest(SocialUrls.TwitterGetUserInfoUrl, Method.GET);

			var parameters = new Dictionary<string, object>
						 {
							 {"access_token", code},
							 {"fields", "id,email,first_name,last_name"}
						 };

			foreach (var parameter in parameters)
			{
				request.AddParameter(parameter.Key, parameter.Value);
			}

			var response = client.Execute(request);

			var user = JsonConvert.DeserializeObject<FbUser>(response.Content);

			if (user == null || string.IsNullOrEmpty(user.Id))
			{
				return null;
			}

			var claims = new List<Claim>
						 {
							 new Claim( ClaimTypes.Name, user.FirstName ),
							 new Claim( ClaimTypes.NameIdentifier, user.Id ),
							 new Claim( ClaimTypes.AuthenticationMethod, DefaultAuthenticationTypes.ExternalCookie ),
						 };

			if (!string.IsNullOrEmpty(user.Email))
			{
				claims.Add(new Claim(ClaimTypes.Email, user.Email));
			}

			var loginInfo = new ExternalLoginInfo
			{
				DefaultUserName = string.IsNullOrEmpty(user.Email) ? string.Format("id{0}", user.Id) : user.FirstName,
				Email = user.Email,
				ExternalIdentity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ExternalCookie),
				Login = new UserLoginInfo(ProviderName, user.Id)
			};

			return loginInfo;
		}

		public ISocialUser GetUser(ExternalLoginInfo loginInfo)
		{
			var user = new FbUser();
			// ID
			var claimId = loginInfo.ExternalIdentity.FindFirst(x => x.Type == ClaimTypes.NameIdentifier);
			if (claimId != null)
			{
				user.Id = claimId.Value;
			}
			// FirstName
			user.FirstName = loginInfo.DefaultUserName;

			// email
			var claimEmail = loginInfo.ExternalIdentity.FindFirst(x => x.Type == ClaimTypes.Email);
			user.Email = claimEmail != null ? claimEmail.Value : loginInfo.DefaultUserName;

			return user;
		}

		public string ProviderName
		{
			get { return ExternalProviderName.Twitter; }
		}
	}
}