using System.ComponentModel.DataAnnotations;

namespace Windyship.Api.Models.Api.v1.UserModels.Social
{
	public class SocialTokenRequest
	{
		[Required]
		public string social_id { get; set; }
	}
}