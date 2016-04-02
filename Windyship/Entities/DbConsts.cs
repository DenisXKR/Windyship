namespace Windyship.Entities
{
	public static class DbConsts
	{
		public const int UserNameFirstMaxLen = 100;
		public const int UserNameSecondMaxLen = 100;
		public const int UserNameMiddleMaxLen = 100;
		internal const int UserNameFullMaxLen = UserNameFirstMaxLen + UserNameSecondMaxLen + UserNameMiddleMaxLen;

		public const int UserEmailLenMax = 100;

		public const int UserPhoneMax = 50;

		public const int UserRoleMin = (int) UserRole.User;
		public const int UserRoleMax = (int) UserRole.Admin;
	}
}