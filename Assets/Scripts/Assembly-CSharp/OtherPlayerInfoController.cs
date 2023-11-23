using System.Collections;
using System.Collections.Generic;
using Allies;
using UnityEngine;

public class OtherPlayerInfoController : Singleton<OtherPlayerInfoController>
{
	private enum PortraitMode
	{
		Leader,
		Facebook,
		Custom
	}

	public UITweenController CloseTween;

	public UILabel PlayerName;

	public UILabel PlayerLevel;

	public LeagueBadge PlayerLeagueBadge;

	public UITexture Flag;

	public UIWidget InviteButton;

	public UILabel SentInvite;

	public UIWidget AlreadyAlly;

	public GameObject DuelButton;

	public GameObject BlockButton;

	public UITweenController ShowTween;

	private ChatMetaData mMetaData;

	private static List<AllyData> alliesList;

	public void Show(ChatMetaData info)
	{
		mMetaData = info;
		PlayerLeagueBadge.NameLabel.text = string.Empty;
		PlayerLeagueBadge.RankNameLabel.text = string.Empty;
		PlayerLeagueBadge.RankNumberLabel.text = string.Empty;
		PortraitMode portraitMode = ((mMetaData.PortraitId == null) ? ((mMetaData.FacebookId != string.Empty && Singleton<PlayerInfoScript>.Instance.IsFacebookLogin()) ? PortraitMode.Facebook : PortraitMode.Leader) : ((mMetaData.PortraitId == "Facebook") ? ((mMetaData.FacebookId != string.Empty && Singleton<PlayerInfoScript>.Instance.IsFacebookLogin()) ? PortraitMode.Facebook : PortraitMode.Leader) : ((!(mMetaData.PortraitId == "Default")) ? PortraitMode.Custom : PortraitMode.Leader)));
		if (portraitMode == PortraitMode.Facebook)
		{
			PlayerLeagueBadge.PopulateOtherPlayerData(mMetaData.Name, string.Empty, mMetaData.CurrentLeague, mMetaData.BestLeague, null);
		}
		else
		{
			string text = null;
			if (portraitMode == PortraitMode.Custom)
			{
				PlayerPortraitData data = PlayerPortraitDataManager.Instance.GetData(mMetaData.PortraitId);
				if (data != null)
				{
					text = data.Texture;
				}
			}
			if (text == null)
			{
				LeaderData data2 = LeaderDataManager.Instance.GetData(mMetaData.Leader);
				text = data2.PortraitTexture;
			}
			PlayerLeagueBadge.PopulateOtherPlayerData(mMetaData.Name, text, mMetaData.CurrentLeague, mMetaData.BestLeague, null);
		}
		PlayerName.text = mMetaData.Name;
		PlayerLevel.text = Language.Get("!!RANK") + " " + mMetaData.Level;
		ShowInviteButtonIfAppropriate();
		if (mMetaData.UserId == Singleton<PlayerInfoScript>.Instance.GetPlayerCode())
		{
			DuelButton.SetActive(false);
			BlockButton.SetActive(false);
		}
		else
		{
			DuelButton.SetActive(true);
			BlockButton.SetActive(true);
		}
		ShowTween.Play();
	}
	private void ShowInviteButtonIfAppropriate()
	{
		ShowBasedOnCurrentAllyList();
		Session theSession = SessionManager.Instance.theSession;
		Ally.GetAlliesList(theSession, AlliesListCallback);
	}

	private void OnZapYes()
	{
	}

	private void OnZapNo()
	{
	}

	private void ShowBasedOnCurrentAllyList()
	{
		if (mMetaData.UserId == Singleton<PlayerInfoScript>.Instance.GetPlayerCode())
		{
			InviteButton.gameObject.SetActive(false);
			AlreadyAlly.gameObject.SetActive(false);
			SentInvite.gameObject.SetActive(false);
		}
		else
		{
			if (alliesList == null)
			{
				return;
			}
			bool flag = false;
			foreach (AllyData allies in alliesList)
			{
				if (allies.Id == mMetaData.UserId)
				{
					flag = true;
				}
			}
			InviteButton.gameObject.SetActive(!flag);
			AlreadyAlly.gameObject.SetActive(flag);
			SentInvite.gameObject.SetActive(false);
		}
	}

	private void AlliesListCallback(List<AllyData> allieslist, ResponseFlag flag)
	{
		if (flag == ResponseFlag.Success)
		{
			alliesList = allieslist;
		}
		else
		{
			alliesList = new List<AllyData>();
		}
		ShowBasedOnCurrentAllyList();
	}

	public void SendInvite()
	{
		InviteButton.gameObject.SetActive(false);
		SentInvite.gameObject.SetActive(true);
		SentInvite.text = string.Format(Language.Get("!!ALLY_INVITE_SENT"), mMetaData.Name);
		string user_id = Singleton<PlayerInfoScript>.Instance.ConvertFormattedPlayerCodeToInternal(mMetaData.UserId);
		Ally.AllyRequest(SessionManager.Instance.theSession, user_id, null);
	}

	public void OnClickIgnore()
	{
		string body = KFFLocalization.Get("!!IGNORE_PLAYER_PROMPT").Replace("<val1>", mMetaData.Name);
		Singleton<SimplePopupController>.Instance.ShowPrompt(string.Empty, body, ConfirmIgnore, null);
	}

	private void ConfirmIgnore()
	{
		if (!Singleton<PlayerInfoScript>.Instance.SaveData.IgnoredPlayers.ContainsKey(mMetaData.UserId))
		{
			Singleton<PlayerInfoScript>.Instance.SaveData.IgnoredPlayers.Add(mMetaData.UserId, null);
			Singleton<PlayerInfoScript>.Instance.Save();
		}
		ChatWindowController.Instance.PurgeBlockedUser(mMetaData.UserId);
		CloseTween.Play();
	}

	public void OnClickMatchChallenge()
	{
		Singleton<PVPSendMatchRequestController>.Instance.SendMatchRequest(mMetaData);
	}
}
