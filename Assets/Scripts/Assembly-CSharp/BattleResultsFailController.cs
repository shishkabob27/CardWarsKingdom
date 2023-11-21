using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleResultsFailController : Singleton<BattleResultsFailController>
{
	private enum NextAction
	{
		NONE,
		WAITING,
		PROCEED,
		ERROR,
		RETRY
	}

	public UITweenController ShowTween;

	public UITweenController ShowTipTween;

	public UITweenController ShowSecondaryTween;

	public UITweenController HideTween;

	public UITweenController MultiplayerShowTween;

	public UILabel AvailableStamina;

	public UILabel RetryStaminaCost;

	public UILabel AvailableHardCurrency;

	public UILabel HardCurrencyCost;

	public UILabel RetryMessage;

	public UILabel TipLabel;

	public UIGrid DropsGrid;

	public UIGrid RewardsGrid;

	public UILabel RewardSoftCurrency;

	public UILabel RewardSocialCurrency;

	public UILabel RewardHardCurrency;

	public GameObject TipPanel;

	private List<GameObject> mSpawnedPortraits = new List<GameObject>();

	private Vector3 mLootGridBasePos;

	private bool mWaitForUserAction;

	private NextAction mUserActionProceed;

	private PlayerSaveData.ProceedNextStep mNextFunction;

	private bool tutorialRevive;

	private void Awake()
	{
		mLootGridBasePos = DropsGrid.transform.localPosition;
	}

	private void Update()
	{
		RefreshStamina();
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

	public void Show()
	{
		if (Singleton<PlayerInfoScript>.Instance.StateData.MultiplayerMode)
		{
			Singleton<AnalyticsManager>.Instance.LogMultiplayerLost();
			MultiplayerShowTween.Play();
			return;
		}
		Singleton<AnalyticsManager>.Instance.LogQuestLost(Singleton<PlayerInfoScript>.Instance.StateData.CurrentActiveQuest.ID);
		if (!Singleton<TutorialController>.Instance.IsBlockComplete("Revive") && !Singleton<TutorialController>.Instance.IsAnyTutorialActive())
		{
			Singleton<TutorialController>.Instance.StartTutorialBlock("Revive");
		}
		ShowTween.Play();
		Populate();
	}

	public void OpenTip()
	{
		HideTween.Play();
		TipPanel.SetActive(true);
		ShowTipTween.Play();
		TipEntry randomTip = TipsDataManager.Instance.GetRandomTip(TipEntry.TipContext.Failure);
		TipLabel.text = randomTip.Text;
	}

	public void OnTipClosed()
	{
		TipPanel.SetActive(false);
		GoToMap();
	}

	private void Populate()
	{
		QuestData currentActiveQuest = Singleton<PlayerInfoScript>.Instance.StateData.CurrentActiveQuest;
		RetryStaminaCost.text = currentActiveQuest.StaminaCost.ToString();
		HardCurrencyCost.text = MiscParams.ReviveCost.ToString();
		string text = KFFLocalization.Get(RetryMessage.text);
		RetryMessage.text = text.Replace("<val1>", MiscParams.ReviveCost.ToString());
		RewardManager.QuestRewards questRewards = RewardManager.AccumulateRewards(false);
		Vector3 localPosition = mLootGridBasePos;
		if (questRewards.SoftCurrency > 0)
		{
			RewardSoftCurrency.transform.parent.gameObject.SetActive(true);
			RewardSoftCurrency.text = questRewards.SoftCurrency.ToString();
		}
		else
		{
			RewardSoftCurrency.transform.parent.gameObject.SetActive(false);
		}
		if (questRewards.SocialCurrency > 0)
		{
			RewardSocialCurrency.transform.parent.gameObject.SetActive(true);
			RewardSocialCurrency.text = questRewards.SocialCurrency.ToString();
		}
		else
		{
			RewardSocialCurrency.transform.parent.gameObject.SetActive(false);
		}
		if (questRewards.HardCurrency > 0)
		{
			RewardHardCurrency.transform.parent.gameObject.SetActive(true);
			RewardHardCurrency.text = questRewards.HardCurrency.ToString();
		}
		else
		{
			RewardHardCurrency.transform.parent.gameObject.SetActive(false);
		}
		RewardsGrid.Reposition();
		InventoryTile.ClearDelegates(true);
		foreach (InventorySlotItem item in questRewards.CreaturesLooted)
		{
			InventoryTile component = DropsGrid.gameObject.InstantiateAsChild(Singleton<PrefabReferences>.Instance.InventoryTile).GetComponent<InventoryTile>();
			component.gameObject.ChangeLayer(DropsGrid.gameObject.layer);
			component.PopulateAndForceDisplay(item.DisplayCreature);
			component.GetComponent<Collider>().enabled = false;
			mSpawnedPortraits.Add(component.gameObject);
		}
		foreach (InventorySlotItem item2 in questRewards.EvoMatsLooted)
		{
			InventoryTile component2 = DropsGrid.gameObject.InstantiateAsChild(Singleton<PrefabReferences>.Instance.InventoryTile).GetComponent<InventoryTile>();
			component2.gameObject.ChangeLayer(DropsGrid.gameObject.layer);
			component2.Populate(item2);
			component2.GetComponent<Collider>().enabled = false;
			mSpawnedPortraits.Add(component2.gameObject);
		}
		foreach (InventorySlotItem item3 in questRewards.XPMatsLooted)
		{
			InventoryTile component3 = DropsGrid.gameObject.InstantiateAsChild(Singleton<PrefabReferences>.Instance.InventoryTile).GetComponent<InventoryTile>();
			component3.gameObject.ChangeLayer(DropsGrid.gameObject.layer);
			component3.Populate(item3);
			component3.GetComponent<Collider>().enabled = false;
			mSpawnedPortraits.Add(component3.gameObject);
		}
		foreach (InventorySlotItem item4 in questRewards.CardsLooted)
		{
			InventoryTile component4 = DropsGrid.gameObject.InstantiateAsChild(Singleton<PrefabReferences>.Instance.InventoryTile).GetComponent<InventoryTile>();
			component4.gameObject.ChangeLayer(DropsGrid.gameObject.layer);
			component4.Populate(item4);
			component4.GetComponent<Collider>().enabled = false;
			mSpawnedPortraits.Add(component4.gameObject);
		}
		DropsGrid.Reposition();
		DropsGrid.transform.localPosition = localPosition;
		RefreshStamina();
	}

	public void Unload()
	{
		foreach (GameObject mSpawnedPortrait in mSpawnedPortraits)
		{
			NGUITools.Destroy(mSpawnedPortrait);
		}
		mSpawnedPortraits.Clear();
	}

	private void RefreshStamina()
	{
		if (AvailableHardCurrency.gameObject.activeInHierarchy)
		{
			AvailableStamina.text = DetachedSingleton<StaminaManager>.Instance.GetStamina(StaminaType.Quests).ToString();
			AvailableHardCurrency.text = Singleton<PlayerInfoScript>.Instance.SaveData.HardCurrency.ToString();
		}
	}

	public void OnClickForfeit()
	{
		ShowSecondaryTween.Play();
	}

	public void OnClickRevive()
	{
		if (Singleton<TutorialController>.Instance.IsBlockActive("Revive"))
		{
			tutorialRevive = true;
			Revive();
		}
		else
		{
			tutorialRevive = false;
			Singleton<SimplePopupController>.Instance.ShowPurchasePrompt(KFFLocalization.Get("!!REVIVE_CONFIRM"), KFFLocalization.Get("!!REVIVE_NOBUY"), MiscParams.ReviveCost, Revive);
		}
	}

	private void Revive()
	{
		Unload();
		HideTween.Play();
		Singleton<BattleResultsController>.Instance.HideFailBannerTween.Play();
		mWaitForUserAction = true;
		mUserActionProceed = NextAction.WAITING;
		Singleton<BusyIconPanelController>.Instance.Show();
		Singleton<PlayerInfoScript>.Instance.SaveData.ConsumeHardCurrency2(MiscParams.ReviveCost, "revive", UserActionCallback);
		mNextFunction = ReviveExecute;
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

	private void ReviveExecute()
	{
		Singleton<AnalyticsManager>.Instance.LogQuestRevive(Singleton<PlayerInfoScript>.Instance.StateData.CurrentActiveQuest.ID);
		Singleton<PlayerInfoScript>.Instance.Save();
		Singleton<PlayerInfoScript>.Instance.StateData.NoReviveRandomDungeonRun = false;
		Singleton<DWGame>.Instance.RevivePlayer();
	}

	public void OnClickReplay()
	{
		QuestData currentActiveQuest = Singleton<PlayerInfoScript>.Instance.StateData.CurrentActiveQuest;
		if (DetachedSingleton<StaminaManager>.Instance.GetStamina(StaminaType.Quests) < currentActiveQuest.StaminaCost)
		{
			Singleton<SimplePopupController>.Instance.ShowPurchasePrompt(KFFLocalization.Get("!!NO_STAMINA_BUY"), KFFLocalization.Get("!!NO_STAMINA_NOBUY"), MiscParams.StaminaRefillCost, RefillStaminaAndReplay);
			return;
		}
		DetachedSingleton<StaminaManager>.Instance.ConsumeStamina(StaminaType.Quests, currentActiveQuest.StaminaCost);
		DetachedSingleton<SceneFlowManager>.Instance.LoadBattleScene();
	}

	private void RefillStaminaAndReplay()
	{
		mWaitForUserAction = true;
		mUserActionProceed = NextAction.WAITING;
		Singleton<BusyIconPanelController>.Instance.Show();
		Singleton<PlayerInfoScript>.Instance.SaveData.ConsumeHardCurrency2(MiscParams.StaminaRefillCost, "stamina refill", UserActionCallback);
		mNextFunction = StaminaRefillExecute;
	}

	private void StaminaRefillExecute()
	{
		DetachedSingleton<StaminaManager>.Instance.RefillStamina();
		Singleton<BuyStaminaPopupController>.Instance.Show(OnClickReplay);
	}

	public void OnClickMap()
	{
		OpenTip();
	}

	public void GoToMap()
	{
		Singleton<SLOTMusic>.Instance.StopMusic(0.5f);
		Singleton<ScreenFadeController>.Instance.ShowLoadScreen(delegate
		{
			StartCoroutine(ChangeScene(Singleton<PlayerInfoScript>.Instance.GetCurrentReturnLocation()));
		});
	}

	public void OnClickTown()
	{
		Singleton<SLOTMusic>.Instance.StopMusic(0.5f);
		Singleton<ScreenFadeController>.Instance.ShowLoadScreen(delegate
		{
			StartCoroutine(ChangeScene(SceneFlowManager.ReturnLocation.Town));
		});
	}

	private IEnumerator ChangeScene(SceneFlowManager.ReturnLocation returnLoc)
	{
		yield return new WaitForSeconds(0.6f);
		Singleton<DWBattleLane>.Instance.ClearPooledData();
		DetachedSingleton<SceneFlowManager>.Instance.LoadFrontEndScene(returnLoc);
		yield return null;
	}
}
