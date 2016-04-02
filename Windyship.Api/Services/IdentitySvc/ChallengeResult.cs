using System.Globalization;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;

namespace Windyship.Api.Services.IdentitySvc
{
	public class ChallengeResult : HttpUnauthorizedResult
	{
		private readonly string _loginProvider;
		private readonly string _redirectUri;
		private readonly int? _userId;
		private readonly string _xsrfKey;

		public ChallengeResult(string provider, string redirectUri, int? userId, string xsrfKey)
		{
			_loginProvider = provider;
			_redirectUri = redirectUri;
			_userId = userId;
			_xsrfKey = xsrfKey;
		}

		public override void ExecuteResult(ControllerContext context)
		{
			var properties = new AuthenticationProperties
			{
				RedirectUri = _redirectUri
			};

			if (_userId.HasValue)
			{
				properties.Dictionary[_xsrfKey] = _userId.Value.ToString(CultureInfo.InvariantCulture);
			}

			context.HttpContext.GetOwinContext().Authentication.Challenge(properties, _loginProvider);
		}
	}
}