using System;
using System.Collections;
using System.Collections.Generic;
using Allies;
using UnityEngine;

public class BattleHistoryItemController : UIStreamingGridListItem
{
	private enum NextAction
	{
		NONE,
		WAITING,
		PROCEED,
		ERROR,
		RETRY
	}

	private const string searchUrlPre = "/games/current/videos?client_id=7396cfa9e9b9c126e67bf9b8b4d13b85911767bd&videoHistoryTag=";

	public UILabel opponentNameLabel;

	public UILabel opponentMatchSeason;

	public UILabel opponentMatchResult;

	public UILabel opponentMatchDate;

	public GameObject AddFriendButton;

	public GameObject ViewReplayButton;

	public LeagueBadge OpponentBadge;

	public UITexture opponentFlag;

	public Transform[] opponentCreatureNodes;

	[SerializeField]
	private Color _DarkGreen;

	[SerializeField]
	private Color _DarkRed;

	private BattleHistory battleHistory;

	private List<InventoryTile> mSpawnedTiles = new List<InventoryTile>();

	private Dictionary<string, object> featuredVideo;

	private bool mWaitingForInvite;

	private ResponseFlag mAllyInviteStatus;

	private bool mWaitForUserAction;

	private NextAction mUserActionProceed;

	private PlayerSaveData.ProceedNextStep mNextFunction;

	public override void Populate(object dataObj)
	{
		battleHistory = dataObj as BattleHistory;
		opponentNameLabel.text = battleHistory.opponentName;
		opponentMatchSeason.text = battleHistory.season;
		if (battleHistory.youWon)
		{
			opponentMatchResult.text = KFFLocalization.Get("!!VICTORY");
			opponentMatchResult.color = Color.green;
			opponentMatchResult.effectColor = _DarkGreen;
		}
		else
		{
			opponentMatchResult.text = KFFLocalization.Get("!!PVP_DEFEAT");
			opponentMatchResult.color = Color.red;
			opponentMatchResult.effectColor = _DarkRed;
		}
		if (opponentMatchResult.LabelShadow != null)
		{
			opponentMatchResult.LabelShadow.ShadowTextColor = opponentMatchResult.effectColor;
			opponentMatchResult.LabelShadow.ShadowEffectColor = opponentMatchResult.effectColor;
			opponentMatchResult.LabelShadow.RefreshShadowLabel();
		}
		uint num = DateTime.UtcNow.UnixTimestamp() - battleHistory.recordTime;
		uint num2 = num / 60u;
		uint num3 = num2 / 60u;
		uint num4 = num3 / 24u;
		if (num4 != 0)
		{
			opponentMatchDate.text = KFFLocalization.Get("!!X_DAYS_AGO").Replace("<val1>", num4.ToString());
		}
		else if (num3 != 0)
		{
			opponentMatchDate.text = KFFLocalization.Get("!!X_HOURS_AGO").Replace("<val1>", num3.ToString());
		}
		else if (num2 != 0)
		{
			opponentMatchDate.text = KFFLocalization.Get("!!X_MINUTES_AGO").Replace("<val1>", num2.ToString());
		}
		else
		{
			opponentMatchDate.text = KFFLocalization.Get("!!X_SECONDS_AGO").Replace("<val1>", num.ToString());
		}
		foreach (InventoryTile mSpawnedTile in mSpawnedTiles)
		{
			NGUITools.Destroy(mSpawnedTile.gameObject);
		}
		mSpawnedTiles.Clear();
		for (int i = 0; i < battleHistory.creatures.Count; i++)
		{
			CreatureData data = CreatureDataManager.Instance.GetData(battleHistory.creatures[i]);
			InventoryTile component = opponentCreatureNodes[i].InstantiateAsChild(Singleton<PrefabReferences>.Instance.InventoryTile).GetComponent<InventoryTile>();
			mSpawnedTiles.Add(component);
			component.PopulateAndForceDisplay(data);
		}
		if (!string.IsNullOrEmpty(battleHistory.opponentFBId) && Singleton<PlayerInfoScript>.Instance.IsFacebookLogin())
		{
			OpponentBadge.PopulateOtherPlayerData(battleHistory.opponentName, string.Empty, battleHistory.currentLeague, battleHistory.bestLeague, null);
		}
		else
		{
			LeaderData data2 = LeaderDataManager.Instance.GetData(battleHistory.opponentLeader);
			OpponentBadge.PopulateOtherPlayerData(battleHistory.opponentName, data2.PortraitTexture, battleHistory.currentLeague, battleHistory.bestLeague, null);
		}
		RefreshInviteButton();
		ViewReplayButton.SetActive(false);
		if (battleHistory.battleVideo != string.Empty)
		{
			featuredVideo = null;
			string text = "/games/current/videos?client_id=7396cfa9e9b9c126e67bf9b8b4d13b85911767bd&videoHistoryTag=" + battleHistory.battleVideo;
		}
	}

	public void RefreshInviteButton()
	{
		if (BattleHistoryController.Instance.alliesList.Find((AllyData m) => m.Id == battleHistory.opponentId) != null || Singleton<PlayerInfoScript>.Instance.AlreadySentMatchHistoryInvite(battleHistory.opponentId))
		{
			AddFriendButton.SetActive(false);
		}
		else
		{
			AddFriendButton.SetActive(true);
		}
	}

	public void AddFriend()
	{
		if (battleHistory != null)
		{
			Singleton<BusyIconPanelController>.Instance.Show();
			mWaitingForInvite = true;
			mAllyInviteStatus = ResponseFlag.None;
			Ally.AllyRequest(SessionManager.Instance.theSession, battleHistory.opponentId, AllyIniteToAUserCallBack);
		}
	}

	private void AllyIniteToAUserCallBack(ResponseFlag flag)
	{
		mAllyInviteStatus = flag;
	}

	private void Update()
	{
		if (mWaitingForInvite && mAllyInviteStatus != ResponseFlag.None)
		{
			mWaitingForInvite = false;
			Singleton<BusyIconPanelController>.Instance.Hide();
			if (mAllyInviteStatus == ResponseFlag.Success)
			{
				Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!INVITE_SENT").Replace("<val1>", battleHistory.opponentName));
				Singleton<PlayerInfoScript>.Instance.OnSendMatchHistoryInvite(battleHistory.opponentId);
				BattleHistoryController.Instance.RefreshInviteButtons();
			}
			else if (mAllyInviteStatus == ResponseFlag.Duplicate)
			{
				Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!INVITE_ALREADY_SENT"));
				Singleton<PlayerInfoScript>.Instance.OnSendMatchHistoryInvite(battleHistory.opponentId);
				BattleHistoryController.Instance.RefreshInviteButtons();
			}
			else if (mAllyInviteStatus == ResponseFlag.Exceedmyallylimit)
			{
				Singleton<SimplePopupController>.Instance.ShowPurchasePrompt(KFFLocalization.Get("!!ALLY_SLOTS_CONFIRM").Replace("<val2>", MiscParams.AllyBoxPerPurchase.ToString()), KFFLocalization.Get("!!ALLY_SLOTS_NOBUY"), MiscParams.AllyBoxPurchaseCost, OnClickConfirmAllySpace);
			}
			else if (mAllyInviteStatus == ResponseFlag.Exceeduserallylimit)
			{
				Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!HELPER_INVITE_EXCEED_HIM"));
			}
			else
			{
				Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!ERROR_SENDING_INVITE_SHORT").Replace("<val1>", battleHistory.opponentName));
			}
		}
		if (!mWaitForUserAction)
		{
			return;
		}
		if (mUserActionProceed == NextAction.PROCEED)
		{
			Singleton<BusyIconPanelController>.Instance.Hide();
			mWaitForUserAction = false;
			mUserActionProceed = NextAction.NONE;
			if (mNextFunction != null)
			{
				mNextFunction();
			}
			mWaitForUserAction = false;
			mUserActionProceed = NextAction.NONE;
		}
		if (mUserActionProceed == NextAction.ERROR)
		{
			mWaitForUserAction = false;
			mUserActionProceed = NextAction.NONE;
			Singleton<BusyIconPanelController>.Instance.Hide();
			Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!SERVER_ERROR_MESSAGE"), OnCloseServerAccessErrorPopup);
		}
	}

	private void OnCloseServerAccessErrorPopup()
	{
	}

	private void OnClickConfirmAllySpace()
	{
		PlayerSaveData saveData = Singleton<PlayerInfoScript>.Instance.SaveData;
		mWaitForUserAction = true;
		mUserActionProceed = NextAction.WAITING;
		Singleton<BusyIconPanelController>.Instance.Show();
		saveData.ConsumeHardCurrency2(MiscParams.AllyBoxPurchaseCost, "ally space", UserActionCallback);
		mNextFunction = AllySpaceExecute;
	}

	public void UserActionCallback(PlayerSaveData.ActionResult result)
	{
		if (result.success)
		{
			mUserActionProceed = NextAction.PROCEED;
		}
		else
		{
			mUserActionProceed = NextAction.ERROR;
		}
	}

	private void AllySpaceExecute()
	{
		PlayerSaveData saveData = Singleton<PlayerInfoScript>.Instance.SaveData;
		saveData.AddEmptyAllySlots(MiscParams.AllyBoxPerPurchase);
		Singleton<PlayerInfoScript>.Instance.Save();
		Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!ALLY_SLOTS_BOUGHT").Replace("<val1>", saveData.AllyBoxSpace.ToString()), OnCloseAllySpacePopup);
	}

	private void OnCloseAllySpacePopup()
	{
		AddFriend();
	}

	public void viewVideo()
	{
		if (battleHistory != null && featuredVideo == null)
		{
		}
	}
}
