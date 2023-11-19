using System.Collections;
using UnityEngine;

public class SetMyHelperController : Singleton<SetMyHelperController>
{
	public UITweenController ShowTween;

	public UITweenController HideTween;

	public UITweenController ShowBackgroundTween;

	public UITweenController HidebackgroundTween;

	public UITweenController ShowFingerTween;

	public UITweenController HideFingerTween;

	public UILabel ShowCreatureButtonLabel;

	public UIStreamingGrid CreatureGrid;

	private UIStreamingGridDataSource<InventorySlotItem> mCreatureGridDataSource = new UIStreamingGridDataSource<InventorySlotItem>();

	public BoxCollider HelperSlot;

	public GameObject CreatureInfoGroup;

	public UILabel HelperCreatureName;

	public UISprite[] HelperCreatureRarityStars;

	public UIGrid RarityGrid;

	public Transform SortButton;

	public Transform CreatureSpawnPoint;

	public GameObject CreaturePedestal;

	private GameObject mCreatureModelInstance;

	private InventoryTile mHelperCreatureTile;

	private InventorySlotItem mHelperCreatureData;

	public GameObject HelperSlotSprite;

	private bool mHelperSwitched;

	public InventoryBarController inventoryBar;

	private bool IsCreatureListOpen;

	public bool SetHelper
	{
		get
		{
			return mHelperSwitched;
		}
		set
		{
			mHelperSwitched = value;
		}
	}

	public void ShowHelperPanel()
	{
		mHelperCreatureData = Singleton<PlayerInfoScript>.Instance.SaveData.FindCreature((InventorySlotItem m) => m.Creature.UniqueId == Singleton<PlayerInfoScript>.Instance.SaveData.MyHelperCreatureID);
		LoadHelperCreatureModel();
		ShowBackgroundTween.Play();
	}

	public void LoadHelperCreatureModel()
	{
		UpdateCreatureInfoGroup();
		StartCoroutine(LoadModelCo());
	}

	public IEnumerator LoadModelCo()
	{
		yield return StartCoroutine(Singleton<SLOTResourceManager>.Instance.LoadCreatureResources(mHelperCreatureData.Creature.Form, delegate(Object objData, Texture2D texture)
		{
			mCreatureModelInstance = CreatureSpawnPoint.InstantiateAsChild((GameObject)objData);
			mHelperCreatureData.Creature.Form.SwapCreatureTexture(mCreatureModelInstance, texture, true);
			float num = Mathf.Max(6f, mHelperCreatureData.Creature.Form.Height);
			mCreatureModelInstance.transform.localScale *= 7f / num;
			VFXRenderQueueSorter vFXRenderQueueSorter = mCreatureModelInstance.AddComponent<VFXRenderQueueSorter>();
			vFXRenderQueueSorter.mType = VFXRenderQueueSorter.RenderSortingType.BACK;
			vFXRenderQueueSorter.mTarget = HelperCreatureName;
			SetWidgetForSpin();
			CreaturePedestal.SetActive(true);
		}));
	}

	public void HelperCreatureList()
	{
		if (!IsCreatureListOpen)
		{
			ShowCreatureList();
		}
		else
		{
			HideCreatureList();
		}
	}

	private void ShowCreatureList()
	{
		ShowTween.Play();
		InventoryBarController.onDoFilterNull();
		InventoryBarController.onDoFilter += PopulateCreatureList;
		inventoryBar.SetFilters(false, true, true, true, true);
		InventoryTile.ResetForNewScreen();
		ShowCreatureButtonLabel.text = KFFLocalization.Get("!!CLOSE_LIST");
		ShowFingerTween.Play();
		IsCreatureListOpen = true;
		SetWidgetForSpin();
		base.gameObject.GetComponent<InventoryBarController>().UpdateInventoryCounter();
		CreatureGrid.transform.parent.GetComponent<UIScrollView>().ResetPosition();
		CreatureGrid.transform.parent.GetComponent<UIScrollView>().UpdateScrollbars();
	}

	public void HideCreatureList()
	{
		if (IsCreatureListOpen)
		{
			HideTween.Play();
			UnloadGrid();
			ShowCreatureButtonLabel.text = KFFLocalization.Get("!!SETMYHELPER");
			HideFingerTween.Play();
			IsCreatureListOpen = false;
			SetWidgetForSpin();
		}
	}

	private void SetWidgetForSpin()
	{
		if (mCreatureModelInstance != null)
		{
			UIWidget component = mCreatureModelInstance.GetComponent<UIWidget>();
			if (component != null)
			{
				component.enabled = !IsCreatureListOpen;
			}
		}
	}

	public void UnloadGrid()
	{
		mCreatureGridDataSource.Clear();
	}

	public void Unload()
	{
		mCreatureGridDataSource.Clear();
		if (mHelperCreatureTile != null)
		{
			NGUITools.Destroy(mHelperCreatureTile.gameObject);
			mHelperCreatureTile = null;
		}
		HelperSlotSprite.SetActive(true);
		UnloadModel();
		CreaturePedestal.SetActive(false);
	}

	public void UnloadModel()
	{
		if (mCreatureModelInstance != null)
		{
			Object.Destroy(mCreatureModelInstance);
			mCreatureModelInstance = null;
		}
		Resources.UnloadUnusedAssets();
	}

	public void PopulateCreatureList()
	{
		InventoryTile.SetDelegates(InventorySlotType.Creature, TileDraggable, OnTileDragBeginFromSlot, OnTileDropped, RefreshTileOverlay, PopulateCreatureList, OnPopupClosed);
		Singleton<PlayerInfoScript>.Instance.SaveData.SortInventory(InventorySlotType.Creature);
		mCreatureGridDataSource.Init(CreatureGrid, Singleton<PrefabReferences>.Instance.InventoryTile, inventoryBar.GetFilteredInventory());
		base.gameObject.GetComponent<InventoryBarController>().ResetScrollView();
	}

	private void OnPopupClosed(InventorySlotItem creature)
	{
		PopulateCreatureList();
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
		if (tile.InventoryItem == mHelperCreatureData)
		{
			return false;
		}
		return true;
	}

	private void OnTileDragBeginFromSlot(InventoryTile tile)
	{
	}

	private bool OnTileDropped(InventoryTile tile, int slot)
	{
		if (slot != -1)
		{
			tile.EquipTween.Play();
			AssignHelperCreature(tile);
			return true;
		}
		return false;
	}

	private void RefreshTileOverlay(InventoryTile tile)
	{
		if (tile.InventoryItem.IsHelper())
		{
			tile.SetOverlayStatus(InventoryTile.TileStatus.SelectedAsTarget, false);
		}
		else
		{
			tile.SetOverlayStatus(InventoryTile.TileStatus.NotSelected, false);
		}
	}

	public void AssignHelperCreature(InventoryTile creature)
	{
		if (mHelperCreatureTile != null)
		{
			NGUITools.Destroy(mHelperCreatureTile.gameObject);
		}
		creature.transform.parent = HelperSlot.transform;
		creature.transform.localPosition = Vector3.zero;
		mHelperCreatureTile = creature;
		mHelperCreatureData = mHelperCreatureTile.InventoryItem;
		Singleton<PlayerInfoScript>.Instance.SaveData.MyHelperCreatureID = mHelperCreatureTile.InventoryItem.Creature.UniqueId;
		mHelperSwitched = true;
		creature.MatchToCollider(HelperSlot);
		UnloadModel();
		LoadHelperCreatureModel();
		creature.AssignedSlot = 0;
	}

	private void UpdateCreatureInfoGroup()
	{
		CreatureInfoGroup.SetActive(true);
		HelperCreatureName.text = mHelperCreatureData.Creature.Form.Name;
		for (int i = 0; i < HelperCreatureRarityStars.Length; i++)
		{
			HelperCreatureRarityStars[i].gameObject.SetActive(i <= mHelperCreatureData.Creature.StarRating - 1);
		}
		RarityGrid.Reposition();
	}

	public void OnClickSort()
	{
		Singleton<SortPopupController>.Instance.Show(SortPopupController.Category.Creatures, SortButton, PopulateCreatureList);
	}
}
