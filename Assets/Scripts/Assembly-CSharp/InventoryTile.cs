using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryTile : UIStreamingGridListItem
{
	private class Settings
	{
		public InventoryTileCheckDelegate mDraggableDelegate;

		public InventoryTileCheckDelegate mClickableDelegate;

		public InventoryTileDelegate mDragBeginFromSlotDelegate;

		public InventoryTileDelegate mRefreshOverlayDelegate;

		public InventoryTileDroppedDelegate mDroppedOnTargetDelegate;

		public SimplePopupController.PopupButtonCallback mPurchaseConfirmedDelegate;

		public CreatureDetailsController.ClosedDelegate mPopupClosedDelegate;

		public InventorySlotType mPrimarySlotType;

		public bool mClickable;

		public Settings Copy()
		{
			return (Settings)MemberwiseClone();
		}
	}

	public enum EvoMatchType
	{
		NotSet,
		Good,
		NoMatch
	}

	private enum NextAction
	{
		NONE,
		WAITING,
		PROCEED,
		ERROR,
		RETRY
	}

	public enum TileStatus
	{
		NotSelected,
		SelectedAsTarget,
		SelectedAsMaterial,
		SelectedButUnavailable
	}

	public delegate bool InventoryTileCheckDelegate(InventoryTile tile);

	public delegate void InventoryTileDelegate(InventoryTile tile);

	public delegate bool InventoryTileDroppedDelegate(InventoryTile tile, int slotIndex);

	private const string DefaultFrame = "Frame_Rounded_1";

	private const string RuneFrame = "Frame_Rune";

	public UITweenController EquipTween;

	public UITweenController BasicRevealTween;

	public UITweenController FancyRevealTween;

	public UITweenController FlyInTween;

	public TweenRotation FlyInTweenRot;

	public UITweenController NewTilePopTween;

	public UITweenController DefaultStarArrangement;

	public UITweenController FrameStarArrangement;

	public UITweenController AlgebraicFrameTween;

	public UITweenController AlgebraicFrameMiniTween;

	public GameObject FlyInStartPos;

	public GameObject CreatureParent;

	public GameObject CardParent;

	public GameObject EvoMaterialParent;

	public GameObject PortraitParent;

	public GameObject CurrencyParent;

	public GameObject BuyInventoryParent;

	public BoxCollider PortraitCollider;

	public UITexture PortraitTexture;

	public UILabel CreatureLevel;

	public UILabel CardCost;

	public UILabel CardName;

	public GameObject FadeGroup;

	public GameObject SelectedGroup;

	public GameObject Selected2Group;

	public GameObject UnavailableGroup;

	public GameObject InUseGroup;

	public GameObject FavoriteGroup;

	public GameObject RarityStarGroup;

	public GameObject HelperTagGroup;

	public GameObject ExpeditionTagGroup;

	public UISprite FrameSprite;

	public UISprite BackgroundSprite;

	public UISprite[] RarityStars;

	public Transform EggSpawnPoint;

	public HelperItem ParentHelper;

	public UISprite CurrencySprite;

	public UILabel CurrencyAmount;

	public GameObject Shine;

	private int _AssignedSlot = -1;

	private bool showLargeRarityFrame;

	public bool suppressRarityFrame;

	public GameObject[] RarityFrames;

	public GameObject[] RarityFramesMini;

	public GameObject[] RarityNewTiles;

	public GameObject[] RarityNewTilesSparklesFX;

	public bool DisableCardEditing;

	private static Settings mStaticSettings = new Settings();

	private Settings mSettings;

	private EvoMatchType _EvoMatch;

	private static int mHighestDepth = 0;

	private static InventoryTile mTileBeingDragged = null;

	public Vector3 Velocity = Vector3.zero;

	public int DepthAdjust = 20;

	private int mAdjustedDepth;

	private Vector3 mBaseColliderSize;

	private int mBaseColliderDepth;

	private CardPrefabScript mSpawnedCard;

	private static string mLvString = null;

	private bool mWaitForUserAction;

	private NextAction mUserActionProceed;

	private PlayerSaveData.ProceedNextStep mNextFunction;

	public List<SortEntry> CurrentSorts = new List<SortEntry>();

	private List<string> SortedLabels = new List<string>();

	private float mTimer;

	public float BlinkTweenInterval = 2f;

	public float BlinkSpeedIndex = 1.2f;

	private int mCount;

	private bool mTriggerStatBlinking;

	private GameObject mEggObject;

	public int AssignedSlot
	{
		get
		{
			return _AssignedSlot;
		}
		set
		{
			_AssignedSlot = value;
		}
	}

	public bool ClickableEvenIfUnknown { get; set; }

	public InventorySlotItem InventoryItem { get; set; }

	public EvoMatchType EvoMatch
	{
		get
		{
			return _EvoMatch;
		}
		set
		{
			_EvoMatch = value;
		}
	}

	public static void SetDelegates(InventorySlotType primarySlotType, InventoryTileCheckDelegate draggable, InventoryTileDelegate onDragBeginFromSlot, InventoryTileDroppedDelegate onDroppedOnTarget, InventoryTileDelegate refreshOverlay, SimplePopupController.PopupButtonCallback purchaseClickedDelegate, CreatureDetailsController.ClosedDelegate popupClosedDelegate, InventoryTileCheckDelegate clickable = null)
	{
		mStaticSettings.mPrimarySlotType = primarySlotType;
		mStaticSettings.mDraggableDelegate = draggable;
		mStaticSettings.mClickableDelegate = clickable;
		mStaticSettings.mDragBeginFromSlotDelegate = onDragBeginFromSlot;
		mStaticSettings.mDroppedOnTargetDelegate = onDroppedOnTarget;
		mStaticSettings.mRefreshOverlayDelegate = refreshOverlay;
		mStaticSettings.mPurchaseConfirmedDelegate = purchaseClickedDelegate;
		mStaticSettings.mPopupClosedDelegate = popupClosedDelegate;
		mStaticSettings.mClickable = true;
	}

	public static void ClearDelegates(bool clickable)
	{
		mStaticSettings.mPrimarySlotType = InventorySlotType.None;
		mStaticSettings.mDraggableDelegate = null;
		mStaticSettings.mDragBeginFromSlotDelegate = null;
		mStaticSettings.mDroppedOnTargetDelegate = null;
		mStaticSettings.mRefreshOverlayDelegate = null;
		mStaticSettings.mPurchaseConfirmedDelegate = null;
		mStaticSettings.mPopupClosedDelegate = null;
		mStaticSettings.mClickable = clickable;
	}

	public static bool IsExactTileBeingDragged(InventoryTile tile)
	{
		return mTileBeingDragged == tile;
	}

	public static bool IsCopyOfTileBeingDragged(InventoryTile tile)
	{
		return mTileBeingDragged != null && mTileBeingDragged.InventoryItem == tile.InventoryItem;
	}

	public static void ResetForNewScreen()
	{
		mHighestDepth = 0;
		mTileBeingDragged = null;
	}

	private void Awake()
	{
		if (mLvString == null)
		{
			mLvString = KFFLocalization.Get("!!LV");
		}
		mSettings = mStaticSettings.Copy();
		mBaseColliderSize = PortraitCollider.size;
		mBaseColliderDepth = PortraitCollider.GetComponent<UIWidget>().depth;
		Shine.SetActive(false);
	}

	public override void Populate(object dataObj)
	{
		CurrencyParent.SetActive(false);
		FrameSprite.gameObject.SetActive(true);
		PortraitTexture.gameObject.SetActive(true);
		BackgroundSprite.gameObject.SetActive(false);
		Shine.SetActive(false);
		if (dataObj is CreatureData)
		{
			InventoryItem = new InventorySlotItem(dataObj as CreatureData);
		}
		else
		{
			if (!(dataObj is InventorySlotItem))
			{
				if (dataObj is GachaSlotData)
				{
					GachaSlotData gachaSlotData = dataObj as GachaSlotData;
					PortraitTexture.ReplaceTexture(gachaSlotData.KeyUITexture);
					FrameSprite.spriteName = "Frame_Rune";
				}
				return;
			}
			InventoryItem = dataObj as InventorySlotItem;
		}
		if (InventoryItem.SlotType == InventorySlotType.Creature)
		{
			CreatureParent.SetActive(true);
			CardParent.SetActive(false);
			EvoMaterialParent.SetActive(false);
			XPLevelData levelData = InventoryItem.Creature.GetLevelData();
			if (InventoryItem.Creature.Form != null)
			{
				PortraitTexture.ReplaceTexture(InventoryItem.Creature.Form.PortraitTexture);
			}
			SetLevelText(levelData.mCurrentLevel, levelData.mIsAtMaxLevel);
			RarityStarGroup.SetActive(true);
			for (int i = 0; i < RarityStars.Length; i++)
			{
				RarityStars[i].gameObject.SetActive(InventoryItem.Creature.StarRating > i);
			}
			if (InventoryItem.Creature.Form != null)
			{
				FrameSprite.spriteName = InventoryItem.Creature.Faction.CreaturePortraitFrameSpriteName();
			}
		}
		else if (InventoryItem.SlotType == InventorySlotType.DisplayCreature)
		{
			PopulateDisplayCreature(false);
		}
		else if (InventoryItem.SlotType == InventorySlotType.Card || InventoryItem.SlotType == InventorySlotType.DisplayCard)
		{
			CreatureParent.SetActive(false);
			CardParent.SetActive(true);
			EvoMaterialParent.SetActive(false);
			RarityStarGroup.SetActive(false);
			CardData cardData = ((InventoryItem.SlotType != InventorySlotType.Card) ? InventoryItem.DisplayCard : InventoryItem.Card.Form);
			CardName.text = cardData.Name;
			CardCost.text = cardData.Cost.ToString();
			Singleton<SLOTResourceManager>.Instance.QueueUITextureLoad(cardData.UITexture, cardData.AssetBundle, "UI/UI/LoadingPlaceholder", PortraitTexture);
			FrameSprite.spriteName = cardData.Faction.CreaturePortraitFrameSpriteName();
		}
		else if (InventoryItem.SlotType == InventorySlotType.EvoMaterial)
		{
			CreatureParent.SetActive(false);
			CardParent.SetActive(false);
			EvoMaterialParent.SetActive(true);
			RarityStarGroup.SetActive(false);
			PortraitTexture.ReplaceTexture(InventoryItem.EvoMaterial.UITexture);
			FrameSprite.width = 147;
			FrameSprite.spriteName = InventoryItem.EvoMaterial.Faction.CreatureShardFrameSpriteName();
			InventoryItem.EvoMaterial.Faction.CreatureShardFrameSpriteName();
			if (InventoryItem.EvoMaterial.AwakenMat)
			{
				Shine.SetActive(true);
			}
		}
		else if (InventoryItem.SlotType == InventorySlotType.XPMaterial)
		{
			CreatureParent.SetActive(false);
			CardParent.SetActive(false);
			EvoMaterialParent.SetActive(false);
			RarityStarGroup.SetActive(false);
			PortraitTexture.ReplaceTexture(InventoryItem.XPMaterial.UITexture);
			FrameSprite.spriteName = InventoryItem.XPMaterial.Faction.CakeFrameSpriteName();
		}
		else if (InventoryItem.SlotType == InventorySlotType.Empty)
		{
			CreatureParent.SetActive(false);
			CardParent.SetActive(false);
			EvoMaterialParent.SetActive(false);
			RarityStarGroup.SetActive(false);
			BackgroundSprite.gameObject.SetActive(true);
			PortraitTexture.ReplaceTexture("UI/UI/UI_Creature_0");
			FrameSprite.spriteName = "Frame_Rounded_1";
		}
		else if (InventoryItem.SlotType == InventorySlotType.Purchase)
		{
			CreatureParent.SetActive(false);
			CardParent.SetActive(false);
			EvoMaterialParent.SetActive(false);
			RarityStarGroup.SetActive(false);
			BackgroundSprite.gameObject.SetActive(true);
			PortraitTexture.ReplaceTexture("UI/UI/UI_Creature_Add");
			FrameSprite.spriteName = "Frame_Rounded_1";
			BuyInventoryParent.SetActive(Singleton<TutorialController>.Instance.IsBlockComplete("UseGacha"));
		}
		ShowRarityFrameMini();
		RefreshOverlay();
	}

	public void SetLevelText(int inCurrentLevel, bool inIsAtMaxLevel)
	{
		CreatureLevel.text = mLvString + " " + inCurrentLevel;
		CreatureLevel.applyGradient = inIsAtMaxLevel;
	}

	private void PopulateDisplayCreature(bool forceShow)
	{
		CreatureParent.SetActive(true);
		CardParent.SetActive(false);
		EvoMaterialParent.SetActive(false);
		CurrencyParent.SetActive(false);
		if (forceShow || InventoryItem.DisplayCreature.AlreadySeen)
		{
			FrameSprite.spriteName = InventoryItem.DisplayCreature.Faction.CreaturePortraitFrameSpriteName();
			PortraitTexture.ReplaceTexture(InventoryItem.DisplayCreature.PortraitTexture);
		}
		else
		{
			FrameSprite.spriteName = "Frame_Rounded_1";
			PortraitTexture.ReplaceTexture("UI/UI/UI_Creature_00");
		}
		if (!forceShow)
		{
			CreatureLevel.depth = 17;
			CreatureLevel.text = "#" + InventoryItem.DisplayCreature.CreatureNumber;
		}
		else
		{
			CreatureLevel.text = string.Empty;
		}
		RarityStarGroup.SetActive(true);
		for (int i = 0; i < RarityStars.Length; i++)
		{
			RarityStars[i].gameObject.SetActive(InventoryItem.DisplayCreature.Rarity > i);
		}
	}

	public void PopulateAndForceDisplay(CreatureData creature)
	{
		InventoryItem = new InventorySlotItem(creature);
		ClickableEvenIfUnknown = true;
		PopulateDisplayCreature(true);
	}

	public override void Unload()
	{
		PortraitTexture.UnloadTexture();
	}

	public bool IsDraggable()
	{
		if (mSettings == null)
		{
			return false;
		}
		if (mSettings.mPrimarySlotType != InventorySlotType.None && InventoryItem.SlotType != mSettings.mPrimarySlotType)
		{
			return false;
		}
		if (mSettings.mDraggableDelegate == null)
		{
			return false;
		}
		if (InventoryItem.IsOnExpedition())
		{
			return false;
		}
		if (Singleton<TutorialController>.Instance.IsBlockActive("IntroBattle") || Singleton<TutorialController>.Instance.IsBlockActive("Q1") || Singleton<TutorialController>.Instance.IsBlockActive("UseEditDeck") || Singleton<TutorialController>.Instance.IsBlockActive("Q2"))
		{
			return false;
		}
		return mSettings.mDraggableDelegate(this);
	}

	public static bool IsUpwardsDrag()
	{
		Vector2 totalDelta = UICamera.currentTouch.totalDelta;
		if (totalDelta.y <= 0f)
		{
			return false;
		}
		if (Mathf.Abs(totalDelta.x) > Mathf.Abs(totalDelta.y * 1.6f))
		{
			return false;
		}
		return true;
	}

	public static bool IsRightToLeftDrag()
	{
		Vector2 totalDelta = UICamera.currentTouch.totalDelta;
		if (totalDelta.x >= 0f)
		{
			return false;
		}
		if (Mathf.Abs(totalDelta.y) > Mathf.Abs(totalDelta.x * 1.6f))
		{
			return false;
		}
		return true;
	}

	private void OnPress(bool isPressed)
	{
	}

	public void OnDragBegin(GameObject cloneSource)
	{
		if (cloneSource != null)
		{
			InventoryTile component = cloneSource.GetComponent<InventoryTile>();
			InventoryItem = component.InventoryItem;
			base.ParentGrid = component.ParentGrid;
			HelperPrefabScript componentInParent = cloneSource.GetComponentInParent<HelperPrefabScript>();
			if (componentInParent != null)
			{
				ParentHelper = componentInParent.Helper;
				Singleton<PreMatchController>.Instance.TriggerTweenForDraggingHelper();
			}
		}
		mHighestDepth++;
		AdjustDepth(mHighestDepth);
		if (AssignedSlot != -1 && mSettings.mDragBeginFromSlotDelegate != null)
		{
			mSettings.mDragBeginFromSlotDelegate(this);
		}
		mTileBeingDragged = this;
		RefreshAllOverlays();
		SetDepthOffset(100);
	}

	public void OnDragFinished()
	{
		mTileBeingDragged = null;
		RefreshAllOverlays();
		SetDepthOffset(-100);
	}

	private void OnClick()
	{
		if (!mSettings.mClickable || (mSettings.mClickableDelegate != null && !mSettings.mClickableDelegate(this)) || InventoryItem == null)
		{
			return;
		}
		if (InventoryItem.SlotType == InventorySlotType.DisplayCreature)
		{
			if (InventoryItem.DisplayCreature.AlreadySeen || ClickableEvenIfUnknown)
			{
				Singleton<CreatureDetailsController>.Instance.ShowCollectionCreature(InventoryItem.DisplayCreature, mSettings.mPopupClosedDelegate);
			}
		}
		else if (InventoryItem.SlotType == InventorySlotType.Purchase)
		{
			if (Singleton<TutorialController>.Instance.IsBlockComplete("UseGacha"))
			{
				OnClickPurchase();
			}
		}
		else if (InventoryItem.SlotType == InventorySlotType.Creature)
		{
			Singleton<CreatureDetailsController>.Instance.ShowCreature(InventoryItem, mSettings.mPopupClosedDelegate, !DisableCardEditing);
		}
		else if (InventoryItem.SlotType == InventorySlotType.Card || InventoryItem.SlotType == InventorySlotType.DisplayCard || InventoryItem.SlotType == InventorySlotType.EvoMaterial || InventoryItem.SlotType == InventorySlotType.XPMaterial)
		{
			if (mSpawnedCard != null)
			{
				StartCoroutine(mSpawnedCard.ToggleZoom());
			}
			else
			{
				SpawnCard(true);
			}
		}
	}

	public bool OnDroppedOnTarget(int slotIndex)
	{
		if (mSettings.mDroppedOnTargetDelegate != null)
		{
			return mSettings.mDroppedOnTargetDelegate(this, slotIndex);
		}
		return false;
	}

	public bool IsAttachedToTarget()
	{
		return base.transform.parent.GetComponent<CreaturePortraitDragTarget>() != null;
	}

	private void RefreshAllOverlays()
	{
		if (!(base.ParentGrid != null))
		{
			return;
		}
		for (int i = 0; i < base.ParentGrid.transform.childCount; i++)
		{
			InventoryTile componentInChildren = base.ParentGrid.transform.GetChild(i).GetComponentInChildren<InventoryTile>();
			if (componentInChildren != null)
			{
				componentInChildren.RefreshOverlay();
			}
		}
	}

	private void RefreshOverlay()
	{
		if (InventoryItem != null)
		{
			if (mSettings.mRefreshOverlayDelegate == null || InventoryItem.SlotType == InventorySlotType.Empty || InventoryItem.SlotType == InventorySlotType.Purchase)
			{
				ClearOverlays();
			}
			else if (mSettings.mPrimarySlotType != InventorySlotType.None && InventoryItem.SlotType != mSettings.mPrimarySlotType)
			{
				SetOverlayStatus(TileStatus.NotSelected);
				FadeGroup.SetActive(true);
			}
			else
			{
				mSettings.mRefreshOverlayDelegate(this);
			}
		}
	}

	private void OnClickPurchase()
	{
		Singleton<SimplePopupController>.Instance.ShowPurchasePrompt(KFFLocalization.Get("!!INVENTORY_SLOTS_CONFIRM").Replace("<val2>", MiscParams.InventorySpacePerPurchase.ToString()), KFFLocalization.Get("!!INVENTORY_SLOTS_NOBUY"), MiscParams.InventorySpacePurchaseCost, OnClickConfirmPurchaseSlots);
	}

	private void OnClickConfirmPurchaseSlots()
	{
		PlayerSaveData saveData = Singleton<PlayerInfoScript>.Instance.SaveData;
		string value = MiscParams.InventorySpacePurchaseCost.ToString();
		string upsightEvent = "Economy.GemExit.IncreaseInventory";
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("cost", value);
		mWaitForUserAction = true;
		mUserActionProceed = NextAction.WAITING;
		Singleton<BusyIconPanelController>.Instance.Show();
		saveData.ConsumeHardCurrency2(MiscParams.InventorySpacePurchaseCost, "inventory space", UserActionCallback);
		mNextFunction = InventorySpaceExecute;
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

	private void InventorySpaceExecute()
	{
		PlayerSaveData saveData = Singleton<PlayerInfoScript>.Instance.SaveData;
		saveData.AddEmptyInventorySlots(MiscParams.InventorySpacePerPurchase);
		Singleton<PlayerInfoScript>.Instance.Save();
		Singleton<BuyInventoryPopupController>.Instance.Show(mSettings.mPurchaseConfirmedDelegate);
	}

	public void AdjustDepth(int depth)
	{
		int num = depth - mAdjustedDepth;
		UIWidget[] componentsInChildren = base.gameObject.GetComponentsInChildren<UIWidget>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].depth += num * DepthAdjust;
		}
		mAdjustedDepth = depth;
	}

	public void SetCreatureToTarget()
	{
		EquipTween.Play();
	}

	public void MatchToCollider(BoxCollider col)
	{
		if (col != null)
		{
			PortraitParent.SetActive(false);
			PortraitCollider.size = col.size;
			PortraitCollider.center = col.center;
			UIWidget component = col.GetComponent<UIWidget>();
			if (component != null)
			{
				PortraitCollider.GetComponent<UIWidget>().depth = component.depth + 1;
			}
		}
		else
		{
			PortraitParent.SetActive(true);
			PortraitCollider.size = mBaseColliderSize;
			PortraitCollider.center = Vector3.zero;
			PortraitCollider.GetComponent<UIWidget>().depth = mBaseColliderDepth;
		}
	}

	public void SpawnCard(bool zoom = false)
	{
		CardPrefabScript component = base.transform.InstantiateAsChild(Singleton<PrefabReferences>.Instance.Card).GetComponent<CardPrefabScript>();
		component.gameObject.ChangeLayer(base.gameObject.layer);
		component.SpawnedFromTile = true;
		component.Mode = CardPrefabScript.CardMode.GeneralFrontEnd;
		if (InventoryItem.SlotType == InventorySlotType.Card)
		{
			component.Populate(InventoryItem.Card.Form);
		}
		else if (InventoryItem.SlotType == InventorySlotType.DisplayCard)
		{
			component.Populate(InventoryItem.DisplayCard);
		}
		else if (InventoryItem.SlotType == InventorySlotType.EvoMaterial)
		{
			component.Populate(InventoryItem.EvoMaterial);
		}
		else if (InventoryItem.SlotType == InventorySlotType.XPMaterial)
		{
			component.Populate(InventoryItem.XPMaterial);
		}
		component.AdjustDepth(1);
		if (zoom)
		{
			component.transform.parent = Singleton<TopmostOverlayController>.Instance.transform;
			component.gameObject.ChangeLayer(Singleton<TopmostOverlayController>.Instance.gameObject.layer);
			component.DestroyOnUnzoom = true;
			StartCoroutine(component.ToggleZoom());
		}
		else
		{
			mSpawnedCard = component;
			MatchToCollider(component.GetComponent<Collider>() as BoxCollider);
			component.GetComponent<Collider>().enabled = false;
		}
	}

	private void Update()
	{
		if (mTriggerStatBlinking && SortedLabels.Count >= 2)
		{
			mTimer += Time.deltaTime;
			mCount = (int)(mTimer / BlinkTweenInterval);
			int index = mCount % SortedLabels.Count;
			CreatureLevel.text = SortedLabels[index];
			float a = (0f - BlinkSpeedIndex) * Mathf.Pow(mTimer % BlinkTweenInterval - 1f, BlinkTweenInterval) + BlinkSpeedIndex;
			Color color = CreatureLevel.color;
			color.a = a;
			CreatureLevel.color = color;
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

	public void StartBlinkStats()
	{
		if (CurrentSorts != null)
		{
			foreach (SortEntry currentSort in CurrentSorts)
			{
				if (currentSort.SortType != SortTypeEnum.Level && currentSort.SortType != SortTypeEnum.Faction)
				{
					string text = currentSort.GetName();
					if (text == "TeamCost")
					{
						text = "Cost";
					}
					string statValue = GetStatValue(currentSort.SortType);
					if (statValue != string.Empty)
					{
						text = text + " " + statValue;
					}
					SortedLabels.Add(text);
				}
			}
			string text2 = InventoryItem.Creature.Level.ToString();
			SortedLabels.Add(mLvString + " " + text2);
		}
		mTriggerStatBlinking = true;
	}

	private string GetStatValue(SortTypeEnum sortType)
	{
		switch (sortType)
		{
		case SortTypeEnum.Level:
			return InventoryItem.Creature.Level.ToString();
		case SortTypeEnum.STR:
			return InventoryItem.Creature.STR.ToString();
		case SortTypeEnum.INT:
			return InventoryItem.Creature.INT.ToString();
		case SortTypeEnum.DEX:
			return InventoryItem.Creature.DEX.ToString();
		case SortTypeEnum.HP:
			return InventoryItem.Creature.HP.ToString();
		case SortTypeEnum.WeightedStats:
			return InventoryItem.Creature.GetWeightedStats().ToString();
		case SortTypeEnum.TeamCost:
			return InventoryItem.Creature.currentTeamCost.ToString();
		case SortTypeEnum.Rarity:
			return InventoryItem.Creature.StarRating.ToString();
		case SortTypeEnum.Faction:
			return string.Empty;
		default:
			return string.Empty;
		}
	}

	public void SetLootMode(bool InBattle = true, int rarity = -1)
	{
		FrameSprite.gameObject.SetActive(false);
		BackgroundSprite.gameObject.SetActive(false);
		PortraitTexture.gameObject.SetActive(false);
		if (rarity == -1)
		{
			rarity = InventoryItem.Rarity;
		}
		GameObject original = Singleton<PrefabReferences>.Instance.LootChests[rarity - 1];
		mEggObject = EggSpawnPoint.InstantiateAsChild(original);
		DWBattleLootObject component = mEggObject.GetComponent<DWBattleLootObject>();
		if (InBattle)
		{
			if (component != null && component.ExtraSparkleFX != null)
			{
				component.ExtraSparkleFX.SetActive(false);
			}
		}
		else
		{
			if (component != null)
			{
				Object.Destroy(component);
			}
			VFXRenderQueueSorter[] componentsInChildren = mEggObject.GetComponentsInChildren<VFXRenderQueueSorter>(true);
			VFXRenderQueueSorter[] array = componentsInChildren;
			foreach (VFXRenderQueueSorter vFXRenderQueueSorter in array)
			{
				vFXRenderQueueSorter.ShouldScaleVFX = true;
			}
		}
		mEggObject.ChangeLayer(base.gameObject.layer);
		mEggObject.GetComponent<Collider>().enabled = false;
		mEggObject.transform.localScale = Vector3.one;
		if (!InBattle)
		{
			mEggObject.SetActive(false);
		}
		CreatureParent.SetActive(false);
		AdjustDepth(1);
	}

	public bool OpenLoot(bool multiGacha = false)
	{
		mEggObject.SetActive(false);
		if (InventoryItem != null)
		{
			if (multiGacha && InventoryItem.SlotType == InventorySlotType.Creature)
			{
				ChatWindowController.SendGachaAnnouncementIfRare(InventoryItem.Creature);
			}
			if (InventoryItem.FirstTimeCollected)
			{
				if (!multiGacha)
				{
					FancyRevealTween.Play();
				}
				Singleton<GachaOpenSequencer>.Instance.ShowGachaSequence(InventoryItem, RevealLootDetails);
				return true;
			}
			if (!multiGacha)
			{
				BasicRevealTween.Play();
			}
			RevealLootDetails();
			return false;
		}
		BasicRevealTween.Play();
		RevealLootDetails();
		return false;
	}

	public void RevealLootDetails()
	{
		PortraitTexture.gameObject.SetActive(true);
		FrameSprite.gameObject.SetActive(true);
		BackgroundSprite.gameObject.SetActive(true);
		if (InventoryItem != null)
		{
			ShowRarityFrame();
			if (InventoryItem.SlotType == InventorySlotType.Creature)
			{
				CreatureParent.SetActive(true);
			}
			else if (InventoryItem.SlotType == InventorySlotType.EvoMaterial)
			{
				EvoMaterialParent.SetActive(true);
				PortraitTexture.ReplaceTexture(InventoryItem.EvoMaterial.UITexture);
			}
			else if (InventoryItem.SlotType == InventorySlotType.XPMaterial)
			{
				PortraitTexture.ReplaceTexture(InventoryItem.XPMaterial.UITexture);
			}
			else if (InventoryItem.SlotType == InventorySlotType.Card)
			{
				CardParent.SetActive(true);
				Singleton<SLOTResourceManager>.Instance.QueueUITextureLoad(InventoryItem.Card.Form.UITexture, InventoryItem.Card.Form.AssetBundle, "UI/UI/LoadingPlaceholder", PortraitTexture);
			}
		}
	}

	public void RevealNewTile()
	{
		for (int i = 0; i < RarityNewTiles.Length; i++)
		{
			if (i == InventoryItem.Rarity - 1)
			{
				RarityNewTiles[i].gameObject.SetActive(true);
			}
			else
			{
				RarityNewTiles[i].gameObject.SetActive(false);
			}
		}
		StartCoroutine(RevealNewTileFXCo());
	}

	public IEnumerator RevealNewTileFXCo()
	{
		yield return new WaitForSeconds(0.2f);
		for (int i = 0; i < RarityNewTilesSparklesFX.Length; i++)
		{
			if (i == InventoryItem.Rarity - 1)
			{
				RarityNewTilesSparklesFX[i].gameObject.SetActive(true);
			}
		}
	}

	public void HideNewTile()
	{
		for (int i = 0; i < RarityNewTiles.Length; i++)
		{
			RarityNewTiles[i].gameObject.SetActive(false);
		}
	}

	public void ShowRarityFrame()
	{
		showLargeRarityFrame = true;
		HideRarityFrame();
		if ((InventoryItem.SlotType != InventorySlotType.Creature && InventoryItem.SlotType != InventorySlotType.DisplayCreature) || suppressRarityFrame)
		{
			return;
		}
		DefaultStarArrangement.Play();
		for (int i = 0; i < RarityFrames.Length; i++)
		{
			if (i == InventoryItem.Rarity - 1)
			{
				RarityFrames[i].gameObject.SetActive(true);
				if (i == 2)
				{
					AlgebraicFrameTween.Play();
				}
			}
			else
			{
				RarityFrames[i].gameObject.SetActive(false);
			}
		}
	}

	public void ShowRarityFrameMini()
	{
		HideRarityFrame();
		if ((InventoryItem.SlotType != InventorySlotType.Creature && InventoryItem.SlotType != InventorySlotType.DisplayCreature) || suppressRarityFrame || InventoryItem.Creature == null || InventoryItem.DisplayCreature == null)
		{
			return;
		}
		FrameStarArrangement.Play();
		for (int i = 0; i < RarityFramesMini.Length; i++)
		{
			if (i == InventoryItem.Rarity - 1)
			{
				RarityFramesMini[i].gameObject.SetActive(true);
				if (i == 2)
				{
					AlgebraicFrameMiniTween.Play();
				}
			}
			else
			{
				RarityFramesMini[i].gameObject.SetActive(false);
			}
		}
	}

	public void HideRarityFrame()
	{
		for (int i = 0; i < RarityFrames.Length; i++)
		{
			RarityFrames[i].gameObject.SetActive(false);
		}
		for (int j = 0; j < RarityFramesMini.Length; j++)
		{
			RarityFramesMini[j].gameObject.SetActive(false);
		}
	}

	public void SetAsDisplayOnly()
	{
		GetComponent<Collider>().enabled = false;
		ClearOverlays();
	}

	public void ClearOverlays()
	{
		FadeGroup.SetActive(false);
		SelectedGroup.SetActive(false);
		Selected2Group.SetActive(false);
		UnavailableGroup.SetActive(false);
		InUseGroup.SetActive(false);
		HelperTagGroup.SetActive(false);
		FavoriteGroup.SetActive(false);
		ExpeditionTagGroup.SetActive(false);
		HideRarityFrame();
		if (InventoryItem != null && InventoryItem.SlotType != InventorySlotType.Purchase)
		{
			BuyInventoryParent.SetActive(false);
		}
	}

	public void SetOverlayStatus(TileStatus status, bool fadeIfInUse = true)
	{
		ClearOverlays();
		if (showLargeRarityFrame)
		{
			ShowRarityFrame();
		}
		else
		{
			ShowRarityFrameMini();
		}
		if (IsExactTileBeingDragged(this) || IsAttachedToTarget())
		{
			return;
		}
		if (IsCopyOfTileBeingDragged(this))
		{
			FadeGroup.SetActive(true);
		}
		else
		{
			switch (status)
			{
			case TileStatus.SelectedAsTarget:
				SelectedGroup.SetActive(true);
				FadeGroup.SetActive(true);
				break;
			case TileStatus.SelectedAsMaterial:
				Selected2Group.SetActive(true);
				FadeGroup.SetActive(true);
				break;
			case TileStatus.SelectedButUnavailable:
				UnavailableGroup.SetActive(true);
				FadeGroup.SetActive(true);
				break;
			default:
				if (InventoryItem.IsOnExpedition())
				{
					ExpeditionTagGroup.SetActive(true);
					FadeGroup.SetActive(true);
				}
				else if (InventoryItem.InUse())
				{
					InUseGroup.SetActive(true);
					FadeGroup.SetActive(fadeIfInUse);
				}
				else if (InventoryItem.IsFavorite())
				{
					FavoriteGroup.SetActive(true);
					FadeGroup.SetActive(fadeIfInUse);
				}
				else if (InventoryItem.IsHelper())
				{
					HelperTagGroup.SetActive(true);
					FadeGroup.SetActive(fadeIfInUse);
				}
				break;
			}
		}
		BuyInventoryParent.SetActive(InventoryItem.SlotType == InventorySlotType.Purchase);
	}

	public void SetDepthOffset(int numberForward)
	{
		RefreshAllOverlays();
		UIWidget[] componentsInChildren = base.gameObject.GetComponentsInChildren<UIWidget>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].depth += numberForward;
		}
	}
}
