using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellScreenController : Singleton<SellScreenController>
{
	public UITweenController SellItemShowTween;

	public UITweenController SellItemCoinsVFXTween;

	public UITweenController SellItemGemsVFXTween;

	public UITweenController InventoryFullTween;

	public UIStreamingGrid ItemGrid;

	private UIStreamingGridDataSource<InventorySlotItem> mItemGridDataSource = new UIStreamingGridDataSource<InventorySlotItem>();

	public BoxCollider[] ContainerColliders;

	public UILabel SoftCurrencyLabel;

	public UILabel HardCurrencyLabel;

	public UILabel SoftSellPriceLabel;

	public UILabel HardSellPriceLabel;

	public GameObject SellGroup;

	public Transform SortButton;

	public UILabel InventorySlotsLabel;

	public InventoryBarController inventoryBar;

	private InventoryTile[] mFodderList = new InventoryTile[10];

	private int mSoftSellPrice;

	private int mHardSellPrice;

	private bool mShouldAutoRefreshCurrencyLabels = true;

	public void Populate()
	{
		InventoryBarController.onDoFilterNull();
		InventoryBarController.onDoFilter += PopulateItemList;
		inventoryBar.SetFilters(false, true, false, true, false);
		Calculate();
		InventoryTile.ResetForNewScreen();
		Update();
		base.gameObject.GetComponent<InventoryBarController>().UpdateInventoryCounter();
		ItemGrid.transform.parent.GetComponent<UIScrollView>().ResetPosition();
		ItemGrid.transform.parent.GetComponent<UIScrollView>().UpdateScrollbars();
	}

	public void Unload()
	{
		Singleton<MouseOrbitCamera>.Instance.CheckTiltCamSettingBeforeTutorial();
		Singleton<TownHudController>.Instance.ReturnToTownView();
		mItemGridDataSource.Clear();
		for (int i = 0; i < mFodderList.Length; i++)
		{
			if (mFodderList[i] != null)
			{
				NGUITools.Destroy(mFodderList[i].gameObject);
				mFodderList[i] = null;
			}
		}
	}

	public void PopulateItemList()
	{
		PlayerSaveData saveData = Singleton<PlayerInfoScript>.Instance.SaveData;
		InventoryTile.SetDelegates(InventorySlotType.None, TileDraggable, OnTileDragBeginFromSlot, OnTileDropped, RefreshTileOverlay, PopulateItemList, OnPopupClosed);
		saveData.SortInventory(InventorySlotType.Creature);
		mItemGridDataSource.Init(ItemGrid, Singleton<PrefabReferences>.Instance.InventoryTile, inventoryBar.GetFilteredInventory());
		base.gameObject.GetComponent<InventoryBarController>().ResetScrollView();
		string text = saveData.FilledInventoryCount + "/" + saveData.InventorySpace.ToString();
		InventorySlotsLabel.text = text;
		if (saveData.IsInventorySpaceFull())
		{
			if (!InventoryFullTween.AnyTweenPlaying())
			{
				InventoryFullTween.Play();
			}
		}
		else if (InventoryFullTween.AnyTweenPlaying())
		{
			InventoryFullTween.StopAndReset();
		}
	}

	private void OnPopupClosed(InventorySlotItem creature)
	{
		for (int i = 0; i < mFodderList.Length; i++)
		{
			if (mFodderList[i] != null && mFodderList[i].InventoryItem.IsFavorite())
			{
				InventoryTile inventoryTile = mFodderList[i];
				RemoveItem(mFodderList[i].AssignedSlot);
				NGUITools.Destroy(inventoryTile.gameObject);
			}
		}
		PopulateItemList();
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
		if (tile.InventoryItem.IsFavorite())
		{
			return false;
		}
		if (tile.InventoryItem.SlotType == InventorySlotType.Creature)
		{
			if (Singleton<PlayerInfoScript>.Instance.IsCreatureInAnyLoadout(tile.InventoryItem.Creature))
			{
				return false;
			}
			if (Singleton<SellScreenController>.Instance.DownToLastCreatureInList())
			{
				return false;
			}
		}
		else if (tile.InventoryItem.SlotType == InventorySlotType.Card)
		{
			if (tile.InventoryItem.Card.CreatureUID != 0)
			{
				return false;
			}
		}
		else if (tile.InventoryItem.SlotType != InventorySlotType.EvoMaterial && tile.InventoryItem.SlotType != InventorySlotType.XPMaterial)
		{
			return false;
		}
		if (Singleton<SellScreenController>.Instance.IsItemSelected(tile))
		{
			return false;
		}
		if (tile.InventoryItem.IsHelper())
		{
			return false;
		}
		return true;
	}

	private void OnTileDragBeginFromSlot(InventoryTile tile)
	{
		RemoveItem(tile.AssignedSlot);
	}

	private bool OnTileDropped(InventoryTile tile, int slotIndex)
	{
		if (slotIndex != -1)
		{
			tile.EquipTween.Play();
			return AddItem(tile, slotIndex);
		}
		return false;
	}

	private void RefreshTileOverlay(InventoryTile tile)
	{
		if (IsItemSelected(tile))
		{
			tile.SetOverlayStatus(InventoryTile.TileStatus.SelectedAsMaterial);
		}
		else
		{
			tile.SetOverlayStatus(InventoryTile.TileStatus.NotSelected);
		}
	}

	private void RefreshCurrency()
	{
		if (SoftCurrencyLabel.gameObject.activeInHierarchy)
		{
			SoftCurrencyLabel.text = Singleton<PlayerInfoScript>.Instance.SaveData.SoftCurrency.ToString();
			HardCurrencyLabel.text = Singleton<PlayerInfoScript>.Instance.SaveData.HardCurrency.ToString();
		}
	}

	public bool AddItem(InventoryTile tile, int slot)
	{
		if (mFodderList[slot] != null)
		{
			NGUITools.Destroy(mFodderList[slot].gameObject);
		}
		mFodderList[slot] = tile;
		tile.transform.parent = ContainerColliders[slot].transform;
		tile.transform.position = ContainerColliders[slot].transform.position;
		tile.AssignedSlot = slot;
		Calculate();
		return true;
	}

	public void RemoveItem(int slot)
	{
		mFodderList[slot] = null;
		Calculate();
	}

	private void Calculate()
	{
		mSoftSellPrice = 0;
		mHardSellPrice = 0;
		bool active = false;
		InventoryTile[] array = mFodderList;
		foreach (InventoryTile inventoryTile in array)
		{
			if (inventoryTile != null)
			{
				active = true;
				if (inventoryTile.InventoryItem.SlotType == InventorySlotType.Creature)
				{
					mHardSellPrice += inventoryTile.InventoryItem.SellPrice();
				}
				else
				{
					mSoftSellPrice += inventoryTile.InventoryItem.SellPrice();
				}
			}
		}
		SellGroup.SetActive(active);
		SoftSellPriceLabel.text = mSoftSellPrice.ToString();
		HardSellPriceLabel.text = mHardSellPrice.ToString();
	}

	private void OnClickSell()
	{
		List<InventorySlotItem> list = new List<InventorySlotItem>();
		InventoryTile[] array = mFodderList;
		foreach (InventoryTile inventoryTile in array)
		{
			if (inventoryTile != null)
			{
				list.Add(inventoryTile.InventoryItem);
			}
		}
		string empty = string.Empty;
		if (mSoftSellPrice > 0 && mHardSellPrice > 0)
		{
			empty = string.Format(KFFLocalization.Get("!!SELL_CONFIRM_BOTH"), mHardSellPrice + " " + KFFLocalization.Get("!!CURRENCY_HARD"), mSoftSellPrice + " " + KFFLocalization.Get("!!CURRENCY_SOFT"));
		}
		else if (mHardSellPrice > 0)
		{
			empty = string.Format(KFFLocalization.Get("!!SELL_CONFIRM"), mHardSellPrice + " " + KFFLocalization.Get("!!CURRENCY_HARD"));
		}
		else
		{
			if (mSoftSellPrice <= 0)
			{
				return;
			}
			empty = string.Format(KFFLocalization.Get("!!SELL_CONFIRM"), mSoftSellPrice + " " + KFFLocalization.Get("!!CURRENCY_SOFT"));
		}
		Singleton<SimplePopupController>.Instance.ShowPromptWithInventoryItems(string.Empty, empty, ConfirmSell, CancelSell, list);
	}

	private void SendKPITrack()
	{
		InventoryTile[] array = mFodderList;
		foreach (InventoryTile inventoryTile in array)
		{
			string empty = string.Empty;
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			string empty2 = string.Empty;
			string empty3 = string.Empty;
			string value = Singleton<PlayerInfoScript>.Instance.SaveData.SoftCurrency.ToString();
			string value2 = Singleton<PlayerInfoScript>.Instance.SaveData.HardCurrency.ToString();
			string value3 = Singleton<PlayerInfoScript>.Instance.SaveData.FilledInventoryCount.ToString();
			string value4 = Singleton<PlayerInfoScript>.Instance.SaveData.InventorySpace.ToString();
			if (!(inventoryTile != null))
			{
				continue;
			}
			if (inventoryTile.InventoryItem.SlotType == InventorySlotType.Creature)
			{
				empty = "Economy.GemEnter.SellingCreatures";
				empty2 = inventoryTile.InventoryItem.Creature.ToString();
				empty3 = inventoryTile.InventoryItem.SellPrice().ToString();
				dictionary.Clear();
				dictionary.Add("creatureID", empty2);
				dictionary.Add("amount", empty3);
				Singleton<KFFUpsightVGController>.Instance.RecordCustomEvent(empty, dictionary);
			}
			else
			{
				empty = "Economy.CoinEnter.SellingIngredients";
				dictionary.Clear();
				string value5 = string.Empty;
				string text = string.Empty;
				if (inventoryTile.InventoryItem.SlotType == InventorySlotType.EvoMaterial)
				{
					text = inventoryTile.InventoryItem.EvoMaterial.ID;
					value5 = InventorySlotType.EvoMaterial.ToString();
				}
				else if (inventoryTile.InventoryItem.SlotType == InventorySlotType.XPMaterial)
				{
					text = inventoryTile.InventoryItem.XPMaterial.ID;
					value5 = InventorySlotType.XPMaterial.ToString();
				}
				empty2 = text;
				empty3 = inventoryTile.InventoryItem.SellPrice().ToString();
				dictionary.Add("ingredientID", text);
				dictionary.Add("type", value5);
				dictionary.Add("amount", empty3);
				Singleton<KFFUpsightVGController>.Instance.RecordCustomEvent(empty, dictionary);
			}
			empty = "Goose.ItemSold";
			dictionary.Clear();
			dictionary.Add("itemID", empty2);
			dictionary.Add("value", empty3);
			dictionary.Add("oldCoinBalance", value);
			dictionary.Add("oldHCBalance", value2);
			dictionary.Add("oldSize", value3);
			dictionary.Add("maxSize", value4);
			Singleton<KFFUpsightVGController>.Instance.RecordCustomEvent(empty, dictionary);
		}
	}

	public void ConfirmSell()
	{
		SendKPITrack();
		PlayerSaveData saveData = Singleton<PlayerInfoScript>.Instance.SaveData;
		int softCurrency = saveData.SoftCurrency;
		int hardCurrency = saveData.HardCurrency;
		saveData.SoftCurrency += mSoftSellPrice;
		Singleton<SLOTAudioManager>.Instance.PlaySound("SFX_SellItems");
		Singleton<PlayerInfoScript>.Instance.AddHardCurrency2(0, mHardSellPrice, "selling creatures", -1, string.Empty);
		List<InventorySlotItem> list = new List<InventorySlotItem>();
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		for (int i = 0; i < mFodderList.Length; i++)
		{
			if (mFodderList[i] != null)
			{
				list.Add(mFodderList[i].InventoryItem);
				if (mFodderList[i].InventoryItem.SlotType == InventorySlotType.Creature)
				{
					num++;
				}
				else if (mFodderList[i].InventoryItem.SlotType == InventorySlotType.Card)
				{
					num2++;
				}
				else if (mFodderList[i].InventoryItem.SlotType == InventorySlotType.EvoMaterial)
				{
					num3++;
				}
				NGUITools.Destroy(mFodderList[i].gameObject);
				mFodderList[i] = null;
			}
		}
		saveData.RemoveInventoryItems(list);
		DetachedSingleton<MissionManager>.Instance.OnCreaturesSold(num);
		DetachedSingleton<MissionManager>.Instance.OnCardsSold(num2);
		DetachedSingleton<MissionManager>.Instance.OnRunesSold(num3);
		mShouldAutoRefreshCurrencyLabels = false;
		StartCoroutine(CountUpCurrencyLabels(softCurrency, hardCurrency, mSoftSellPrice, mHardSellPrice));
		SellItemShowTween.Play();
		if (mSoftSellPrice > 0)
		{
			SellItemCoinsVFXTween.Play();
		}
		if (mHardSellPrice > 0)
		{
			SellItemGemsVFXTween.Play();
		}
		PopulateItemList();
		Calculate();
		Singleton<PlayerInfoScript>.Instance.Save();
	}

	private IEnumerator CountUpCurrencyLabels(int inSoftCurrency, int inHardCurrency, int inSoftSellPrice, int inHardSellPrice)
	{
		int largerCurrencyGain = Mathf.Max(inSoftSellPrice, inHardSellPrice);
		float totalAnimTime = Mathf.Min((float)largerCurrencyGain * 0.05f, 1.8f);
		float frameDuration = 0.05f;
		int numLoops = Mathf.RoundToInt(totalAnimTime / frameDuration);
		if (largerCurrencyGain <= 5)
		{
			numLoops = largerCurrencyGain;
			frameDuration = 0.15f;
		}
		if (largerCurrencyGain <= 10)
		{
			frameDuration = 0.1f;
		}
		float softIncrement = (float)inSoftSellPrice / (float)numLoops;
		float hardIncrement = (float)inHardSellPrice / (float)numLoops;
		float softFloat = inSoftCurrency;
		float hardFloat = inHardCurrency;
		int softInt2 = 0;
		int hardInt2 = 0;
		for (int i = 0; i < numLoops; i++)
		{
			softFloat += softIncrement;
			hardFloat += hardIncrement;
			softInt2 = Mathf.Min(inSoftCurrency + inSoftSellPrice, Mathf.RoundToInt(softFloat));
			hardInt2 = Mathf.Min(inHardCurrency + inHardSellPrice, Mathf.RoundToInt(hardFloat));
			if (inSoftSellPrice > 0)
			{
				SoftCurrencyLabel.text = softInt2.ToString();
			}
			if (inHardSellPrice > 0)
			{
				HardCurrencyLabel.text = hardInt2.ToString();
			}
			yield return new WaitForSeconds(frameDuration);
		}
		mShouldAutoRefreshCurrencyLabels = true;
	}

	private void CancelSell()
	{
		InventoryTile.SetDelegates(InventorySlotType.None, TileDraggable, OnTileDragBeginFromSlot, OnTileDropped, RefreshTileOverlay, PopulateItemList, OnPopupClosed);
	}

	public bool IsItemSelected(InventoryTile tile)
	{
		return mFodderList.Find((InventoryTile m) => m != null && m.InventoryItem == tile.InventoryItem) != null;
	}

	private void Update()
	{
		if (mShouldAutoRefreshCurrencyLabels)
		{
			RefreshCurrency();
		}
	}

	public bool DownToLastCreatureInList()
	{
		int num = 0;
		for (int i = 0; i < mFodderList.Length; i++)
		{
			if (mFodderList[i] != null && mFodderList[i].InventoryItem.SlotType == InventorySlotType.Creature)
			{
				num++;
			}
		}
		return Singleton<PlayerInfoScript>.Instance.SaveData.CreatureCount - num <= 1;
	}

	public void OnClickSort()
	{
		Singleton<SortPopupController>.Instance.Show(SortPopupController.Category.Creatures, SortButton, PopulateItemList, true);
	}
}
