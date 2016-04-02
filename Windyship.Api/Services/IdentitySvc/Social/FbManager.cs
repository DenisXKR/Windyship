using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using Windyship.Identity;
using Windyship.Api.Models.Api.v1.UserModels.Social;
using RestSharp;


namespace Windyship.Api.Services.IdentitySvc.Social
{
	public class FbManager: ISocialManager
	{
		public ExternalLoginInfo GetLoginInfo(string code)
		{
			if (string.IsNullOrEmpty( code ))
			{
				return null;
			}

			var client = new RestClient();
			var request = new RestRequest( SocialUrls.FbGetUserInfoUrl, Method.GET );

			var parameters = new Dictionary<string, object>
						 {
							 {"access_token", code},
							 {"fields", "id,email,first_name,last_name,picture"}
						 };

			foreach (var parameter in parameters)
			{
				request.AddParameter( parameter.Key, parameter.Value );
			}

			var response = client.Execute( request );
			
			var user = JsonConvert.DeserializeObject<FbUser>( response.Content );
			
			if (user == null || string.IsNullOrEmpty( user.Id ))
			{
				return null;
			}

			var claims = new List<Claim>
						 {
							 new Claim( ClaimTypes.Name, user.FirstName ),
							 new Claim( ClaimTypes.NameIdentifier, user.Id ),
							 new Claim( "urn:facebook:name", string.Format( "{0} {1}", user.FirstName, user.LastName ) ),
							 new Claim( ClaimTypes.AuthenticationMethod, DefaultAuthenticationTypes.ExternalCookie ),
							 new Claim( "urn:facebook:link", string.Format( "https://www.facebook.com/app_scoped_user_id/{0}", user.Id ) )
						 };

			if (!string.IsNullOrEmpty(user.Email))
			{
				claims.Add( new Claim( ClaimTypes.Email, user.Email ) );
			}

			var loginInfo = new ExternalLoginInfo
			{
				DefaultUserName = string.IsNullOrEmpty( user.Email ) ? string.Format( "id{0}", user.Id ) : user.FirstName,
				Email = user.Email,
				ExternalIdentity = new ClaimsIdentity( claims, DefaultAuthenticationTypes.ExternalCookie ),
				Login = new UserLoginInfo( ProviderName, user.Id )
			};

			return loginInfo;
		}

		public ISocialUser GetUser(ExternalLoginInfo loginInfo)
		{
			var user = new FbUser();
			// ID
			var claimId = loginInfo.ExternalIdentity.FindFirst( x => x.Type == ClaimTypes.NameIdentifier );
			if (claimId != null)
			{
				user.Id = claimId.Value;
			}
			// FirstName
			var claimFirtName = loginInfo.ExternalIdentity.FindFirst( x => x.Type == "urn:facebook:name" );
			if (claimFirtName != null)
			{
				user.FirstName = claimFirtName.Value;
			}
			// email
			var claimEmail = loginInfo.ExternalIdentity.FindFirst( x => x.Type == ClaimTypes.Email );
			user.Email = claimEmail != null ? claimEmail.Value : loginInfo.DefaultUserName;
			// avatar
			if (claimId != null)
			{
				try
				{
					using (var webClient = new WebClient())
					{
						var url = string.Format( "http://graph.facebook.com/{0}/picture?type=square&width=300&height=300", claimId.Value );
						var data = webClient.DownloadData( url );
						user.Avatar = data;
					}
				}
				catch (WebException ex)
				{
					user.Avatar = null;
				}
			}
			return user;
		}

		public string ProviderName
		{
			get { return ExternalProviderName.Facebook;  }
		}
	}
}