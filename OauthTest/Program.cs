using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;

namespace OauthTest
{
	class Program
	{
		static void Main(string[] args)
		{
			//string baseAddress = "http://localhost:2411";
			string baseAddress = "http://windyship.com";

			using (var client = new HttpClient())
			{
				var form = new Dictionary<string, string>    
               {    
                   {"grant_type", "password"},    
                   {"username", "+54645645"},    
                   {"password", "lnQfp2wW6OgRyXjRJnPr7iXfU1pd3d74UTEoiW7oWB5kBvSfH5d7dN5voc1rwCVRGraFEpDH8DMnXarwRwTlvMAXVXfApdR3h8iUFvVmOr5vL33hEY6EfJqzoAplgg/o9UagiWNKPB2wNCGpIQtrmw=="}
               };

				try
				{
					/*
					var byteArray = new UTF8Encoding().GetBytes("<clientid>:<clientsecret>");
					client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));*/

					var tokenResponse = client.PostAsync(baseAddress + "/token", new FormUrlEncodedContent(form)).Result;
					var token = tokenResponse.Content.ReadAsAsync<Token>(new[] { new JsonMediaTypeFormatter() }).Result;

					if (string.IsNullOrEmpty(token.error))
					{
						Console.WriteLine("Token issued is: {0}", token.access_token);
					}
					else
					{
						Console.WriteLine("Error : {0}", token.error);
					}

					using (HttpClient httpClient1 = new HttpClient())
					{
						httpClient1.BaseAddress = new Uri(baseAddress);
						httpClient1.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.access_token);
						HttpResponseMessage response = httpClient1.PostAsync("/api/UploadAvatar", new FormUrlEncodedContent(new Dictionary<string, string>())).Result;
						if (response.IsSuccessStatusCode)
						{
							System.Console.WriteLine("Success");
						}
						string message = response.Content.ReadAsStringAsync().Result;
						System.Console.WriteLine("URL responese : " + message);
					}
				}
				catch (Exception ex)
				{
					Console.Write(ex.Message);
				}

				Console.Read();
			}
		}
	}
}
