using System;

namespace Windyship.Api.Models.Api.v1.UserModels
{
	public sealed class AvatarUploadResponse
	{
		private string _message;

		public AvatarUploadResponseCode Code { get; set; }

		public DateTime? Dummy { get; set; }

		public string Message
		{
			get { return _message ?? Code.ToString(); }
			set { _message = value; }
		}
	}
}