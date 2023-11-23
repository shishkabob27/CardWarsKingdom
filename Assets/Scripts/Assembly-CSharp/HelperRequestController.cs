using System.Collections.Generic;
using Allies;
using UnityEngine;

public class HelperRequestController : Singleton<HelperRequestController>
{
	private enum NextAction
	{
		NONE,
		WAITING,
		PROCEED,
		ERROR,
		RETRY
	}

	public enum InviteResults
	{
		Accepted,
		AcceptedExpand,
		Declined,
		DeclinedFull
	}

	public enum InviteFull
	{
		Sender,
		Receiver
	}

	public delegate void PopupButtonCallback();

	public GameObject HelperTilePrefab;

	public Transform HelperTileParent;

	private GameObject mHelperPrefabObject;

	public UITweenController ShowTween;

	public UITweenController HideTween;

	public UILabel Title;

	public UILabel Body;

	public Transform YesButton;

	public Transform NoButton;

	public Transform OkButton;

	public Transform CloseButton;

	public UILabel YesButtonLabel;

	public UILabel NoButtonLabel;

	public UILabel OkButtonLabel;

	public AllyInviteStatusHolder mStatusHolder = new AllyInviteStatusHolder();

	private PopupButtonCallback mNoCallback;

	private PopupButtonCallback mYesCallback;

	public HelperItem CurrentHelper;

	private bool mWaitForUserAction;

	private NextAction mUserActionProceed;

	private PlayerSaveData.ProceedNextStep mNextFunction;

	private int myAlliesCount;

	private InviteResults friendInviteResult;

	private InviteFull inviteFull;

	public bool ShouldSendAllyInvite;

	private void Awake()
	{
		if (DetachedSingleton<SceneFlowManager>.Instance.InBattleScene())
		{
			base.gameObject.ChangeLayer(LayerMask.NameToLayer("TopGUI"));
		}
	}

	private void Start()
	{
		if (SessionManager.Instance.theSession == null)
		{
			return;
		}
		List<AllyData> tempList = new List<AllyData>();
		Ally.GetAlliesList(SessionManager.Instance.theSession, delegate(List<AllyData> alliesList, ResponseFlag flag)
		{
			if (flag == ResponseFlag.Success)
			{
				tempList = alliesList;
				myAlliesCount = tempList.Count;
			}
		});
	}

	public void ShowRemoveAllyConfirm(string title, string body, PopupButtonCallback yesCallback)
	{
		mYesCallback = yesCallback;
		PopulateNestedHelperObject(HelperMode.AllyListRemoveAllyConfirm);
		Title.text = title;
		Body.text = body;
		YesButton.gameObject.SetActive(false);
		NoButton.gameObject.SetActive(false);
		OkButton.gameObject.SetActive(true);
		CloseButton.gameObject.SetActive(true);
		OkButtonLabel.text = KFFLocalization.Get("!!REMOVE");
		ShowTween.Play();
	}

	public void ShowAllyInviteAcceptConfirm(string title, string body, PopupButtonCallback yesCallback, PopupButtonCallback noCallback)
	{
		mYesCallback = yesCallback;
		mNoCallback = noCallback;
		PopulateNestedHelperObject(HelperMode.MailBoxAllyInviteConfirm);
		Title.text = title;
		Body.text = body;
		YesButton.gameObject.SetActive(true);
		NoButton.gameObject.SetActive(true);
		OkButton.gameObject.SetActive(false);
		CloseButton.gameObject.SetActive(true);
		YesButtonLabel.text = KFFLocalization.Get("!!ACCEPT");
		NoButtonLabel.text = KFFLocalization.Get("!!DECLINE");
		ShowTween.Play();
	}

	public void ShowAllyInvite(string title, string body, PopupButtonCallback yesCallback, PopupButtonCallback noCallback)
	{
		mYesCallback = yesCallback;
		mNoCallback = noCallback;
		PopulateNestedHelperObject(HelperMode.PostMatchAllyInvite);
		Title.text = title;
		Body.text = body;
		YesButton.gameObject.SetActive(true);
		NoButton.gameObject.SetActive(true);
		OkButton.gameObject.SetActive(false);
		CloseButton.gameObject.SetActive(false);
		ShowTween.Play();
	}

	public void ShowHelpRewardConfirm(string title, string body, PopupButtonCallback callback)
	{
		mYesCallback = callback;
		PopulateNestedHelperObject(HelperMode.PostMatchHelpReward);
		Title.text = title;
		Body.text = body;
		YesButton.gameObject.SetActive(false);
		NoButton.gameObject.SetActive(false);
		OkButton.gameObject.SetActive(true);
		CloseButton.gameObject.SetActive(false);
		ShowTween.Play();
	}

	private void PopulateNestedHelperObject(HelperMode mode)
	{
		if (mHelperPrefabObject == null)
		{
			mHelperPrefabObject = HelperTileParent.InstantiateAsChild(HelperTilePrefab);
		}
		mHelperPrefabObject.ChangeLayer(base.gameObject.layer);
		HelperPrefabScript component = mHelperPrefabObject.GetComponent<HelperPrefabScript>();
		component.Mode = mode;
		if (CurrentHelper == null)
		{
			CurrentHelper = Singleton<PlayerInfoScript>.Instance.StateData.SelectedHelper;
		}
		if (CurrentHelper != null)
		{
			component.Populate(CurrentHelper);
			component.Mode = HelperMode.AllyListRemoveAllyConfirm;
			component.RefreshOverlay();
		}
	}

	public void OnClickYes()
	{
		HideTween.Play();
		PopupButtonCallback popupButtonCallback = mYesCallback;
		mYesCallback = null;
		if (popupButtonCallback != null)
		{
			popupButtonCallback();
		}
	}

	private void KPIInviteResult()
	{
		string upsightEvent = "Invites." + friendInviteResult;
		if (friendInviteResult == InviteResults.Accepted || friendInviteResult == InviteResults.AcceptedExpand)
		{
			myAlliesCount++;
		}
		if (Singleton<PlayerInfoScript>.Instance.StateData.SelectedHelper != null)
		{
			string helperID = Singleton<PlayerInfoScript>.Instance.StateData.SelectedHelper.HelperID;
			string value = Singleton<PlayerInfoScript>.Instance.StateData.SelectedHelper.HelperRank.ToString();
			string value2 = myAlliesCount.ToString();
			string value3 = Singleton<PlayerInfoScript>.Instance.SaveData.AllyBoxSpace.ToString();
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("playerID", helperID);
			dictionary.Add("playerLevel", value);
			dictionary.Add("allyCount", value2);
			dictionary.Add("maxAllySize", value3);
			if (friendInviteResult == InviteResults.AcceptedExpand)
			{
				dictionary.Add("balance", Singleton<PlayerInfoScript>.Instance.SaveData.HardCurrency.ToString());
			}
			else if (friendInviteResult == InviteResults.DeclinedFull)
			{
				dictionary.Add("source", Singleton<PlayerInfoScript>.Instance.SaveData.HardCurrency.ToString());
			}
		}
		friendInviteResult = InviteResults.Accepted;
		inviteFull = InviteFull.Sender;
	}

	public void OnClickNo()
	{
		HideTween.Play();
		PopupButtonCallback popupButtonCallback = mNoCallback;
		mNoCallback = null;
		if (popupButtonCallback != null)
		{
			popupButtonCallback();
		}
	}

	public void SendAllyInviteAndReward()
	{
		if (ShouldSendAllyInvite)
		{
			SendAllyInvite();
		}
		SendRewardHelper();
	}

	public void SendAllyInvite()
	{
		Session theSession = SessionManager.Instance.theSession;
		if (CurrentHelper == null)
		{
			CurrentHelper = Singleton<PlayerInfoScript>.Instance.StateData.SelectedHelper;
		}
		if (ShouldSendAllyInvite && !mStatusHolder.IsSuccess)
		{
			Ally.AllyRequest(theSession, CurrentHelper.HelperID, AllyInviteCallback);
		}
	}

	public void SendRewardHelper()
	{
		Session theSession = SessionManager.Instance.theSession;
		if (CurrentHelper == null)
		{
			CurrentHelper = Singleton<PlayerInfoScript>.Instance.StateData.SelectedHelper;
		}
		if (!mStatusHolder.IsRewardSuccess)
		{
			if (CurrentHelper.IsAlly == 1)
			{
				Ally.UseTheAlly(theSession, CurrentHelper.HelperID, RewardHelperCallback);
			}
			else
			{
				Ally.UseThePlayer(theSession, CurrentHelper.HelperID, RewardHelperCallback);
			}
		}
	}

	public void SendAllyInviteAccept()
	{
		Session theSession = SessionManager.Instance.theSession;
		Ally.ConfirmAllyRequest(theSession, CurrentHelper.HelperID, AllyInviteAcceptCallback);
	}

	public void SendAllyInviteReject()
	{
		Session theSession = SessionManager.Instance.theSession;
		Ally.DenyAllyRequest(theSession, CurrentHelper.HelperID, AllyInviteRejectCallback);
	}

	public void SendRemoveAlly()
	{
		Session theSession = SessionManager.Instance.theSession;
		Ally.RemoveFromTheAllies(theSession, CurrentHelper.HelperID, RemoveAllyCallback);
	}

	private void AllyInviteCallback(ResponseFlag flag)
	{
		mStatusHolder.SetInviteStatus(flag);
		mStatusHolder.IsInviteResponseSet = true;
	}

	private void RewardHelperCallback(ResponseFlag flag)
	{
		mStatusHolder.SetRewardStatus(flag == ResponseFlag.Success);
		mStatusHolder.IsRewardResponseSet = true;
	}

	private void AllyInviteAcceptCallback(ResponseFlag flag)
	{
		mStatusHolder.Mode = AllyInviteStatusHolder.AllyCallbackMode.Accepted;
		mStatusHolder.SetStatus(flag == ResponseFlag.Success);
		mStatusHolder.SetInviteStatus(flag);
	}

	private void AllyInviteRejectCallback(ResponseFlag flag)
	{
		mStatusHolder.Mode = AllyInviteStatusHolder.AllyCallbackMode.Rejected;
		mStatusHolder.SetStatus(flag == ResponseFlag.Success);
		mStatusHolder.SetInviteStatus(flag);
	}

	private void RemoveAllyCallback(ResponseFlag flag)
	{
		mStatusHolder.Mode = AllyInviteStatusHolder.AllyCallbackMode.Removed;
		mStatusHolder.SetStatus(flag == ResponseFlag.Success);
		mStatusHolder.SetInviteStatus(flag);
	}

	private void Update()
	{
		if (DetachedSingleton<SceneFlowManager>.Instance.GetCurrentScene() == SceneFlowManager.Scene.Battle)
		{
			if (!mStatusHolder.IsRewardResponseSet || (ShouldSendAllyInvite && !mStatusHolder.IsInviteResponseSet))
			{
				return;
			}
			if ((ShouldSendAllyInvite && mStatusHolder.InviteResponse == ResponseFlag.Success && mStatusHolder.IsRewardSuccess) || (!ShouldSendAllyInvite && mStatusHolder.InviteResponse == ResponseFlag.None && mStatusHolder.IsRewardSuccess))
			{
				int num = ((CurrentHelper.IsAlly != 1) ? MiscParams.HelpPointForExplorer : MiscParams.HelpPointForAlly);
				Singleton<PlayerInfoScript>.Instance.SaveData.PvPCurrency += num;
				Singleton<PlayerInfoScript>.Instance.Save();
				Singleton<BattleResultsController>.Instance.ProceedToFrontEnd();
				ResetHolderFlags();
			}
			else if (mStatusHolder.InviteResponse == ResponseFlag.Error || mStatusHolder.InviteResponse == ResponseFlag.None || mStatusHolder.IsRewardError)
			{
				string body = KFFLocalization.Get("!!CONNECTIONFAILED");
				Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, body, OnRetryAllyInviteAndRewardClicked, KFFLocalization.Get("!!RETRY"));
				ResetHolderFlags();
			}
			else if (mStatusHolder.InviteResponse == ResponseFlag.Exceedmyallylimit)
			{
				ResetHolderFlags();
				Singleton<SimplePopupController>.Instance.ShowPurchasePrompt(KFFLocalization.Get("!!ALLY_SLOTS_FULL_BUY").Replace("<val2>", MiscParams.AllyBoxPerPurchase.ToString()), KFFLocalization.Get("!!ALLY_SLOTS_FULL_NOBUY"), MiscParams.AllyBoxPurchaseCost, OnConfirmAllySpace, OnCancelAllyInviteAndRewardClicked);
			}
			else if (mStatusHolder.InviteResponse == ResponseFlag.Exceeduserallylimit)
			{
				inviteFull = InviteFull.Receiver;
				string body2 = KFFLocalization.Get("!!HELPER_INVITE_EXCEED_HIM");
				Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, body2, OnCancelAllyInviteAndRewardClicked);
				ResetHolderFlags();
			}
			else if (mStatusHolder.InviteResponse == ResponseFlag.Duplicate)
			{
				Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!INVITE_ALREADY_SENT"), OnCancelAllyInviteAndRewardClicked);
				ResetHolderFlags();
			}
		}
		if (DetachedSingleton<SceneFlowManager>.Instance.GetCurrentScene() == SceneFlowManager.Scene.FrontEnd)
		{
			if (mStatusHolder.IsSuccess)
			{
				if (mStatusHolder.Mode == AllyInviteStatusHolder.AllyCallbackMode.Removed)
				{
					Singleton<SocialController>.Instance.GetAllyListController().ShowInbox(true);
				}
				else
				{
					KPIInviteResult();
					Singleton<MailController>.Instance.DeleteAllyInvite(CurrentHelper);
					Singleton<SocialController>.Instance.RefreshCurrentTab();
				}
				mStatusHolder.IsSuccess = false;
			}
			if (mStatusHolder.IsError)
			{
				if (mStatusHolder.InviteResponse == ResponseFlag.Exceedmyallylimit)
				{
					Singleton<SimplePopupController>.Instance.ShowPurchasePrompt(KFFLocalization.Get("!!ALLY_SLOTS_FULL_BUY").Replace("<val2>", MiscParams.AllyBoxPerPurchase.ToString()), KFFLocalization.Get("!!ALLY_SLOTS_FULL_NOBUY"), MiscParams.AllyBoxPurchaseCost, OnConfirmAllySpace, OnCancelAllyInviteAndRewardClicked);
				}
				else if (mStatusHolder.InviteResponse == ResponseFlag.Exceeduserallylimit)
				{
					friendInviteResult = InviteResults.DeclinedFull;
					inviteFull = InviteFull.Receiver;
					KPIInviteResult();
					string body3 = KFFLocalization.Get("!!HELPER_INVITE_EXCEED_HIM");
					Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, body3);
				}
				else
				{
					friendInviteResult = InviteResults.Declined;
					string body4 = KFFLocalization.Get("!!CONNECTIONFAILED");
					if (mStatusHolder.Mode == AllyInviteStatusHolder.AllyCallbackMode.Rejected)
					{
						Singleton<SimplePopupController>.Instance.ShowPrompt(string.Empty, body4, OnRetryInviteRejectClicked, OnCancelInviteRejectClicked, "Retry", "Cancel");
					}
					else if (mStatusHolder.Mode == AllyInviteStatusHolder.AllyCallbackMode.Accepted)
					{
						Singleton<SimplePopupController>.Instance.ShowPrompt(string.Empty, body4, OnRetryInviteAcceptClicked, OnCancelInviteAcceptClicked, "Retry", "Cancel");
					}
					else if (mStatusHolder.Mode == AllyInviteStatusHolder.AllyCallbackMode.Removed)
					{
						Singleton<SimplePopupController>.Instance.ShowPrompt(string.Empty, body4, OnRetryRemoveAllyClicked, OnCancelRemoveAllyClicked, "Retry", "Cancel");
					}
				}
				mStatusHolder.IsError = false;
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

	private void OnConfirmAllySpace()
	{
		PlayerSaveData saveData = Singleton<PlayerInfoScript>.Instance.SaveData;
		mWaitForUserAction = true;
		mUserActionProceed = NextAction.WAITING;
		Singleton<BusyIconPanelController>.Instance.Show();
		saveData.ConsumeHardCurrency2(MiscParams.AllyBoxPurchaseCost, "ally space", UserActionCallback);
		mNextFunction = AllySpaceExecute;
		friendInviteResult = InviteResults.AcceptedExpand;
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
		mStatusHolder.IsRewardResponseSet = true;
		SendAllyInviteAndReward();
	}

	private void ResetHolderFlags()
	{
		mStatusHolder.IsInviteResponseSet = false;
		mStatusHolder.IsRewardResponseSet = false;
	}

	private void OnRetryAllyInviteAndRewardClicked()
	{
		ResetHolderFlags();
		SendAllyInviteAndReward();
	}

	private void OnCancelAllyInviteAndRewardClicked()
	{
		if (DetachedSingleton<SceneFlowManager>.Instance.InBattleScene())
		{
			Singleton<BattleResultsController>.Instance.ProceedToFrontEnd();
		}
		else if (DetachedSingleton<SceneFlowManager>.Instance.InFrontEnd())
		{
			friendInviteResult = InviteResults.Declined;
			KPIInviteResult();
		}
	}

	private void OnRetryInviteAcceptClicked()
	{
		SendAllyInviteAccept();
	}

	private void OnCancelInviteAcceptClicked()
	{
		KPIInviteResult();
	}

	private void OnRetryInviteRejectClicked()
	{
		SendAllyInviteReject();
	}

	private void OnCancelInviteRejectClicked()
	{
		KPIInviteResult();
	}

	private void OnRetryRemoveAllyClicked()
	{
		SendRemoveAlly();
	}

	private void OnCancelRemoveAllyClicked()
	{
	}

	public void Unload()
	{
		if (mHelperPrefabObject != null)
		{
			Object.Destroy(mHelperPrefabObject);
		}
	}
}
