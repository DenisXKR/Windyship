using Newtonsoft.Json;

namespace Windyship.Api.Models.Api.v1.UserModels.Social
{
	public class FbUser: ISocialUser
	{
		[JsonProperty( "id" )]
		public string Id { get; set; }

		[JsonProperty( "first_name" )]
		public string FirstName { get; set; }

		[JsonProperty( "last_name" )]
		public string LastName { get; set; }

		[JsonProperty( "email" )]
		public string Email { get; set; }

		public byte[] Avatar { get; set; }

		[JsonProperty( "picture" )]
		public FbPicture Picture { get; set; }

		public override string ToString()
		{
			return string.Format( "Id: {0} , FN: {1},LN: {2},E: {3}, A: {4}", Id, FirstName, LastName, Email,
				Avatar != null && Picture.Picture != null ? Picture.Picture.Url : string.Empty );
		}
	}

	public class FbPicture
	{
		[JsonProperty( "data" )]
		public FbDataPicture Picture { get; set; }
	}

	public class FbDataPicture
	{
		[JsonProperty( "is_silhouette" )]
		public bool IsSilhouette { get; set; }

		[JsonProperty( "url" )]
		public string Url { get; set; }
	}
}