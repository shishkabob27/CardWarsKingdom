using System;
using System.Collections;
using UnityEngine;

public class FriendListItem : UIStreamingGridListItem
{
	private delegate void LoadPictureCallback(Texture texture);

	public GameObject LayoutFriendInvited;

	public GameObject LayoutFriendInvitable;

	public GameObject LayoutFriendInviteBack;

	public GameObject LayoutFriendInviteCheckmark;

	public GameObject LayoutFiiendInviteLabel;

	public GameObject LayoutFriendOwned;

	public GameObject LayoutCreature;

	public UILabel LabelRankPoints;

	public UILabel LabelCurrentRank;

	public UILabel LabelUserRecord;

	public UILabel LabelName;

	public UITexture Avatar;

	public bool IsInvited;

	public FriendList.GameFriend Friend { get; private set; }

	public event Action<FriendListItem> OnInviteEvent;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void OnClick()
	{
	}

	public void ActivateInviteLabel(bool activate)
	{
		LayoutFriendInviteBack.SetActive(activate);
		LayoutFiiendInviteLabel.SetActive(!activate);
	}

	public void SetStatus()
	{
		if (Friend.invited)
		{
			IsInvited = Friend.invited;
		}
		LayoutFriendInvited.SetActive(IsInvited);
		LayoutFriendInvitable.SetActive(!IsInvited && !Friend.owned);
		LayoutFriendInviteCheckmark.SetActive(IsInvited);
		LayoutFriendOwned.SetActive(Friend.owned);
	}

	public override void Populate(object dataObj)
	{
		Friend = dataObj as FriendList.GameFriend;
		StartCoroutine(LoadPicture(Friend.AvatarURL, OnLoadPictureFinished));
		LabelName.text = Friend.Name;
		LabelCurrentRank.text = string.Empty;
		LabelRankPoints.text = string.Empty;
		LabelUserRecord.text = string.Empty;
		if (null != LayoutCreature)
		{
			LayoutCreature.SetActive(false);
		}
		SetStatus();
	}

	public override void Unload()
	{
		base.Unload();
	}

	private IEnumerator LoadPicture(string url, LoadPictureCallback callback)
	{
		WWW www = new WWW(url);
		yield return www;
		callback(www.texture);
	}

	private void OnLoadPictureFinished(Texture texture)
	{
		Avatar.mainTexture = texture;
	}
}
