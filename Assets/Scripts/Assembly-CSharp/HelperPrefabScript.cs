using UnityEngine;

public class HelperPrefabScript : UIStreamingGridListItem
{
	public GameObject mTilePrefabObj;

	public InventoryTile TileScript;

	public UILabel HelperName;

	public UILabel Rank;

	public UILabel Type;

	public UILabel HelpReward;

	public UILabel LastActivity;

	public Transform CreatureTileSpawnNode;

	public GameObject OnlineObject;

	public UILabel OfflineLabel;

	public GameObject HighlightObject;

	public HelperItem Helper;

	public HelperMode Mode;

	public bool IsSelected;

	public bool ShowOnlineStatus { get; set; }

	public override void Populate(object dataObj)
	{
		if (!(dataObj is HelperItem))
		{
			return;
		}
		Helper = dataObj as HelperItem;
		HelperName.text = Helper.HelperName;
		Rank.text = KFFLocalization.Get("!!RANK") + " : " + Helper.HelperRank;
		LastActivity.gameObject.SetActive(false);
		Type.text = ((Helper.IsAlly != 1) ? KFFLocalization.Get("!!EXPLORER") : KFFLocalization.Get("!!ALLY"));
		if (Helper.IsAlly == 1)
		{
			HelpReward.text = "+ " + MiscParams.HelpPointForAlly + " " + KFFLocalization.Get("!!CURRENCY_PVP");
			HelpReward.color = Color.yellow;
			HelpReward.fontSize = 30;
			Type.color = Color.cyan;
		}
		else
		{
			HelpReward.text = "+ " + MiscParams.HelpPointForExplorer + " " + KFFLocalization.Get("!!CURRENCY_PVP");
			HelpReward.color = Color.green;
			HelpReward.fontSize = 20;
			Type.color = Color.magenta;
		}
		InventoryTile.SetDelegates(InventorySlotType.Creature, InventoryTileDraggable, OnInventoryTileDragBeginFromSlot, OnInventoryTileDropped, RefreshInventoryTileOverlay, null, null);
		if (mTilePrefabObj == null)
		{
			mTilePrefabObj = CreatureTileSpawnNode.InstantiateAsChild(Singleton<PrefabReferences>.Instance.InventoryTile);
		}
		mTilePrefabObj.ChangeLayer(CreatureTileSpawnNode.gameObject.layer);
		TileScript = mTilePrefabObj.GetComponent<InventoryTile>();
		TileScript.ParentHelper = Helper;
		TileScript.DisableCardEditing = true;
		if (Helper.HelperCreature != null)
		{
			TileScript.Populate(Helper.HelperCreature);
			TileScript.AssignedSlot = -1;
			if (Mode == HelperMode.HelperList)
			{
				TileScript.ParentGrid = Singleton<PreMatchHelperSelectController>.Instance.HelperGrid;
			}
		}
		OnlineObject.SetActive(false);
		OfflineLabel.gameObject.SetActive(false);
	}

	private bool InventoryTileDraggable(InventoryTile tile)
	{
		if (Singleton<PreMatchHelperSelectController>.Instance != null && !Singleton<PreMatchHelperSelectController>.Instance.IsHelperListDisplayed)
		{
			return false;
		}
		if (tile.IsAttachedToTarget())
		{
			return true;
		}
		if (!InventoryTile.IsRightToLeftDrag())
		{
			return false;
		}
		if (Singleton<PreMatchHelperSelectController>.Instance != null && Singleton<PreMatchHelperSelectController>.Instance.IsThisTileAssigned(tile))
		{
			return false;
		}
		return true;
	}

	private void OnInventoryTileDragBeginFromSlot(InventoryTile tile)
	{
		RemoveCreatureTile(tile, tile.AssignedSlot);
		Singleton<PreMatchController>.Instance.OnClickRemoveHelper();
		tile.transform.parent = Singleton<PreMatchHelperSelectController>.Instance.HelperGrid.transform.parent;
		tile.gameObject.ChangeLayer(Singleton<PreMatchHelperSelectController>.Instance.HelperGrid.gameObject.layer);
	}

	private bool OnInventoryTileDropped(InventoryTile tile, int slotIndex)
	{
		Singleton<PreMatchController>.Instance.StopTweenForDraggingHelper();
		if (slotIndex == 5)
		{
			tile.EquipTween.Play();
			HelperPrefabScript helperPrefab = Singleton<PreMatchHelperSelectController>.Instance.GetHelperPrefab(tile.ParentHelper);
			helperPrefab.SetHelperToSlot(tile, slotIndex);
			Singleton<PreMatchHelperSelectController>.Instance.HideTween.Play();
			MenuStackManager.RemoveTopItemFromStack(true);
			return true;
		}
		return false;
	}

	public void RemoveCreatureTile(InventoryTile creature, int slot)
	{
		Singleton<PreMatchHelperSelectController>.Instance.AssignedHelper = null;
		Singleton<PreMatchHelperSelectController>.Instance.SelectedHelperCreatureTile = null;
	}

	private void RefreshInventoryTileOverlay(InventoryTile tile)
	{
		if (Mode == HelperMode.HelperList)
		{
			if (Singleton<PreMatchHelperSelectController>.Instance.IsThisTileAssigned(tile))
			{
				tile.SetOverlayStatus(InventoryTile.TileStatus.SelectedAsTarget);
			}
			else
			{
				tile.SetOverlayStatus(InventoryTile.TileStatus.NotSelected, false);
			}
			tile.HelperTagGroup.SetActive(false);
		}
		else
		{
			tile.ClearOverlays();
		}
	}

	public void RefreshOverlay()
	{
		if (Mode == HelperMode.PostMatchAllyInvite || Mode == HelperMode.PostMatchHelpReward || Mode == HelperMode.AllyList || Mode == HelperMode.AllyListRemoveAllyConfirm)
		{
			Type.gameObject.SetActive(false);
			HelpReward.gameObject.SetActive(false);
			HighlightObject.SetActive(false);
		}
	}

	public override void Unload()
	{
		TileScript.Unload();
	}

	public void SetHelperToSlot(InventoryTile tile, int slot)
	{
		if (Mode == HelperMode.HelperList)
		{
			Singleton<PreMatchHelperSelectController>.Instance.SetHelperToLoadout(Helper);
			Singleton<PreMatchController>.Instance.PlayTweenForSetHelperCreature(Helper.HelperCreature.Creature);
			if (Singleton<PreMatchHelperSelectController>.Instance.SelectedHelperCreatureTile != null)
			{
				NGUITools.Destroy(Singleton<PreMatchHelperSelectController>.Instance.SelectedHelperCreatureTile.gameObject);
			}
			Singleton<PreMatchHelperSelectController>.Instance.SelectedHelperCreatureTile = tile;
			tile.gameObject.ChangeLayer(Singleton<PreMatchController>.Instance.HelperSlot.gameObject.layer);
			tile.transform.parent = Singleton<PreMatchController>.Instance.HelperSlot.transform;
			NGUITools.MarkParentAsChanged(Singleton<PreMatchController>.Instance.HelperSlot.gameObject);
			tile.transform.position = Singleton<PreMatchController>.Instance.HelperSlot.transform.position;
			tile.AssignedSlot = slot;
		}
	}

	public void OnClick()
	{
		if (Mode == HelperMode.AllyList)
		{
			Singleton<AllyInfoPopup>.Instance.Show(Helper);
		}
		else if (Mode == HelperMode.PvpMatch)
		{
			Singleton<PvPAllySelectController>.Instance.OnAllyClicked(this);
		}
	}

	private void Update()
	{
		if (Helper == null)
		{
			return;
		}
		if (ShowOnlineStatus)
		{
			if (Helper.OnlineStatus)
			{
				OnlineObject.SetActive(true);
				OfflineLabel.gameObject.SetActive(false);
				return;
			}
			OnlineObject.SetActive(false);
			OfflineLabel.gameObject.SetActive(true);
			string newValue = PlayerInfoScript.FormatTimeString(Helper.SinceLastActiveTime);
			OfflineLabel.text = KFFLocalization.Get("!!LAST_ONLINE_X_AGO").Replace("<val1>", newValue);
		}
		else
		{
			OnlineObject.SetActive(false);
			OfflineLabel.gameObject.SetActive(false);
		}
	}
}
