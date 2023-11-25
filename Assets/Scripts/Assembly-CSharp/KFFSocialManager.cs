#define ASSERTS_ON
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
			FbInitCalled = true;
		}
		else
		{
			InitComplete();
		}
	}

	public List<object> FB_GetInvitedFriendList()
	{
		return null;
	}

	public List<object> FB_GetInvitableFriendList()
	{
		return null;
	}

	private void InitComplete()
	{
		Status_FB = APIStatus.INITIALIZED;
		if (KFFSocialManager.FB_InitSucessfulEvent != null)
		{
			KFFSocialManager.FB_InitSucessfulEvent();
		}
		Status_FB |= APIStatus.INITIALIZED;
	}

	private void Start()
	{
		FB_Start();
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
}
