using System.Security.Cryptography;
using System.Text;

namespace Windyship.Api.Services.IdentitySvc
{
	// http://stackoverflow.com/a/1344255
	public static class TokenGenerator
	{
		private static readonly char[] chars = "ABCDEFGHJKLMNPRSTUVWXYZ23456789".ToCharArray();
		private static readonly char[] digits = "1234567890".ToCharArray();

		public static string GetUnique(int maxSize)
		{
			var data = new byte[1];

			var crypto = new RNGCryptoServiceProvider();
			crypto.GetNonZeroBytes(data);
			data = new byte[maxSize];
			crypto.GetNonZeroBytes(data);

			var result = new StringBuilder(maxSize);
			foreach (var b in data)
			{
				result.Append(chars[b%(chars.Length)]);
			}

			return result.ToString();
		}

		public static string GetUniqueDigits(int maxSize)
		{
			var data = new byte[1];

			var crypto = new RNGCryptoServiceProvider();
			crypto.GetNonZeroBytes(data);
			data = new byte[maxSize];
			crypto.GetNonZeroBytes(data);

			var result = new StringBuilder(maxSize);
			foreach (var b in data)
			{
				result.Append(digits[b % (digits.Length)]);
			}

			return result.ToString();
		}
	}
}