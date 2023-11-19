using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Multiplayer
{
	public class Multiplayer
	{
		public static string alphabets = "tqhueickbrwnxofjmupsverthelazydg";

		public static void GetMultiplayerStatus(Session session, MultiplayerDataCallback callback)
		{
			TFServer.JsonResponseHandler callback2 = delegate(Dictionary<string, object> data, HttpStatusCode status)
			{
				if (data == null)
				{
					callback(null, ResponseFlag.Error);
				}
				else if (status == HttpStatusCode.OK && data.ContainsKey("success") && (bool)data["success"])
				{
					Dictionary<string, object> dict = (Dictionary<string, object>)data["data"];
					callback(new MultiplayerData(dict), ResponseFlag.Success);
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
			session.Server.MultiplayerPlayerInfo(session.ThePlayer.playerId, callback2);
		}

		public static void CreateMultiplayerUser(Session session, PlayerInfoScript playerInfo, MultiplayerDataCallback callback)
		{
			CreateMultiplayerUser(session, playerInfo.SaveData.MultiplayerPlayerName, playerInfo.GetCurrentLoadout().Leader.SelectedSkin.PortraitTexture, string.Empty, 0f, playerInfo.PvPSerialize(), playerInfo.SaveData.MyHelperCreatureID.ToString(), playerInfo.RankXpLevelData.mCurrentLevel, 0, callback);
		}

		public static void CreateMultiplayerUser(Session session, string name, string icon, string deck, float deckRank, string landscapes, string leader, int leaderLevel, int maxLevel, MultiplayerDataCallback callback)
		{
			TFServer.JsonResponseHandler callback2 = delegate(Dictionary<string, object> data, HttpStatusCode status)
			{
				if (data == null)
				{
					callback(null, ResponseFlag.Error);
				}
				else
				{
					switch (status)
					{
					case HttpStatusCode.OK:
						if (data.ContainsKey("success") && (bool)data["success"])
						{
							Dictionary<string, object> dict = (Dictionary<string, object>)data["data"];
							callback(new MultiplayerData(dict), ResponseFlag.Success);
						}
						else
						{
							callback(null, ResponseFlag.Error);
						}
						break;
					case HttpStatusCode.Forbidden:
						callback(null, ResponseFlag.Invalid);
						break;
					default:
						callback(null, ResponseFlag.Error);
						break;
					}
				}
			};
			PlayerInfoScript instance = Singleton<PlayerInfoScript>.Instance;
			string empty = string.Empty;
			string text = instance.PvPSerialize();
			string helpercreature = instance.SerializeHelperCreature();
			int allyboxspace = instance.SaveData.AllyBoxSpace;
			string deck2 = Convert.ToBase64String(Encoding.UTF8.GetBytes(deck));
			session.Server.MultiplayerNewPlayer(name, icon, deck2, deckRank, landscapes, helpercreature, leader, leaderLevel, maxLevel, allyboxspace, callback2);
		}

		public static void PlayerRecord(Session session, ExtendedRecordCallback callback)
		{
			PlayerRecord(session, session.ThePlayer.playerId, callback);
		}

		public static void PlayerRecord(Session session, string playerId, ExtendedRecordCallback callback)
		{
			TFServer.JsonResponseHandler callback2 = delegate(Dictionary<string, object> data, HttpStatusCode status)
			{
				if (data != null && status == HttpStatusCode.OK)
				{
					bool flag = data.ContainsKey("success") && (bool)data["success"];
					List<object> data2 = (List<object>)data["data"];
					if (flag)
					{
						callback(ExtendedRecordData.ProcessList(data2));
						return;
					}
				}
				callback(null);
			};
			session.Server.MultiplayerExtendedRecord(playerId, callback2);
		}

		public static void RecentBattles(Session session, NotificationCallback callback)
		{
			TFServer.JsonResponseHandler callback2 = delegate(Dictionary<string, object> data, HttpStatusCode status)
			{
				if (data != null && status == HttpStatusCode.OK && data.ContainsKey("success") && (bool)data["success"])
				{
					Dictionary<string, object> dict = (Dictionary<string, object>)data["data"];
					callback(new RecentNotification(dict));
				}
				else
				{
					callback(null);
				}
			};
			session.Server.MultiplayerNotification(callback2);
		}

		public static void AttackRecord(Session session, RecordCallback callback)
		{
			TFServer.JsonResponseHandler callback2 = delegate(Dictionary<string, object> data, HttpStatusCode status)
			{
				if (data != null && status == HttpStatusCode.OK)
				{
					bool flag = data.ContainsKey("success") && (bool)data["success"];
					List<object> data2 = (List<object>)data["data"];
					if (flag)
					{
						callback(RecordData.ProcessList(data2));
						return;
					}
				}
				callback(null);
			};
			session.Server.MultiplayerPersonalRecord("attack", callback2);
		}

		public static void DefenseRecord(Session session, RecordCallback callback)
		{
			TFServer.JsonResponseHandler callback2 = delegate(Dictionary<string, object> data, HttpStatusCode status)
			{
				if (data != null && status == HttpStatusCode.OK)
				{
					bool flag = data.ContainsKey("success") && (bool)data["success"];
					List<object> data2 = (List<object>)data["data"];
					if (flag)
					{
						callback(RecordData.ProcessList(data2));
						return;
					}
				}
				callback(null);
			};
			session.Server.MultiplayerPersonalRecord("defense", callback2);
		}

		public static void TournamentReward(Session session, RewardsCallback callback)
		{
			TFServer.JsonResponseHandler callback2 = delegate(Dictionary<string, object> data, HttpStatusCode status)
			{
				if (status == HttpStatusCode.OK && data != null && data.ContainsKey("success") && (bool)data["success"])
				{
					List<object> data2 = (List<object>)data["data"];
					callback(RewardData.ProcessList(data2), ResponseFlag.Success);
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
			session.Server.MultiplayerTournamentPlayerResult(callback2);
		}

		public static void TopLeaderboard(Session session, LeaderboardCallback callback)
		{
			TFServer.JsonResponseHandler callback2 = delegate(Dictionary<string, object> data, HttpStatusCode status)
			{
				if (status == HttpStatusCode.OK && data.ContainsKey("success") && data != null && (bool)data["success"])
				{
					List<object> data2 = (List<object>)data["data"];
					callback(LeaderboardData.ProcessList(data2));
				}
				else
				{
					callback(null);
				}
			};
			session.Server.MultiplayerLeaderboardTop(callback2);
		}

		public static void NearbyLeaderboard(Session session, LeaderboardCallback callback)
		{
			TFServer.JsonResponseHandler callback2 = delegate(Dictionary<string, object> data, HttpStatusCode status)
			{
				if (status == HttpStatusCode.OK && data != null && data.ContainsKey("success") && (bool)data["success"])
				{
					List<object> data2 = (List<object>)data["data"];
					callback(LeaderboardData.ProcessList(data2));
				}
				else
				{
					callback(null);
				}
			};
			session.Server.MultiplayerLeaderboardPlayer(session.ThePlayer.playerId, callback2);
		}

		public static void MatchMake(Session session, int maxLevel, MatchDataCallback callback)
		{
			TFServer.JsonResponseHandler callback2 = delegate(Dictionary<string, object> data, HttpStatusCode status)
			{
				if (status == HttpStatusCode.OK && data != null && data.ContainsKey("success") && (bool)data["success"])
				{
					Dictionary<string, object> dict = (Dictionary<string, object>)data["data"];
					callback(new MatchData(dict), ResponseFlag.Success);
				}
				else
				{
					callback(null, ResponseFlag.Error);
				}
			};
			session.Server.MultiplayerFindMatch(maxLevel, callback2);
		}

		public static void MatchGetDeck(Session session, string matchid, float deckRank, string leader, int leaderLevel, StringCallback callback)
		{
			TFServer.JsonStringHandler callback2 = delegate(string data, HttpStatusCode status)
			{
				if (status == HttpStatusCode.OK && data != null)
				{
					callback(data, ResponseFlag.Success);
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
			session.Server.MultiplayerStartMatch(matchid, deckRank, leader, leaderLevel, callback2);
		}

		public static void MatchFinish(Session session, string matchId, bool loss, StringCallback callback)
		{
			TFServer.JsonResponseHandler callback2 = delegate(Dictionary<string, object> data, HttpStatusCode status)
			{
				if (status == HttpStatusCode.OK && data != null && data.ContainsKey("success") && (bool)data["success"])
				{
					Dictionary<string, object> dictionary = (Dictionary<string, object>)data["data"];
					callback(dictionary["trophies"].ToString(), ResponseFlag.Success);
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
			session.Server.MultiplayerEndMatch(matchId, loss, callback2);
		}

		public static void UpdateMultiplayerUser(Session session, string name, string icon, int maxLevel, SuccessCallback callback)
		{
			TFServer.JsonResponseHandler callback2 = delegate(Dictionary<string, object> data, HttpStatusCode status)
			{
				if (status == HttpStatusCode.OK && data != null && data.ContainsKey("success") && (bool)data["success"])
				{
					callback(ResponseFlag.Success);
				}
				else if (status == HttpStatusCode.Forbidden)
				{
					callback(ResponseFlag.Invalid);
				}
				else
				{
					callback(ResponseFlag.Error);
				}
			};
			session.Server.MultiplayerUpdatePlayer(name, icon, maxLevel, callback2);
		}

		public static void UpdateDeck(Session session, string name, string deck, int needUpdate, string landscapes, string helpercreature, string leader, int leaderLevel, int allyboxspace, SuccessCallback callback)
		{
			TFServer.JsonResponseHandler callback2 = delegate(Dictionary<string, object> data, HttpStatusCode status)
			{
				if (status == HttpStatusCode.OK && data != null && data.ContainsKey("success") && (bool)data["success"])
				{
					callback(ResponseFlag.Success);
				}
				else
				{
					callback(ResponseFlag.Error);
				}
			};
			string deck2 = Convert.ToBase64String(Encoding.UTF8.GetBytes(deck));
			session.Server.MultiplayerUpdateDeck(name, deck2, needUpdate, landscapes, helpercreature, leader, leaderLevel, allyboxspace, callback2);
		}

		public static void CheckRedeemCode(Session session, string redeemcode, string version, string subject, string message, DateTime start_date, DateTime end_date, int soft_currency, SuccessCallback callback)
		{
			TFServer.JsonResponseHandler callback2 = delegate(Dictionary<string, object> data, HttpStatusCode status)
			{
				if (status == HttpStatusCode.OK && data != null && data.ContainsKey("success") && (bool)data["success"])
				{
					callback(ResponseFlag.Success);
				}
				else if (status == HttpStatusCode.OK && data != null && data.ContainsKey("error") && (string)data["error"] == "maximum reword count reached")
				{
					callback(ResponseFlag.Exceed);
				}
				else if (status == HttpStatusCode.OK && data != null && data.ContainsKey("error") && (string)data["error"] == "invalid redeemcode")
				{
					callback(ResponseFlag.Invalid);
				}
				else
				{
					callback(ResponseFlag.Error);
				}
			};
			session.Server.RedeemcodeCheck(redeemcode, version, subject, message, start_date, end_date, soft_currency, callback2);
		}

		public static void GetTournamentEndDate(Session session, bool cheater, TournamentDataCallback callback)
		{
			TFServer.JsonResponseHandler callback2 = delegate(Dictionary<string, object> data, HttpStatusCode status)
			{
				if (status == HttpStatusCode.OK && data != null && data.ContainsKey("success") && (bool)data["success"])
				{
					Dictionary<string, object> dict = (Dictionary<string, object>)data["data"];
					callback(new TournamentData(dict), ResponseFlag.Success);
				}
				else
				{
					callback(null, ResponseFlag.Error);
				}
			};
			if (cheater)
			{
				session.Server.MultiplayerCheaterTournamentEnd(callback2);
			}
			else
			{
				session.Server.MultiplayerGetTournamentEnd(callback2);
			}
		}

		public static void CompleteTournamentReward(Session session, int tournamentId, SuccessCallback callback)
		{
			TFServer.JsonResponseHandler callback2 = delegate(Dictionary<string, object> data, HttpStatusCode status)
			{
				if (status == HttpStatusCode.OK && data != null && data.ContainsKey("success") && (bool)data["success"])
				{
					callback(ResponseFlag.Success);
				}
				else
				{
					callback(ResponseFlag.Error);
				}
			};
			session.Server.MultiplayerRedeemReward(tournamentId, callback2);
		}

		public static void GetRank(Session session, bool global, StringCallback callback)
		{
			TFServer.JsonResponseHandler callback2 = delegate(Dictionary<string, object> data, HttpStatusCode status)
			{
				if (status == HttpStatusCode.OK && data != null && data.ContainsKey("success") && (bool)data["success"])
				{
					Dictionary<string, object> dictionary = (Dictionary<string, object>)data["data"];
					callback(dictionary["rank"].ToString(), ResponseFlag.Success);
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
			session.Server.MultiplayerGetRank(global, callback2);
		}

		public static string GenerateRedeemCode(string id)
		{
			char[] array = alphabets.ToCharArray();
			char[] array2 = id.ToCharArray();
			string empty = string.Empty;
			int num = 0;
			int num2 = 0;
			int num3 = -1;
			Random random = new Random(Environment.TickCount);
			int num4 = random.Next(0, 10);
			int num5 = random.Next(0, 30);
			int num6 = random.Next(0, 30);
			int num7 = random.Next(0, 3);
			int num8 = 0;
			empty += array[num5];
			if (num4 < 5)
			{
				num = 0;
				empty += "l";
			}
			else
			{
				num = 1;
				empty += "a";
			}
			for (int i = 0; i < array2.Length; i++)
			{
				if (array2[i] == '_')
				{
					switch (num7)
					{
					case 0:
						num = 11;
						empty += "y";
						break;
					case 1:
						num = 12;
						empty += "d";
						break;
					default:
						num = 13;
						empty += "g";
						break;
					}
					empty += array[num6];
				}
				else
				{
					num2 = (int)char.GetNumericValue(array2[i]);
					if (num3 == num2)
					{
						empty += "z";
						num3 = -1;
					}
					else
					{
						empty += array[num2 + num];
						num3 = num2;
					}
					num8 += num2;
				}
			}
			num8 %= 20;
			return empty + array[num8];
		}

		public static string DecodeRedeemCode(string code)
		{
			char[] array = alphabets.ToCharArray();
			char[] array2 = code.ToCharArray();
			string text = string.Empty;
			int num = 0;
			int num2 = -1;
			int num3 = 0;
			int num4 = 0;
			switch (char.ToLower(array2[1]))
			{
			case 'l':
				num3 = 0;
				break;
			case 'a':
				num3 = 1;
				break;
			default:
				return string.Empty;
			}
			bool flag = false;
			for (int i = 2; i < array2.Length - 1; i++)
			{
				if (flag)
				{
					flag = false;
					continue;
				}
				char c = char.ToLower(array2[i]);
				switch (c)
				{
				case 'y':
					num3 = 11;
					text += "_";
					flag = true;
					continue;
				case 'd':
					num3 = 12;
					text += "_";
					flag = true;
					continue;
				case 'g':
					num3 = 13;
					text += "_";
					flag = true;
					continue;
				case 'z':
					text += num2;
					num4 += num2;
					num2 = -1;
					continue;
				}
				bool flag2 = false;
				int num5 = 0;
				for (num5 = 0; num5 < 10; num5++)
				{
					if (c == array[num5 + num3])
					{
						text += num5;
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
				}
				num2 = num5;
				num4 += num5;
			}
			num4 %= 20;
			if (char.ToLower(array2[array2.Length - 1]) != array[num4])
			{
				text = string.Empty;
			}
			if (text == Singleton<PlayerInfoScript>.Instance.GetPlayerCode())
			{
				text = string.Empty;
			}
			return text;
		}
	}
}
