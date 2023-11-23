using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using MiniJSON;

namespace Allies
{
	public class Ally
	{
		private static AllyDataCallback AlliesListcallback;

		private static HelperListCallback HelperCallback;

		private static AllyDataCallback AllyRequestListcallback;

		private static SuccessCallback AllyDenycallback;

		private static SuccessCallback DeleteAllycallback;

		public static void GetAlliesList(Session session, AllyDataCallback callback)
		{
			AlliesListcallback = callback;
			session.GetFriendsList();
		}

		public static void AlliesListCallback(string myid, TFWebFileResponse response)
		{
			if (AlliesListcallback != null)
			{
				ResponseFlag flag = ResponseFlag.Error;
				if (response.StatusCode == HttpStatusCode.OK)
				{
					flag = ResponseFlag.Success;
				}
				AlliesListcallback(DecodeAllyInfo(myid, AllyStatus.STATUS_APPROVED, response.Data), flag);
				AlliesListcallback = null;
			}
		}

		public static void AllyRequest(Session session, string user_id, SuccessCallback callback)
		{
			TFServer.JsonResponseHandler callback2 = delegate(Dictionary<string, object> data, HttpStatusCode status)
			{
				if (data == null)
				{
					callback(ResponseFlag.Error);
				}
				else if (status == HttpStatusCode.OK && data["success"].ToString() == "True")
				{
					if (data.ContainsKey("info"))
					{
						if (data["info"].ToString().Contains("exceed"))
						{
							if (data["info"].ToString().Contains("me"))
							{
								callback(ResponseFlag.Exceedmyallylimit);
							}
							else
							{
								callback(ResponseFlag.Exceeduserallylimit);
							}
						}
						else if (data["info"].ToString().Contains("duplicate"))
						{
							callback(ResponseFlag.Duplicate);
						}
						else
						{
							callback(ResponseFlag.Success);
						}
					}
					else
					{
						callback(ResponseFlag.Success);
					}
				}
				else if (status == HttpStatusCode.NotFound)
				{
					callback(ResponseFlag.None);
				}
				else
				{
					callback(ResponseFlag.Error);
				}
			};
			session.Server.Friend_request_with_myinfo(user_id, callback2);
		}

		public static void GetHelperList(Session session, List<string> excludeIDs, HelperListCallback callback)
		{
			bool flag = Singleton<TutorialController>.Instance.IsBlockActive("Social");
			TFServer.JsonResponseHandler callback2 = delegate(Dictionary<string, object> data, HttpStatusCode status)
			{
				if (status == HttpStatusCode.OK && data != null && data["success"].ToString() == "True")
				{
					callback(DecodeAllyInfo(null, AllyStatus.STATUS_NONE, (string)data["data"]), ResponseFlag.Success);
				}
				else
				{
					List<AllyData> helper_list = new List<AllyData>();
					callback(helper_list, ResponseFlag.Success);
				}
			};
			session.Server.Friend_get_helpers("10", excludeIDs, callback2);
		}

		public static void GetHelper(Session session, string targetHelper, HelperListCallback callback)
		{
			TFServer.JsonResponseHandler callback2 = delegate(Dictionary<string, object> data, HttpStatusCode status)
			{
				if (status == HttpStatusCode.OK && data != null && data["success"].ToString() == "True")
				{
					callback(DecodeAllyInfo(null, AllyStatus.STATUS_NONE, (string)data["data"]), ResponseFlag.Success);
				}
				else
				{
					List<AllyData> helper_list = new List<AllyData>();
					callback(helper_list, ResponseFlag.Success);
				}
			};
			session.Server.Friend_get_helpers(targetHelper, null, callback2);
		}

		public static void GetAllyRequestList(Session session, AllyDataCallback callback)
		{
			AllyRequestListcallback = callback;
			session.GetFriendRequests();
		}

		public static void AllyRequestListCallback(string myid, TFWebFileResponse response)
		{
			if (AllyRequestListcallback != null)
			{
				ResponseFlag flag = ResponseFlag.Error;
				if (response.StatusCode == HttpStatusCode.OK)
				{
					flag = ResponseFlag.Success;
				}
				AllyRequestListcallback(DecodeAllyInfo(myid, AllyStatus.STATUS_INITIAL, response.Data), flag);
				AllyRequestListcallback = null;
			}
		}

		public static void ConfirmAllyRequest(Session session, string user_id, SuccessCallback callback)
		{
			TFServer.JsonResponseHandler callback2 = delegate(Dictionary<string, object> data, HttpStatusCode status)
			{
				if (data == null)
				{
					callback(ResponseFlag.Error);
				}
				else if (status == HttpStatusCode.OK && data["success"].ToString() == "True")
				{
					if (data.ContainsKey("info"))
					{
						if (data["info"].ToString().Contains("exceed"))
						{
							if (data["info"].ToString().Contains("me"))
							{
								callback(ResponseFlag.Exceedmyallylimit);
							}
							else
							{
								callback(ResponseFlag.Exceeduserallylimit);
							}
						}
						else
						{
							callback(ResponseFlag.Success);
						}
					}
					else
					{
						callback(ResponseFlag.Success);
					}
				}
				else if (status == HttpStatusCode.NotFound)
				{
					callback(ResponseFlag.None);
				}
				else
				{
					callback(ResponseFlag.Error);
				}
			};
			session.Server.Friend_confirm_with_myinfo(user_id, callback2);
		}

		public static void DenyAllyRequest(Session session, string user_id, SuccessCallback callback)
		{
			AllyDenycallback = callback;
			session.DenyFriendRequest(user_id);
		}

		public static void DenyAllyRequestCallback(TFWebFileResponse response)
		{
			if (AllyDenycallback != null)
			{
				ResponseFlag flag = ResponseFlag.Error;
				if (response.StatusCode == HttpStatusCode.OK && response.Data.IndexOf("success") != -1 && response.Data.IndexOf("True") != -1)
				{
					flag = ResponseFlag.Success;
				}
				AllyDenycallback(flag);
				AllyDenycallback = null;
			}
		}

		public static void RemoveFromTheAllies(Session session, string user_id, SuccessCallback callback)
		{
			DeleteAllycallback = callback;
			session.RemoveFriend(user_id);
		}

		public static void RemoveAllyCallback(TFWebFileResponse response)
		{
			if (DeleteAllycallback != null)
			{
				ResponseFlag flag = ResponseFlag.Error;
				if (response.StatusCode == HttpStatusCode.OK && response.Data.IndexOf("success") != -1 && response.Data.IndexOf("True") != -1)
				{
					flag = ResponseFlag.Success;
				}
				DeleteAllycallback(flag);
				DeleteAllycallback = null;
			}
		}

		public static void UpdateMyAllyInfo(Session session, bool resetHelpCount, bool resetAnonymousHelpCount, SuccessCallback callback)
		{
			TFServer.JsonResponseHandler callback2 = delegate(Dictionary<string, object> data, HttpStatusCode status)
			{
				if (data == null)
				{
					callback(ResponseFlag.Error);
				}
				else if (status == HttpStatusCode.OK && data["success"].ToString() == "True")
				{
					callback(ResponseFlag.Success);
				}
				else if (status == HttpStatusCode.NotFound)
				{
					callback(ResponseFlag.None);
				}
				else
				{
					callback(ResponseFlag.Error);
				}
			};
			session.Server.Friend_update_myinfo(resetHelpCount, resetAnonymousHelpCount, callback2);
		}

		public static void UseTheAlly(Session session, string user_id, SuccessCallback callback)
		{
			TFServer.JsonResponseHandler callback2 = delegate(Dictionary<string, object> data, HttpStatusCode status)
			{
				if (data == null)
				{
					callback(ResponseFlag.Error);
				}
				else if (status == HttpStatusCode.OK && data["success"].ToString() == "True")
				{
					callback(ResponseFlag.Success);
				}
				else if (status == HttpStatusCode.NotFound)
				{
					callback(ResponseFlag.None);
				}
				else
				{
					callback(ResponseFlag.Error);
				}
			};
			session.Server.Friend_use_friend(user_id, callback2);
		}

		public static void UseThePlayer(Session session, string user_id, SuccessCallback callback)
		{
			TFServer.JsonResponseHandler callback2 = delegate(Dictionary<string, object> data, HttpStatusCode status)
			{
				if (data == null)
				{
					callback(ResponseFlag.Error);
				}
				else if (status == HttpStatusCode.OK && data["success"].ToString() == "True")
				{
					callback(ResponseFlag.Success);
				}
				else if (status == HttpStatusCode.NotFound)
				{
					callback(ResponseFlag.None);
				}
				else
				{
					callback(ResponseFlag.Error);
				}
			};
			session.Server.Friend_use_player(user_id, callback2);
		}

		public static void RetrieveAllyInformation(Session session, string user_id, AllyDataCallback callback)
		{
			TFServer.JsonResponseHandler callback2 = delegate(Dictionary<string, object> data, HttpStatusCode status)
			{
				if (data == null)
				{
					callback(null, ResponseFlag.Error);
				}
				else if (status == HttpStatusCode.OK && data["success"].ToString() == "True")
				{
					callback(DecodeAllyInfo(null, AllyStatus.STATUS_NONE, (string)data["data"]), ResponseFlag.Success);
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
			session.Server.Friend_get_userinfo(user_id, callback2);
		}

		public static void GetAllyStatus(Session session, AllyDataCallback callback)
		{
			TFServer.JsonResponseHandler jsonResponseHandler = delegate(Dictionary<string, object> data, HttpStatusCode status)
			{
				if (data == null)
				{
					callback(null, ResponseFlag.Error);
				}
				else if (status == HttpStatusCode.OK && data["success"].ToString() == "True")
				{
					Dictionary<string, object> dictionary = (Dictionary<string, object>)data["data"];
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
		}

		public static void CreateAllyUser(Session session, string name, string icon, string deck, float deckRank, string landscapes, string leader, int leaderLevel, int maxLevel, AllyDataCallback callback)
		{
			TFServer.JsonResponseHandler jsonResponseHandler = delegate(Dictionary<string, object> data, HttpStatusCode status)
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
						if (data["success"].ToString() == "True")
						{
							Dictionary<string, object> dictionary = (Dictionary<string, object>)data["data"];
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
			string text = Convert.ToBase64String(Encoding.UTF8.GetBytes(deck));
		}

		private static List<AllyData> DecodeAllyInfoOld(string myid, AllyStatus friendstatus, string json_data)
		{
			List<AllyData> list = new List<AllyData>();
			List<string> list2 = new List<string>();
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			List<object> list3 = (List<object>)Json.Deserialize(json_data);
			foreach (object item3 in list3)
			{
				Dictionary<string, object> dictionary2 = (Dictionary<string, object>)item3;
				if (!dictionary2.ContainsKey("fields"))
				{
					continue;
				}
				Dictionary<string, object> dictionary3 = (Dictionary<string, object>)dictionary2["fields"];
				string text = Convert.ToString(dictionary3["requesting_user_id"]);
				string text2 = Convert.ToString(dictionary3["requested_user_id"]);
				int num = Convert.ToInt32(dictionary3["status"]);
				if (num == (int)friendstatus)
				{
					if (text != myid && !list2.Contains(text) && dictionary3["json_dataS"].ToString().Length != 0)
					{
						string text3 = dictionary3["json_dataS"].ToString();
						Dictionary<string, object> dictionary4 = new Dictionary<string, object>();
						dictionary4.Add("id", dictionary3["requesting_user_id"]);
						string text4 = dictionary3["json_dataS"].ToString();
						string text5 = text4.Substring(0, text4.IndexOf("\"landscapes"));
						text5 += "}";
						string text6 = text4.Substring(text4.IndexOf("landscapes") + "landscapes\"".Length);
						string text7 = text6.Substring(text6.IndexOf(":") + 1);
						string text8 = text7.Substring(text7.IndexOf("\"") + 1);
						string value = text8.Substring(0, text8.LastIndexOf("\""));
						Dictionary<string, object> dictionary5 = (Dictionary<string, object>)Json.Deserialize(text5);
						dictionary4.Add("name", dictionary5["name"]);
						dictionary4.Add("icon", dictionary5["icon"]);
						dictionary4.Add("leader", dictionary5["leader"]);
						dictionary4.Add("level", dictionary3["rankxpS"]);
						dictionary4.Add("helpcount", dictionary3["helpcountS"]);
						dictionary4.Add("anonymoushelpcount", dictionary3["anonymoushelpcount"]);
						dictionary4.Add("helpercreatureid", dictionary3["helpercreatureid"]);
						dictionary4.Add("helpercreature", dictionary3["helpercreature"]);
						dictionary4.Add("landscapes", value);
						AllyData item = new AllyData(dictionary4);
						list.Add(item);
						list2.Add(dictionary3["requesting_user_id"].ToString());
					}
					if (text2 != myid && !list2.Contains(text2) && dictionary3["json_dataR"].ToString().Length != 0)
					{
						Dictionary<string, object> dictionary6 = new Dictionary<string, object>();
						dictionary6.Add("id", dictionary3["requested_user_id"]);
						string text9 = dictionary3["json_dataR"].ToString();
						string text10 = text9.Substring(0, text9.IndexOf("\"landscapes"));
						text10 += "}";
						string text11 = text9.Substring(text9.IndexOf("landscapes") + "landscapes\"".Length);
						string text12 = text11.Substring(text11.IndexOf(":") + 1);
						string text13 = text12.Substring(text12.IndexOf("\"") + 1);
						string value2 = text13.Substring(0, text13.LastIndexOf("\""));
						Dictionary<string, object> dictionary7 = (Dictionary<string, object>)Json.Deserialize(text10);
						dictionary6.Add("name", dictionary7["name"]);
						dictionary6.Add("icon", dictionary7["icon"]);
						dictionary6.Add("leader", dictionary7["leader"]);
						dictionary6.Add("level", dictionary3["rankxpR"]);
						dictionary6.Add("helpcount", dictionary3["helpcountR"]);
						dictionary6.Add("anonymoushelpcount", dictionary3["anonymoushelpcount"]);
						dictionary6.Add("helpercreatureid", dictionary3["helpercreatureid"]);
						dictionary6.Add("helpercreature", dictionary3["helpercreature"]);
						dictionary6.Add("landscapes", value2);
						AllyData item2 = new AllyData(dictionary6);
						list.Add(item2);
						list2.Add(dictionary3["requested_user_id"].ToString());
					}
				}
			}
			return list;
		}

		private static List<AllyData> DecodeAllyInfo(string myid, AllyStatus friendstatus, string json_data)
		{
			List<AllyData> list = new List<AllyData>();
			List<string> list2 = new List<string>();
			List<object> list3 = (List<object>)Json.Deserialize(json_data);
			if (list3 != null)
			{
				foreach (object item3 in list3)
				{
					Dictionary<string, object> dictionary2 = (Dictionary<string, object>)item3;
					if (dictionary2.ContainsKey("fields"))
					{
						Dictionary<string, object> dictionary3 = (Dictionary<string, object>)dictionary2["fields"];
						string item = Convert.ToString(dictionary3["user_id"]);
						if (!list2.Contains(item) && dictionary3.ContainsKey("user_id") && dictionary3.ContainsKey("name") && dictionary3.ContainsKey("icon") && dictionary3.ContainsKey("rankxp") && dictionary3.ContainsKey("ally") && dictionary3.ContainsKey("helpcount") && dictionary3.ContainsKey("anonymoushelpcount") && dictionary3.ContainsKey("helpercreatureid") && dictionary3.ContainsKey("helpercreature") && dictionary3.ContainsKey("landscapes") && dictionary3.ContainsKey("sincelastactivedate"))
						{
							Dictionary<string, object> dictionary4 = new Dictionary<string, object>();
							dictionary4.Add("id", dictionary3["user_id"]);
							dictionary4.Add("name", dictionary3["name"]);
							dictionary4.Add("icon", dictionary3["icon"]);
							dictionary4.Add("level", dictionary3["rankxp"]);
							dictionary4.Add("ally", dictionary3["ally"]);
							dictionary4.Add("helpcount", dictionary3["helpcount"]);
							dictionary4.Add("anonymoushelpcount", dictionary3["anonymoushelpcount"]);
							dictionary4.Add("helpercreatureid", dictionary3["helpercreatureid"]);
							dictionary4.Add("helpercreature", dictionary3["helpercreature"]);
							dictionary4.Add("landscapes", dictionary3["landscapes"]);
							dictionary4.Add("sincelastactivedate", dictionary3["sincelastactivedate"]);
							AllyData item2 = new AllyData(dictionary4);
							list.Add(item2);
							list2.Add(dictionary3["user_id"].ToString());
						}
					}
				}
				return list;
			}
			return list;
		}

		private static AllyData DecodeOneAllyInfoOld(string user_id, AllyStatus friendstatus, string json_data)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			List<object> list = (List<object>)Json.Deserialize(json_data);
			foreach (object item in list)
			{
				Dictionary<string, object> dictionary2 = (Dictionary<string, object>)item;
				if (dictionary2.ContainsKey("fields"))
				{
					Dictionary<string, object> dictionary3 = (Dictionary<string, object>)dictionary2["fields"];
					string text = Convert.ToString(dictionary3["requesting_user_id"]);
					string text2 = Convert.ToString(dictionary3["requested_user_id"]);
					int num = Convert.ToInt32(dictionary3["status"]);
					if (num == (int)friendstatus && user_id == text)
					{
						Dictionary<string, object> dictionary4 = new Dictionary<string, object>();
						dictionary4.Add("id", dictionary3["requesting_user_id"]);
						string text3 = dictionary3["json_dataS"].ToString();
						string text4 = text3.Substring(0, text3.IndexOf("\"landscapes"));
						text4 += "}";
						string text5 = text3.Substring(text3.IndexOf("landscapes") + "landscapes\"".Length);
						string text6 = text5.Substring(text5.IndexOf(":") + 1);
						string text7 = text6.Substring(text6.IndexOf("\"") + 1);
						string value = text7.Substring(0, text7.LastIndexOf("\""));
						Dictionary<string, object> dictionary5 = (Dictionary<string, object>)Json.Deserialize(text4);
						dictionary4.Add("name", dictionary5["name"]);
						dictionary4.Add("icon", dictionary5["icon"]);
						dictionary4.Add("leader", dictionary5["leader"]);
						dictionary4.Add("level", dictionary3["rankxpS"]);
						dictionary4.Add("helpcount", dictionary3["helpcountS"]);
						dictionary4.Add("anonymoushelpcount", dictionary3["anonymoushelpcount"]);
						dictionary4.Add("helpercreatureid", dictionary3["helpercreatureid"]);
						dictionary4.Add("helpercreature", dictionary3["helpercreature"]);
						dictionary4.Add("landscapes", value);
						return new AllyData(dictionary4);
					}
					if (num == (int)friendstatus && user_id == text2)
					{
						Dictionary<string, object> dictionary6 = new Dictionary<string, object>();
						dictionary6.Add("id", dictionary3["requested_user_id"]);
						string text8 = dictionary3["json_dataR"].ToString();
						string text9 = text8.Substring(0, text8.IndexOf("\"landscapes"));
						text9 += "}";
						string text10 = text8.Substring(text8.IndexOf("landscapes") + "landscapes\"".Length);
						string text11 = text10.Substring(text10.IndexOf(":") + 1);
						string text12 = text11.Substring(text11.IndexOf("\"") + 1);
						string value2 = text12.Substring(0, text12.LastIndexOf("\""));
						Dictionary<string, object> dictionary7 = (Dictionary<string, object>)Json.Deserialize(text9);
						dictionary6.Add("name", dictionary7["name"]);
						dictionary6.Add("icon", dictionary7["icon"]);
						dictionary6.Add("leader", dictionary7["leader"]);
						dictionary6.Add("level", dictionary3["rankxpR"]);
						dictionary6.Add("helpcount", dictionary3["helpcountR"]);
						dictionary6.Add("anonymoushelpcount", dictionary3["anonymoushelpcount"]);
						dictionary6.Add("helpercreatureid", dictionary3["helpercreatureid"]);
						dictionary6.Add("helpercreature", dictionary3["helpercreature"]);
						dictionary6.Add("landscapes", value2);
						return new AllyData(dictionary6);
					}
				}
			}
			return null;
		}

		public static void FakeFriendRequestReceive()
		{
			Session theSession = SessionManager.Instance.theSession;
			TFServer.JsonResponseHandler handler = delegate(Dictionary<string, object> data, HttpStatusCode status)
			{
				if (status != HttpStatusCode.OK)
				{
				}
			};
			theSession.Server.Friend_fake_request_with_myinfo(handler);
		}
	}
}
