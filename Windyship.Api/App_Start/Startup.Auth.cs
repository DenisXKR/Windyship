using LoginModule;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.Twitter;
using Owin;
using System;
using System.Web.Configuration;

[assembly: OwinStartup(typeof(OwinStart))]
namespace LoginModule
{
	public class OwinStart
	{
		internal static IDataProtectionProvider DataProtectionProvider { get; private set; }

		public void Configuration(IAppBuilder app)
		{
			ConfigureAuth(app);
		}

		// For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
		public void ConfigureAuth(IAppBuilder app)
		{
			DataProtectionProvider = app.GetDataProtectionProvider();

			// Enable the application to use a cookie to store information for the signed in user
			// and to use a cookie to temporarily store information about a user logging in with a third party login provider
			// Configure the sign in cookie
			app.UseCookieAuthentication(new CookieAuthenticationOptions
			{
				AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
				LoginPath = new PathString("/Account/Login"),
				Provider = new CookieAuthenticationProvider
				{
					OnApplyRedirect = ctx =>
					{
						if (IsWebRequest(ctx.Request))
						{
							ctx.Response.Redirect(ctx.RedirectUri);
						}
					}
				}
			});

			app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

			// Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
			app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

			// Enables the application to remember the second login verification factor such as phone or email.
			// Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
			// This is similar to the RememberMe option when you log in.
			app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

			// Uncomment the following lines to enable logging in with third party login providers
			//app.UseMicrosoftAccountAuthentication(
			//    clientId: "",
			//    clientSecret: "");

			app.UseTwitterAuthentication(new TwitterAuthenticationOptions
			{
				ConsumerKey = WebConfigurationManager.AppSettings.Get("TwitterAppId"),
				ConsumerSecret = WebConfigurationManager.AppSettings.Get("TwitterAppSecret"),
				BackchannelCertificateValidator = new Microsoft.Owin.Security.CertificateSubjectKeyIdentifierValidator(new[]
			   {
			       "A5EF0B11CEC04103A34A659048B21CE0572D7D47", // VeriSign Class 3 Secure Server CA - G2
			       "0D445C165344C1827E1D20AB25F40163D8BE79A5", // VeriSign Class 3 Secure Server CA - G3
			       "7FD365A7C2DDECBBF03009F34339FA02AF333133", // VeriSign Class 3 Public Primary Certification Authority - G5
			       "39A55D933676616E73A761DFA16A7E59CDE66FAD", // Symantec Class 3 Secure Server CA - G4
			       "‎add53f6680fe66e383cbac3e60922e3b4c412bed", // Symantec Class 3 EV SSL CA - G3
			       "4eb6d578499b1ccf5f581ead56be3d9b6744a5e5", // VeriSign Class 3 Primary CA - G5
			       "5168FF90AF0207753CCCD9656462A212B859723B", // DigiCert SHA2 High Assurance Server C‎A 
			       "B13EC36903F8BF4701D498261A0802EF63642BC3" // DigiCert High Assurance EV Root CA
			   })
			});

			var fbAppId = WebConfigurationManager.AppSettings.Get("FbAppId");
			var fbAppSecret = WebConfigurationManager.AppSettings.Get("FbAppSecret");

			var fbOptions = new FacebookAuthenticationOptions
			{
				AppId = fbAppId,
				AppSecret = fbAppSecret
			};

			fbOptions.Scope.Add("email");
			app.UseFacebookAuthentication(fbOptions);
		}

		private static bool IsWebRequest(IOwinRequest request)
		{
			var pathStartsWithApi = request.Path.StartsWithSegments(new PathString("/api"));
			var pathStartsWithImage = request.Path.StartsWithSegments(new PathString("/image"));
			if (pathStartsWithApi || pathStartsWithImage)
			{
				return false;
			}

			var query = request.Query;
			if (query != null)
			{
				if (query["X-Requested-With"] == "XMLHttpRequest")
				{
					return false;
				}

				if (query["Content-Type"] == "application/json")
				{
					return false;
				}
			}

			var headers = request.Headers;
			if (headers != null)
			{
				if (headers["X-Requested-With"] == "XMLHttpRequest")
				{
					return false;
				}

				if (headers["Content-Type"] == "application/json")
				{
					return false;
				}
			}

			return true;
		}
	}
}
