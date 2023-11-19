using UnityEngine;

public class MuseumController : Singleton<MuseumController>
{
	public UILabel TitleLabel;

	public UIToggle InventoryToggle;

	public UIToggle CollectionToggle;

	public UIToggle GemCollectionToggle;

	public Transform SortButton;

	public UITweenController ShowTween;

	public Transform ZoomPosition;

	public Collider ZoomCollider;

	public Transform ZoomCardReparentNode;

	public float CardScaleInGrid = 0.7f;

	public UIStreamingGrid InventoryGrid;

	private UIStreamingGridDataSource<InventorySlotItem> mInventoryDataSource = new UIStreamingGridDataSource<InventorySlotItem>();

	public UIStreamingGrid CollectionGrid;

	private UIStreamingGridDataSource<CreatureData> mCollectionDataSource = new UIStreamingGridDataSource<CreatureData>();

	[Header("Loc Strings for UIToggle Titles")]
	public string InventoryTitleText = "!!INVENTORY";

	public string CreaturesTitleText = "!!CREATURE_COLLECTION";

	public string GemsTitleText = "!!GEM_VIEW";

	public string RunesTitleText = "!!RUNE_COLLECTION";

	public string CardsTitleText = "!!CARD_COLLECTION";

	private UIToggle mCurrentToggle;

	private CardPrefabScript mSpawnedCard;

	private Vector3 lastKnownPos = new Vector3(0f, 0f, 0f);

	public void Populate()
	{
		Invoke("RefreshCurrentTab", 0.1f);
	}

	public void SetTitleLabel(string inLabelText)
	{
		if (TitleLabel != null)
		{
			TitleLabel.text = KFFLocalization.Get(inLabelText);
		}
	}

	public void OnPanelCloseDone()
	{
		Singleton<TownHudController>.Instance.ReturnToTownView();
	}

	public void Unload()
	{
		Singleton<MouseOrbitCamera>.Instance.CheckTiltCamSettingBeforeTutorial();
		mInventoryDataSource.Clear();
		mCollectionDataSource.Clear();
	}

	public void RefreshCurrentTab()
	{
		Unload();
		if (mCurrentToggle == null)
		{
			mCurrentToggle = InventoryToggle;
			InventoryToggle.gameObject.SendMessage("OnClick");
		}
		else
		{
			mCurrentToggle = UIToggle.GetActiveToggle(InventoryToggle.group);
		}
		if (mCurrentToggle == InventoryToggle)
		{
			mCollectionDataSource.Clear();
			InventoryTile.SetDelegates(InventorySlotType.None, null, null, null, RefreshTileOverlay, RefreshCurrentTab, OnPopupClosed);
			Singleton<PlayerInfoScript>.Instance.SaveData.SortInventory(InventorySlotType.Creature);
			mInventoryDataSource.Init(InventoryGrid, Singleton<PrefabReferences>.Instance.InventoryTile, Singleton<PlayerInfoScript>.Instance.SaveData.InventorySlots);
		}
		else if (mCurrentToggle == CollectionToggle)
		{
			mInventoryDataSource.Clear();
			InventoryTile.SetDelegates(InventorySlotType.None, null, null, null, RefreshTileOverlay, RefreshCurrentTab, OnPopupClosed);
			Singleton<PlayerInfoScript>.Instance.SaveData.SortInventory(InventorySlotType.Creature);
			mCollectionDataSource.Init(CollectionGrid, Singleton<PrefabReferences>.Instance.InventoryTile, CreatureDataManager.Instance.GetDatabase());
		}
		SortButton.gameObject.SetActive(mCurrentToggle == InventoryToggle);
		RefreshRarityFramesOnTile();
	}

	private void OnPopupClosed(InventorySlotItem creature)
	{
		RefreshCurrentTab();
	}

	private void RefreshTileOverlay(InventoryTile tile)
	{
		if (mCurrentToggle == InventoryToggle)
		{
			tile.ShowRarityFrame();
			tile.SetOverlayStatus(InventoryTile.TileStatus.NotSelected, false);
			tile.FadeGroup.SetActive(false);
			return;
		}
		tile.ShowRarityFrame();
		tile.SetOverlayStatus(InventoryTile.TileStatus.NotSelected, false);
		CreatureData displayCreature = tile.InventoryItem.DisplayCreature;
		if (displayCreature != null)
		{
			tile.FadeGroup.SetActive(displayCreature.AlreadySeen && !displayCreature.AlreadyCollected);
			tile.RarityStarGroup.SetActive(displayCreature.AlreadyCollected);
		}
	}

	private void RefreshRarityFramesOnTile()
	{
		InventoryTile[] componentsInChildren = CollectionGrid.GetComponentsInChildren<InventoryTile>();
		InventoryTile[] array = componentsInChildren;
		foreach (InventoryTile inventoryTile in array)
		{
			CreatureData displayCreature = inventoryTile.InventoryItem.DisplayCreature;
			if (displayCreature != null)
			{
				if (displayCreature.AlreadySeen || displayCreature.AlreadyCollected)
				{
					inventoryTile.ShowRarityFrame();
				}
				else
				{
					inventoryTile.HideRarityFrame();
				}
			}
			else
			{
				inventoryTile.HideRarityFrame();
			}
		}
	}

	public void OnClickSort()
	{
		Singleton<SortPopupController>.Instance.Show(SortPopupController.Category.Creatures, SortButton, RefreshCurrentTab, true);
	}

	public void UnzoomCard()
	{
		mSpawnedCard.Unzoom();
	}

	private void Update()
	{
		if (mCurrentToggle == CollectionToggle && CollectionGrid.gameObject.activeInHierarchy)
		{
			RefreshRarityFramesOnTile();
		}
	}
}
