using System.Collections.Generic;
using System.Net;

namespace DW_Leaderboards
{
	public class DW_Leaderboard
	{
		public static void PlaceMeOnLeaderboard(Session session, string user_id, string currentSeasonID, SuccessCallback callback)
		{
			TFServer.JsonResponseHandler callback2 = delegate(Dictionary<string, object> data, HttpStatusCode status)
			{
				if (data == null)
				{
					callback(ResponseFlag.Error);
				}
				else if (status == HttpStatusCode.OK && data != null && data["success"].ToString() == "True")
				{
					callback(ResponseFlag.Success);
				}
				else
				{
					callback(ResponseFlag.Error);
				}
			};
			session.Server.PlaceMeOnLeaderboard(user_id, currentSeasonID, callback2);
		}

		public static void RegisterMatchResult(Session session, string user_id, string currentSeasonID, string opponentID, bool didIWin, RegisterMatchResultCallback callback)
		{
			bool flag = Singleton<TutorialController>.Instance.IsBlockActive("Social");
			TFServer.JsonResponseHandler callback2 = delegate(Dictionary<string, object> data, HttpStatusCode status)
			{
				if (data == null)
				{
					callback(0, ResponseFlag.Error);
				}
				else if (status == HttpStatusCode.OK && data != null && data["success"].ToString() == "True")
				{
					callback((int)data["data"], ResponseFlag.Success);
				}
				else
				{
					callback(0, ResponseFlag.Error);
				}
			};
			session.Server.RegisterMatchResult(user_id, currentSeasonID, opponentID, didIWin, callback2);
		}

		public static void FetchLeaderboardsEntries(Session session, int startPosition, int endPosition, LeaderboardListCallback callback)
		{
			TFServer.JsonResponseHandler callback2 = delegate(Dictionary<string, object> data, HttpStatusCode status)
			{
				if (data == null)
				{
					callback(null, ResponseFlag.Error);
				}
				else if (status == HttpStatusCode.OK && data["success"].ToString() == "True")
				{
					callback(LeaderboardData.ProcessJsonToList((string)data["data"]), ResponseFlag.Success);
				}
				else if (status == HttpStatusCode.NotFound)
				{
					callback(null, ResponseFlag.None);
				}
				else
				{
					callback(null, ResponseFlag.Error);
				}
			};
			session.Server.FetchLeaderboardsEntries(startPosition, endPosition, callback2);
		}

		public static void HasSeasonEnded(Session session, string user_id, HasSeasonEnded callback)
		{
			TFServer.JsonResponseHandler callback2 = delegate(Dictionary<string, object> data, HttpStatusCode status)
			{
				if (data == null)
				{
					callback(0, ResponseFlag.Error);
				}
				else if (status == HttpStatusCode.OK && data["success"].ToString() == "True")
				{
					callback((int)data["data"], ResponseFlag.Success);
				}
				else
				{
					callback(0, ResponseFlag.Error);
				}
			};
			session.Server.HasSeasonEnded(user_id, callback2);
		}

		public static void RpcGetData(string user_id, RpcGetDataCallback callback)
		{
			Singleton<TBPvPManager>.Instance.GameClientInstance.WebRpcGetData(user_id, callback);
		}
	}
}
