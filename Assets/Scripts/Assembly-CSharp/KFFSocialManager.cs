#define ASSERTS_ON
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Facebook.Unity;
using MiniJSON;
using UnityEngine;

public class KFFSocialManager : Singleton<KFFSocialManager>
{
	public enum APIStatus
	{
		FAIL_INITIALIZATION = 1,
		INITIALIZED = 2,
		LOGGEDIN = 4,
		FAILED_LOGIN = 8,
		PROFILE_AQUIRED = 0x10,
		AVATAR_AQUIRED = 0x20
	}

	public class FB_Profile
	{
		public Texture2D Avatar;

		public string Name;

		public string UserId;

		public AccessToken accessToken;

		public List<object> Friends;

		public List<object> InvitableFriends;

		public List<object> InvitedFriends;
	}

	public enum AchievementIDs
	{
		DW_NEWGAME,
		DW_LVL_2,
		DW_LVL_10,
		DW_LVL_30,
		DW_LVL_50,
		DW_ALL_HEROS,
		DW_QUEST_5,
		DW_QUEST_10,
		DW_QUEST_50,
		DW_EARN_GEM_100,
		DW_BATTLES_4,
		DW_BATTLES_10,
		DW_BATTLES_20,
		DW_BATTLES_40,
		DW_BATTLES_100
	}

	public delegate void LoadPictureCallback(Texture texture);

	private const string LAST_FBUSER_FILE = "lastFBUser";

	private const string FB_ME_QUERY = "/me?fields=id,first_name,last_name,friends.limit(500).fields(first_name,last_name,id,picture.width(128).height(128))";

	private FB_Profile User = new FB_Profile();

	public static APIStatus Status_FB = APIStatus.FAIL_INITIALIZATION;

	private static bool FbInitCalled;

	private ISocialGamingNetworkListener Listener;

	public FB_Profile FBUser
	{
		get
		{
			return User;
		}
	}

	public static event Action FB_InitSucessfulEvent;

	public static event Action FB_InitFailEvent;

	public static event Action FB_LoginSuccessfulEvent;

	public static event Action FB_LoginExistingEvent;

	public static event Action FB_LoginUserCancelEvent;

	public static event Action<string> FB_LoginFailEvent;

	public static event Action<FB_Profile> FB_ProfileInfoLoadedEvent;

	public static event Action<Texture2D> FB_ProfileAvatarLoadedEvent;

	public static event Action FB_WallPostSuccessfulEvent;

	public static event Action FB_WallPostFailEvent;

	public static event Action<List<object>> FB_InviteResultEvent;

	public void FB_Start()
	{
		if (!FbInitCalled)
		{
			//FB.Init(InitComplete);
			FbInitCalled = true;
		}
		else
		{
			InitComplete();
		}
	}

	public void FB_Disable()
	{
	}

	public bool FB_isPreviousLogin()
	{
		string text = FB_LoadIdFromLocalFile();
		return text != "ua";
	}

	public string FB_LoadIdFromLocalFile()
	{
		string result = null;
		string path = Path.Combine(Application.persistentDataPath, "lastFBUser");
		if (File.Exists(path))
		{
			result = File.ReadAllText(path);
		}
		return result;
	}

	public void FB_SaveIdToLocalFile()
	{
		string path = Path.Combine(Application.persistentDataPath, "lastFBUser");
		string contents = ((!FB.IsLoggedIn) ? "ua" : User.UserId);
		File.WriteAllText(path, contents);
	}

	public void FB_ResetId(bool clearFbLogin = true)
	{
		string path = Path.Combine(Application.persistentDataPath, "lastFBUser");
		if (clearFbLogin)
		{
			File.Delete(path);
		}
		else
		{
			File.WriteAllText(path, "ua");
		}
	}

	private IEnumerator FB_DoLogin()
	{
		while ((Status_FB & APIStatus.INITIALIZED) == 0)
		{
			yield return null;
		}
		if (FB.IsLoggedIn)
		{
			TFUtils.DebugLog("H: FB: user already logged in!");
			Status_FB |= APIStatus.LOGGEDIN;
			FB_LoadProfileInfo();
		}
		else
		{
			TFUtils.DebugLog("H: FB: Starting Authentication");
			List<string> perms = new List<string> { "email,user_friends" };
			FB.LogInWithReadPermissions(perms, LoginCallback);
		}
	}

	public void FB_Login()
	{
		StartCoroutine("FB_DoLogin");
	}

	public void FB_SendInvite()
	{
		if (!FB.IsLoggedIn)
		{
			return;
		}
		List<string> list = new List<string>();
		List<object> list2 = FB_GetInvitedFriendList();
		int num = 0;
		if (list2 != null && list2.Count > 0)
		{
			num = list2.Count;
			int num2 = 0;
			foreach (Dictionary<string, object> item2 in list2)
			{
				TFUtils.Assert(item2.ContainsKey("id"));
				string item = item2["id"] as string;
				list.Add(item);
			}
		}
		if (num != 0)
		{
			string[] excludeIds = list.ToArray();
			FB.AppRequest("Come play Dragon Wars!", null, null, excludeIds, 500, string.Empty, "Invite Your Friends!", SendInviteCallback);
		}
		else
		{
			FB.AppRequest("Come play Dragon Wars!", null, null, null, 500, string.Empty, "Invite Your Friends!", SendInviteCallback);
		}
	}

	public void FB_SendInvite(FriendList.GameFriend friend)
	{
		if (FB.IsLoggedIn)
		{
			TFUtils.DebugLog("H: FB_SendInvite to : " + friend.ID);
			List<string> list = new List<string>();
			list.Add(friend.ID);
			string[] to = list.ToArray();
			FacebookDelegate<IAppRequestResult> callback = SendInviteCallback;
			FB.AppRequest("Come play Dragon Wars!", to, null, null, null, string.Empty, string.Empty, callback);
		}
	}

	public void FB_SendInvite(List<string> playerIds)
	{
		if (FB.IsLoggedIn)
		{
			TFUtils.DebugLog("H: FB_SendInvite - number of friends : " + playerIds.Count);
			string[] to = playerIds.ToArray();
			FacebookDelegate<IAppRequestResult> callback = SendInviteCallback;
			FB.AppRequest("Come play Dragon Wars!", to, null, null, null, string.Empty, string.Empty, callback);
		}
	}

	public void FB_PostOnWall(string Caption, string PictureURL, string LinkName)
	{
		if (FB.IsLoggedIn)
		{
			FB.FeedShare(string.Empty, new Uri("http://apps.facebook.com/" + FB.AppId + "/?challenge_brag=" + ((!FB.IsLoggedIn) ? "guest" : User.UserId)), LinkName, Caption, string.Empty, new Uri(PictureURL), string.Empty, PostWallCallback);
		}
	}

	public void FB_Logout()
	{
		if (FB.IsLoggedIn)
		{
			TFUtils.DebugLog("H: FB: logout");
			FB.LogOut();
		}
	}

	private void FB_LoadProfileInfo()
	{
		if (FB.IsLoggedIn)
		{
			TFUtils.DebugLog("H: FB_LoadProfileInfo callse FB.API(FB_ME_QUERY) : /me?fields=id,first_name,last_name,friends.limit(500).fields(first_name,last_name,id,picture.width(128).height(128))");
			FB.API("/me?fields=id,first_name,last_name,friends.limit(500).fields(first_name,last_name,id,picture.width(128).height(128))", HttpMethod.GET, ProfileInfoCallback);
		}
	}

	private void FB_LoadProfileAvatar()
	{
		if (FB.IsLoggedIn)
		{
			LoadPicture(FBUtil.GetPictureURL("me", 128, 128), ProfilePictureCallback);
		}
	}

	public List<object> FB_GetFriendList()
	{
		if (!FB.IsLoggedIn)
		{
			return null;
		}
		return User.Friends;
	}

	public List<object> FB_GetInvitedFriendList()
	{
		if (!FB.IsLoggedIn)
		{
			return null;
		}
		return User.InvitedFriends;
	}

	public List<object> FB_GetInvitableFriendList()
	{
		if (!FB.IsLoggedIn)
		{
			return null;
		}
		return User.InvitableFriends;
	}

	private void InitComplete()
	{
		if (!FB.IsLoggedIn)
		{
			Status_FB = APIStatus.INITIALIZED;
		}
		if (KFFSocialManager.FB_InitSucessfulEvent != null)
		{
			KFFSocialManager.FB_InitSucessfulEvent();
		}
		FB.LogAppEvent("fb_mobile_activate_app");
		Status_FB |= APIStatus.INITIALIZED;
	}

	private void LoginCallback(ILoginResult result)
	{
		if (result.Error != null)
		{
			TFUtils.DebugLog("H: FB: Error ResponsFBInitFinishedEvente:\n" + result.Error);
			if (KFFSocialManager.FB_LoginFailEvent != null)
			{
				KFFSocialManager.FB_LoginFailEvent(result.Error);
			}
			return;
		}
		if (!FB.IsLoggedIn)
		{
			TFUtils.DebugLog("H: FB: Login cancelled by Player");
			if (KFFSocialManager.FB_LoginUserCancelEvent != null)
			{
				KFFSocialManager.FB_LoginUserCancelEvent();
			}
			return;
		}
		TFUtils.DebugLog("H: FB: Login was successful!");
		if (FB.IsLoggedIn)
		{
			Status_FB |= APIStatus.LOGGEDIN;
		}
		string userId = result.AccessToken.UserId;
		User.UserId = userId;
		User.accessToken = result.AccessToken;
		FB_SaveIdToLocalFile();
		Singleton<PlayerInfoScript>.Instance.SaveData.HasAuthenticated = true;
		FB_LoadProfileInfo();
	}

	private void ProfileInfoCallback(IGraphResult result)
	{
		if (result.Error != null)
		{
			FBUtil.Log("H: ProfileInfoCallback error : " + result.Error);
			FBUtil.LogError(result.Error);
			FB.API("/me?fields=id,first_name,last_name,friends.limit(500).fields(first_name,last_name,id,picture.width(128).height(128))", HttpMethod.GET, ProfileInfoCallback);
			return;
		}
		Dictionary<string, string> dictionary = FBUtil.DeserializeJSONProfile(result.RawResult);
		User.Name = dictionary["first_name"];
		User.UserId = dictionary["id"];
		User.accessToken = AccessToken.CurrentAccessToken;
		if (User.UserId.Length == 0)
		{
			User.UserId = dictionary["id"];
		}
		User.InvitableFriends = FBUtil.DeserializeJSONInvitableFriends(result.RawResult);
		User.InvitedFriends = FBUtil.DeserializeJSONInvitedFriends(result.RawResult);
		User.Friends = User.InvitedFriends.Concat(User.InvitableFriends).ToList();
		TFUtils.DebugLog("H: FB: Username = " + User.Name + ", ID = " + User.UserId);
		Status_FB |= APIStatus.PROFILE_AQUIRED;
		if (KFFSocialManager.FB_ProfileInfoLoadedEvent != null)
		{
			KFFSocialManager.FB_ProfileInfoLoadedEvent(User);
		}
		FB_LoadProfileAvatar();
	}

	private void ProfilePictureCallback(Texture texture)
	{
		FBUtil.Log("H: ProfilePictureCallback");
		if (texture == null)
		{
			LoadPicture(FBUtil.GetPictureURL("me", 128, 128), ProfilePictureCallback);
			return;
		}
		User.Avatar = (Texture2D)texture;
		Status_FB |= APIStatus.AVATAR_AQUIRED;
		if (KFFSocialManager.FB_ProfileAvatarLoadedEvent != null)
		{
			KFFSocialManager.FB_ProfileAvatarLoadedEvent(User.Avatar);
		}
		if (KFFSocialManager.FB_LoginSuccessfulEvent != null)
		{
			KFFSocialManager.FB_LoginSuccessfulEvent();
		}
	}

	private void PostWallCallback(IShareResult result)
	{
		if (result.Error != null)
		{
			if (KFFSocialManager.FB_WallPostFailEvent != null)
			{
				KFFSocialManager.FB_WallPostFailEvent();
			}
		}
		else if (KFFSocialManager.FB_WallPostSuccessfulEvent != null)
		{
			KFFSocialManager.FB_WallPostSuccessfulEvent();
		}
	}

	private void SendInviteCallback(IAppRequestResult result)
	{
		bool flag = false;
		if (result.Error != null)
		{
			flag = true;
		}
		Dictionary<string, object> dictionary = (Dictionary<string, object>)Json.Deserialize(result.RawResult);
		if (dictionary.ContainsKey("cancelled"))
		{
			if (Convert.ToBoolean(dictionary["cancelled"]))
			{
				flag = true;
			}
		}
		else if (dictionary.ContainsKey("error_code"))
		{
			flag = true;
		}
		else if (dictionary.ContainsKey("error"))
		{
			string text = Convert.ToString(dictionary["error"]);
			int num = text.IndexOf("httpResponseCode");
			int num2 = text.IndexOf("-1");
			if (num >= 0 && num2 >= 0)
			{
				flag = true;
			}
		}
		if (flag)
		{
		}
		if (KFFSocialManager.FB_InviteResultEvent != null)
		{
			if (!flag && dictionary.ContainsKey("to"))
			{
				List<object> obj = (List<object>)dictionary["to"];
				KFFSocialManager.FB_InviteResultEvent(obj);
			}
			else
			{
				KFFSocialManager.FB_InviteResultEvent(null);
			}
		}
	}

	private IEnumerator LoadPictureEnumerator(string url, LoadPictureCallback callback)
	{
		WWW www = new WWW(url);
		yield return www;
		callback(www.texture);
	}

	public void LoadPicture(string url, LoadPictureCallback callback)
	{
		FB.API(url, HttpMethod.GET, delegate(IGraphResult result)
		{
			if (result.Error != null)
			{
				FBUtil.LogError(result.Error);
			}
			else
			{
				string url2 = FBUtil.DeserializePictureURLString(result.RawResult);
				StartCoroutine(LoadPictureEnumerator(url2, callback));
			}
		});
	}

	private void Start()
	{
		FB_Start();
		Listener = new KFFGooglePlusListener();
		if (Listener != null)
		{
			Listener.OnEnable();
		}
	}

	private void OnDisable()
	{
		if (Listener != null)
		{
			Listener.OnDisable();
		}
	}

	public bool IsAuthenticated()
	{
		if (Listener == null)
		{
			return false;
		}
		return Listener.IsAuthenticated();
	}

	public void ReportAchievement(AchievementIDs aID, int aStep = 1)
	{
		if (Listener == null)
		{
			return;
		}
		List<AchievementData> database = AchievementDataManager.Instance.GetDatabase();
		string text = aID.ToString();
		foreach (AchievementData item in database)
		{
			if (item.ID == text)
			{
				Listener.ReportAchievement(item.Android_ID, aStep);
			}
		}
	}

	public void ShowAchievements()
	{
		if (Listener != null)
		{
			Listener.ShowAchievements();
		}
	}

	public void ShowBannerAchievement()
	{
		if (Listener != null)
		{
			Listener.ShowBannerAchievement();
		}
	}
}
