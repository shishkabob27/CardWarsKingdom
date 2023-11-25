using System;
using System.Collections;
using System.Collections.Generic;
using Allies;
using UnityEngine;

public class TownController : Singleton<TownController>
{
	private enum IntroState
	{
		NotStarted,
		VersionCheck,
		RankUp,
		BuildingUnlocks,
		TutorialTriggers,
		Calendar,
		HelperReward,
		NewsPopup,
		NewPvpSeason,
		GoToReturnLocation,
		WaitForUpsight,
		ShowSale,
		ShowSaleResults,
		Done
	}

	public float StartRotateFactor = 0.1f;

	public float StartHeightFactor = 0.3f;

	public float PanSensitivity = 1f;

	public float ZoomSensitivity = 1f;

	public float RotateSensitivity = 1f;

	public float MinHeight = 1f;

	public float MaxHeight = 10f;

	public float MinOffset = 10f;

	public float MaxOffset = 20f;

	public float GachaGateWaitTime;

	public float GachaChestSpacing;

	public float GachaChestFinishTime;

	public float GachaMultiChestFinishTime;

	public MeshCollider CameraMeshCollider;

	private bool mPopulated;

	private bool mMessageRetrieved;

	private List<UICamera.MouseOrTouch> mTouches = new List<UICamera.MouseOrTouch>();

	private Camera mTownCam;

	private Camera mUICam;

	private Vector3 mCamLookAt = Vector3.zero;

	private Vector3 mTargetCamLookAt = Vector3.zero;

	private float mHeightFactor;

	private float mTargetHeightFactor;

	private float mRotateFactor;

	private float mTargetRotateFactor;

	private float mUpsightWaitTimer = -1f;

	public bool UseUpdateCamera;

	public float LockedDesatAmount;

	public float LockedAddAmount;

	public Color LockedColor;

	public bool UseRealtimeLockColorUpdate;

	public GameObject BuildingUnlockEffect;

	public GameObject GachaBackground;

	public Animator GachaAnimator;

	public Transform GachaKeyParent;

	public Transform GachaChestPassByParent;

	public GameObject GachaKeyholeEffect;

	public GameObject GachaOpeningVFX;

	public List<Collider> TownColliders = new List<Collider>();

	private IntroState mIntroState;

	private bool mOnlyLevelUpIntro;

	private bool mHelperBonusCheckDone;

	private int mAllyHelpCount;

	private int mStrangerHelpCount;

	private UICamera.MouseOrTouch mDummyTouch = new UICamera.MouseOrTouch();

	private float mPrevTargetHeightFactor;

	public bool UnlockAtOnceByDebug;

	public Camera GetUICam()
	{
		return mUICam;
	}

	public Camera GetTownCam()
	{
		return mTownCam;
	}

	public bool IsPopulatedDone()
	{
		return mPopulated;
	}

	private void Awake()
	{
		mTownCam = GameObject.Find("3DFrontEnd").GetComponent<Camera>();
		mUICam = GameObject.Find("Camera_Main").GetComponent<Camera>();
		mHeightFactor = (mTargetHeightFactor = StartHeightFactor);
		mRotateFactor = (mTargetRotateFactor = StartRotateFactor);
	}

	private void Start()
	{
		SetSoundMusicVolume();
		ForceLockBuildingState();
		PopulateIfReady();
	}

	private void SetSoundMusicVolume()
	{
		float musicVolume = Singleton<SLOTAudioManager>.Instance.musicVolume;
		float soundVolume = Singleton<SLOTAudioManager>.Instance.soundVolume;
		Singleton<SLOTAudioManager>.Instance.SetMusicVolumeMasterAudio(soundVolume);
		Singleton<SLOTAudioManager>.Instance.SetMusicVolumeMasterAudio(musicVolume);
	}

	private void ForceLockBuildingState()
	{
		Singleton<TownHudController>.Instance.RefreshBuildingLocks(true);
		TownBuildingScript[] array = UnityEngine.Object.FindObjectsOfType(typeof(TownBuildingScript)) as TownBuildingScript[];
		TownBuildingScript[] array2 = array;
		foreach (TownBuildingScript townBuildingScript in array2)
		{
			townBuildingScript.BuildingLockOnMeshes(false);
		}
	}

	private void Update()
	{
		Singleton<TutorialController>.Instance.TiltOffIfTargetingBuilding();
		PopulateIfReady();
		if (UseUpdateCamera)
		{
			UpdateCamera();
		}
		if (mIntroState == IntroState.HelperReward)
		{
			if (mHelperBonusCheckDone)
			{
				Singleton<BusyIconPanelController>.Instance.Hide();
				HandleHelperResults();
			}
		}
		else if (mIntroState == IntroState.WaitForUpsight)
		{
			bool flag = true;
			mUpsightWaitTimer -= Time.deltaTime;
			if (mUpsightWaitTimer <= 0f)
			{
				mUpsightWaitTimer = -1f;
				flag = true;
			}
			if (flag)
			{
				Singleton<BusyIconPanelController>.Instance.Hide();
				AdvanceIntroState();
			}
		}
		if (Singleton<PlayerInfoScript>.Instance.StateData.QuestWinToBroadcast != null && IsIntroDone())
		{
			ChatWindowController.SendDungeonAnnouncement(Singleton<PlayerInfoScript>.Instance.StateData.QuestWinToBroadcast);
			Singleton<PlayerInfoScript>.Instance.StateData.QuestWinToBroadcast = null;
		}
		if (IsIntroDone() && !Singleton<TutorialController>.Instance.IsAnyTutorialActive())
		{
			Singleton<TownHudController>.Instance.ShowGachaAttractionPointer();
		}
	}

	private void HandleHelperResults()
	{
		mHelperBonusCheckDone = false;
		if (mAllyHelpCount > 0 || mStrangerHelpCount > 0)
		{
			int num = mAllyHelpCount;
			int num2 = mStrangerHelpCount;
			int num3 = MiscParams.HelpPointForAlly * num + MiscParams.HelpPointForExplorer * num2;
			string text = KFFLocalization.Get("!!CURRENCY_PVP");
			Singleton<PlayerInfoScript>.Instance.SaveData.PvPCurrency += num3;
			int pvPCurrency = Singleton<PlayerInfoScript>.Instance.SaveData.PvPCurrency;
			Singleton<PlayerInfoScript>.Instance.Save();
			SendWishboneKPIData(mAllyHelpCount.ToString(), mStrangerHelpCount.ToString(), num3.ToString());
			string body = string.Format(KFFLocalization.Get("!!HELPER_REWARD_WHEN_GAMESTART"), num.ToString(), num2.ToString(), num3.ToString(), text, pvPCurrency.ToString());
			Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, body, OnCloseHelperRewardPopup);
			Ally.UpdateMyAllyInfo(SessionManager.Instance.theSession, true, true, ResetMyAllyHelpCountCallback);
		}
		else
		{
			AdvanceIntroState();
		}
	}

	private void SendWishboneKPIData(string allyCount, string anomCount, string amount)
	{
		string upsightEvent = "Economy.WishboneEnter.Helper";
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("allyCount", allyCount);
		dictionary.Add("anonCount", anomCount);
		dictionary.Add("amount", amount);
	}

	private void ResetMyAllyHelpCountCallback(ResponseFlag flag)
	{
	}

	private IEnumerator MailCheck()
	{
		bool firstTime = true;
		while (true)
		{
			if (firstTime || !Singleton<TutorialController>.Instance.IsAnyTutorialActive())
			{
				firstTime = false;
				Singleton<MailController>.Instance.RetrieveMailsAndAllyInvites();
				while (!Singleton<MailController>.Instance.IsMessageRetrieveDone())
				{
					yield return null;
				}
				Singleton<PlayerInfoScript>.Instance.UpdateBadgeCount(BadgeEnum.Mail);
			}
			yield return new WaitForSeconds(300f);
		}
	}

	private void PopulateIfReady()
	{
		if (!mPopulated && SessionManager.Instance.IsLoadDataDone() && Singleton<PlayerInfoScript>.Instance.IsInitialized && !LoadingScreenController.ShowingLoadingScreenNotFadingOut())
		{
			mPopulated = true;
			StartCoroutine(MailCheck());
			HelperBonusCheck();
			Singleton<MouseOrbitCamera>.Instance.CheckTiltCamSettingBeforeTutorial();
			Singleton<PlayerInfoScript>.Instance.StateData.MultiplayerMode = false;
			Singleton<TownHudController>.Instance.InitializeBuildings();
			RefreshLockedBuildingEffect();
			if (!string.IsNullOrEmpty(Singleton<PlayerInfoScript>.Instance.SaveData.MultiplayerPlayerName))
			{
				RewardManager.UpdatePvPInfo();
			}
			AdvanceIntroState();
		}
	}

	private void HelperBonusCheck()
	{
		mHelperBonusCheckDone = false;
		mAllyHelpCount = 0;
		mStrangerHelpCount = 0;
		Ally.RetrieveAllyInformation(SessionManager.Instance.theSession, Singleton<PlayerInfoScript>.Instance.GetPlayerCode(), delegate(List<AllyData> alliesList, ResponseFlag flag)
		{
			if (flag == ResponseFlag.Success && alliesList != null && alliesList.Count > 0)
			{
				mAllyHelpCount = alliesList[0].HelpCount;
				mStrangerHelpCount = alliesList[0].AnonymousHelpcount;
			}
			mHelperBonusCheckDone = true;
		});
	}

	public void AdvanceIntroState()
	{
		SetIntroState(mIntroState + 1);
	}

	public void CheckIntroStateAfterXPGain()
	{
		mOnlyLevelUpIntro = true;
		SetIntroState(IntroState.RankUp);
	}

	public bool NeedToShowTown()
	{
		return mIntroState == IntroState.BuildingUnlocks || mIntroState == IntroState.TutorialTriggers;
	}

	private void SetIntroState(IntroState state)
	{
		mIntroState = state;
		if (mIntroState == IntroState.VersionCheck)
		{
			Singleton<VersionChecker>.Instance.CheckVersion(AdvanceIntroState);
		}
		else if (mIntroState == IntroState.RankUp)
		{
			if (!Singleton<BattleResultsLevelUpController>.Instance.ShowLevelUp())
			{
				AdvanceIntroState();
			}
		}
		else if (mIntroState == IntroState.BuildingUnlocks)
		{
			if (!CheckUnlockedBuildings())
			{
				AdvanceIntroState();
			}
		}
		else if (mIntroState == IntroState.TutorialTriggers)
		{
			if (Singleton<TutorialController>.Instance.CheckTutorialTriggers())
			{
				SetIntroState(IntroState.GoToReturnLocation);
			}
			else if (mOnlyLevelUpIntro)
			{
				SetIntroState(IntroState.Done);
				mOnlyLevelUpIntro = false;
			}
			else
			{
				AdvanceIntroState();
			}
		}
		else if (mIntroState == IntroState.Calendar)
		{
			if (!CalendarCheck())
			{
				AdvanceIntroState();
			}
		}
		else if (mIntroState == IntroState.HelperReward)
		{
			if (mHelperBonusCheckDone)
			{
				HandleHelperResults();
			}
			else
			{
				Singleton<BusyIconPanelController>.Instance.Show();
			}
		}
		else if (mIntroState == IntroState.NewsPopup)
		{
			Singleton<NewsPopupController>.Instance.CheckNewsToShow();
		}
		else if (mIntroState == IntroState.NewPvpSeason)
		{
			if (!Singleton<PlayerInfoScript>.Instance.CheckPvpSeasonStatus())
			{
				AdvanceIntroState();
			}
		}
		else if (mIntroState == IntroState.GoToReturnLocation)
		{
			Singleton<PlayerInfoScript>.Instance.UpdateAllBadgeCounts();
			SceneFlowManager.ReturnLocation returnLocation = DetachedSingleton<SceneFlowManager>.Instance.GetReturnLocation();
			DetachedSingleton<SceneFlowManager>.Instance.ClearReturnLocation();
			if (Singleton<TutorialController>.Instance.IsAnyTutorialActive())
			{
				SetIntroState(IntroState.Done);
				return;
			}
			TownBuildingScript townBuildingScript = null;
			switch (returnLocation)
			{
			case SceneFlowManager.ReturnLocation.Map:
				townBuildingScript = GetBuildingScript("TBuilding_Quests");
				break;
			case SceneFlowManager.ReturnLocation.SpecialQuests:
				townBuildingScript = GetBuildingScript("TBuilding_Dungeon");
				break;
			case SceneFlowManager.ReturnLocation.Pvp:
				townBuildingScript = GetBuildingScript("TBuilding_PVP");
				break;
			}
			if (townBuildingScript != null)
			{
				townBuildingScript.ShowImmediately();
			}
			mUpsightWaitTimer = 5f;
			Singleton<BusyIconPanelController>.Instance.Show();
			UpsightRequester.RequestContent("main_menu");
			AdvanceIntroState();
		}
		else if (mIntroState == IntroState.ShowSale)
		{
			//SpecialSaleData saleToDisplay = Singleton<PlayerInfoScript>.Instance.GetSaleToDisplay();
			//if (saleToDisplay != null)
			//{
			//	Singleton<SalePopupController>.Instance.Show(saleToDisplay, SalePopupController.ShowLocation.TownIntro);
			//}
			//else
			//{
				AdvanceIntroState();
			//}
		}
		else if (mIntroState == IntroState.ShowSaleResults && !Singleton<SalePopupController>.Instance.CheckGrantedSale())
		{
			AdvanceIntroState();
		}
	}

	public bool IsIntroDone()
	{
		return mIntroState == IntroState.Done;
	}

	private void OnCloseHelperRewardPopup()
	{
		AdvanceIntroState();
	}

	public TownBuildingScript GetBuildingScript(string buildingId)
	{
		TownBuildingScript[] componentsInChildren = base.gameObject.GetComponentsInChildren<TownBuildingScript>();
		return componentsInChildren.Find((TownBuildingScript match) => match.BuildingId == buildingId);
	}

	public GameObject GetBuildingObject(string buildingId)
	{
		TownBuildingScript buildingScript = GetBuildingScript(buildingId);
		if (buildingScript != null)
		{
			return buildingScript.gameObject;
		}
		return null;
	}

	private void UpdateCamera()
	{
		if (!mPopulated)
		{
			return;
		}
		if (UICamera.IsInputLocked() || UICamera.ColliderRestrictionList.Count > 0)
		{
			mTouches.Clear();
		}
		if (mTouches.Count == 1)
		{
			float magnitude = (mTownCam.transform.position - mCamLookAt).magnitude;
			float y = (0f - mRotateFactor) * 360f;
			Vector3 zero = Vector3.zero;
			zero.x = 0f - mTouches[0].delta.x;
			zero.z = 0f - mTouches[0].delta.y;
			zero = Quaternion.Euler(0f, y, 0f) * zero;
			Vector3 vector = mTargetCamLookAt;
			mTargetCamLookAt += PanSensitivity * zero * magnitude;
			Ray ray = new Ray(mTargetCamLookAt, new Vector3(0f, -1f, 0f));
			RaycastHit hitInfo;
			if (!CameraMeshCollider.Raycast(ray, out hitInfo, 10f))
			{
				mTargetCamLookAt = vector;
			}
		}
		else if (mTouches.Count >= 2)
		{
			Vector2[] array = new Vector2[2]
			{
				mTouches[0].pos - mTouches[0].delta,
				mTouches[1].pos - mTouches[1].delta
			};
			float magnitude2 = (array[1] - array[0]).magnitude;
			float magnitude3 = (mTouches[1].pos - mTouches[0].pos).magnitude;
			mTargetHeightFactor -= (magnitude3 - magnitude2) * ZoomSensitivity;
			if (mTargetHeightFactor < 0f)
			{
				mTargetHeightFactor = 0f;
			}
			else if (mTargetHeightFactor > 1f)
			{
				mTargetHeightFactor = 1f;
			}
			float num = (float)Math.Atan2(array[1].y - array[0].y, array[1].x - array[0].x);
			float num2 = (float)Math.Atan2(mTouches[1].pos.y - mTouches[0].pos.y, mTouches[1].pos.x - mTouches[0].pos.x);
			float num3 = num2 - num;
			if ((double)num3 > Math.PI)
			{
				num3 -= (float)Math.PI * 2f;
			}
			else if ((double)num3 < -Math.PI)
			{
				num3 += (float)Math.PI * 2f;
			}
			mTargetRotateFactor -= num3 * RotateSensitivity;
		}
		mCamLookAt = NGUIMath.SpringLerp(mCamLookAt, mTargetCamLookAt, 10f, Time.deltaTime);
		mHeightFactor = NGUIMath.SpringLerp(mHeightFactor, mTargetHeightFactor, 10f, Time.deltaTime);
		mRotateFactor = NGUIMath.SpringLerp(mRotateFactor, mTargetRotateFactor, 10f, Time.deltaTime);
		float num4 = (float)((double)(mRotateFactor * 2f) * Math.PI - Math.PI / 2.0);
		Vector3 position = default(Vector3);
		position.y = MinHeight + mHeightFactor * (MaxHeight - MinHeight);
		float num5 = MinOffset + mHeightFactor * (MaxOffset - MinOffset);
		position.x = mCamLookAt.x + (float)Math.Cos(num4) * num5;
		position.z = mCamLookAt.z + (float)Math.Sin(num4) * num5;
		mTownCam.transform.position = position;
		mTownCam.transform.LookAt(mCamLookAt);
	}

	public void MoveCameraToZoomOn(Transform tr)
	{
		mPrevTargetHeightFactor = mTargetHeightFactor;
		mTargetHeightFactor = 0f;
		mTargetCamLookAt = tr.position;
	}

	public void MoveCameraToZoomOut()
	{
		mTargetHeightFactor = mPrevTargetHeightFactor;
	}

	private void OnPress(bool pressed)
	{
		if (pressed)
		{
			mTouches.Add(UICamera.currentTouch);
		}
		else
		{
			mTouches.Remove(UICamera.currentTouch);
		}
	}

	public void OnBuildingPressed(bool pressed)
	{
		OnPress(pressed);
	}

	public void RefreshLockedBuildingEffect()
	{
		Singleton<TownHudController>.Instance.RefreshBuildingLocks();
		TownBuildingScript[] array = UnityEngine.Object.FindObjectsOfType(typeof(TownBuildingScript)) as TownBuildingScript[];
		TownBuildingScript[] array2 = array;
		foreach (TownBuildingScript townBuildingScript in array2)
		{
			townBuildingScript.BuildingLockOnMeshes(Singleton<PlayerInfoScript>.Instance.IsFeatureUnlocked(townBuildingScript.BuildingId));
		}
	}

	public bool CheckUnlockedBuildings()
	{
		int mCurrentLevel = Singleton<PlayerInfoScript>.Instance.RankXpLevelData.mCurrentLevel;
		bool flag = false;
		PlayerRankData playerRankData = null;
		foreach (PlayerRankData unlockRank in PlayerRankDataManager.Instance.UnlockRanks)
		{
			if (unlockRank.Level > mCurrentLevel)
			{
				break;
			}
			if (unlockRank.UnlockType != 0 && !Singleton<PlayerInfoScript>.Instance.IsFeatureUnlocked(unlockRank.UnlockId))
			{
				Singleton<PlayerInfoScript>.Instance.UnlockFeature(unlockRank.UnlockId);
				flag = true;
				if (unlockRank.UnlockType == UnlockTypeEnum.Building && !UnlockAtOnceByDebug)
				{
					StartCoroutine("PlayBuildingUnlockEffect", unlockRank);
					playerRankData = unlockRank;
				}
			}
		}
		if (flag)
		{
			Singleton<TownHudController>.Instance.RefreshBuildingLocks();
		}
		return playerRankData != null;
	}

	private IEnumerator PlayBuildingUnlockEffect(PlayerRankData unlockRank)
	{
		GameObject buildingObj = GetBuildingObject(unlockRank.UnlockId);
		TownBuildingScript buildingScript = buildingObj.GetComponent<TownBuildingScript>();
		UICamera.LockInput();
		Singleton<TownHudController>.Instance.HideUIBar(true);
		Singleton<MouseOrbitCamera>.Instance.ZoomToButton(buildingScript, true);
		yield return new WaitForSeconds(Singleton<MouseOrbitCamera>.Instance.ZoomSpeedWhenUnlock);
		yield return new WaitForSeconds(0.5f);
		buildingScript.BuildingLockOnMeshes(true);
		buildingScript.StartClickAnimOnBuilding();
		GameObject fxObj = buildingObj.transform.InstantiateAsChild(BuildingUnlockEffect);
		fxObj.ChangeLayer(buildingObj.layer);
		yield return new WaitForSeconds(1f);
		TownBuildingData unlockedBuilding = TownBuildingDataManager.Instance.GetData(unlockRank.UnlockId);
		Singleton<TownHudController>.Instance.BuildingUnlockLabel.text = KFFLocalization.Get(unlockedBuilding.Name) + " " + KFFLocalization.Get("!!UNLOCKED");
		Singleton<TownHudController>.Instance.ShowBuildingUnlockBanner.PlayWithCallback(DelayUnlockInput);
	}

	private void DelayUnlockInput()
	{
		UICamera.UnlockInput();
		Singleton<MouseOrbitCamera>.Instance.UnZoom();
		AdvanceIntroState();
		Singleton<TownHudController>.Instance.RefreshBuildingLocks();
	}

	public void HideTextMesh(bool hide)
	{
		TextMesh[] array = UnityEngine.Object.FindObjectsOfType(typeof(TextMesh)) as TextMesh[];
		TextMesh[] array2 = array;
		foreach (TextMesh textMesh in array2)
		{
			Renderer component = textMesh.GetComponent<Renderer>();
			component.enabled = !hide;
		}
	}

	public bool CalendarCheck()
	{
		if (Singleton<PlayerInfoScript>.Instance.HasUnclaimedCalendarGift())
		{
			ToggleTownColliders(false);
			Singleton<CalendarGiftController>.Instance.PopupGift();
			return true;
		}
		return false;
	}

	private void InitTownCollider()
	{
		TownBuildingScript[] componentsInChildren = base.gameObject.GetComponentsInChildren<TownBuildingScript>();
		TownBuildingScript[] array = componentsInChildren;
		foreach (TownBuildingScript townBuildingScript in array)
		{
			Collider component = townBuildingScript.gameObject.GetComponent<Collider>();
			TownColliders.Add(component);
		}
	}

	public void ToggleTownColliders(bool enable)
	{
		if (TownColliders.Count == 0)
		{
			InitTownCollider();
		}
		foreach (Collider townCollider in TownColliders)
		{
			townCollider.enabled = enable;
		}
	}

	public IEnumerator PlayGachaGateOpenAnim(GameObject keyObject)
	{
		keyObject.transform.SetParent(GachaKeyParent);
		keyObject.ChangeLayerToParent();
		keyObject.transform.localPosition = Vector3.zero;
		keyObject.transform.localRotation = Quaternion.identity;
		keyObject.transform.localScale = Vector3.one;
		Animator animator = keyObject.GetComponentInChildren<Animator>();
		animator.Play("Turn");
		GachaAnimator.Play("DoorUnlock");
		Singleton<SLOTAudioManager>.Instance.PlaySound("gacha/SFX_GachaKeyUnlock");
		yield return new WaitForSeconds(GachaGateWaitTime);
	}

	public IEnumerator SpawnGachaPassByChests(List<InventorySlotItem> items)
	{
		TownScheduleData scheduleData = TownScheduleDataManager.Instance.GetCurrentScheduledTownData();
		foreach (InventorySlotItem item in items)
		{
			GameObject chest = GachaChestPassByParent.InstantiateAsChild(Singleton<PrefabReferences>.Instance.GachaChests[item.Rarity - 1]);
			chest.ChangeLayerToParent();
			chest.transform.localScale = Vector3.one;
			if (items.Count == 1)
			{
				Singleton<SLOTAudioManager>.Instance.PlaySound("gacha/SFX_GachaSplashx1");
				if (item.Rarity == 1)
				{
					Singleton<SLOTAudioManager>.Instance.PlaySound("gacha/SFX_GachaChest_Rarity1x1");
				}
				if (item.Rarity == 2)
				{
					Singleton<SLOTAudioManager>.Instance.PlaySound("gacha/SFX_GachaChest_Rarity2x1");
				}
				if (item.Rarity == 3)
				{
					Singleton<SLOTAudioManager>.Instance.PlaySound("gacha/SFX_GachaChest_Rarity5x1");
				}
			}
			else
			{
				Singleton<SLOTAudioManager>.Instance.PlaySound("gacha/SFX_GachaSplash");
				if (item.Rarity == 1)
				{
					Singleton<SLOTAudioManager>.Instance.PlaySound("gacha/SFX_GachaChest_Rarity1");
				}
				if (item.Rarity == 2)
				{
					Singleton<SLOTAudioManager>.Instance.PlaySound("gacha/SFX_GachaChest_Rarity2");
				}
				if (item.Rarity == 3)
				{
					Singleton<SLOTAudioManager>.Instance.PlaySound("gacha/SFX_GachaChest_Rarity5");
				}
			}
			UnityEngine.Object.Destroy(chest, GachaChestFinishTime);
			Animator anim = chest.GetComponentInChildren<Animator>();
			if (items.Count == 1)
			{
				anim.Play(scheduleData.SingleGachaChestAnim);
				continue;
			}
			anim.Play(scheduleData.MultiGachaChestAnim);
			yield return new WaitForSeconds(GachaChestSpacing);
		}
		if (items.Count == 1)
		{
			yield return new WaitForSeconds(GachaChestFinishTime);
		}
		else
		{
			yield return new WaitForSeconds(GachaMultiChestFinishTime);
		}
	}

	public void ResetGachaGate()
	{
		GachaAnimator.Play("DoorClose");
	}

	public void ShowGachaOpeningVFX(bool toogleOn)
	{
		if (GachaOpeningVFX != null)
		{
			GachaOpeningVFX.SetActive(toogleOn);
		}
	}
}
