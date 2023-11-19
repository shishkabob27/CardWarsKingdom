using UnityEngine;

public class EditDeckController : Singleton<EditDeckController>
{
	private enum NextAction
	{
		NONE,
		WAITING,
		PROCEED,
		ERROR,
		RETRY
	}

	public delegate void Callback();

	public UITweenController ShowTween;

	public UITweenController ExceededTeamCostTween;

	public UITweenController UnlockLeaderTween;

	public UITweenController CardLayoutUnlockLeaderTween;

	public UITweenController PulseTutorialButtonsTween;

	public UITexture LeaderImage;

	public GameObject[] CreatureSlots;

	public UIStreamingGrid CreatureGrid;

	private GameObject[] CreatureSpawnNodes;

	private UILabel[] CreatureNames;

	private UILabel[] CreatureCosts;

	private UILabel[] CreatureAttackCosts;

	private UILabel[,] CreatureStats;

	private GameObject[] CardParents;

	private GameObject[,] CardSpawnNodes;

	private UISprite[,] CardIcons;

	private UISprite[,] ExCardIcons;

	private GameObject[,] ExCardLocks;

	public UILabel LeaderSkillName;

	public UILabel LeaderSkillDesc;

	public UILabel LeaderName;

	public UILabel TeamName;

	public UILabel TeamCost;

	public Transform SortButton;

	public UILabel LeaderCost;

	public GameObject LeaderCostObject;

	public UILabel HardCurrencyLabel;

	public GameObject CreatureLayoutParent;

	public GameObject CreatureLayoutLeaderSelectButtons;

	public GameObject CustomizeButton;

	public GameObject CardLayoutParent;

	public GameObject CardLayoutCreatureSlotsParent;

	private GameObject[] CardLayoutCreatureSlots;

	public GameObject CardLayoutCreatureCardsParent;

	private GameObject[,] CardLayoutCardNodes;

	private Transform[][] CardLayoutExCardNodes;

	private GameObject[,] CardLayoutExCardLocks;

	private GameObject[] CardLayoutLeaderCardNodes;

	public UITexture CardLayoutLeaderImage;

	public GameObject CardLayoutLeaderSelectButtons;

	public GameObject CardLayoutLeaderCostObject;

	public UILabel CardLayoutLeaderCost;

	public UILabel CardLayoutLeaderName;

	public InventoryBarController inventoryBar;

	private InventoryTile[] mLoadedCreatures;

	private CardPrefabScript[,] mLoadedCards;

	private CardPrefabScript[,] mCardLayoutSpawnedCards;

	private CardPrefabScript[] mCardLayoutSpawnedLeaderCards;

	private InventoryTile[][] mCardLayoutSpawnedExCardTiles;

	private UIStreamingGridDataSource<InventorySlotItem> mCreatureGridDataSource = new UIStreamingGridDataSource<InventorySlotItem>();

	private Callback mFinishedCallback;

	private LeaderData mSelectedLeader;

	private LeaderData mDisplayedLeaderSkin;

	private InventorySlotType mShowingSlotType = InventorySlotType.Creature;

	private int mEditingExCardOfCreature = -1;

	private bool mInitialized;

	public GameObject ButtonTeamNext;

	public GameObject ButtonTeamPrev;

	private bool mWaitForUserAction;

	private NextAction mUserActionProceed;

	private PlayerSaveData.ProceedNextStep mNextFunction;

	private bool IsFromPrepScreen;

	private int mEditingCardsSlot = -1;

	private void Initialize()
	{
		CreatureSpawnNodes = new GameObject[MiscParams.CreaturesOnTeam];
		CreatureNames = new UILabel[MiscParams.CreaturesOnTeam];
		CreatureCosts = new UILabel[MiscParams.CreaturesOnTeam];
		CreatureAttackCosts = new UILabel[MiscParams.CreaturesOnTeam];
		CreatureStats = new UILabel[MiscParams.CreaturesOnTeam, 6];
		CardParents = new GameObject[MiscParams.CreaturesOnTeam];
		CardSpawnNodes = new GameObject[MiscParams.CreaturesOnTeam, 3];
		CardIcons = new UISprite[MiscParams.CreaturesOnTeam, 5];
		ExCardIcons = new UISprite[MiscParams.CreaturesOnTeam, 3];
		ExCardLocks = new GameObject[MiscParams.CreaturesOnTeam, 3];
		CardLayoutCreatureSlots = new GameObject[MiscParams.CreaturesOnTeam];
		CardLayoutCardNodes = new GameObject[MiscParams.CreaturesOnTeam, 5];
		CardLayoutExCardNodes = new Transform[MiscParams.CreaturesOnTeam][];
		CardLayoutExCardLocks = new GameObject[MiscParams.CreaturesOnTeam, 3];
		CardLayoutLeaderCardNodes = new GameObject[5];
		mLoadedCreatures = new InventoryTile[MiscParams.CreaturesOnTeam];
		mLoadedCards = new CardPrefabScript[MiscParams.CreaturesOnTeam, 3];
		mCardLayoutSpawnedCards = new CardPrefabScript[MiscParams.CreaturesOnTeam, 5];
		mCardLayoutSpawnedLeaderCards = new CardPrefabScript[5];
		mCardLayoutSpawnedExCardTiles = new InventoryTile[MiscParams.CreaturesOnTeam][];
		for (int i = 0; i < MiscParams.CreaturesOnTeam; i++)
		{
			CreatureSpawnNodes[i] = CreatureSlots[i].FindInChildren("Creature_S" + (i + 1));
			CreatureNames[i] = CreatureSlots[i].FindInChildren("Val_CreatureName").GetComponent<UILabel>();
			CreatureCosts[i] = CreatureSlots[i].FindInChildren("Val_CreatureCost").GetComponent<UILabel>();
			CreatureAttackCosts[i] = CreatureSlots[i].FindInChildren("Val_AttackCost").GetComponent<UILabel>();
			for (int j = 0; j < 6; j++)
			{
				string text = "Val_" + (CreatureStat)j;
				GameObject gameObject = CreatureSlots[i].FindInChildren(text);
				UILabel uILabel = null;
				if (gameObject != null)
				{
					uILabel = gameObject.GetComponent<UILabel>();
				}
				CreatureStats[i, j] = uILabel;
			}
			CardParents[i] = CreatureSlots[i].FindInChildren("ExCards");
			Transform transform = CreatureSlots[i].transform.FindChild("ActionCardIcons");
			for (int k = 0; k < 5; k++)
			{
				CardIcons[i, k] = transform.FindChild("Card_" + (k + 1).ToString("D2")).GetComponent<UISprite>();
			}
			Transform transform2 = CreatureSlots[i].transform.FindChild("ExCards");
			for (int l = 0; l < 3; l++)
			{
				ExCardIcons[i, l] = transform2.FindChild("CardSlot_0" + (l + 1)).GetComponent<UISprite>();
				ExCardLocks[i, l] = ExCardIcons[i, l].transform.FindChild("Lock").gameObject;
			}
			CardLayoutCreatureSlots[i] = CardLayoutCreatureSlotsParent.FindInChildren("Creature_" + (i + 1));
			GameObject gameObject2 = CardLayoutCreatureCardsParent.FindInChildren("ActionCards_" + (i + 1));
			for (int m = 0; m < 5; m++)
			{
				CardLayoutCardNodes[i, m] = gameObject2.FindInChildren("ActionCardSpawn_0" + (m + 1));
			}
			CardLayoutExCardNodes[i] = new Transform[3];
			for (int n = 0; n < 3; n++)
			{
				CardLayoutExCardNodes[i][n] = gameObject2.transform.FindChild("ExCardSlot_" + (n + 1)).transform;
				CardLayoutExCardLocks[i, n] = CardLayoutExCardNodes[i][n].transform.FindChild("Lock").gameObject;
			}
			mCardLayoutSpawnedExCardTiles[i] = new InventoryTile[3];
		}
		GameObject obj = CardLayoutCreatureCardsParent.FindInChildren("ActionCards_Leader");
		for (int num = 0; num < 5; num++)
		{
			CardLayoutLeaderCardNodes[num] = obj.FindInChildren("ActionCardSpawn_0" + (num + 1));
		}
		base.gameObject.GetComponent<InventoryBarController>().UpdateInventoryCounter();
		CreatureGrid.transform.parent.GetComponent<UIScrollView>().ResetPosition();
		CreatureGrid.transform.parent.GetComponent<UIScrollView>().UpdateScrollbars();
		mInitialized = true;
	}

	public void Populate()
	{
		if (!mInitialized)
		{
			Initialize();
		}
		CreatureLayoutParent.SetActive(mShowingSlotType == InventorySlotType.Creature);
		CardLayoutParent.SetActive(mShowingSlotType == InventorySlotType.Card);
		InventoryBarController.onDoFilterNull();
		InventoryBarController.onDoFilter += PopulateCreatureList;
		inventoryBar.SetFilters(false, true, true, true, true);
		RefreshCurrency();
		RefreshLoadout();
		mFinishedCallback = null;
		InventoryTile.ResetForNewScreen();
		bool active = Singleton<PlayerInfoScript>.Instance.CanEquipExCards();
		for (int i = 0; i < MiscParams.CreaturesOnTeam; i++)
		{
			CardParents[i].SetActive(active);
		}
		bool active2 = Singleton<PlayerInfoScript>.Instance.CanBuyLeaders();
		CreatureLayoutLeaderSelectButtons.SetActive(active2);
		CardLayoutLeaderSelectButtons.SetActive(active2);
		CustomizeButton.SetActive(active2);
		Singleton<TutorialController>.Instance.AdvanceIfTargetingBuilding("TBuilding_EditDeck");
		if (Singleton<TutorialController>.Instance.IsBlockActive("UseEditDeck"))
		{
			PulseTutorialButtonsTween.Play();
		}
	}

	public void Show(Callback finishedCallback = null)
	{
		ShowTween.Play();
		Populate();
		mFinishedCallback = finishedCallback;
		IsFromPrepScreen = true;
	}

	public void Unload()
	{
		if (!IsFromPrepScreen)
		{
			Singleton<TownHudController>.Instance.ReturnToTownView();
		}
		LeaderImage.UnloadTexture();
		CardLayoutLeaderImage.UnloadTexture();
		for (int i = 0; i < mLoadedCreatures.Length; i++)
		{
			UnloadSlot(i);
		}
		for (int j = 0; j < 5; j++)
		{
			if (mCardLayoutSpawnedLeaderCards[j] != null)
			{
				NGUITools.Destroy(mCardLayoutSpawnedLeaderCards[j].gameObject);
				mCardLayoutSpawnedLeaderCards[j] = null;
			}
		}
		mCreatureGridDataSource.Clear();
	}

	private void RefreshLoadout()
	{
		Loadout currentLoadout = Singleton<PlayerInfoScript>.Instance.GetCurrentLoadout();
		GameObject[] array = ((mShowingSlotType != InventorySlotType.Creature) ? CardLayoutCreatureSlots : CreatureSpawnNodes);
		for (int i = 0; i < CreatureSpawnNodes.Length; i++)
		{
			UnloadSlot(i);
			if (currentLoadout.CreatureSet[i] != null)
			{
				GameObject gameObject = array[i].InstantiateAsChild(Singleton<PrefabReferences>.Instance.InventoryTile);
				gameObject.ChangeLayer(base.gameObject.layer);
				mLoadedCreatures[i] = gameObject.GetComponent<InventoryTile>();
				mLoadedCreatures[i].Populate(currentLoadout.CreatureSet[i]);
				mLoadedCreatures[i].AssignedSlot = i;
				mLoadedCreatures[i].ParentGrid = CreatureGrid;
				mLoadedCreatures[i].GetComponent<InventoryTileDragDrop>().cloneOnDrag = false;
				RefreshSlot(i);
			}
		}
		int selectedLoadout = Singleton<PlayerInfoScript>.Instance.SaveData.SelectedLoadout;
		TeamName.text = Singleton<PlayerInfoScript>.Instance.GetTeamName(selectedLoadout);
		mSelectedLeader = currentLoadout.Leader.Form;
		SetLeaderIfValid();
		RefreshLeader();
		RefreshTeamCost();
		Singleton<PlayerInfoScript>.Instance.Save();
	}

	private void UnloadSlot(int slot)
	{
		if (mLoadedCreatures[slot] != null)
		{
			NGUITools.Destroy(mLoadedCreatures[slot].gameObject);
			mLoadedCreatures[slot] = null;
		}
		for (int i = 0; i < 5; i++)
		{
			if (mCardLayoutSpawnedCards[slot, i] != null)
			{
				NGUITools.Destroy(mCardLayoutSpawnedCards[slot, i].gameObject);
				mCardLayoutSpawnedCards[slot, i] = null;
			}
		}
		for (int j = 0; j < 3; j++)
		{
			if (mCardLayoutSpawnedExCardTiles[slot][j] != null)
			{
				NGUITools.Destroy(mCardLayoutSpawnedExCardTiles[slot][j].gameObject);
				mCardLayoutSpawnedExCardTiles[slot][j] = null;
			}
			CardLayoutExCardNodes[slot][j].gameObject.SetActive(false);
			ExCardIcons[slot, j].gameObject.SetActive(false);
		}
		CreatureNames[slot].text = string.Empty;
		CreatureCosts[slot].text = "0";
		CreatureAttackCosts[slot].text = "0";
		for (int k = 0; k < 6; k++)
		{
			UILabel uILabel = CreatureStats[slot, k];
			if (uILabel != null)
			{
				uILabel.text = "0";
			}
		}
		for (int l = 0; l < 5; l++)
		{
			CardIcons[slot, l].gameObject.SetActive(false);
		}
	}

	public void OnClickNextLeader()
	{
		mSelectedLeader = mSelectedLeader.GetNextPlayableLeader(true);
		SetLeaderIfValid();
		RefreshLeader();
	}

	public void OnClickPrevLeader()
	{
		mSelectedLeader = mSelectedLeader.GetNextPlayableLeader(false);
		SetLeaderIfValid();
		RefreshLeader();
	}

	private void RefreshLeader()
	{
		mSelectedLeader.ParseKeywords();
		if (mShowingSlotType == InventorySlotType.Creature)
		{
			Singleton<SLOTResourceManager>.Instance.QueueUITextureLoad(mDisplayedLeaderSkin.PortraitTexture, "FTUEBundle", "UI/UI/LoadingPlaceholder", LeaderImage);
			LeaderName.text = mSelectedLeader.Name;
			return;
		}
		CardLayoutLeaderName.text = mSelectedLeader.Name;
		Singleton<SLOTResourceManager>.Instance.QueueUITextureLoad(mDisplayedLeaderSkin.PortraitTexture, "FTUEBundle", "UI/UI/LoadingPlaceholder", CardLayoutLeaderImage);
		for (int i = 0; i < 5; i++)
		{
			if (mCardLayoutSpawnedLeaderCards[i] != null)
			{
				NGUITools.Destroy(mCardLayoutSpawnedLeaderCards[i].gameObject);
			}
			mCardLayoutSpawnedLeaderCards[i] = CardLayoutLeaderCardNodes[i].InstantiateAsChild(Singleton<PrefabReferences>.Instance.Card).GetComponent<CardPrefabScript>();
			mCardLayoutSpawnedLeaderCards[i].gameObject.ChangeLayer(base.gameObject.layer);
			mCardLayoutSpawnedLeaderCards[i].Mode = CardPrefabScript.CardMode.GeneralFrontEnd;
			mCardLayoutSpawnedLeaderCards[i].Populate(mSelectedLeader.ActionCards[i]);
			mCardLayoutSpawnedLeaderCards[i].AdjustDepth(1);
		}
	}

	private void SetLeaderIfValid()
	{
		LeaderItem leaderItem = Singleton<PlayerInfoScript>.Instance.SaveData.Leaders.Find((LeaderItem m) => m.Form == mSelectedLeader);
		if (leaderItem != null)
		{
			Loadout currentLoadout = Singleton<PlayerInfoScript>.Instance.GetCurrentLoadout();
			currentLoadout.Leader = leaderItem;
			LeaderCostObject.SetActive(false);
			CardLayoutLeaderCostObject.SetActive(false);
			mDisplayedLeaderSkin = leaderItem.SelectedSkin;
			Singleton<TownHudController>.Instance.UpdateLeaderIcon();
		}
		else
		{
			LeaderCostObject.SetActive(true);
			UnlockLeaderTween.StopAndReset();
			LeaderCost.text = mSelectedLeader.BuyCost.ToString();
			CardLayoutLeaderCostObject.SetActive(true);
			CardLayoutUnlockLeaderTween.StopAndReset();
			CardLayoutLeaderCost.text = LeaderCost.text;
			mDisplayedLeaderSkin = mSelectedLeader;
		}
		Singleton<PlayerInfoScript>.Instance.Save();
	}

	private void RefreshTeamCost()
	{
		int teamCost = Singleton<PlayerInfoScript>.Instance.GetCurrentLoadout().GetTeamCost();
		int teamCost2 = Singleton<PlayerInfoScript>.Instance.RankData.TeamCost;
		TeamCost.text = teamCost + " / " + teamCost2;
		if (teamCost > teamCost2)
		{
			ExceededTeamCostTween.Play();
			Singleton<TutorialController>.Instance.StartTutorialBlockIfNotComplete("TeamCost");
		}
		else
		{
			ExceededTeamCostTween.StopAndReset();
		}
	}

	public void OnClickNextLoadout()
	{
		Singleton<PlayerInfoScript>.Instance.GoToNextLoadout(Singleton<TutorialController>.Instance.IsBlockActive("UseEditDeck"));
		PopulateCreatureList();
		RefreshLoadout();
		Singleton<TownHudController>.Instance.UpdateLeaderIcon();
	}

	public void OnClickPrevLoadout()
	{
		Singleton<PlayerInfoScript>.Instance.GoToPrevLoadout(Singleton<TutorialController>.Instance.IsBlockActive("UseEditDeck"));
		PopulateCreatureList();
		RefreshLoadout();
		Singleton<TownHudController>.Instance.UpdateLeaderIcon();
	}

	public void PopulateCreatureList()
	{
		InventoryTile.SetDelegates(InventorySlotType.None, TileDraggable, OnTileDragBeginFromSlot, OnTileDropped, RefreshTileOverlay, PopulateCreatureList, OnPopupClosed);
		Singleton<PlayerInfoScript>.Instance.SaveData.SortInventory(mShowingSlotType);
		mCreatureGridDataSource.Init(CreatureGrid, Singleton<PrefabReferences>.Instance.InventoryTile, inventoryBar.GetFilteredInventory());
		base.gameObject.GetComponent<InventoryBarController>().ResetScrollView();
	}

	private void OnPopupClosed(InventorySlotItem creature)
	{
		PopulateCreatureList();
		for (int i = 0; i < mLoadedCreatures.Length; i++)
		{
			if (mLoadedCreatures[i] != null && mLoadedCreatures[i].InventoryItem == creature)
			{
				RefreshTileOverlay(mLoadedCreatures[i]);
				RefreshSlot(i);
				break;
			}
		}
	}

	private bool TileDraggable(InventoryTile tile)
	{
		if (Singleton<TutorialController>.Instance.IsStateActive("GA_TapCreature"))
		{
			CreatureGrid.transform.parent.GetComponent<UIScrollView>().ResetPosition();
			CreatureGrid.transform.parent.GetComponent<UIScrollView>().UpdateScrollbars();
			return false;
		}
		if (Singleton<TutorialController>.Instance.IsStateActive("GA_AddCreature"))
		{
			CreatureGrid.transform.parent.GetComponent<UIScrollView>().ResetPosition();
			CreatureGrid.transform.parent.GetComponent<UIScrollView>().UpdateScrollbars();
			if (tile.AssignedSlot != -1)
			{
				return false;
			}
		}
		if (mShowingSlotType == InventorySlotType.Creature)
		{
			if (tile.InventoryItem.SlotType != InventorySlotType.Creature)
			{
				return false;
			}
		}
		else if (tile.InventoryItem.SlotType != InventorySlotType.Creature && tile.InventoryItem.SlotType != InventorySlotType.Card)
		{
			return false;
		}
		if (tile.IsAttachedToTarget())
		{
			if (tile.InventoryItem.SlotType == InventorySlotType.Card)
			{
				int num = (tile.AssignedSlot - 5) / 3;
				int slot = (tile.AssignedSlot - 5) % 3;
				mEditingExCardOfCreature = num;
				CardEquipper.ClearCard(mCardLayoutSpawnedExCardTiles[num], mLoadedCreatures[num].InventoryItem.Creature, slot, OnExCardChanged);
				return false;
			}
			return true;
		}
		if (!InventoryTile.IsUpwardsDrag())
		{
			return false;
		}
		if (tile.InventoryItem.SlotType == InventorySlotType.Creature && Singleton<PlayerInfoScript>.Instance.GetCurrentLoadout().ContainsCreature(tile.InventoryItem.Creature))
		{
			return false;
		}
		return true;
	}

	private void OnTileDragBeginFromSlot(InventoryTile tile)
	{
		if (tile.InventoryItem.SlotType == InventorySlotType.Creature)
		{
			ClearTeamMember(tile.AssignedSlot);
		}
	}

	private bool OnTileDropped(InventoryTile tile, int slotIndex)
	{
		if (tile.InventoryItem.SlotType == InventorySlotType.Creature)
		{
			if (slotIndex >= 0 && slotIndex <= 4)
			{
				SetTeamMember(tile, slotIndex, tile.AssignedSlot);
				tile.EquipTween.Play();
				Singleton<TutorialController>.Instance.AdvanceIfDraggingTile();
				return true;
			}
		}
		else if (tile.InventoryItem.SlotType == InventorySlotType.Card && slotIndex >= 5)
		{
			SetCard(tile, slotIndex);
			return false;
		}
		return false;
	}

	private void SetCard(InventoryTile tile, int slotIndex)
	{
		int num = (slotIndex - 5) / 3;
		int slot = (slotIndex - 5) % 3;
		CreatureItem creature = mLoadedCreatures[num].InventoryItem.Creature;
		if (tile.InventoryItem.Card.CreatureUID != creature.UniqueId)
		{
			mEditingExCardOfCreature = num;
			CardEquipper.SetCard(mCardLayoutSpawnedExCardTiles[num], CardLayoutExCardNodes[num], creature, tile.InventoryItem, slot, OnExCardChanged, slotIndex);
		}
	}

	private void OnExCardChanged()
	{
		PopulateCreatureList();
		ClearRemovedCards();
		RefreshExCardLocks(mEditingExCardOfCreature);
	}

	private void ClearRemovedCards()
	{
		for (int i = 0; i < MiscParams.CreaturesOnTeam; i++)
		{
			if (mLoadedCreatures[i] == null)
			{
				continue;
			}
			CreatureItem creature = mLoadedCreatures[i].InventoryItem.Creature;
			for (int j = 0; j < 3; j++)
			{
				if (!(mCardLayoutSpawnedExCardTiles[i][j] == null))
				{
					InventorySlotItem card = mCardLayoutSpawnedExCardTiles[i][j].InventoryItem;
					if (creature.ExCards.Find((InventorySlotItem m) => m == card) == null)
					{
						NGUITools.Destroy(mCardLayoutSpawnedExCardTiles[i][j].gameObject);
						mCardLayoutSpawnedExCardTiles[i][j] = null;
					}
				}
			}
		}
	}

	private void RefreshExCardLocks(int creatureSlot)
	{
		CreatureItem creature = mLoadedCreatures[creatureSlot].InventoryItem.Creature;
		for (int i = 0; i < 3; i++)
		{
			CardLayoutExCardLocks[creatureSlot, i].SetActive(i >= creature.ExCardSlotsUnlocked);
		}
	}

	private void RefreshExCards(int creatureSlot)
	{
		CreatureItem creature = mLoadedCreatures[creatureSlot].InventoryItem.Creature;
		bool flag = Singleton<PlayerInfoScript>.Instance.CanEquipExCards();
		for (int i = 0; i < 3; i++)
		{
			if (flag)
			{
				CardLayoutExCardNodes[creatureSlot][i].gameObject.SetActive(true);
				if (mCardLayoutSpawnedExCardTiles[creatureSlot][i] != null)
				{
					NGUITools.Destroy(mCardLayoutSpawnedExCardTiles[creatureSlot][i]);
					mCardLayoutSpawnedExCardTiles[creatureSlot][i] = null;
				}
				if (creature.ExCards[i] != null)
				{
					InventoryTile component = CardLayoutExCardNodes[creatureSlot][i].InstantiateAsChild(Singleton<PrefabReferences>.Instance.InventoryTile).GetComponent<InventoryTile>();
					component.gameObject.ChangeLayer(base.gameObject.layer);
					component.AssignedSlot = creatureSlot * 3 + i + 5;
					component.Populate(creature.ExCards[i]);
					component.SpawnCard();
					mCardLayoutSpawnedExCardTiles[creatureSlot][i] = component;
				}
			}
			else
			{
				CardLayoutExCardNodes[creatureSlot][i].gameObject.SetActive(false);
			}
		}
		RefreshExCardLocks(creatureSlot);
	}

	private void RefreshTileOverlay(InventoryTile tile)
	{
		if (mShowingSlotType == InventorySlotType.Creature)
		{
			if (tile.InventoryItem.SlotType == InventorySlotType.Creature)
			{
				if (Singleton<PlayerInfoScript>.Instance.GetCurrentLoadout().ContainsCreature(tile.InventoryItem.Creature))
				{
					tile.SetOverlayStatus(InventoryTile.TileStatus.SelectedAsTarget, false);
				}
				else
				{
					tile.SetOverlayStatus(InventoryTile.TileStatus.NotSelected, false);
				}
			}
			else
			{
				tile.SetOverlayStatus(InventoryTile.TileStatus.NotSelected);
				tile.FadeGroup.SetActive(true);
			}
		}
		else
		{
			if (mShowingSlotType != InventorySlotType.Card)
			{
				return;
			}
			if (tile.InventoryItem.SlotType == InventorySlotType.Creature)
			{
				if (Singleton<PlayerInfoScript>.Instance.GetCurrentLoadout().ContainsCreature(tile.InventoryItem.Creature))
				{
					tile.SetOverlayStatus(InventoryTile.TileStatus.SelectedAsTarget, false);
				}
				else
				{
					tile.SetOverlayStatus(InventoryTile.TileStatus.NotSelected, false);
				}
			}
			else if (tile.InventoryItem.SlotType == InventorySlotType.Card)
			{
				if (IsCardEquippedOnVisibleCreature(tile.InventoryItem.Card))
				{
					tile.SetOverlayStatus(InventoryTile.TileStatus.SelectedAsTarget, false);
				}
				else
				{
					tile.SetOverlayStatus(InventoryTile.TileStatus.NotSelected, false);
				}
				tile.FadeGroup.SetActive(false);
			}
			else
			{
				tile.SetOverlayStatus(InventoryTile.TileStatus.NotSelected);
				tile.FadeGroup.SetActive(true);
			}
		}
	}

	private bool IsCardEquippedOnVisibleCreature(CardItem card)
	{
		if (card.CreatureUID == 0)
		{
			return false;
		}
		Loadout currentLoadout = Singleton<PlayerInfoScript>.Instance.GetCurrentLoadout();
		foreach (InventorySlotItem item in currentLoadout.CreatureSet)
		{
			if (item != null && item.Creature.UniqueId == card.CreatureUID)
			{
				return true;
			}
		}
		return false;
	}

	public void SetTeamMember(InventoryTile portraitObject, int slotIndex, int prevSlotIndex)
	{
		Loadout currentLoadout = Singleton<PlayerInfoScript>.Instance.GetCurrentLoadout();
		GameObject[] array = ((mShowingSlotType != InventorySlotType.Creature) ? CardLayoutCreatureSlots : CreatureSpawnNodes);
		if (mLoadedCreatures[slotIndex] != null)
		{
			if (prevSlotIndex != -1)
			{
				UnloadSlot(prevSlotIndex);
				InventoryTile inventoryTile = mLoadedCreatures[slotIndex];
				currentLoadout.CreatureSet[prevSlotIndex] = inventoryTile.InventoryItem;
				inventoryTile.AssignedSlot = prevSlotIndex;
				inventoryTile.transform.parent = array[prevSlotIndex].transform;
				inventoryTile.transform.localPosition = Vector3.zero;
				mLoadedCreatures[prevSlotIndex] = inventoryTile;
				RefreshSlot(prevSlotIndex);
			}
			else
			{
				UnloadSlot(slotIndex);
			}
		}
		currentLoadout.CreatureSet[slotIndex] = portraitObject.InventoryItem;
		portraitObject.AssignedSlot = slotIndex;
		portraitObject.transform.parent = array[slotIndex].transform;
		portraitObject.transform.localPosition = Vector3.zero;
		mLoadedCreatures[slotIndex] = portraitObject;
		RefreshSlot(slotIndex);
		RefreshTeamCost();
		Singleton<PlayerInfoScript>.Instance.Save();
	}

	public void ClearTeamMember(int slotIndex)
	{
		Loadout currentLoadout = Singleton<PlayerInfoScript>.Instance.GetCurrentLoadout();
		currentLoadout.CreatureSet[slotIndex] = null;
		mLoadedCreatures[slotIndex] = null;
		UnloadSlot(slotIndex);
		RefreshTeamCost();
	}

	public void OnClickClose()
	{
		Singleton<MouseOrbitCamera>.Instance.CheckTiltCamSettingBeforeTutorial();
		if (mFinishedCallback != null)
		{
			mFinishedCallback();
			mFinishedCallback = null;
			IsFromPrepScreen = true;
		}
		else
		{
			IsFromPrepScreen = false;
		}
		Singleton<PlayerInfoScript>.Instance.Save();
		if (Singleton<TutorialController>.Instance.IsBlockActive("UseEditDeck"))
		{
			Singleton<PlayerInfoScript>.Instance.StateData.ActiveTutorialState.ContinueThrough = true;
			Singleton<TutorialController>.Instance.AdvanceTutorialState();
		}
	}

	public void OnClickLeader()
	{
		LeaderItem leaderItem = Singleton<PlayerInfoScript>.Instance.GetLeaderItem(mSelectedLeader);
		if (leaderItem == null)
		{
			leaderItem = new LeaderItem(mSelectedLeader);
		}
		Singleton<LeaderDetailsController>.Instance.Show(leaderItem, OnLeaderPopupClosed);
	}

	public void OnClickLeaderHideHardCurrency()
	{
		OnClickLeader();
	}

	private void OnLeaderPopupClosed()
	{
		SetLeaderIfValid();
		RefreshLeader();
	}

	private void OnCustomizationScreenClosed()
	{
		SetLeaderIfValid();
		RefreshLeader();
	}

	public GameObject GetUnusedCreature()
	{
		InventorySlotItem dataItem = Singleton<PlayerInfoScript>.Instance.SaveData.FindCreature((InventorySlotItem m) => !Singleton<PlayerInfoScript>.Instance.IsCreatureInAnyLoadout(m.Creature));
		return mCreatureGridDataSource.FindPrefab(dataItem);
	}

	public GameObject GetNewestCreature()
	{
		InventorySlotItem newestCreature = Singleton<PlayerInfoScript>.Instance.SaveData.GetNewestCreature();
		return mCreatureGridDataSource.FindPrefab(newestCreature);
	}

	private void RefreshSlot(int slot)
	{
		CreatureItem creature = mLoadedCreatures[slot].InventoryItem.Creature;
		if (mShowingSlotType == InventorySlotType.Creature)
		{
			CreatureNames[slot].text = creature.Form.Name;
			CreatureCosts[slot].transform.parent.gameObject.SetActive(true);
			CreatureCosts[slot].text = creature.currentTeamCost.ToString();
			CreatureAttackCosts[slot].transform.parent.gameObject.SetActive(true);
			CreatureAttackCosts[slot].text = creature.Form.AttackCost.ToString();
			for (int i = 0; i < 6; i++)
			{
				UILabel uILabel = CreatureStats[slot, i];
				if (uILabel != null)
				{
					uILabel.text = creature.GetStat((CreatureStat)i) + ((!((CreatureStat)i).IsPercent()) ? string.Empty : "%");
				}
			}
			for (int j = 0; j < 5; j++)
			{
				CardIcons[slot, j].gameObject.SetActive(true);
				CardIcons[slot, j].spriteName = creature.Form.ActionCards[j].Faction.GetIcon();
			}
			for (int k = 0; k < 3; k++)
			{
				ExCardIcons[slot, k].gameObject.SetActive(true);
				if (creature.ExCardSlotsUnlocked > k)
				{
					ExCardLocks[slot, k].SetActive(false);
					if (creature.ExCards[k] != null)
					{
						ExCardIcons[slot, k].spriteName = creature.ExCards[k].Card.Faction.GetIcon();
					}
					else
					{
						ExCardIcons[slot, k].spriteName = "Icon_Card_Empty";
					}
				}
				else
				{
					ExCardLocks[slot, k].SetActive(true);
					ExCardIcons[slot, k].spriteName = "Icon_Card_Empty";
				}
			}
		}
		else
		{
			for (int l = 0; l < 5; l++)
			{
				mCardLayoutSpawnedCards[slot, l] = CardLayoutCardNodes[slot, l].InstantiateAsChild(Singleton<PrefabReferences>.Instance.Card).GetComponent<CardPrefabScript>();
				mCardLayoutSpawnedCards[slot, l].gameObject.ChangeLayer(base.gameObject.layer);
				mCardLayoutSpawnedCards[slot, l].Mode = CardPrefabScript.CardMode.GeneralFrontEnd;
				mCardLayoutSpawnedCards[slot, l].Populate(creature.Form.ActionCards[l]);
				mCardLayoutSpawnedCards[slot, l].AdjustDepth(1);
			}
			RefreshExCards(slot);
		}
	}

	public void OnClickGems1()
	{
		OnClickGems(0);
	}

	public void OnClickGems2()
	{
		OnClickGems(1);
	}

	public void OnClickGems3()
	{
		OnClickGems(2);
	}

	public void OnClickGems4()
	{
		OnClickGems(3);
	}

	public void OnClickGems5()
	{
		OnClickGems(4);
	}

	private void OnClickGems(int slot)
	{
	}

	private void SpawnCards(int slot)
	{
		for (int i = 0; i < 3; i++)
		{
			if (mLoadedCards[slot, i] != null)
			{
				NGUITools.Destroy(mLoadedCards[slot, i].gameObject);
			}
			mLoadedCards[slot, i] = null;
		}
		if (!(mLoadedCreatures[slot] != null))
		{
			return;
		}
		CreatureItem creature = mLoadedCreatures[slot].InventoryItem.Creature;
		for (int j = 0; j < 3; j++)
		{
			if (creature.ExCards[j] != null)
			{
				GameObject gameObject = CardSpawnNodes[slot, j].InstantiateAsChild(Singleton<PrefabReferences>.Instance.Card);
				mLoadedCards[slot, j] = gameObject.GetComponent<CardPrefabScript>();
				mLoadedCards[slot, j].Populate(creature.ExCards[j]);
			}
		}
	}

	public void OnClickCards1()
	{
		OnClickCards(0);
	}

	public void OnClickCards2()
	{
		OnClickCards(1);
	}

	public void OnClickCards3()
	{
		OnClickCards(2);
	}

	public void OnClickCards4()
	{
		OnClickCards(3);
	}

	public void OnClickCards5()
	{
		OnClickCards(4);
	}

	private void OnClickCards(int slot)
	{
		Loadout currentLoadout = Singleton<PlayerInfoScript>.Instance.GetCurrentLoadout();
		InventorySlotItem inventorySlotItem = currentLoadout.CreatureSet[slot];
		if (inventorySlotItem != null)
		{
			mEditingCardsSlot = slot;
			Singleton<CardEquipController>.Instance.Show(inventorySlotItem, DoneEditingCards);
		}
	}

	private void DoneEditingCards()
	{
		PopulateCreatureList();
		RefreshSlot(mEditingCardsSlot);
		mEditingCardsSlot = -1;
	}

	public void OnClickSort()
	{
		Singleton<SortPopupController>.Instance.Show(SortPopupController.Category.Creatures, SortButton, PopulateCreatureList);
	}

	public void OnPurchaseLeader()
	{
		mWaitForUserAction = true;
		mUserActionProceed = NextAction.WAITING;
		Singleton<BusyIconPanelController>.Instance.Show();
		Singleton<PlayerInfoScript>.Instance.SaveData.ConsumeHardCurrency2(mSelectedLeader.BuyCost, "leader purchase", UserActionCallback);
		mNextFunction = LeaderPurchaseExecute;
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

	private void LeaderPurchaseExecute()
	{
		Singleton<PlayerInfoScript>.Instance.SaveData.Leaders.Add(new LeaderItem(mSelectedLeader.ID));
		SetLeaderIfValid();
		if (mShowingSlotType == InventorySlotType.Creature)
		{
			UnlockLeaderTween.Play();
		}
		else
		{
			CardLayoutUnlockLeaderTween.Play();
		}
		Singleton<PlayerInfoScript>.Instance.Save();
		Singleton<AnalyticsManager>.Instance.logHeroBought(mSelectedLeader.ID);
		Singleton<BuyHeroPopupController>.Instance.Show(mSelectedLeader);
		Singleton<LeaderDetailsController>.Instance.OnPurchaseComplete();
	}

	private void Update()
	{
		RefreshCurrency();
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

	private void RefreshCurrency()
	{
		if (HardCurrencyLabel.gameObject.activeInHierarchy)
		{
			HardCurrencyLabel.text = Singleton<PlayerInfoScript>.Instance.SaveData.HardCurrency.ToString();
		}
	}

	public GameObject GetDragTargetOfCreatureClass(CreatureFaction faction)
	{
		for (int i = 0; i < mLoadedCreatures.Length; i++)
		{
			if (mLoadedCreatures[i] != null && mLoadedCreatures[i].InventoryItem.Creature.Faction == faction)
			{
				return mLoadedCreatures[i].gameObject;
			}
		}
		return null;
	}

	public void ToggleLayout()
	{
		for (int i = 0; i < mLoadedCreatures.Length; i++)
		{
			UnloadSlot(i);
		}
		if (mShowingSlotType == InventorySlotType.Creature)
		{
			mShowingSlotType = InventorySlotType.Card;
		}
		else
		{
			mShowingSlotType = InventorySlotType.Creature;
		}
		Callback callback = mFinishedCallback;
		Populate();
		mFinishedCallback = callback;
	}

	public void OnClickExSlot()
	{
		Transform transform = UICamera.currentTouch.current.transform;
		int num = -1;
		int num2 = -1;
		for (int i = 0; i < CardLayoutExCardNodes.Length; i++)
		{
			for (int j = 0; j < CardLayoutExCardNodes[i].Length; j++)
			{
				if (CardLayoutExCardNodes[i][j] == transform)
				{
					num = i;
					num2 = j;
					break;
				}
			}
		}
		if (num != -1)
		{
			CreatureItem creature = mLoadedCreatures[num].InventoryItem.Creature;
			mEditingExCardOfCreature = num;
		}
	}

	public GameObject GetBestCreatureCardObject()
	{
		int num = -1;
		for (int i = 0; i < mLoadedCreatures.Length; i++)
		{
			if (mLoadedCreatures[i] != null && (num == -1 || mLoadedCreatures[i].InventoryItem.Creature.StarRating > mLoadedCreatures[num].InventoryItem.Creature.StarRating))
			{
				num = i;
			}
		}
		return CardParents[num];
	}

	public void OnClickCustomize()
	{
		Singleton<PlayerCustomizationController>.Instance.ShowFromEditTeam(OnCustomizationScreenClosed);
	}

	public void OnClickStats()
	{
		Singleton<StatsDescriptionPopup>.Instance.ShowPanel();
	}

	public void OnClickWeight()
	{
		Singleton<StatsDescriptionPopup>.Instance.ShowWeightPanel();
	}
}
