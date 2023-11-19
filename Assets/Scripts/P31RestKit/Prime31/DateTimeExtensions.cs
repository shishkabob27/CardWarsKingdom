using System;

namespace Prime31
{
	public static class DateTimeExtensions
	{
		public static long toEpochTime(this DateTime self)
		{
			DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			return Convert.ToInt64((self - dateTime).TotalSeconds);
		}

		public static DateTime fromEpochTime(this long unixTime)
		{
			return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unixTime);
		}
	}
}
