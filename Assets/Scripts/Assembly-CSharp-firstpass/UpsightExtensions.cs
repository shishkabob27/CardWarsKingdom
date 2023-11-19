using System;

public static class UpsightExtensions
{
	private static readonly DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

	public static double ToUnixTimestamp(this DateTime dateTime)
	{
		return (dateTime - unixEpoch).TotalSeconds;
	}

	public static DateTime ToDateTime(this long timestamp)
	{
		return unixEpoch.AddSeconds(timestamp);
	}

	public static DateTime ToDateTime(this double timestamp)
	{
		return unixEpoch.AddSeconds(timestamp);
	}
}
