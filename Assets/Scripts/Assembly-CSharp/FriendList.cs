#define ASSERTS_ON
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class FriendList : MonoBehaviour
{
	public class GameFriend
	{
		public string ID;

		public string FirstName;

		public string LastName;

		public string AvatarURL;

		public int Level;

		public int Coins;

		public int Gems;

		public int etc;

		public bool owned;

		public bool invited;

		public bool inviteCheckMark;

		public bool needInviteDisplay;

		public string Name
		{
			get
			{
				return FirstName + " " + LastName;
			}
		}
	}

	public GameObject TemplateFriendItem;

	public GameObject Tab_FriendList;

	public GameObject Tab_FriendInvitaion;

	public GameObject Tab_InviteRewards;

	public GameObject Tab_Mail;

	public GameObject Tab_AllyBox;

	public GameObject ButtonInviteAll;

	public GameObject FacebookNotConnected;

	public UIStreamingGrid FriendsUIGrid;

	private List<string> mSendInviteIDlist = new List<string>();

	private List<string> mNewInviteIDList = new List<string>();

	private UIStreamingGridDataSource<GameFriend> mFriendsGridDataSource = new UIStreamingGridDataSource<GameFriend>();

	private List<GameFriend> GameFriendList = new List<GameFriend>();

	private bool mFriendListNeedsRefresh = true;

	private bool mUsingFacebookAccount;

	public static event Action<List<GameFriend>> GetFriendEvent;

	public static event Action<string> GetFriendFailEvent;

	public void Start()
	{
		LoadInvitedListLocal();
		PlayerInfoScript instance = Singleton<PlayerInfoScript>.Instance;
		mUsingFacebookAccount = instance.IsFacebookLogin();
		if (mUsingFacebookAccount)
		{
			if (mFriendListNeedsRefresh)
			{
			}
			Tab_FriendInvitaion.GetComponent<TabButtonHandler>().Clicked += InviteTabClicked;
			Tab_InviteRewards.GetComponent<TabButtonHandler>().Clicked += InviteRewardTabClicked;
			ButtonInviteAll.GetComponent<TabButtonHandler>().Clicked += SendInviteClicked;
			KFFSocialManager.FB_InviteResultEvent += FB_InviteDone;
		}
	}

	public void OnAwake()
	{
		if (mFriendListNeedsRefresh)
		{
			GetFriendList();
		}
	}

	public void Update()
	{
		foreach (GameFriend gameFriend in GameFriendList)
		{
			if (gameFriend.inviteCheckMark)
			{
				ButtonInviteAll.SetActive(true);
				return;
			}
		}
		ButtonInviteAll.SetActive(false);
	}

	public void Unload()
	{
		mFriendsGridDataSource.Clear();
		Tab_FriendInvitaion.GetComponent<TabButtonHandler>().Clicked -= InviteTabClicked;
		Tab_InviteRewards.GetComponent<TabButtonHandler>().Clicked -= InviteRewardTabClicked;
		ButtonInviteAll.GetComponent<TabButtonHandler>().Clicked -= SendInviteClicked;
		KFFSocialManager.FB_InviteResultEvent -= FB_InviteDone;
	}

	public void PopulateGrid()
	{
		mFriendsGridDataSource.Init(FriendsUIGrid, TemplateFriendItem, GameFriendList.AsReadOnly(), false, true);
	}

	public void Clear()
	{
		mFriendsGridDataSource.Clear();
	}

	private void HandleOnInviteEvent(FriendListItem obj)
	{
	}

	public void InviteTabClicked()
	{
		PopulateGrid();
		ButtonInviteAll.SetActive(false);
	}

	public void OnInviteFriendButtonClicked()
	{
		foreach (GameFriend gameFriend in GameFriendList)
		{
			gameFriend.needInviteDisplay = true;
		}
	}

	public void InviteRewardTabClicked()
	{
		mFriendsGridDataSource.Clear();
	}

	public void SendInviteClicked()
	{
		mSendInviteIDlist.Clear();
		foreach (GameFriend gameFriend in GameFriendList)
		{
			if (gameFriend.inviteCheckMark)
			{
				mSendInviteIDlist.Add(gameFriend.ID);
			}
		}
	}

	private GameFriend CreateGameFriend(Dictionary<string, object> friend)
	{
		GameFriend gameFriend = new GameFriend();
		TFUtils.Assert(friend.ContainsKey("id"));
		gameFriend.ID = friend["id"] as string;
		TFUtils.Assert(friend.ContainsKey("first_name"));
		TFUtils.Assert(friend.ContainsKey("last_name"));
		gameFriend.FirstName = friend["first_name"] as string;
		gameFriend.LastName = friend["last_name"] as string;
		TFUtils.Assert(friend.ContainsKey("picture"));
		Dictionary<string, object> dictionary = friend["picture"] as Dictionary<string, object>;
		TFUtils.Assert(dictionary.ContainsKey("data"));
		Dictionary<string, object> dictionary2 = dictionary["data"] as Dictionary<string, object>;
		TFUtils.Assert(dictionary2.ContainsKey("url"));
		gameFriend.AvatarURL = dictionary2["url"] as string;
		return gameFriend;
	}

	public void GetFriendList()
	{
		if ((KFFSocialManager.Status_FB & KFFSocialManager.APIStatus.PROFILE_AQUIRED) == 0)
		{
			if (FriendList.GetFriendFailEvent != null)
			{
				FriendList.GetFriendFailEvent("profile no loaded!");
			}
			return;
		}
		GameFriendList.Clear();
		mSendInviteIDlist.Clear();
		List<object> list = Singleton<KFFSocialManager>.Instance.FB_GetInvitableFriendList();
		List<object> list2 = Singleton<KFFSocialManager>.Instance.FB_GetInvitedFriendList();
		TFUtils.DebugLog("H: FBInvitable/Invited Friends : " + list.Count + "/" + list2.Count);
		foreach (Dictionary<string, object> item in list)
		{
			GameFriend gameFriend = CreateGameFriend(item);
			gameFriend.invited = false;
			GameFriendList.Add(gameFriend);
		}
		foreach (Dictionary<string, object> item2 in list2)
		{
			GameFriend gameFriend2 = CreateGameFriend(item2);
			gameFriend2.owned = true;
			GameFriendList.Add(gameFriend2);
			mSendInviteIDlist.Add(gameFriend2.ID);
			TFUtils.DebugLog("H: Not Invitable : " + gameFriend2.FirstName + gameFriend2.LastName);
		}
		GameFriendList = GameFriendList.OrderBy((GameFriend x) => x.LastName).ToList();
		PopulateGrid();
		if (FriendList.GetFriendEvent != null)
		{
			FriendList.GetFriendEvent(GameFriendList);
		}
		mFriendListNeedsRefresh = false;
	}

	public void FB_InviteDone(List<object> inviteesList)
	{
		if (inviteesList == null)
		{
			return;
		}
		bool flag = false;
		mNewInviteIDList.Clear();
		foreach (object invitees in inviteesList)
		{
			bool flag2 = false;
			foreach (string item in mSendInviteIDlist)
			{
				if (item == invitees.ToString())
				{
					flag2 = true;
					break;
				}
			}
			if (!flag2)
			{
				flag = true;
				mSendInviteIDlist.Add(invitees.ToString());
				mNewInviteIDList.Add(invitees.ToString());
			}
		}
		if (flag)
		{
			Singleton<SocialController>.Instance.IncrementFriendRequestsSent(mNewInviteIDList.Count);
			SaveInvitedListLocal();
		}
	}

	private void SaveInvitedListLocal()
	{
		string data = string.Join(",", mSendInviteIDlist.ToArray());
		string asciiMask = Singleton<PlayerInfoScript>.Instance.GetPlayerCode() + " h H";
		XorCrypto xorCrypto = new XorCrypto(asciiMask);
		string filename = Path.Combine(Application.persistentDataPath, "libamt");
		string data2 = xorCrypto.Encrypt(data);
		TFUtils.WriteFile(filename, data2);
	}

	private void LoadInvitedListLocal()
	{
		string text = Path.Combine(Application.persistentDataPath, "libamt");
		if (File.Exists(text))
		{
			string data = TFUtils.ReadFile(text);
			string asciiMask = Singleton<PlayerInfoScript>.Instance.GetPlayerCode() + " h H";
			XorCrypto xorCrypto = new XorCrypto(asciiMask);
			string text2 = xorCrypto.Decrypt(data);
			string[] array = text2.Split(',');
			for (int i = 0; i < array.Length; i++)
			{
				mSendInviteIDlist.Add(array[i]);
			}
		}
		else
		{
			mSendInviteIDlist.Clear();
		}
	}
}
