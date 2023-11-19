using System.Collections;
using System.Collections.Generic;
using MiniJSON;

namespace DW_Leaderboards
{
	public class LeaderboardData
	{
		public int ranking;

		public string PlayerID;

		public string PlayerName;

		public int Points;

		public LeaderboardData(int position, string id, string name, int point)
		{
			ranking = position;
			PlayerID = id;
			PlayerName = name;
			Points = point;
		}

		public static List<LeaderboardData> ProcessJsonToList(string json_data)
		{
			List<LeaderboardData> list = new List<LeaderboardData>();
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			IList list2 = (IList)Json.Deserialize(json_data);
			foreach (IDictionary item in list2)
			{
				int position = (int)item["ranking"];
				string id = (string)item["playerid"];
				string name = (string)item["playername"];
				int point = (int)item["score"];
				list.Add(new LeaderboardData(position, id, name, point));
			}
			return list;
		}
	}
}
