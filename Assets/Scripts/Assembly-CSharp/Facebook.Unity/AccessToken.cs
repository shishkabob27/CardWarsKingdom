using System;
using System.Collections.Generic;
using System.Linq;
using Facebook.MiniJSON;

namespace Facebook.Unity
{
	public class AccessToken
	{
		public static AccessToken CurrentAccessToken { get; internal set; }

		public string TokenString { get; private set; }

		public DateTime ExpirationTime { get; private set; }

		public IEnumerable<string> Permissions { get; private set; }

		public string UserId { get; private set; }

		public DateTime? LastRefresh { get; private set; }

		internal AccessToken(string tokenString, string userId, DateTime expirationTime, IEnumerable<string> permissions, DateTime? lastRefresh)
		{
			if (string.IsNullOrEmpty(tokenString))
			{
				throw new ArgumentNullException("tokenString");
			}
			if (string.IsNullOrEmpty(userId))
			{
				throw new ArgumentNullException("userId");
			}
			if (expirationTime == DateTime.MinValue)
			{
				throw new ArgumentException("Expiration time is unassigned");
			}
			if (permissions == null)
			{
				throw new ArgumentNullException("permissions");
			}
			TokenString = tokenString;
			ExpirationTime = expirationTime;
			Permissions = permissions;
			UserId = userId;
			LastRefresh = lastRefresh;
		}

		internal string ToJson()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary[LoginResult.PermissionsKey] = string.Join(",", Permissions.ToArray());
			dictionary[LoginResult.ExpirationTimestampKey] = ExpirationTime.TotalSeconds().ToString();
			dictionary[LoginResult.AccessTokenKey] = TokenString;
			dictionary[LoginResult.UserIdKey] = UserId;
			if (LastRefresh.HasValue)
			{
				dictionary["last_refresh"] = LastRefresh.Value.TotalSeconds().ToString();
			}
			return Json.Serialize(dictionary);
		}
	}
}
