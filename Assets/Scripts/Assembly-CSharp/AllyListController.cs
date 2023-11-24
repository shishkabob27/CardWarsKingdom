using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Allies;
using UnityEngine;

public class AllyListController : MonoBehaviour
{
	private enum NextAction
	{
		NONE,
		WAITING,
		PROCEED,
		ERROR,
		RETRY
	}

	public delegate void AlliesPopulatedCallback();

	public GameObject HelperTilePrefab;

	public UITweenController EnterInviteIdShowTween;

	public UITweenController EnterInviteIdHideTween;

	public UILabel PlayerIdLabel;

	public UILabel InviteInputLabel;

	public UILabel NoAlliesYetLabel;

	public UIInput InviteInputObject;

	public UIStreamingGrid AllyListGrid;

	private UIStreamingGridDataSource<HelperItem> mAlliesGridDataSource = new UIStreamingGridDataSource<HelperItem>();

	public Transform SortButton;

	public UILabel AllyCountLabel;

	private string mInvitingID;

	private string mIDToRetry;

	private bool mWaitingForInvite;

	private bool mInviteBlockerUp;

	private bool mShowOnlineStatus;

	private static ResponseFlag mAllyInviteStatus;

	private List<AllyData> mFetchedAllies = new List<AllyData>();

	private List<HelperItem> mHelperItemList = new List<HelperItem>();

	[SerializeField]
	private bool ShouldUseDebugDummyList;

	private bool mWaitForUserAction;

	private NextAction mUserActionProceed;

	private PlayerSaveData.ProceedNextStep mNextFunction;

	private bool mShowInbox;

	private bool mRetrieveTriggered;

	public bool IsHelperAssigned;

	public bool UseDummyAllies;

	public int DummyAllyListCount;

	private void Awake()
	{
		InviteInputObject.keyboardType = UIInput.KeyboardType.Default;
	}

	public void ShowInbox(bool showOnlineStatus, AlliesPopulatedCallback callback = null)
	{
		mShowOnlineStatus = showOnlineStatus;
		if (AllyCountLabel != null)
		{
			AllyCountLabel.text = string.Empty;
		}
		Unload();
		if (ShouldUseDebugDummyList)
		{
			DebugDummyList();
			PopulateAllyListGrid();
			if (callback != null)
			{
				callback();
			}
		}
		else
		{
			StartCoroutine(RetrieveAlliesList(callback));
		}
	}

	private IEnumerator RetrieveAlliesList(AlliesPopulatedCallback callback)
	{
		bool retry = false;
		do
		{
			mFetchedAllies.Clear();
			bool waiting = true;
			bool failed = false;
			Singleton<BusyIconPanelController>.Instance.Show();
			Ally.GetAlliesList(SessionManager.Instance.theSession, delegate(List<AllyData> alliesList, ResponseFlag flag)
			{
				if (flag == ResponseFlag.Success)
				{
					mFetchedAllies = alliesList;
				}
				else
				{
					failed = true;
				}
				waiting = false;
			});
			while (waiting)
			{
				yield return null;
			}
			Singleton<BusyIconPanelController>.Instance.Hide();
			if (failed)
			{
				bool promptUp = true;
				retry = false;
				Singleton<SimplePopupController>.Instance.ShowPrompt(string.Empty, KFFLocalization.Get("!!CONNECTIONFAILED"), delegate
				{
					promptUp = false;
					retry = true;
				}, delegate
				{
					promptUp = false;
					retry = false;
				}, KFFLocalization.Get("!!RETRY"), KFFLocalization.Get("!!CANCEL"));
				while (promptUp)
				{
					yield return null;
				}
				continue;
			}
			break;
		}
		while (retry);
		PopulateAllyListGrid();
		if (callback != null)
		{
			callback();
		}
	}

	public void PopulateAllyListGrid()
	{
		mHelperItemList.Clear();
		foreach (AllyData mFetchedAlly in mFetchedAllies)
		{
			HelperItem item = new HelperItem(mFetchedAlly);
			mHelperItemList.Add(item);
		}
		if (NoAlliesYetLabel != null)
		{
			NoAlliesYetLabel.gameObject.SetActive(mFetchedAllies.Count == 0);
		}
		SortAllies();
		mAlliesGridDataSource.Init(AllyListGrid, HelperTilePrefab, mHelperItemList);
		for (int i = 0; i < AllyListGrid.transform.childCount; i++)
		{
			HelperPrefabScript component = AllyListGrid.transform.GetChild(i).GetComponent<HelperPrefabScript>();
			if (component != null)
			{
				component.ShowOnlineStatus = mShowOnlineStatus;
				component.Mode = HelperMode.AllyList;
				component.RefreshOverlay();
			}
		}
		InventoryTile[] componentsInChildren = AllyListGrid.GetComponentsInChildren<InventoryTile>(true);
		InventoryTile[] array = componentsInChildren;
		foreach (InventoryTile inventoryTile in array)
		{
			inventoryTile.CurrentSorts = Singleton<PlayerInfoScript>.Instance.SaveData.HelperSorts;
			inventoryTile.StartBlinkStats();
		}
		UpdateAllyCountLabel();
		AllyListGrid.transform.parent.GetComponent<UIScrollView>().ResetPosition();
		AllyListGrid.transform.parent.GetComponent<UIScrollView>().UpdateScrollbars();
		if (mShowOnlineStatus)
		{
			Singleton<ChatManager>.Instance.RegisterMonitorUsers(mHelperItemList);
		}
	}

	private void UpdateAllyCountLabel()
	{
		if (AllyCountLabel != null)
		{
			AllyCountLabel.text = mHelperItemList.Count + " / " + Singleton<PlayerInfoScript>.Instance.SaveData.AllyBoxSpace.ToString();
		}
	}

	public void Unload()
	{
		Singleton<ChatManager>.Instance.UnMonitorFriendsList();
		mAlliesGridDataSource.Clear();
		mHelperItemList.Clear();
		mFetchedAllies.Clear();
	}

	private void Update()
	{
		if (mWaitingForInvite && mAllyInviteStatus != ResponseFlag.None)
		{
			HandleInviteResult();
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

	private void DebugDummyList()
	{
		mHelperItemList.Clear();
		if (DummyAllyListCount <= 0)
		{
			return;
		}
		for (int i = 0; i < DummyAllyListCount; i++)
		{
			HelperItem helperItem = new HelperItem(1);
			helperItem.HelperName = "Test ally " + i;
			int index = Random.Range(0, CreatureDataManager.Instance.GetDatabase().Count);
			bool flag = false;
			CreatureData creatureData = null;
			while (!flag)
			{
				creatureData = CreatureDataManager.Instance.GetData(index);
				if (creatureData.SoftLaunch == "YES")
				{
					flag = true;
				}
				else
				{
					index = Random.Range(0, CreatureDataManager.Instance.GetDatabase().Count);
				}
			}
			CreatureItem creatureItem = new CreatureItem(creatureData);
			creatureItem.FromOtherPlayer = true;
			InventorySlotItem inventorySlotItem = (helperItem.HelperCreature = new InventorySlotItem(creatureItem));
			helperItem.HelperRank = Random.Range(0, 20);
			mHelperItemList.Add(helperItem);
		}
	}

	public void OnClickSort()
	{
		Singleton<SortPopupController>.Instance.Show(SortPopupController.Category.Helpers, SortButton, PopulateAllyListGrid);
	}

	public void SortAllies()
	{
		HelperComparer comparer = new HelperComparer(Singleton<PlayerInfoScript>.Instance.SaveData.HelperSorts);
		mHelperItemList.Sort(comparer);
	}

	public void OnClickInvite()
	{
		EnterInviteIdShowTween.Play();
		mInviteBlockerUp = true;
		PlayerIdLabel.text = KFFLocalization.Get("!!ID_INFO").Replace("<val1>", Singleton<PlayerInfoScript>.Instance.GetFormattedPlayerCode());
	}

	public void OnAllyIDInput()
	{
		EnterInviteIdHideTween.Play();
		mInviteBlockerUp = false;
		mInvitingID = InviteInputLabel.text;
		string internalID = Singleton<PlayerInfoScript>.Instance.ConvertFormattedPlayerCodeToInternal(mInvitingID);
		InviteInputObject.value = null;
		InviteInputLabel.text = string.Empty;
		if (!IsValidPlayerID(mInvitingID))
		{
			Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!INVALID_INVITE_ID"));
			return;
		}
		if (mInvitingID == Singleton<PlayerInfoScript>.Instance.GetPlayerCode())
		{
			Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!CANT_INVITE_SELF"));
			return;
		}
		if (mHelperItemList.Find((HelperItem m) => m.HelperID == mInvitingID) != null)
		{
			Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!PLAYER_ALREADY_ALLY").Replace("<val1>", mInvitingID));
			return;
		}
		Singleton<BusyIconPanelController>.Instance.Show();
		mIDToRetry = internalID;
		mAllyInviteStatus = ResponseFlag.None;
		mWaitingForInvite = true;
		Ally.AllyRequest(SessionManager.Instance.theSession, internalID, AllyIniteToAUserCallBack);
	}

	private bool IsValidPlayerID(string text)
	{
		Regex regex = new Regex("[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}");
		return regex.IsMatch(text);
    }

	public void OnAllIDInputCancel()
	{
		if (mInviteBlockerUp)
		{
			EnterInviteIdHideTween.Play();
			mInviteBlockerUp = false;
		}
	}

	private void AllyIniteToAUserCallBack(ResponseFlag flag)
	{
		mAllyInviteStatus = flag;
	}

	private void HandleInviteResult()
	{
		mWaitingForInvite = false;
		Singleton<BusyIconPanelController>.Instance.Hide();
		if (mAllyInviteStatus == ResponseFlag.Success)
		{
			Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!INVITE_SENT").Replace("<val1>", mInvitingID));
		}
		else if (mAllyInviteStatus == ResponseFlag.Duplicate)
		{
			Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!INVITE_ALREADY_SENT"));
		}
		else if (mAllyInviteStatus == ResponseFlag.Exceedmyallylimit)
		{
			Singleton<SimplePopupController>.Instance.ShowPurchasePrompt(KFFLocalization.Get("!!ALLY_SLOTS_FULL_BUY").Replace("<val2>", MiscParams.AllyBoxPerPurchase.ToString()), KFFLocalization.Get("!!ALLY_SLOTS_FULL_NOBUY"), MiscParams.AllyBoxPurchaseCost, OnClickConfirmAllySpace);
			Singleton<SLOTAudioManager>.Instance.PlayErrorSound();
		}
		else if (mAllyInviteStatus == ResponseFlag.Exceeduserallylimit)
		{
			Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!HELPER_INVITE_EXCEED_HIM"));
		}
		else
		{
			Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!ERROR_SENDING_INVITE").Replace("<val1>", mInvitingID));
		}
	}

	public void OnClickBuyAllyListSpace()
	{
		mIDToRetry = null;
		Singleton<SimplePopupController>.Instance.ShowPurchasePrompt(KFFLocalization.Get("!!ALLY_SLOTS_CONFIRM").Replace("<val2>", MiscParams.AllyBoxPerPurchase.ToString()), KFFLocalization.Get("!!ALLY_SLOTS_NOBUY"), MiscParams.AllyBoxPurchaseCost, OnClickConfirmAllySpace);
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
		UpdateAllyCountLabel();
		if (mIDToRetry != null)
		{
			Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!ALLY_SLOTS_BOUGHT").Replace("<val1>", saveData.AllyBoxSpace.ToString()), RetryInviteAfterSpaceBought);
		}
		else
		{
			Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!ALLY_SLOTS_BOUGHT").Replace("<val1>", saveData.AllyBoxSpace.ToString()));
		}
	}

	private void RetryInviteAfterSpaceBought()
	{
		Singleton<BusyIconPanelController>.Instance.Show();
		mAllyInviteStatus = ResponseFlag.None;
		mWaitingForInvite = true;
		Ally.AllyRequest(SessionManager.Instance.theSession, mIDToRetry, AllyIniteToAUserCallBack);
	}

	public void OnCloseClicked()
	{
		Unload();
	}

	public void OnBackButtonPressed()
	{
		Singleton<PvPModeSelectController>.Instance.ShowElements();
	}

	public void OnCloseButtonClick()
	{
		Singleton<PvPModeSelectController>.Instance.Hide();
	}
}
