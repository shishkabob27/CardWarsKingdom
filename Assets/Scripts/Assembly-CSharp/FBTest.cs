using System.Collections.Generic;
using UnityEngine;

public class FBTest : MonoBehaviour
{
	private FriendList _Friendlist;

	private void OnEnable()
	{
		KFFSocialManager.FB_LoginSuccessfulEvent += LoginSuccessfulEvent;
		KFFSocialManager.FB_LoginFailEvent += LoginFailEvent;
		KFFSocialManager.FB_LoginUserCancelEvent += LoginUserCancelEvent;
		FriendList.GetFriendEvent += GetFriendSucessfulEvent;
		FriendList.GetFriendFailEvent += GetFriendfailEvent;
	}

	private void OnDisable()
	{
		KFFSocialManager.FB_LoginSuccessfulEvent -= LoginSuccessfulEvent;
		KFFSocialManager.FB_LoginFailEvent -= LoginFailEvent;
		KFFSocialManager.FB_LoginUserCancelEvent -= LoginUserCancelEvent;
		FriendList.GetFriendEvent -= GetFriendSucessfulEvent;
		FriendList.GetFriendFailEvent -= GetFriendfailEvent;
	}

	private void Start()
	{
		_Friendlist = GetComponent<FriendList>();
	}

	private void GetFriendfailEvent(string a_Error)
	{
	}

	private void OnClick()
	{
	}

	private void LoginSuccessfulEvent()
	{
	}

	private void LoginFailEvent(string Error)
	{
	}

	private void LoginUserCancelEvent()
	{
	}

	private void GetFriendSucessfulEvent(List<FriendList.GameFriend> a_FriendList)
	{
		FriendList component = GetComponent<FriendList>();
		if ((bool)component)
		{
			component.GetFriendList();
		}
		if (a_FriendList != null)
		{
			Singleton<KFFSocialManager>.Instance.FB_PostOnWall("I just outscore you! Can you beat it?", "http://www.friendsmash.com/images/logo_large.jpg", "Checkout my Friend Smash greatness!");
		}
	}
}
