using System.Collections.Generic;
using Newtonsoft.Json;

namespace Windyship.Api.Models.Api.v1.UserModels.Social
{
	public class FbUserResponse
	{
		[JsonProperty( "data" )]
		public List<FbUser> Users { get; set; }
	}
}