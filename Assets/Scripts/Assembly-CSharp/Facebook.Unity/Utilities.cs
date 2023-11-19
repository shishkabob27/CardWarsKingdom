using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Facebook.MiniJSON;

namespace Facebook.Unity
{
	internal static class Utilities
	{
		private const string WarningMissingParameter = "Did not find expected value '{0}' in dictionary";

		public static bool TryGetValue<T>(this IDictionary<string, object> dictionary, string key, out T value)
		{
			object value2;
			if (dictionary.TryGetValue(key, out value2) && value2 is T)
			{
				value = (T)value2;
				return true;
			}
			value = default(T);
			return false;
		}

		public static long TotalSeconds(this DateTime dateTime)
		{
			return (long)(dateTime - new DateTime(1970, 1, 1)).TotalSeconds;
		}

		public static T GetValueOrDefault<T>(this IDictionary<string, object> dictionary, string key, bool logWarning = true)
		{
			T value;
			if (!dictionary.TryGetValue<T>(key, out value))
			{
				FacebookLogger.Warn("Did not find expected value '{0}' in dictionary", key);
			}
			return value;
		}

		public static string ToCommaSeparateList(this IEnumerable<string> list)
		{
			if (list == null)
			{
				return string.Empty;
			}
			return string.Join(",", list.ToArray());
		}

		public static string AbsoluteUrlOrEmptyString(this Uri uri)
		{
			if (uri == null)
			{
				return string.Empty;
			}
			return uri.AbsoluteUri;
		}

		public static string GetUserAgent(string productName, string productVersion)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}/{1}", productName, productVersion);
		}

		public static string ToJson(this IDictionary<string, object> dictionary)
		{
			return Json.Serialize(dictionary);
		}

		public static void AddAllKVPFrom<T1, T2>(this IDictionary<T1, T2> dest, IDictionary<T1, T2> source)
		{
			foreach (T1 key in source.Keys)
			{
				dest[key] = source[key];
			}
		}

		public static AccessToken ParseAccessTokenFromResult(IDictionary<string, object> resultDictionary)
		{
			string valueOrDefault = resultDictionary.GetValueOrDefault<string>(LoginResult.UserIdKey);
			string valueOrDefault2 = resultDictionary.GetValueOrDefault<string>(LoginResult.AccessTokenKey);
			DateTime expirationTime = ParseExpirationDateFromResult(resultDictionary);
			ICollection<string> permissions = ParsePermissionFromResult(resultDictionary);
			DateTime? lastRefresh = ParseLastRefreshFromResult(resultDictionary);
			return new AccessToken(valueOrDefault2, valueOrDefault, expirationTime, permissions, lastRefresh);
		}

		private static DateTime ParseExpirationDateFromResult(IDictionary<string, object> resultDictionary)
		{
			if (Constants.IsWeb)
			{
				return DateTime.Now.AddSeconds(resultDictionary.GetValueOrDefault<long>(LoginResult.ExpirationTimestampKey));
			}
			string valueOrDefault = resultDictionary.GetValueOrDefault<string>(LoginResult.ExpirationTimestampKey);
			int result;
			if (int.TryParse(valueOrDefault, out result) && result > 0)
			{
				return FromTimestamp(result);
			}
			return DateTime.MaxValue;
		}

		private static DateTime? ParseLastRefreshFromResult(IDictionary<string, object> resultDictionary)
		{
			string valueOrDefault = resultDictionary.GetValueOrDefault<string>(LoginResult.ExpirationTimestampKey);
			int result;
			if (int.TryParse(valueOrDefault, out result) && result > 0)
			{
				return FromTimestamp(result);
			}
			return null;
		}

		private static ICollection<string> ParsePermissionFromResult(IDictionary<string, object> resultDictionary)
		{
			string value;
			IEnumerable<object> value2;
			if (resultDictionary.TryGetValue<string>(LoginResult.PermissionsKey, out value))
			{
				value2 = value.Split(',');
			}
			else if (!resultDictionary.TryGetValue<IEnumerable<object>>(LoginResult.PermissionsKey, out value2))
			{
				value2 = new string[0];
				FacebookLogger.Warn("Failed to find parameter '{0}' in login result", LoginResult.PermissionsKey);
			}
			return value2.Select((object permission) => permission.ToString()).ToList();
		}

		private static DateTime FromTimestamp(int timestamp)
		{
			return new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(timestamp);
		}
	}
}
