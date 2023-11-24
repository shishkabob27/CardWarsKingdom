using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class SQServer
{
	public const string SECRET_KEY = "xc0u18^g0!ac3k%0+2vgglmnr1)x^!o(n6@$m3t^(7l!(#kv!-";

	private const string CROOZ_SUPPORT_LOGIN = "api/GetLoginKey";

	private const string FB_LOGIN_URL = "account/fbTokenAuth/";

	private const string FB_CONNECT_URL_ANDROID = "account/fb_connect_android/";

	private const string FB_CONNECT_URL = "account/fb_connect/";

	private const string GC_LOGIN_URL = "account/gcAuth/";

	private const string PRE_AUTH_URL = "account/preAuth/";

	private const string CONFIRM_PURCHASE_URL = "billing/confirm_purchase/";

	private const string GET_UNCONFIRMED_PURCHASE_URL = "billing/get_unconfirmed_hard_currency/";

	private const string RECORD_PURCHASE_URL = "billing/record_purchase/";

	private const string GET_PURCHASES_URL = "billing/getPurchases/";

	private const string GET_STORE_PRODUCTS = "billing/iap_products/";

	private const string GET_SERVER_TIME = "time/";

	private const int MP_VERSION = 0;

	private const string POST_NEW_PLAYER_URL = "multiplayer/new_player/";

	private const string GET_PLAYER_URL = "multiplayer/player/";

	private const string POST_UPDATE_PLAYER_URL = "multiplayer/update_player/";

	private const string POST_UPDATE_DECK_URL = "multiplayer/update_deck_name/";

	private const string GET_PLAYER_RECORD_URL = "multiplayer/player_record/";

	private const string POST_PERSONAL_RECORD_URL = "multiplayer/record/";

	private const string POST_TOURNAMENT_PLAYER_RESULT = "multiplayer/tournament/player/";

	private const string TOURNAMENT_END_DATE = "multiplayer/tournament/expiration/";

	private const string GET_LEADERBOARD_URL = "multiplayer/active_leaderboard/";

	private const string POST_MATCHMAKE_URL = "multiplayer/matchmake/";

	private const string POST_TOURNAMENT_REDEMPTION = "multiplayer/tournament/complete/";

	private const string POST_REDEEMCODE_CHECK = "multiplayer/redeemcodeDW/";

	private const string POST_FRIEND_MYINFO_UPDATE_URL = "persist/friends_update_myinfoDW/";

	private const string POST_FRIEND_REQUEST_WITH_MYINFO_URL = "persist/friends_request_withmyinfoDW/";

	private const string POST_FRIEND_CONFIRM_WITH_MYINFO_URL = "persist/friends_confirm_request_withmyinfoDW/";

	private const string POST_FRIEND_FAKEREQESTRECEIVE_URL = "persist/friendsfakereq/";

	private const string POST_FRIEND_USE_FRIEND_URL = "persist/friends_use_friendDW/";

	private const string POST_FRIEND_USE_PLAYER_URL = "persist/friends_use_playerDW/";

	private const string POST_FRIEND_HELPER_LIST_URL = "persist/friends_find_candidatesDW/";

	private const string POST_FRIEND_GETINFO_URL = "persist/friends_informationDW/";

	private const string POST_USER_HISTORY_URL = "persist/user_history/";

	private const string POST_USER_HISTORY2_URL = "persist/user_history2/";

	private const string POST_USER_HISTORY_ANDROID_URL = "persist/user_history_android/";

	private const string POST_USER_ACTION2 = "persist/user_action2/";

	private const string POST_USER_GETCC = "persist/getcc/";

	public const string POST_PLACE_ME_LEADERBOARD = "dw_leaderboard/placeme/";

	public const string POST_REGISTER_RESULT = "dw_leaderboard/registerresult/";

	public const string POST_FETCH_ENTRIES = "dw_leaderboard/fetchentries/";

	public const string POST_HAS_ENDED = "dw_leaderboard/hasended/";

	private TFServer tfServer;

	private string nonce = string.Empty;

	public static string IAP_VERIFICATION_SERVER_URL
	{
		get
		{
			return "http://dw-live.kffgames.com";
		}
	}

	public SQServer(CookieContainer cookies)
	{
		tfServer = new TFServer(cookies, SQSettings.PATCHING_FILE_LIMIT);
	}

	public void SetLoggedOut()
	{
		tfServer.ShortCircuitAllRequests();
	}

	public bool IsNetworkError(Dictionary<string, object> response)
	{
		return response.ContainsKey("error") && "Network error".Equals(response["error"]);
	}

	public void PreAuth(TFServer.JsonResponseHandler callback)
	{
		GetToJSON(SQSettings.SERVER_URL + "account/preAuth/", callback);
	}

	public void GetToJSON(string url, TFServer.JsonResponseHandler callback)
	{
		tfServer.GetToJSON(url, callback);
	}

	public void GetPurchases(string playerId, TFServer.JsonResponseHandler callback)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["playerId"] = playerId;
		dictionary["signature"] = SignDictionary(dictionary, string.Empty, "xc0u18^g0!ac3k%0+2vgglmnr1)x^!o(n6@$m3t^(7l!(#kv!-");
		tfServer.PostToJSON(SQSettings.SERVER_URL + "billing/getPurchases/", dictionary, callback);
	}

	public void GetUnconfirmedPurchases(string playerId, TFServer.JsonResponseHandler callback)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["player_id"] = playerId;
		dictionary["signature"] = SignDictionary(dictionary, string.Empty, "xc0u18^g0!ac3k%0+2vgglmnr1)x^!o(n6@$m3t^(7l!(#kv!-");
		tfServer.PostToJSON(SQSettings.SERVER_URL + "billing/get_unconfirmed_hard_currency/", dictionary, callback);
	}

	public void SavePurchase(string store_id, string store, string sandbox, string partial, string bundle_id, string productId, string playerId, string receipt, TFServer.JsonResponseHandler callback)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["player_id"] = playerId;
		dictionary["store_id"] = store_id;
		dictionary["language"] = partial;
		dictionary["store"] = store;
		dictionary["environment"] = sandbox;
		dictionary["bundle_id"] = bundle_id;
		dictionary["product_id"] = productId;
		dictionary["receipt"] = receipt;
		dictionary["signature"] = SignDictionary(dictionary, nonce, "xc0u18^g0!ac3k%0+2vgglmnr1)x^!o(n6@$m3t^(7l!(#kv!-");
		tfServer.PostToJSON(SQSettings.SERVER_URL + "billing/record_purchase/", dictionary, callback);
	}

	public void ConfirmPurchase(string playerId, string transaction_id, TFServer.JsonResponseHandler callback)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["player_id"] = playerId;
		dictionary["transaction_id"] = transaction_id;
		dictionary["signature"] = SignDictionary(dictionary, nonce, "xc0u18^g0!ac3k%0+2vgglmnr1)x^!o(n6@$m3t^(7l!(#kv!-");
		tfServer.PostToJSON(IAP_VERIFICATION_SERVER_URL + "billing/confirm_purchase/", dictionary, callback);
	}

	public void GetStoreProducts(string store, TFServer.JsonResponseHandler callback)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["signature"] = SignDictionary(dictionary, nonce, "xc0u18^g0!ac3k%0+2vgglmnr1)x^!o(n6@$m3t^(7l!(#kv!-");
		tfServer.PostToJSON(IAP_VERIFICATION_SERVER_URL + "billing/iap_products/" + store + "/", dictionary, callback);
	}

	public void FbLogin(string playerId, string fbAccessToken, string fbExpirationDate, string nonce, TFServer.JsonResponseHandler callback)
	{
		this.nonce = nonce;
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["fb_access_token"] = fbAccessToken;
		dictionary["fb_expiration_date"] = 0;
		dictionary["gc_id"] = playerId;
		dictionary["signature"] = SignDictionary(dictionary, nonce, "xc0u18^g0!ac3k%0+2vgglmnr1)x^!o(n6@$m3t^(7l!(#kv!-");
		RuntimePlatform platform = Application.platform;
		if (Application.platform == RuntimePlatform.Android && MiscParams.AndroidUsesDifferentFBAccount)
		{
			tfServer.PostToJSON(SQSettings.SERVER_URL + "account/fb_connect_android/", dictionary, callback);
		}
		else
		{
			tfServer.PostToJSON(SQSettings.SERVER_URL + "account/fb_connect/", dictionary, callback);
		}
	}

	public void GcLogin(string playerId, string alias, string nonce, TFServer.JsonResponseHandler callback)
	{
		this.nonce = nonce;
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["player_id"] = playerId;
		dictionary["alias"] = alias;
		dictionary["signature"] = SignDictionary(dictionary, nonce, "xc0u18^g0!ac3k%0+2vgglmnr1)x^!o(n6@$m3t^(7l!(#kv!-");
		tfServer.PostToJSON(SQSettings.SERVER_URL + "account/gcAuth/", dictionary, callback);
	}

	public void GetTime(TFServer.JsonResponseHandler handler)
	{
		tfServer.GetToJSON(SQSettings.SERVER_URL + "time/", handler);
	}

	public void MultiplayerPlayerInfo(string playerId, TFServer.JsonResponseHandler callback)
	{
		tfServer.GetToJSON(SQSettings.SERVER_URL + "multiplayer/player/" + playerId, callback);
	}

	public void MultiplayerNewPlayer(string name, string icon, string deck, float deckRank, string landscapes, string helpercreature, string leader, int leaderLevel, int maxLevel, int allyboxspace, TFServer.JsonResponseHandler callback)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
        dictionary["player_id"] = PlayerInfoScript.Instance.GetPlayerCode();
        dictionary["name"] = name;
		dictionary["icon"] = icon;
		dictionary["deck"] = deck;
		dictionary["deck_rank"] = deckRank;
		dictionary["landscapes"] = landscapes;
		dictionary["helper_creature"] = helpercreature;
		dictionary["leader"] = leader;
		dictionary["leader_level"] = leaderLevel;
		dictionary["allyboxspace"] = allyboxspace;
		dictionary["level"] = maxLevel;
		dictionary["version"] = 0;
		dictionary["signature"] = SignDictionary(dictionary, nonce, "xc0u18^g0!ac3k%0+2vgglmnr1)x^!o(n6@$m3t^(7l!(#kv!-");
		tfServer.PostToJSON(SQSettings.SERVER_URL + "multiplayer/new_player/", dictionary, callback, true);
	}

	public void MultiplayerUpdateDeck(string name, string deck, int needUpdate, string landscapes, string helpercreature, string leader, int leaderLevel, int allyboxspace, TFServer.JsonResponseHandler callback)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["player_id"] = PlayerInfoScript.Instance.GetPlayerCode();
		dictionary["name"] = name;
		dictionary["deck"] = deck;
		dictionary["deck_rank"] = needUpdate;
		dictionary["landscapes"] = landscapes;
		dictionary["helper_creature"] = helpercreature;
		dictionary["leader"] = leader;
		dictionary["leader_level"] = leaderLevel;
		dictionary["allyboxspace"] = allyboxspace;
		dictionary["version"] = 0;
		dictionary["signature"] = SignDictionary(dictionary, nonce, "xc0u18^g0!ac3k%0+2vgglmnr1)x^!o(n6@$m3t^(7l!(#kv!-");
		tfServer.PostToJSON(SQSettings.SERVER_URL + "multiplayer/update_deck_name/", dictionary, callback, true);
	}

	public void MultiplayerUpdatePlayer(string name, string icon, int maxLevel, TFServer.JsonResponseHandler callback)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["name"] = name;
		dictionary["icon"] = icon;
		dictionary["level"] = maxLevel;
		dictionary["signature"] = SignDictionary(dictionary, nonce, "xc0u18^g0!ac3k%0+2vgglmnr1)x^!o(n6@$m3t^(7l!(#kv!-");
		tfServer.PostToJSON(SQSettings.SERVER_URL + "multiplayer/update_player/", dictionary, callback);
	}

	public void RedeemcodeCheck(string redeemcode, string version, string subject, string message, DateTime start_date, DateTime end_date, int soft_currency, TFServer.JsonResponseHandler callback)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["redeemcode"] = redeemcode;
		dictionary["version"] = 0;
		dictionary["subject"] = subject;
		dictionary["message"] = message;
		dictionary["start_date"] = start_date.ToString("u");
		dictionary["end_date"] = end_date.ToString("u");
		dictionary["soft_currency"] = soft_currency;
		dictionary["signature"] = SignDictionary(dictionary, nonce, "xc0u18^g0!ac3k%0+2vgglmnr1)x^!o(n6@$m3t^(7l!(#kv!-");
		tfServer.PostToJSON(SQSettings.SERVER_URL + "multiplayer/redeemcodeDW/", dictionary, callback);
	}

	public void MultiplayerExtendedRecord(string playerId, TFServer.JsonResponseHandler callback)
	{
		tfServer.GetToJSON(SQSettings.SERVER_URL + "multiplayer/player_record/" + playerId, callback);
	}

	public void MultiplayerLeaderboardPlayer(string playerId, TFServer.JsonResponseHandler callback)
	{
		tfServer.GetToJSON(SQSettings.SERVER_URL + "multiplayer/active_leaderboard/" + playerId, callback);
	}

	public void MultiplayerLeaderboardTop(TFServer.JsonResponseHandler callback)
	{
		tfServer.GetToJSON(SQSettings.SERVER_URL + "multiplayer/active_leaderboard/", callback);
	}

	public void MultiplayerGetRank(bool global, TFServer.JsonResponseHandler callback)
	{
		if (global)
		{
			tfServer.GetToJSON(SQSettings.SERVER_URL + "multiplayer/active_leaderboard/globalrank/", callback);
		}
		else
		{
			tfServer.GetToJSON(SQSettings.SERVER_URL + "multiplayer/active_leaderboard/rank/", callback);
		}
	}

	public void MultiplayerPersonalRecord(string target, TFServer.JsonResponseHandler callback)
	{
		tfServer.GetToJSON(SQSettings.SERVER_URL + "multiplayer/record/" + target, callback);
	}

	public void MultiplayerNotification(TFServer.JsonResponseHandler callback)
	{
		tfServer.PostToJSON(SQSettings.SERVER_URL + "multiplayer/record/recent/", new Dictionary<string, object>(), callback);
	}

	public void MultiplayerTournamentPlayerResult(TFServer.JsonResponseHandler callback)
	{
		tfServer.GetToJSON(SQSettings.SERVER_URL + "multiplayer/tournament/player/", callback);
	}

	public void MultiplayerFindMatch(int maxLevel, TFServer.JsonResponseHandler callback)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["level"] = maxLevel;
		dictionary["version"] = 0;
		dictionary["signature"] = SignDictionary(dictionary, nonce, "xc0u18^g0!ac3k%0+2vgglmnr1)x^!o(n6@$m3t^(7l!(#kv!-");
		tfServer.PostToJSON(SQSettings.SERVER_URL + "multiplayer/matchmake/find/", dictionary, callback);
	}

	public void MultiplayerStartMatch(string matchId, float deckRank, string leader, int leaderLevel, TFServer.JsonStringHandler callback)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["match_id"] = matchId;
		dictionary["deck_rank"] = deckRank;
		dictionary["leader"] = leader;
		dictionary["leader_level"] = leaderLevel;
		dictionary["version"] = 0;
		dictionary["signature"] = SignDictionary(dictionary, nonce, "xc0u18^g0!ac3k%0+2vgglmnr1)x^!o(n6@$m3t^(7l!(#kv!-");
		tfServer.PostToString(SQSettings.SERVER_URL + "multiplayer/matchmake/start/", dictionary, callback);
	}

	public void MultiplayerEndMatch(string matchId, bool loss, TFServer.JsonResponseHandler callback)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["match_id"] = matchId;
		dictionary["loss"] = loss;
		dictionary["signature"] = SignDictionary(dictionary, nonce, "xc0u18^g0!ac3k%0+2vgglmnr1)x^!o(n6@$m3t^(7l!(#kv!-");
		tfServer.PostToJSON(SQSettings.SERVER_URL + "multiplayer/matchmake/complete/", dictionary, callback);
	}

	public void MultiplayerCheaterTournamentEnd(TFServer.JsonResponseHandler callback)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["sync"] = true;
		dictionary["signature"] = SignDictionary(dictionary, nonce, "xc0u18^g0!ac3k%0+2vgglmnr1)x^!o(n6@$m3t^(7l!(#kv!-");
		tfServer.PostToJSON(SQSettings.SERVER_URL + "multiplayer/tournament/expiration/", dictionary, callback);
	}

	public void MultiplayerGetTournamentEnd(TFServer.JsonResponseHandler callback)
	{
		tfServer.GetToJSON(SQSettings.SERVER_URL + "multiplayer/tournament/expiration/", callback);
	}

	public void MultiplayerRedeemReward(int tournamentId, TFServer.JsonResponseHandler callback)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["tournament_id"] = tournamentId;
		dictionary["signature"] = SignDictionary(dictionary, nonce, "xc0u18^g0!ac3k%0+2vgglmnr1)x^!o(n6@$m3t^(7l!(#kv!-");
		tfServer.PostToJSON(SQSettings.SERVER_URL + "multiplayer/tournament/complete/", dictionary, callback);
	}

	public bool SessionValid()
	{
		Cookie cookie = tfServer.GetCookie(new Uri(SQSettings.SERVER_URL), "sessionid");
		return cookie != null && !cookie.Expired;
	}

	private string SignDictionary(Dictionary<string, object> data, string nonce, string secret)
	{
		//Discarded unreachable code: IL_00ad
		string text = string.Empty;
		List<string> list = new List<string>(data.Keys);
		list.Sort();
		foreach (string item in list)
		{
			text = text + item + data[item];
		}
		text += nonce;
		using (HMACSHA256 hMACSHA = new HMACSHA256())
		{
			hMACSHA.Key = Encoding.ASCII.GetBytes(secret);
			byte[] inArray = hMACSHA.ComputeHash(Encoding.ASCII.GetBytes(text));
			return Convert.ToBase64String(inArray).Replace('+', '-').Replace('/', '_');
		}
	}

	public void Friend_update_myinfo(bool helpcount, bool anonymoushelpcount, TFServer.JsonResponseHandler callback)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["resethelp"] = (helpcount ? 1 : 0);
		dictionary["resetahelp"] = (anonymoushelpcount ? 1 : 0);
		dictionary["signature"] = SignDictionary(dictionary, nonce, "xc0u18^g0!ac3k%0+2vgglmnr1)x^!o(n6@$m3t^(7l!(#kv!-");
		tfServer.PostToJSON(SQSettings.SERVER_URL + "persist/friends_update_myinfoDW/", dictionary, callback);
	}

	public void Friend_use_friend(string friend_id, TFServer.JsonResponseHandler callback)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["friendid"] = friend_id;
		dictionary["signature"] = SignDictionary(dictionary, nonce, "xc0u18^g0!ac3k%0+2vgglmnr1)x^!o(n6@$m3t^(7l!(#kv!-");
		tfServer.PostToJSON(SQSettings.SERVER_URL + "persist/friends_use_friendDW/", dictionary, callback);
	}

	public void Friend_use_player(string user_id, TFServer.JsonResponseHandler callback)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["userid"] = user_id;
		dictionary["signature"] = SignDictionary(dictionary, nonce, "xc0u18^g0!ac3k%0+2vgglmnr1)x^!o(n6@$m3t^(7l!(#kv!-");
		tfServer.PostToJSON(SQSettings.SERVER_URL + "persist/friends_use_playerDW/", dictionary, callback);
	}

	public void Friend_request_with_myinfo(string playerId, TFServer.JsonResponseHandler callback)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["player_id"] = PlayerInfoScript.Instance.GetPlayerCode();
		dictionary["invite_id"] = playerId;
		dictionary["signature"] = SignDictionary(dictionary, nonce, "xc0u18^g0!ac3k%0+2vgglmnr1)x^!o(n6@$m3t^(7l!(#kv!-");
		tfServer.PostToJSON(SQSettings.SERVER_URL + "persist/friends_request_withmyinfoDW/", dictionary, callback);
	}

	public void Friend_fake_request_with_myinfo(TFServer.JsonResponseHandler handler)
	{
		tfServer.GetToJSON(SQSettings.SERVER_URL + "persist/friendsfakereq/", handler);
	}

	public void Friend_confirm_with_myinfo(string playerId, TFServer.JsonResponseHandler callback)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["player_id"] = PlayerInfoScript.Instance.GetPlayerCode();
		dictionary["invite_id"] = playerId;
		dictionary["signature"] = SignDictionary(dictionary, nonce, "xc0u18^g0!ac3k%0+2vgglmnr1)x^!o(n6@$m3t^(7l!(#kv!-");
		tfServer.PostToJSON(SQSettings.SERVER_URL + "persist/friends_confirm_request_withmyinfoDW/", dictionary, callback);
	}

	public void Friend_get_userinfo(string playerId, TFServer.JsonResponseHandler callback)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["userid"] = playerId;
		dictionary["signature"] = SignDictionary(dictionary, nonce, "xc0u18^g0!ac3k%0+2vgglmnr1)x^!o(n6@$m3t^(7l!(#kv!-");
		tfServer.PostToJSON(SQSettings.SERVER_URL + "persist/friends_informationDW/", dictionary, callback);
	}

	public void Friend_get_helpers(string optionTarget, List<string> excludeIDs, TFServer.JsonResponseHandler callback)
	{
		string value = string.Join(",", excludeIDs.ToArray());
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["level"] = optionTarget;
		dictionary["version"] = value;
		dictionary["signature"] = SignDictionary(dictionary, nonce, "xc0u18^g0!ac3k%0+2vgglmnr1)x^!o(n6@$m3t^(7l!(#kv!-");
		tfServer.PostToJSON(SQSettings.SERVER_URL + "persist/friends_find_candidatesDW/", dictionary, callback);
	}

	public void User_currency_history2(int num, TFServer.JsonResponseHandler callback)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["number"] = num;
		dictionary["signature"] = SignDictionary(dictionary, nonce, "xc0u18^g0!ac3k%0+2vgglmnr1)x^!o(n6@$m3t^(7l!(#kv!-");
		tfServer.PostToJSON(SQSettings.SERVER_URL + "persist/user_history2/", dictionary, callback);
	}

	public void User_currency_history(string country, int transaction, int tier, int paid, int free, TFServer.JsonResponseHandler callback)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["country"] = country;
		dictionary["transaction"] = transaction.ToString();
		dictionary["tier"] = tier.ToString();
		dictionary["paid"] = paid.ToString();
		dictionary["free"] = free.ToString();
		dictionary["signature"] = SignDictionary(dictionary, nonce, "xc0u18^g0!ac3k%0+2vgglmnr1)x^!o(n6@$m3t^(7l!(#kv!-");
		tfServer.PostToJSON(SQSettings.SERVER_URL + "persist/user_history_android/", dictionary, callback);
	}

	public void User_action(int pd, int fr, int cu, int dp, int df, int dc, string us, int hd, int misc, string evt, string cc, TFServer.JsonResponseHandler callback)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["pd"] = pd.ToString();
		dictionary["fr"] = fr.ToString();
		dictionary["cu"] = cu.ToString();
		dictionary["dp"] = dp.ToString();
		dictionary["df"] = df.ToString();
		dictionary["dc"] = dc.ToString();
		dictionary["us"] = us;
		dictionary["hd"] = hd.ToString();
		dictionary["misc"] = misc.ToString();
		dictionary["evt"] = evt;
		dictionary["cc"] = cc;
		dictionary["ad"] = 1;
		dictionary["signature"] = SignDictionary(dictionary, nonce, "xc0u18^g0!ac3k%0+2vgglmnr1)x^!o(n6@$m3t^(7l!(#kv!-");
		tfServer.PostToJSON(SQSettings.SERVER_URL + "persist/user_action2/", dictionary, callback);
	}

	public void GetCC(TFServer.JsonResponseHandler handler)
	{
		tfServer.GetToJSON(SQSettings.SERVER_URL + "persist/getcc/", handler);
	}

	public void PlaceMeOnLeaderboard(string user_id, string currentSeasonID, TFServer.JsonResponseHandler callback)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["userid"] = user_id;
		dictionary["currentseasonid"] = currentSeasonID;
		dictionary["signature"] = SignDictionary(dictionary, nonce, "xc0u18^g0!ac3k%0+2vgglmnr1)x^!o(n6@$m3t^(7l!(#kv!-");
		tfServer.PostToJSON(SQSettings.SERVER_URL + "dw_leaderboard/placeme/", dictionary, callback);
	}

	public void RegisterMatchResult(string user_id, string currentSeasonID, string opnentID, bool didIWin, TFServer.JsonResponseHandler callback)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["userid"] = user_id;
		dictionary["currentseasonid"] = currentSeasonID;
		dictionary["opponentid"] = opnentID;
		dictionary["didiwin"] = didIWin.ToString();
		dictionary["signature"] = SignDictionary(dictionary, nonce, "xc0u18^g0!ac3k%0+2vgglmnr1)x^!o(n6@$m3t^(7l!(#kv!-");
		tfServer.PostToJSON(SQSettings.SERVER_URL + "dw_leaderboard/registerresult/", dictionary, callback);
	}

	public void FetchLeaderboardsEntries(int startPosition, int endPosition, TFServer.JsonResponseHandler callback)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["startpos"] = startPosition.ToString();
		dictionary["endpos"] = endPosition.ToString();
		dictionary["signature"] = SignDictionary(dictionary, nonce, "xc0u18^g0!ac3k%0+2vgglmnr1)x^!o(n6@$m3t^(7l!(#kv!-");
		tfServer.PostToJSON(SQSettings.SERVER_URL + "dw_leaderboard/fetchentries/", dictionary, callback);
	}

	public void HasSeasonEnded(string user_id, TFServer.JsonResponseHandler callback)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["userid"] = user_id;
		dictionary["signature"] = SignDictionary(dictionary, nonce, "xc0u18^g0!ac3k%0+2vgglmnr1)x^!o(n6@$m3t^(7l!(#kv!-");
		tfServer.PostToJSON(SQSettings.SERVER_URL + "dw_leaderboard/hasended/", dictionary, callback);
	}

	public void CompassSupportLogin(string project_id, string support_id, string p, string user_id, string checkkey, TFServer.JsonStringHandler callback)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["project_id"] = project_id;
		dictionary["support_id"] = support_id;
		dictionary["p"] = p;
		dictionary["user_id"] = user_id;
		dictionary["checkkey"] = checkkey;
		tfServer.PostToString(MiscParams.CompassSupportURL + "api/GetLoginKey", dictionary, callback);
	}
}
