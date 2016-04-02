namespace Windyship.Api.Models.Api.v1.UserModels.Social
{
	public interface ISocialUser
	{
		string Id { get; set; }
		string FirstName { get; set; }
		string LastName { get; set; }
		string Email { get; set; }
		byte[] Avatar { get; set; }
	}
}