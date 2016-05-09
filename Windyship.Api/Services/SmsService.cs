using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace Windyship.Api.Services
{
	public static class WindySmsService
	{
		const string Sender = "Windyship";
		const string User = "966550618047";
		const string Password = "windy87ship";

		public static void SendMessage(string msg, string numbers)
		{
			HttpWebRequest req = (HttpWebRequest)
			WebRequest.Create("http://www.mobily.ws/api/msgSend.php");
			req.Method = "POST";
			req.ContentType = "application/x-www-form-urlencoded";
			string postData = string.Format("mobile={0}&password={1}&numbers={2}&sender={3}&msg={4}&applicationType=24", User, Password, numbers, Sender, ConvertToUnicode(msg));
			req.ContentLength = postData.Length;

			StreamWriter stOut = new
			StreamWriter(req.GetRequestStream(),
			System.Text.Encoding.ASCII);
			stOut.Write(postData);
			stOut.Close();
			// Do the request to get the response
			StreamReader stIn = new StreamReader(req.GetResponse().GetResponseStream());
			string strResponse = stIn.ReadToEnd();
			stIn.Close();
		}

		private static string ConvertToUnicode(string val)
		{
			string msg2 = string.Empty;

			for (int i = 0; i < val.Length; i++)
			{
				msg2 += convertToUnicode(System.Convert.ToChar(val.Substring(i, 1)));
			}

			return msg2;
		}

		private static string convertToUnicode(char ch)
		{
			System.Text.UnicodeEncoding class1 = new System.Text.UnicodeEncoding();
			byte[] msg = class1.GetBytes(System.Convert.ToString(ch));

			return fourDigits(msg[1] + msg[0].ToString("X"));
		}

		private static string fourDigits(string val)
		{
			string result = string.Empty;

			switch (val.Length)
			{
				case 1: result = "000" + val; break;
				case 2: result = "00" + val; break;
				case 3: result = "0" + val; break;
				case 4: result = val; break;
			}

			return result;
		}
	}
}