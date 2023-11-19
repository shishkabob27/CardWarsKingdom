using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpeditionStartController : Singleton<ExpeditionStartController>
{
	public string[] LootMeterTextures;

	public float NeedleLerpSpeed;

	public float NeedleRandomBounceSpeed;

	public float NeedleRandomBounceAmount;

	public float NeedleRestingAngle;

	public GameObject ExpeditionPrefab;

	public UILabel Name;

	public UILabel Duration;

	public UILabel FavoredClass;

	public UILabel DragCreaturesPromptLabel;

	public UILabel TimeUntilRefreshLabel;

	public UIGrid SlotGrid;

	public UITexture ArtTexture;

	public UIStreamingGrid ItemGrid;

	private UIStreamingGridDataSource<InventorySlotItem> mItemGridDataSource = new UIStreamingGridDataSource<InventorySlotItem>();

	public Transform SortButton;

	public GameObject StartButton;

	public GameObject SpeedUpButton;

	public GameObject CancelButton;

	public GameObject DragCreaturesPrompt;

	public UILabel ClaimCompleteLabel;

	public GameObject BigCheckmarkIcon;

	public UIStreamingGrid AvailableGrid;

	public Transform LootNeedle;

	public UITexture LootMeterGraphic;

	public GameObject[] LootMeterSegments;

	public GameObject ExpeditionsAvailableParent;

	public GameObject NoExpeditionsAvailableParent;

	public UILabel RepopulateCostLabel;

	public UITexture CreaturesFrame;

	public ParticleSystem LootMeterGraphicChangeVFX;

	public GameObject ItemTileDropVFX;

	public InventoryBarController InventoryBar;

	public UITexture frame;

	private List<GameObject> CreatureSlots = new List<GameObject>();

	private ExpeditionItem mExpedition;

	private InventoryTile[] mSelectedCreatures;

	private UIStreamingGridDataSource<ExpeditionItem> mAvailableGridDataSource = new UIStreamingGridDataSource<ExpeditionItem>();

	private int mHighlightedIndex;

	private static bool mShowing;

	private bool mIsCompleteAnimPlaying;

	private float mStartNeedleAngle;

	private float mTargetNeedleAngle;

	private float mCurrentNeedleAngle;

	private float mNeedleRandomPerlin;

	[SerializeField]
	private UIGrid _ExpeditionRewardsPopUpGrid;

	[SerializeField]
	private UIScrollView _ExpeditionRewardsPopUpScrollView;

	[SerializeField]
	private ExpeditionRewardsMenuItem _ExpeditionRewardsMenuItemPrefab;

	[Header("Tweens")]
	public UITweenController ShowTween;

	public UITweenController HideTween;

	public UITweenController ShowRewardsPopupTween;

	public UITweenController HideRewardsPopupTween;

	public UITweenController ExpeditionCompleteTween;

	[SerializeField]
	[Header("Info Card Colorize")]
	private UISprite _TitleBarOutline;

	[SerializeField]
	private UISprite _TitleBarFill;

	[SerializeField]
	private UISprite _BgRect;

	[SerializeField]
	private UISprite _MiddleBarOutline;

	[SerializeField]
	private UISprite _MiddleBarFill;

	[SerializeField]
	private UITexture _BottomChiseledField;

	[SerializeField]
	private UISprite _LeftArrow;

	[SerializeField]
	private UISprite _RightArrow;

	[SerializeField]
	private UISprite _FactionIcon;

	[SerializeField]
	private TweenColor _DragPromptTweenColor;

	public ExpeditionsColorPalette[] ColorPalette = new ExpeditionsColorPalette[7];

	public ExpeditionsColorPalette[] DifficultyPalette = new ExpeditionsColorPalette[6];

	public bool Showing
	{
		get
		{
			return mShowing;
		}
	}

	public ExpeditionListPrefab SelectedExpeditionPrefab { get; set; }

	public ExpeditionListPrefab BuySlotItem { get; set; }

	public bool AnimatingExtraSlotBuy { get; private set; }

	public ExpeditionItem ShowingExpedition()
	{
		return mExpedition;
	}

	private void Awake()
	{
		CreatureSlots.Clear();
		int num = 1;
		while (true)
		{
			Transform transform = SlotGrid.transform.FindChild("CreatureSlot_" + num.ToString("D2"));
			if (transform == null)
			{
				break;
			}
			CreatureSlots.Add(transform.gameObject);
			num++;
		}
		TimeUntilRefreshLabel.text = string.Empty;
		mSelectedCreatures = new InventoryTile[CreatureSlots.Count];
		mStartNeedleAngle = LootNeedle.localEulerAngles.z;
		AdjustFrameBoarderOnLowRes();
	}

	private void Update()
	{
		if (mExpedition != null)
		{
			if (mIsCompleteAnimPlaying)
			{
				UpdateNeedleAngle();
				return;
			}
			if (mExpedition.InProgress)
			{
				uint num = TFUtils.ServerTime.UnixTimestamp();
				if (num < mExpedition.EndTime)
				{
					SpeedUpButton.SetActive(true);
					CancelButton.SetActive(true);
					ClaimCompleteLabel.gameObject.SetActive(false);
					BigCheckmarkIcon.SetActive(false);
					Duration.gameObject.SetActive(true);
					int totalSeconds = (int)(mExpedition.EndTime - num);
					Duration.text = PlayerInfoScript.FormatTimeString(totalSeconds, true);
				}
				else
				{
					DateTime serverTime = TFUtils.ServerTime;
					mIsCompleteAnimPlaying = true;
					SpeedUpButton.SetActive(false);
					CancelButton.SetActive(false);
					ClaimCompleteLabel.gameObject.SetActive(true);
					Duration.gameObject.SetActive(false);
					ExpeditionCompleteTween.Play();
				}
				StartButton.SetActive(false);
				DragCreaturesPrompt.SetActive(false);
			}
			else
			{
				ClaimCompleteLabel.gameObject.SetActive(false);
				SpeedUpButton.SetActive(false);
				CancelButton.SetActive(false);
				Duration.gameObject.SetActive(true);
				BigCheckmarkIcon.SetActive(false);
				if (EnoughCreaturesAdded())
				{
					StartButton.SetActive(true);
					DragCreaturesPrompt.SetActive(false);
				}
				else
				{
					StartButton.SetActive(false);
					DragCreaturesPrompt.SetActive(true);
				}
			}
			UpdateNeedleAngle();
		}
		if (mShowing && Singleton<TownController>.Instance.NeedToShowTown())
		{
			mShowing = false;
			HideTween.Play();
		}
	}

	public void Show()
	{
		ShowTween.Play();
		mShowing = true;
		if (DetachedSingleton<ExpeditionManager>.Instance.CurrentExpeditions.Count > 0)
		{
			if (mHighlightedIndex >= DetachedSingleton<ExpeditionManager>.Instance.CurrentExpeditions.Count)
			{
				mHighlightedIndex = DetachedSingleton<ExpeditionManager>.Instance.CurrentExpeditions.Count - 1;
			}
			ShowExpedition(DetachedSingleton<ExpeditionManager>.Instance.CurrentExpeditions[mHighlightedIndex], true);
		}
		else
		{
			ExpeditionsAvailableParent.SetActive(false);
			NoExpeditionsAvailableParent.SetActive(true);
		}
		RepopulateCostLabel.text = ExpeditionParams.RepopulateCost.ToString();
		Singleton<TutorialController>.Instance.StartTutorialBlockIfNotComplete("ExpedTut");
	}

	public void AdjustFrameBoarderOnLowRes()
	{
		if (!(frame == null) && KFFLODManager.IsLowEndDevice())
		{
			frame.border = new Vector4(frame.border.x * 0.7f, frame.border.y * 0.7f, frame.border.z * 0.7f, frame.border.w * 0.7f);
		}
	}

	public void PopulateLists()
	{
		List<ExpeditionItem> list = DetachedSingleton<ExpeditionManager>.Instance.CurrentExpeditions.Copy();
		if (ExpeditionSlotCostDataManager.Instance.GetNextSlotPurchaseCost() != -1)
		{
			list.Add(null);
		}
		mAvailableGridDataSource.Init(AvailableGrid, ExpeditionPrefab, list);
	}

	private void RefreshList()
	{
		mAvailableGridDataSource.RepopulateObjects();
	}

	public void ShowExpedition(ExpeditionItem expedition, bool loadingIntoScreen)
	{
		mIsCompleteAnimPlaying = false;
		DestroyCreatureTiles();
		NoExpeditionsAvailableParent.SetActive(false);
		ExpeditionsAvailableParent.SetActive(true);
		InventoryTile.ClearDelegates(true);
		mExpedition = expedition;
		mHighlightedIndex = DetachedSingleton<ExpeditionManager>.Instance.CurrentExpeditions.IndexOf(expedition);
		if (mHighlightedIndex == -1)
		{
			mHighlightedIndex = 0;
		}
		Singleton<SLOTResourceManager>.Instance.QueueUITextureLoad(mExpedition.NameData.Texture, mExpedition.NameData.TextureAssetBundle, "UI/UI/LoadingPlaceholder", ArtTexture);
		Name.text = mExpedition.NameData.Name;
		string newValue = PlayerInfoScript.FormatTimeString(mExpedition.Duration, true, true);
		Duration.text = KFFLocalization.Get("!!EXPEDITION_DURATION").Replace("<val1>", newValue);
		if (mExpedition.FavoredClass == CreatureFaction.Count)
		{
			FavoredClass.text = string.Empty;
		}
		else
		{
			FavoredClass.text = KFFLocalization.Get("!!X_CLASS_PREFERRED").Replace("<val1>", mExpedition.FavoredClass.ClassDisplayName());
		}
		for (int i = 0; i < CreatureSlots.Count; i++)
		{
			CreatureSlots[i].SetActive(i < mExpedition.CreatureCount);
		}
		SlotGrid.Reposition();
		CreaturesFrame.width = Mathf.Min(mExpedition.CreatureCount * 86 + 60, 480);
		for (int j = 0; j < LootMeterSegments.Length; j++)
		{
			LootMeterSegments[j].SetActive(j < mExpedition.Difficulty.Difficulty);
		}
		if (mExpedition.InProgress)
		{
			for (int k = 0; k < mExpedition.UsedCreatureUIDs.Count; k++)
			{
				int uniqueId = mExpedition.UsedCreatureUIDs[k];
				InventorySlotItem creatureItem = Singleton<PlayerInfoScript>.Instance.SaveData.GetCreatureItem(uniqueId);
				if (creatureItem != null)
				{
					mSelectedCreatures[k] = CreatureSlots[k].transform.InstantiateAsChild(Singleton<PrefabReferences>.Instance.InventoryTile).GetComponent<InventoryTile>();
					mSelectedCreatures[k].Populate(creatureItem);
					mSelectedCreatures[k].AdjustDepth(6);
				}
			}
		}
		UpdateRewardMeter(true);
		if (loadingIntoScreen)
		{
			PopulateLists();
		}
		else
		{
			RefreshList();
		}
		ColorInfoCardDisplay(mExpedition.FavoredClass);
		InventoryBarController.onDoFilterNull();
		InventoryBarController.onDoFilter += PopulateItemList;
		InventoryBar.SetFilters(false, false, true, true, true);
		InventoryBar.UpdateInventoryCounter();
		Update();
	}

	private void ColorInfoCardDisplay(CreatureFaction inFaction)
	{
		ExpeditionsColorPalette expeditionsColorPalette = ColorPalette[(int)inFaction];
		Color effectColor = new Color(expeditionsColorPalette.Colors[1].r, expeditionsColorPalette.Colors[1].g, expeditionsColorPalette.Colors[1].b, 0.1f);
		Name.effectColor = effectColor;
		FavoredClass.effectColor = effectColor;
		if (_FactionIcon != null)
		{
			_FactionIcon.spriteName = inFaction.IconTexture();
			_FactionIcon.gameObject.SetActive(!string.IsNullOrEmpty(_FactionIcon.spriteName));
		}
		ClaimCompleteLabel.effectColor = expeditionsColorPalette.Colors[0];
		_TitleBarOutline.color = expeditionsColorPalette.Colors[1];
		_TitleBarFill.color = expeditionsColorPalette.Colors[0];
		_MiddleBarOutline.color = expeditionsColorPalette.Colors[1];
		_MiddleBarFill.color = expeditionsColorPalette.Colors[0];
		_BgRect.color = expeditionsColorPalette.Colors[2];
		_BottomChiseledField.color = expeditionsColorPalette.Colors[0];
		_LeftArrow.color = expeditionsColorPalette.Colors[0];
		_RightArrow.color = expeditionsColorPalette.Colors[0];
		_DragPromptTweenColor.from = expeditionsColorPalette.SelectorColors[0];
		_DragPromptTweenColor.to = expeditionsColorPalette.SelectorColors[1];
	}

	private void UpdateRewardMeter(bool firstTime)
	{
		float num = 0f;
		InventoryTile[] array = mSelectedCreatures;
		foreach (InventoryTile inventoryTile in array)
		{
			if (inventoryTile != null)
			{
				float num2 = inventoryTile.InventoryItem.Creature.StarRating;
				if (inventoryTile.InventoryItem.Creature.Form.Faction == mExpedition.FavoredClass)
				{
					num2 *= ExpeditionParams.FavoredFactionValue;
				}
				num += num2;
			}
		}
		int tierFromStars = ExpeditionDifficultyDataManager.Instance.GetTierFromStars((int)num);
		tierFromStars = Mathf.Min(tierFromStars, mExpedition.Difficulty.Difficulty);
		string empty = string.Empty;
		string empty2 = string.Empty;
		float num3 = mStartNeedleAngle * 2f / (float)(LootMeterSegments.Length - 1);
		if (tierFromStars == 0)
		{
			mTargetNeedleAngle = NeedleRestingAngle - 5f;
			if (LootMeterGraphic.mainTexture != null)
			{
				empty = LootMeterGraphic.mainTexture.name;
			}
			LootMeterGraphic.ReplaceTexture(LootMeterTextures[0]);
			empty2 = LootMeterGraphic.mainTexture.name;
		}
		else
		{
			mTargetNeedleAngle = mStartNeedleAngle - (float)(tierFromStars - 1) * num3;
			if (LootMeterGraphic.mainTexture != null)
			{
				empty = LootMeterGraphic.mainTexture.name;
			}
			LootMeterGraphic.ReplaceTexture(LootMeterTextures[tierFromStars - 1]);
			empty2 = LootMeterGraphic.mainTexture.name;
		}
		if (firstTime)
		{
			mCurrentNeedleAngle = mTargetNeedleAngle;
			LootNeedle.localEulerAngles = new Vector3(0f, 0f, mTargetNeedleAngle);
		}
		else if (empty2 != empty && empty != null)
		{
			LootMeterGraphicChangeVFX.Play();
		}
	}

	private void UpdateNeedleAngle()
	{
		mNeedleRandomPerlin += Time.deltaTime * NeedleRandomBounceSpeed;
		mCurrentNeedleAngle = Mathf.Lerp(mCurrentNeedleAngle, mTargetNeedleAngle, NeedleLerpSpeed * Time.deltaTime);
		float num = 2f * (Mathf.PerlinNoise(mNeedleRandomPerlin, 0f) - 0.5f) * NeedleRandomBounceAmount;
		if (mCurrentNeedleAngle > mStartNeedleAngle)
		{
			num *= 1f - (mCurrentNeedleAngle - mStartNeedleAngle) / (NeedleRestingAngle - mStartNeedleAngle);
		}
		LootNeedle.localEulerAngles = new Vector3(0f, 0f, mCurrentNeedleAngle + num);
	}

	public void PopulateItemList()
	{
		InventoryTile.SetDelegates(InventorySlotType.Creature, TileDraggable, OnTileDragBeginFromSlot, OnTileDropped, RefreshTileOverlay, PopulateItemList, OnPopupClosed);
		Singleton<PlayerInfoScript>.Instance.SaveData.SortInventory(InventorySlotType.Creature);
		mItemGridDataSource.Init(ItemGrid, Singleton<PrefabReferences>.Instance.InventoryTile, InventoryBar.GetFilteredInventory());
		base.gameObject.GetComponent<InventoryBarController>().ResetScrollView();
	}

	private void OnPopupClosed(InventorySlotItem creature)
	{
		PopulateItemList();
		InventoryTile[] array = mSelectedCreatures;
		foreach (InventoryTile inventoryTile in array)
		{
			if (inventoryTile != null)
			{
				RefreshTileOverlay(inventoryTile);
			}
		}
	}

	private bool TileDraggable(InventoryTile tile)
	{
		if (tile.IsAttachedToTarget())
		{
			return true;
		}
		if (!InventoryTile.IsUpwardsDrag())
		{
			return false;
		}
		if (IsCreatureSelected(tile.InventoryItem.Creature))
		{
			return false;
		}
		if (!IsValidCreature(tile.InventoryItem))
		{
			return false;
		}
		return true;
	}

	private void OnTileDragBeginFromSlot(InventoryTile tile)
	{
		mSelectedCreatures[tile.AssignedSlot] = null;
		UpdateRewardMeter(false);
	}

	private bool OnTileDropped(InventoryTile tile, int slotIndex)
	{
		if (slotIndex == -1)
		{
			return false;
		}
		if (mExpedition.InProgress)
		{
			return false;
		}
		if (mSelectedCreatures[slotIndex] != null)
		{
			NGUITools.Destroy(mSelectedCreatures[slotIndex].gameObject);
		}
		mSelectedCreatures[slotIndex] = tile;
		tile.transform.parent = CreatureSlots[slotIndex].transform;
		tile.transform.position = CreatureSlots[slotIndex].transform.position;
		tile.AssignedSlot = slotIndex;
		GameObject gameObject = tile.transform.parent.gameObject.InstantiateAsChild(ItemTileDropVFX);
		gameObject.transform.localScale = Vector3.one;
		gameObject.transform.localPosition = Vector3.zero;
		tile.EquipTween.Play();
		Singleton<TutorialController>.Instance.AdvanceIfDraggingTile();
		UpdateRewardMeter(false);
		Update();
		return true;
	}

	private void RefreshTileOverlay(InventoryTile tile)
	{
		if (IsCreatureSelected(tile.InventoryItem.Creature))
		{
			tile.SetOverlayStatus(InventoryTile.TileStatus.SelectedAsTarget, false);
		}
		else if (!IsValidCreature(tile.InventoryItem))
		{
			tile.SetOverlayStatus(InventoryTile.TileStatus.NotSelected, false);
			tile.FadeGroup.SetActive(true);
		}
		else
		{
			tile.SetOverlayStatus(InventoryTile.TileStatus.NotSelected, false);
		}
	}

	private bool IsCreatureSelected(CreatureItem creature)
	{
		return mSelectedCreatures.Find((InventoryTile m) => m != null && m.InventoryItem.Creature == creature) != null;
	}

	private bool IsValidCreature(InventorySlotItem creature)
	{
		if (creature.InUse() || creature.IsHelper())
		{
			return false;
		}
		return true;
	}

	private int AssignedCreatureCount()
	{
		int num = 0;
		InventoryTile[] array = mSelectedCreatures;
		foreach (InventoryTile inventoryTile in array)
		{
			if (inventoryTile != null)
			{
				num++;
			}
		}
		return num;
	}

	private bool EnoughCreaturesAdded()
	{
		return AssignedCreatureCount() >= 1;
	}

	public void OnClickSort()
	{
		Singleton<SortPopupController>.Instance.Show(SortPopupController.Category.Creatures, SortButton, PopulateItemList);
	}

	public void OnClickStart()
	{
		if (!EnoughCreaturesAdded())
		{
			Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!NOT_ENOUGH_CREATURES_FOR_EXPEDITION"));
			return;
		}
		DetachedSingleton<ExpeditionManager>.Instance.BeginExpedition(mExpedition, GetSelectedCreatureItems());
		ShowExpedition(mExpedition, false);
	}

	private void StaminaRefillExecute()
	{
		DetachedSingleton<StaminaManager>.Instance.RefillStamina();
		Singleton<BuyStaminaPopupController>.Instance.Show(OnClickStart);
	}

	public void OnClickCancel()
	{
		string body = KFFLocalization.Get("!!CONFIRM_CANCEL_EXPEDITION").Replace("<val1>", mExpedition.NameData.Name);
		Singleton<SimplePopupController>.Instance.ShowPrompt(string.Empty, body, CancelExpedition, null);
	}

	private void CancelExpedition()
	{
		DetachedSingleton<ExpeditionManager>.Instance.CancelExpedition(mExpedition);
		ShowExpedition(mExpedition, false);
	}

	private List<InventorySlotItem> GetSelectedCreatureItems()
	{
		List<InventorySlotItem> list = new List<InventorySlotItem>();
		InventoryTile[] array = mSelectedCreatures;
		foreach (InventoryTile inventoryTile in array)
		{
			if (inventoryTile != null)
			{
				list.Add(inventoryTile.InventoryItem);
			}
		}
		return list;
	}

	private void DestroyCreatureTiles()
	{
		for (int i = 0; i < mSelectedCreatures.Length; i++)
		{
			if (mSelectedCreatures[i] != null)
			{
				NGUITools.Destroy(mSelectedCreatures[i].gameObject);
				mSelectedCreatures[i] = null;
			}
		}
	}

	public void Unload()
	{
		mAvailableGridDataSource.Clear();
		mItemGridDataSource.Clear();
		DestroyCreatureTiles();
		LootMeterGraphic.UnloadTexture();
		mExpedition = null;
		mShowing = false;
	}

	public static void RefreshListIfShowing()
	{
		if (mShowing)
		{
			Singleton<ExpeditionStartController>.Instance.ShowExpedition(DetachedSingleton<ExpeditionManager>.Instance.CurrentExpeditions[Singleton<ExpeditionStartController>.Instance.mHighlightedIndex], true);
		}
	}

	public void OnRewardsClosed()
	{
		if (DetachedSingleton<ExpeditionManager>.Instance.CurrentExpeditions.Count > 0)
		{
			mHighlightedIndex = 0;
			ShowExpedition(DetachedSingleton<ExpeditionManager>.Instance.CurrentExpeditions[mHighlightedIndex], true);
		}
		else
		{
			ExpeditionsAvailableParent.SetActive(false);
			NoExpeditionsAvailableParent.SetActive(true);
		}
		Singleton<TownController>.Instance.CheckIntroStateAfterXPGain();
	}

	public void BuyExtraSlot(ExpeditionListPrefab inListItem)
	{
		StartCoroutine(BuyExtraSlotCo(inListItem));
	}

	private IEnumerator BuyExtraSlotCo(ExpeditionListPrefab inListItem)
	{
		AnimatingExtraSlotBuy = true;
		PlayerSaveData saveData = Singleton<PlayerInfoScript>.Instance.SaveData;
		Singleton<BusyIconPanelController>.Instance.Show();
		if (SelectedExpeditionPrefab != null)
		{
			SelectedExpeditionPrefab.Deselect();
		}
		SelectedExpeditionPrefab = null;
		int status = -1;
		saveData.ConsumeHardCurrency2(ExpeditionSlotCostDataManager.Instance.GetNextSlotPurchaseCost(), "expedition slot", delegate(PlayerSaveData.ActionResult result)
		{
			if (result.success)
			{
				status = 1;
			}
			else
			{
				status = 0;
			}
		});
		while (status == -1)
		{
			yield return null;
		}
		Singleton<BusyIconPanelController>.Instance.Hide();
		if (status == 0)
		{
			Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!SERVER_ERROR_MESSAGE"), null);
			yield break;
		}
		++saveData.ExpeditionSlots;
		DetachedSingleton<ExpeditionManager>.Instance.GenerateExpeditions(1);
		Singleton<PlayerInfoScript>.Instance.Save();
		int numExpeditions = DetachedSingleton<ExpeditionManager>.Instance.CurrentExpeditions.Count;
		mHighlightedIndex = numExpeditions - 1;
		ExpeditionItem expeditionItem = DetachedSingleton<ExpeditionManager>.Instance.CurrentExpeditions[mHighlightedIndex];
		yield return new WaitForSeconds(0.3f);
		inListItem.Populate(expeditionItem);
		Singleton<ExpeditionStartController>.Instance.ShowExpedition(expeditionItem, false);
		if (BuySlotItem != null)
		{
			BuySlotItem.UnlockTween.Play();
		}
		yield return new WaitForSeconds(2.2f);
		AnimatingExtraSlotBuy = false;
		PopulateLists();
	}

	public void RepopulateList()
	{
		Singleton<SimplePopupController>.Instance.ShowPurchasePrompt(KFFLocalization.Get("!!CONFIRM_EXPEDITION_REPOPULATE"), KFFLocalization.Get("!!EXPEDITION_REPOPULATE_NOBUY"), ExpeditionParams.RepopulateCost, ConfirmRepopulateList);
	}

	public void ConfirmRepopulateList()
	{
		StartCoroutine(RepopulateListCo());
	}

	private IEnumerator RepopulateListCo()
	{
		PlayerSaveData saveData = Singleton<PlayerInfoScript>.Instance.SaveData;
		Singleton<BusyIconPanelController>.Instance.Show();
		int status = -1;
		saveData.ConsumeHardCurrency2(ExpeditionParams.RepopulateCost, "expedition refresh", delegate(PlayerSaveData.ActionResult result)
		{
			if (result.success)
			{
				status = 1;
			}
			else
			{
				status = 0;
			}
		});
		while (status == -1)
		{
			yield return null;
		}
		Singleton<BusyIconPanelController>.Instance.Hide();
		if (status == 0)
		{
			Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!SERVER_ERROR_MESSAGE"), null);
			yield break;
		}
		DetachedSingleton<ExpeditionManager>.Instance.AssignNewExpeditions();
		Singleton<PlayerInfoScript>.Instance.Save();
		PopulateLists();
	}

	public void OnClickSpeedUp()
	{
		ShowSpeedUpPopup(mExpedition);
	}

	public void ShowSpeedUpPopup(ExpeditionItem expedition)
	{
		StartCoroutine(ShowSpeedUpPopupCo(expedition));
	}

	public void SetTimeUntilRefresh(int inTimeLeft)
	{
		string text = KFFLocalization.Get("!!EXPEDITOINS_TIME_UNTIL_REFRESH").Replace("<val1>", PlayerInfoScript.FormatTimeString(inTimeLeft, true));
		TimeUntilRefreshLabel.text = text;
	}

	private IEnumerator ShowSpeedUpPopupCo(ExpeditionItem expedition)
	{
		bool done = false;
		SpeedUpData selectedSpeedup = null;
		Singleton<UseSpeedUpsPopup>.Instance.Show(delegate(SpeedUpData selected)
		{
			selectedSpeedup = selected;
			done = true;
		});
		while (!done)
		{
			yield return null;
		}
		if (selectedSpeedup != null)
		{
			expedition.EndTime -= (uint)(selectedSpeedup.Minutes * 60);
			uint duration = expedition.EndTime - TFUtils.ServerTime.UnixTimestamp();
			Singleton<KFFNotificationManager>.Instance.cancelLocalNotification(expedition.NameData.ID);
			Singleton<KFFNotificationManager>.Instance.scheduleAdventureCompleteNotification((int)duration, expedition.NameData.ID);
			ShowExpedition(mExpedition, false);
			Singleton<PlayerInfoScript>.Instance.Save();
		}
	}

	public void HandleInfoButtonPress()
	{
		_ExpeditionRewardsPopUpGrid.transform.DestroyAllChildren();
		ShowRewardsPopupTween.Play();
		for (int num = 5; num >= 0; num--)
		{
			ExpeditionDifficultyData dataByDifficulty = ExpeditionDifficultyDataManager.Instance.GetDataByDifficulty(num + 1);
			SampleExpeditionRewardsData inData = new SampleExpeditionRewardsData(KFFLocalization.Get(dataByDifficulty.Name), ColorPalette[num].Colors, dataByDifficulty.MinRarity, dataByDifficulty.MaxRarity, dataByDifficulty.MaxSoftCurrency, dataByDifficulty.MaxHardCurrency, true, true, true, true);
			SpawnSampleRewardItem(inData, num);
		}
		RepositionGrid();
		_ExpeditionRewardsPopUpScrollView.ResetPosition();
		Invoke("RepositionGrid", 0.1f);
	}

	private void SpawnSampleRewardItem(SampleExpeditionRewardsData inData, int inItemIndex)
	{
		GameObject gameObject = _ExpeditionRewardsPopUpGrid.transform.InstantiateAsChild(_ExpeditionRewardsMenuItemPrefab.gameObject);
		ExpeditionRewardsMenuItem component = gameObject.GetComponent<ExpeditionRewardsMenuItem>();
		component.Configure(inData, inItemIndex);
	}

	private void RepositionGrid()
	{
		_ExpeditionRewardsPopUpGrid.Reposition();
	}
}
