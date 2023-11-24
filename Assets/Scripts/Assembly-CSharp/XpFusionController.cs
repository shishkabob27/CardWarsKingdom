using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XpFusionController : Singleton<XpFusionController>
{
	private const int SlotCount = 5;

	public UIStreamingGrid CreatureGrid;

	private UIStreamingGridDataSource<InventorySlotItem> mCreatureGridDataSource = new UIStreamingGridDataSource<InventorySlotItem>();

	public BoxCollider PowerUpSlot;

	public GameObject PowerUpSlotSprite;

	public GameObject[] ContainerColliders;

	public UILabel PowerUpCreatureName;

	public UISprite[] PowerUpCreatureRarityStars;

	public UIGrid RarityGrid;

	public GameObject CreatureInfoGroup;

	public UILabel SoftCurrencyLabel;

	public UILabel HardCurrencyLabel;

	public UILabel CostLabel;

	public UILabel CreatureCostLabel;

	public UILabel XpPercentage;

	public UISprite XpBar;

	public UILabel CurrentLevel;

	public UILabel NewLevel;

	public UILabel[] CurrentStats = new UILabel[6];

	public UILabel[] NewStats = new UILabel[6];

	public UILabel CurrentPassiveLevel;

	public UILabel CurrentPassiveDesc;

	public GameObject PowerUpGroup;

	public GameObject GuagesGroup;

	public Transform SortButton;

	public GameObject[] BonusLabels = new GameObject[5];

	public GameObject[] SlotHighlights = new GameObject[5];

	public GameObject[] PlatformLights = new GameObject[5];

	public Material FlashMaterial;

	public Color[] FactionColors = new Color[6];

	[Header("Tweens")]
	public UITweenController ShowTween;

	public UITweenController HideMainTween;

	public UITweenController ShowSlotsTween;

	public UITweenController HideSlotsTween;

	public UITweenController ShowAfterFuseTween;

	public UITweenController HideForFuseTween;

	public UITweenController CantAffordTween;

	public UITweenController[] BonusTweens = new UITweenController[5];

	public UITweenController ShowPowerUpGroup;

	public UITweenController HidePowerUpGroup;

	public UITweenController ShowGuages;

	public UITweenController HideGuages;

	public UITweenController MovePlatformToCenterTween;

	public UITweenController DropPlatformTween;

	public UITweenController PopCreatureTween;

	public UITweenController MovePlatformBackTween;

	public UITweenController ShakeStartLoopTween;

	public UITweenController ShakeStopTween;

	private CreatureItem mPowerUpCreatureBefore;

	private InventoryTile mPowerUpCreature;

	public InventoryTile[] mFodderList = new InventoryTile[5];

	private int mXpGranted;

	private int mCost;

	private int mBuyingGoldPacks;

	public GameObject ResultCreatureBGFX;

	public GameObject PowerUpCompFX;

	private bool isMaxed;

	public GameObject[] upgradeGlows;

	public LaboratorySequence RevealSequenceScript;

	public Camera CreatureCamera_Masked;

	public Camera CreatureCamera_Full;

	public InventoryBarController inventoryBar;

	public Transform CreatureSpawnPoint;

	private GameObject mCreatureModelInstance;

	public InventoryTile GetPowerUpCreature()
	{
		return mPowerUpCreature;
	}

	public void Populate()
	{
		if (Singleton<TutorialController>.Instance.IsBlockActive("XpFusion"))
		{
			Singleton<TutorialController>.Instance.AddCreaturesForFusion();
		}
		ShowTween.Play();
		CreatureInfoGroup.SetActive(false);
		PowerUpGroup.SetActive(false);
		GuagesGroup.SetActive(false);
		InventoryBarController.onDoFilterNull();
		InventoryBarController.onDoFilter += PopulateContents;
		inventoryBar.SetFilters(false, false, true, true, true);
		TurnOffHighlights();
		base.gameObject.GetComponent<InventoryBarController>().UpdateInventoryCounter();
		CreatureGrid.transform.parent.GetComponent<UIScrollView>().ResetPosition();
		CreatureGrid.transform.parent.GetComponent<UIScrollView>().UpdateScrollbars();
	}

	private void PopulateContents()
	{
		for (int i = 0; i < BonusLabels.Length; i++)
		{
			BonusLabels[i].SetActive(false);
		}
		PopulateCreatureList();
		RefreshCurrency();
		InventoryTile.ResetForNewScreen();
	}

	public void Unload()
	{
		mCreatureGridDataSource.Clear();
		if (mPowerUpCreature != null)
		{
			NGUITools.Destroy(mPowerUpCreature.gameObject);
			mPowerUpCreature = null;
		}
		InventoryTile[] array = mFodderList;
		foreach (InventoryTile inventoryTile in array)
		{
			if (inventoryTile != null)
			{
				NGUITools.Destroy(inventoryTile.gameObject);
			}
		}
		PowerUpSlotSprite.SetActive(true);
		mFodderList.Initialize();
		UnloadModel();
		CreatureStatsPanel component = base.gameObject.GetComponent<CreatureStatsPanel>();
		if (component != null)
		{
			component.Unload();
		}
	}

	public void PopulateCreatureList()
	{
		InventoryTile.SetDelegates(InventorySlotType.None, TileDraggable, OnTileDragBeginFromSlot, OnTileDropped, RefreshTileOverlay, PopulateCreatureList, OnPopupClosed);
		InventorySlotType primaryType = ((!(mPowerUpCreature != null)) ? InventorySlotType.Creature : InventorySlotType.XPMaterial);
		Singleton<PlayerInfoScript>.Instance.SaveData.SortInventory(primaryType);
		mCreatureGridDataSource.Init(CreatureGrid, Singleton<PrefabReferences>.Instance.InventoryTile, inventoryBar.GetFilteredInventory());
		base.gameObject.GetComponent<InventoryBarController>().ResetScrollView();
	}

	private void OnPopupClosed(InventorySlotItem creature)
	{
		for (int i = 0; i < mFodderList.Length; i++)
		{
			if (mFodderList[i] != null && mFodderList[i].InventoryItem.IsFavorite())
			{
				InventoryTile inventoryTile = mFodderList[i];
				RemoveCreature(mFodderList[i], mFodderList[i].AssignedSlot);
				NGUITools.Destroy(inventoryTile.gameObject);
			}
		}
		PopulateCreatureList();
	}

	private bool TileDraggable(InventoryTile tile)
	{
		if (tile.InventoryItem.SlotType != InventorySlotType.Creature && tile.InventoryItem.SlotType != InventorySlotType.XPMaterial)
		{
			return false;
		}
		if (IsMismatchedXPMaterial(tile))
		{
			return false;
		}
		if (tile.IsAttachedToTarget())
		{
			return true;
		}
		if (!InventoryTile.IsUpwardsDrag())
		{
			return false;
		}
		if (Singleton<XpFusionController>.Instance.IsTileSelectedForFodder(tile))
		{
			return false;
		}
		if (tile.InventoryItem.SlotType == InventorySlotType.Creature && Singleton<XpFusionController>.Instance.IsCreatureSelectedForPowerUp(tile.InventoryItem.Creature))
		{
			return false;
		}
		return true;
	}

	private void OnTileDragBeginFromSlot(InventoryTile tile)
	{
		RemoveCreature(tile, tile.AssignedSlot);
		PopulateCreatureList();
	}

	private bool OnTileDropped(InventoryTile tile, int slotIndex)
	{
		if (slotIndex != -1)
		{
			if (AddCreature(tile, slotIndex))
			{
				PopulateCreatureList();
				Singleton<TutorialController>.Instance.AdvanceIfOnState("XP_AddFodder2");
				Singleton<TutorialController>.Instance.AdvanceIfOnState("XP_AddFodder1");
				Singleton<TutorialController>.Instance.AdvanceIfOnState("XP_AddCreature");
				return true;
			}
			return false;
		}
		return false;
	}

	private void RefreshTileOverlay(InventoryTile tile)
	{
		if (tile.InventoryItem.SlotType == InventorySlotType.Creature || tile.InventoryItem.SlotType == InventorySlotType.XPMaterial)
		{
			if (IsMismatchedXPMaterial(tile))
			{
				tile.ClearOverlays();
				tile.FadeGroup.SetActive(true);
			}
			else if (IsCreatureSelectedForPowerUp(tile.InventoryItem.Creature))
			{
				tile.SetOverlayStatus(InventoryTile.TileStatus.SelectedAsTarget, false);
			}
			else if (IsTileSelectedForFodder(tile))
			{
				tile.SetOverlayStatus(InventoryTile.TileStatus.SelectedAsMaterial, false);
			}
			else
			{
				tile.SetOverlayStatus(InventoryTile.TileStatus.NotSelected, false);
			}
		}
		else
		{
			tile.ClearOverlays();
			tile.FadeGroup.SetActive(true);
		}
	}

	private bool IsMismatchedXPMaterial(InventoryTile tile)
	{
		if (tile == null)
		{
			return false;
		}
		if (tile.InventoryItem.SlotType != InventorySlotType.XPMaterial)
		{
			return false;
		}
		if (!tile.InventoryItem.XPMaterial.FactionLimited)
		{
			return false;
		}
		if (mPowerUpCreature == null)
		{
			return true;
		}
		return tile.InventoryItem.XPMaterial.Faction != mPowerUpCreature.InventoryItem.Creature.Form.Faction;
	}

	private void RefreshCurrency()
	{
		if (SoftCurrencyLabel.gameObject.activeInHierarchy)
		{
			SoftCurrencyLabel.text = Singleton<PlayerInfoScript>.Instance.SaveData.SoftCurrency.ToString();
			HardCurrencyLabel.text = Singleton<PlayerInfoScript>.Instance.SaveData.HardCurrency.ToString();
		}
	}

	public bool AddCreature(InventoryTile tile, int slot)
	{
		if (slot == 0)
		{
			if (tile.InventoryItem.SlotType != InventorySlotType.Creature)
			{
				return false;
			}
			if (mPowerUpCreature != null)
			{
				NGUITools.Destroy(mPowerUpCreature.gameObject);
			}
			tile.transform.parent = PowerUpSlot.transform;
			tile.transform.localPosition = Vector3.zero;
			mPowerUpCreature = tile;
			CreatureInfoGroup.SetActive(true);
			PowerUpCreatureName.text = mPowerUpCreature.InventoryItem.Creature.Form.Name;
			for (int i = 0; i < PowerUpCreatureRarityStars.Length; i++)
			{
				PowerUpCreatureRarityStars[i].gameObject.SetActive(i <= mPowerUpCreature.InventoryItem.Creature.StarRating - 1);
			}
			RarityGrid.Reposition();
			PowerUpGroup.SetActive(true);
			GuagesGroup.SetActive(true);
			ShowPowerUpGroup.Play();
			ShowGuages.Play();
			StartCoroutine(LoadModelForPrepPowerUpCo());
			PowerUpSlotSprite.SetActive(false);
			tile.MatchToCollider(PowerUpSlot);
			for (int j = 0; j < mFodderList.Length; j++)
			{
				if (IsMismatchedXPMaterial(mFodderList[j]))
				{
					NGUITools.Destroy(mFodderList[j].gameObject);
					mFodderList[j] = null;
					BonusLabels[j].SetActive(false);
				}
			}
			CreatureStatsPanel component = base.gameObject.GetComponent<CreatureStatsPanel>();
			if (component != null)
			{
				component.Populate(tile.InventoryItem);
			}
			isMaxed = mPowerUpCreature.InventoryItem.Creature.Level == mPowerUpCreature.InventoryItem.Creature.MaxLevel;
			GameObject[] array = upgradeGlows;
			foreach (GameObject gameObject in array)
			{
				gameObject.SetActive(!isMaxed);
			}
		}
		else
		{
			if (tile.InventoryItem.IsFavorite())
			{
				return false;
			}
			if (!MiscParams.AllowCreatureFusion && tile.InventoryItem.SlotType != InventorySlotType.XPMaterial)
			{
				return false;
			}
			if (tile.InventoryItem.SlotType == InventorySlotType.Creature)
			{
				if (Singleton<PlayerInfoScript>.Instance.IsCreatureSetAsHelper(tile.InventoryItem.Creature))
				{
					return false;
				}
				if (Singleton<PlayerInfoScript>.Instance.IsCreatureInAnyLoadout(tile.InventoryItem.Creature))
				{
					return false;
				}
			}
			if (mFodderList[slot - 1] != null)
			{
				NGUITools.Destroy(mFodderList[slot - 1].gameObject);
			}
			mFodderList[slot - 1] = tile;
			ContainerColliders[slot - 1].transform.SetLocalScaleX(1f);
			ContainerColliders[slot - 1].transform.SetLocalScaleY(1f);
			tile.transform.parent = ContainerColliders[slot - 1].transform;
			tile.transform.position = ContainerColliders[slot - 1].transform.position;
			ContainerColliders[slot - 1].transform.SetLocalScaleX(0.7f);
			ContainerColliders[slot - 1].transform.SetLocalScaleY(0.7f);
		}
		tile.AssignedSlot = slot;
		tile.EquipTween.Play();
		Calculate();
		return true;
	}

	public void RemoveCreature(InventoryTile creature, int slot)
	{
		if (slot == 0)
		{
			PowerUpSlotSprite.SetActive(true);
			mPowerUpCreature.MatchToCollider(null);
			mPowerUpCreature = null;
			HidePowerUpGroup.Play();
			HideGuages.Play();
			CreatureInfoGroup.SetActive(false);
			UnloadModel();
			for (int i = 0; i < mFodderList.Length; i++)
			{
				if (IsMismatchedXPMaterial(mFodderList[i]))
				{
					NGUITools.Destroy(mFodderList[i].gameObject);
					mFodderList[i] = null;
					BonusLabels[i].SetActive(false);
				}
			}
			CreatureStatsPanel component = base.gameObject.GetComponent<CreatureStatsPanel>();
			if (component != null)
			{
				component.Unload();
			}
		}
		else
		{
			mFodderList[slot - 1] = null;
			BonusLabels[slot - 1].SetActive(false);
		}
		Calculate();
	}

	private void Calculate()
	{
		List<CreatureItem> list = new List<CreatureItem>();
		List<XPMaterialData> list2 = new List<XPMaterialData>();
		TurnOffHighlights();
		int num = 0;
		InventoryTile[] array = mFodderList;
		foreach (InventoryTile inventoryTile in array)
		{
			if (!(inventoryTile != null))
			{
				continue;
			}
			bool flag = false;
			if (inventoryTile.InventoryItem.SlotType == InventorySlotType.Creature)
			{
				list.Add(inventoryTile.InventoryItem.Creature);
				if (mPowerUpCreature != null)
				{
					flag = mPowerUpCreature.InventoryItem.Creature.SameClassBonusXp(inventoryTile.InventoryItem.Creature);
				}
			}
			else
			{
				list2.Add(inventoryTile.InventoryItem.XPMaterial);
				if (mPowerUpCreature != null)
				{
					flag = mPowerUpCreature.InventoryItem.Creature.SameClassBonusXp(inventoryTile.InventoryItem.XPMaterial);
				}
			}
			if (flag)
			{
				BonusTweens[inventoryTile.AssignedSlot - 1].Play();
			}
			else
			{
				BonusLabels[inventoryTile.AssignedSlot - 1].SetActive(false);
			}
			SlotHighlights[inventoryTile.AssignedSlot - 1].SetActive(true);
			PlatformLights[num].SetActive(true);
			num++;
		}
		if (mPowerUpCreature != null)
		{
			mPowerUpCreature.InventoryItem.Creature.CalculateXpFusion(list, list2, out mXpGranted, out mCost);
		}
		else
		{
			mXpGranted = 0;
			mCost = 0;
		}
		CostLabel.text = mCost.ToString();
		if (mCost > Singleton<PlayerInfoScript>.Instance.SaveData.SoftCurrency)
		{
			CantAffordTween.Play();
		}
		else
		{
			CantAffordTween.StopAndReset();
		}
		if (!(mPowerUpCreature != null))
		{
			return;
		}
		CreatureItem creature = mPowerUpCreature.InventoryItem.Creature;
		XPLevelData levelData = creature.GetLevelData();
		CurrentLevel.text = levelData.mCurrentLevel.ToString();
		CurrentPassiveDesc.text = creature.BuildPassiveDescriptionString(false);
		int[] array2 = new int[6];
		for (int j = 0; j < 6; j++)
		{
			array2[j] = creature.GetStat((CreatureStat)j);
			CurrentStats[j].text = array2[j] + ((!((CreatureStat)j).IsPercent()) ? string.Empty : "%");
		}
		int num2 = creature.Xp;
		CreatureCostLabel.text = levelData.mCurrentLevel.ToString();
		try
		{
			creature.Xp = (int)creature.Xp + mXpGranted;
			XPLevelData levelData2 = creature.GetLevelData();
			int num3 = levelData2.mCurrentLevel - levelData.mCurrentLevel;
			if (num3 > 0)
			{
				NewLevel.text = levelData2.mCurrentLevel.ToString();
				CreatureCostLabel.text = levelData2.mCurrentLevel.ToString();
				CreatureStatsPanel component = base.gameObject.GetComponent<CreatureStatsPanel>();
				if (component != null && component.InventoryTile != null)
				{
					component.InventoryTile.SetLevelText(levelData2.mCurrentLevel, levelData2.mIsAtMaxLevel);
				}
				for (int k = 0; k < 6; k++)
				{
					int stat = creature.GetStat((CreatureStat)k);
					NewStats[k].text = stat + ((!((CreatureStat)k).IsPercent()) ? string.Empty : "%");
					if (stat > array2[k])
					{
						NewStats[k].color = Color.green;
					}
					else
					{
						NewStats[k].color = CurrentStats[k].color;
					}
				}
			}
			else
			{
				NewLevel.text = string.Empty;
				CreatureCostLabel.text = levelData.mCurrentLevel.ToString();
				for (int l = 0; l < 6; l++)
				{
					NewStats[l].text = string.Empty;
				}
			}
			if (levelData2.mIsAtMaxLevel)
			{
				XpPercentage.text = "MAX";
				XpBar.fillAmount = 1f;
			}
			else
			{
				int num4 = (int)(levelData2.mPercentThroughCurrentLevel * 100f);
				XpPercentage.text = num4 + 100 * num3 + "%";
				if (num3 > 0)
				{
					XpBar.fillAmount = 1f;
				}
				else
				{
					XpBar.fillAmount = levelData2.mPercentThroughCurrentLevel;
				}
			}
		}
		catch (Exception)
		{
		}
		creature.Xp = num2;
	}

	private void TurnOffHighlights()
	{
		for (int i = 0; i < SlotHighlights.Length; i++)
		{
			SlotHighlights[i].SetActive(false);
		}
		for (int j = 0; j < PlatformLights.Length; j++)
		{
			PlatformLights[j].SetActive(false);
		}
	}

	public void OnClickFuse()
	{
		List<InventoryTile> minimumFodderList = GetMinimumFodderList();
		if (mPowerUpCreature == null)
		{
			string body = KFFLocalization.Get("!!ERROR_POWERUP_NO_CREATURE_SELECTED");
			Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, body, true);
		}
		else if (minimumFodderList.Count == 0)
		{
			string body2 = KFFLocalization.Get("!!ERROR_POWERUP_NO_CREATURE_TOFUSE");
			Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, body2, true);
		}
		else
		{
			if (mPowerUpCreature.InventoryItem.Creature.Level == mPowerUpCreature.InventoryItem.Creature.MaxLevel)
			{
				return;
			}
			if (Singleton<PlayerInfoScript>.Instance.SaveData.SoftCurrency < mCost)
			{
				if (!Singleton<TutorialController>.Instance.IsBlockActive("XpFusion"))
				{
					int totalGold;
					int totalGemCost;
					Singleton<PlayerInfoScript>.Instance.CalculateGoldPacksNeeded(mCost, out mBuyingGoldPacks, out totalGold, out totalGemCost);
					string confirmString = string.Format(KFFLocalization.Get("!!ERROR_POWERUP_NOT_ENOUGH_CURRENCY_BUY"), mCost, totalGold);
					string insufficientString = string.Format(KFFLocalization.Get("!!ERROR_POWERUP_NOT_ENOUGH_CURRENCY_NOBUY"), mCost);
					Singleton<SimplePopupController>.Instance.ShowPurchasePrompt(confirmString, insufficientString, totalGemCost, OnClickConfirmBuyGold);
					return;
				}
				Singleton<PlayerInfoScript>.Instance.SaveData.SoftCurrency = mCost;
			}
			StartCoroutine(FuseSequence());
			CreatureItem creature = mPowerUpCreature.InventoryItem.Creature;
			CreatureItem creatureItem = new CreatureItem(creature.Form);
			creatureItem.StarRating = creature.StarRating;
			creatureItem.Xp = creature.Xp;
			creatureItem.PassiveSkillLevel = creature.PassiveSkillLevel;
			Singleton<FuseStatsPanelController>.Instance.ResultCreatureBefore = creatureItem;
			creature.Xp = (int)creature.Xp + mXpGranted;
			Singleton<FuseStatsPanelController>.Instance.ResultCreature = creature;
			Singleton<FuseStatsPanelController>.Instance.EarnedXP = mXpGranted;
			Singleton<PlayerInfoScript>.Instance.SaveData.SoftCurrency -= mCost;
			List<InventorySlotItem> list = new List<InventorySlotItem>();
			List<InventorySlotItem> list2 = new List<InventorySlotItem>();
			InventoryTile[] array = mFodderList;
			foreach (InventoryTile inventoryTile in array)
			{
				if (!(inventoryTile == null))
				{
					if (inventoryTile.InventoryItem.SlotType == InventorySlotType.Creature)
					{
						list.Add(inventoryTile.InventoryItem);
					}
					else
					{
						list2.Add(inventoryTile.InventoryItem);
					}
				}
			}
			DetachedSingleton<MissionManager>.Instance.OnCreaturesFused(list);
			Singleton<PlayerInfoScript>.Instance.SaveData.RemoveCreatures(list);
			Singleton<PlayerInfoScript>.Instance.SaveData.RemoveXPMaterials(list2);
			Singleton<PlayerInfoScript>.Instance.Save();
			mFodderList.Initialize();
			InventoryTile[] array2 = mFodderList;
			foreach (InventoryTile inventoryTile2 in array2)
			{
				if (inventoryTile2 != null)
				{
					NGUITools.Destroy(inventoryTile2.gameObject);
				}
			}
			Singleton<PlayerInfoScript>.Instance.UpdateBadgeCount(BadgeEnum.Evo);
			inventoryBar.UpdateInventoryCounter();
		}
	}

	private void OnClickConfirmBuyGold()
	{
		Singleton<PlayerInfoScript>.Instance.BuyGold(mBuyingGoldPacks);
	}

	public bool IsCreatureSelectedForPowerUp(CreatureItem creature)
	{
		if (mPowerUpCreature != null && mPowerUpCreature.InventoryItem.Creature == creature)
		{
			return true;
		}
		return false;
	}

	public bool IsTileSelectedForFodder(InventoryTile tile)
	{
		InventoryTile[] array = mFodderList;
		foreach (InventoryTile inventoryTile in array)
		{
			if (inventoryTile != null && inventoryTile.InventoryItem == tile.InventoryItem)
			{
				return true;
			}
		}
		return false;
	}

	private void Update()
	{
		RefreshCurrency();
		AdjustCameraDepth();
	}

	private void AdjustCameraDepth()
	{
		if (!CreatureCamera_Masked.isActiveAndEnabled || !(mCreatureModelInstance != null) || mPowerUpCreature.InventoryItem == null || mPowerUpCreature.InventoryItem.Creature.Form.InitNumOfLoad <= 0)
		{
			return;
		}
		Renderer[] componentsInChildren = mCreatureModelInstance.GetComponentsInChildren<Renderer>();
		bool flag = false;
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i] != null && componentsInChildren[i].isVisible)
			{
				flag = true;
			}
		}
		if (flag)
		{
			CreatureCamera_Masked.fieldOfView = 30f;
		}
		else
		{
			CreatureCamera_Masked.fieldOfView = 45f;
		}
	}

	private List<InventoryTile> GetMinimumFodderList()
	{
		List<InventoryTile> list = new List<InventoryTile>();
		InventoryTile[] array = mFodderList;
		foreach (InventoryTile inventoryTile in array)
		{
			if (inventoryTile != null)
			{
				list.Add(inventoryTile);
			}
		}
		return list;
	}

	public GameObject GetUnusedCreature()
	{
		InventorySlotItem dataItem = Singleton<PlayerInfoScript>.Instance.SaveData.FindCreature((InventorySlotItem m) => m != mPowerUpCreature.InventoryItem && mFodderList.Find((InventoryTile fm) => fm != null && fm.InventoryItem == m) == null && !Singleton<PlayerInfoScript>.Instance.IsCreatureInAnyLoadout(m.Creature));
		return mCreatureGridDataSource.FindPrefab(dataItem);
	}

	public GameObject GetBestCreature()
	{
		InventorySlotItem bestCreature = Singleton<PlayerInfoScript>.Instance.SaveData.GetBestCreature();
		return mCreatureGridDataSource.FindPrefab(bestCreature);
	}

	public IEnumerator LoadModelForPrepPowerUpCo()
	{
		UnloadModel();
		yield return StartCoroutine(Singleton<SLOTResourceManager>.Instance.LoadCreatureResources(mPowerUpCreature.InventoryItem.Creature.Form, delegate(UnityEngine.GameObject objData, Texture2D texture)
		{
			CreatureSpawnPoint.transform.localRotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
            GameObject resource = Resources.Load("Creatures/" + mPowerUpCreature.InventoryItem.Creature.Form.Prefab + "/" + mPowerUpCreature.InventoryItem.Creature.Form.Prefab, typeof(GameObject)) as GameObject;
            Texture2D texture2D = Resources.Load("Creatures/" + mPowerUpCreature.InventoryItem.Creature.Form.Prefab + "/Textures/" + mPowerUpCreature.InventoryItem.Creature.Form.Faction + "/" + mPowerUpCreature.InventoryItem.Creature.Form.PrefabTexture, typeof(Texture2D)) as Texture2D;
            mCreatureModelInstance = CreatureSpawnPoint.InstantiateAsChild(resource);
			mPowerUpCreature.InventoryItem.Creature.Form.SwapCreatureTexture(mCreatureModelInstance, texture2D);
			mCreatureModelInstance.transform.localScale = PowerUpCreatureScale();
			VFXRenderQueueSorter vFXRenderQueueSorter = mCreatureModelInstance.AddComponent<VFXRenderQueueSorter>();
			vFXRenderQueueSorter.mType = VFXRenderQueueSorter.RenderSortingType.BACK;
			vFXRenderQueueSorter.mTarget = PowerUpCreatureName;
		}));
	}

	private Vector3 PowerUpCreatureScale()
	{
		float num = Mathf.Max(4f, mPowerUpCreature.InventoryItem.Creature.Form.Height);
		float num2 = Mathf.Min(1f, 5f / num);
		return Vector3.one * num2;
	}

	public void UnloadModel()
	{
		if (mCreatureModelInstance != null)
		{
			UnityEngine.Object.Destroy(mCreatureModelInstance);
			mCreatureModelInstance = null;
		}
		Resources.UnloadUnusedAssets();
	}

	public void OnClickSort()
	{
		Singleton<SortPopupController>.Instance.Show(SortPopupController.Category.Creatures, SortButton, PopulateCreatureList);
	}

	private IEnumerator FuseSequence()
	{
		if (RevealSequenceScript == null)
		{
			RevealSequenceScript = base.gameObject.GetComponentInChildren<LaboratorySequence>();
		}
		List<UITexture> fodderTextures = new List<UITexture>();
		for (int i = 0; i < mFodderList.Length; i++)
		{
			if (mFodderList[i] != null)
			{
				fodderTextures.Add(mFodderList[i].PortraitTexture);
			}
		}
		CreatureSpawnPoint.transform.localRotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
		HideForFuseTween.Play();
		HidePowerUpGroup.Play();
		HideGuages.Play();
		HideSlotsTween.Play();
		MovePlatformToCenterTween.Play();
		CreatureCamera_Masked.gameObject.SetActive(false);
		CreatureCamera_Full.gameObject.SetActive(true);
		LaboratorySequence.onElectricStart += OnElectricStart;
		LaboratorySequence.onSequencePop += OnSequencePop;
		LaboratorySequence.onSequenceComplete += OnClickCloseResult;
		TurnOffHighlights();
		if (RevealSequenceScript != null)
		{
			RevealSequenceScript.StartModuleSequence(fodderTextures);
		}
		else
		{
			OnClickCloseResult();
		}
		yield break;
	}

	public void OnElectricStart()
	{
		DropPlatformTween.Play();
		TurnOffHighlights();
		ShakeStartLoopTween.Play();
	}

	public void OnSequencePop()
	{
		PopCreatureTween.Play();
		TurnOffHighlights();
		ShakeStartLoopTween.End();
		ShakeStopTween.Play();
		StartCoroutine(FlashCreatureCo());
	}

	public IEnumerator FlashCreatureCo()
	{
		yield return new WaitForSeconds(0.01f);
		FlashCreature();
		yield return new WaitForSeconds(0.08f);
		UnFlashCreature();
		yield return new WaitForSeconds(0.07f);
		FlashCreature();
		yield return new WaitForSeconds(0.08f);
		UnFlashCreature();
		yield return new WaitForSeconds(0.1f);
		FlashCreature();
		yield return new WaitForSeconds(0.13f);
		UnFlashCreature();
		yield return new WaitForSeconds(0.3f);
		FlashCreature();
		yield return new WaitForSeconds(0.23f);
		UnFlashCreature();
	}

	public void OnClickCloseResult()
	{
		StartCoroutine(ShowMainAfterFuse());
	}

	private IEnumerator ShowMainAfterFuse()
	{
		ShowAfterFuseTween.Play();
		ShowSlotsTween.Play();
		MovePlatformBackTween.PlayWithCallback(SwapToMaskedCamera);
		TurnOffHighlights();
		ShowPowerUpGroup.Play();
		ShowGuages.Play();
		PopulateContents();
		Calculate();
		yield return new WaitForSeconds(1f);
		UICamera.UnlockInput();
	}

	public void SwapToMaskedCamera()
	{
		CreatureCamera_Masked.gameObject.SetActive(true);
		CreatureCamera_Full.gameObject.SetActive(false);
	}

	public void OnClickBackToTown()
	{
		Singleton<MouseOrbitCamera>.Instance.CheckTiltCamSettingBeforeTutorial();
		HideMainTween.Play();
		Singleton<LabBuildingController>.Instance.Hide();
	}

	public GameObject GetTutorialFuseFodder()
	{
		List<InventorySlotItem> allXPMaterials = Singleton<PlayerInfoScript>.Instance.SaveData.GetAllXPMaterials();
		InventoryTile[] array = mFodderList;
		InventoryTile tile;
		for (int i = 0; i < array.Length; i++)
		{
			tile = array[i];
			if (tile != null)
			{
				allXPMaterials.RemoveAll((InventorySlotItem m) => m.XPMaterial == tile.InventoryItem.XPMaterial);
			}
		}
		allXPMaterials.Reverse();
		InventorySlotItem dataItem = allXPMaterials.Find((InventorySlotItem m) => m.GivenForTutorial);
		return mCreatureGridDataSource.FindPrefab(dataItem);
	}

	public void FlashCreature()
	{
		GameObject gameObject = CreatureSpawnPoint.GetChild(0).gameObject;
		BattleCreatureAnimState componentInChildren = gameObject.GetComponentInChildren<BattleCreatureAnimState>();
		foreach (SkinnedMeshRenderer orignalMesh in componentInChildren.orignalMeshes)
		{
			orignalMesh.material = FlashMaterial;
		}
	}

	public void UnFlashCreature()
	{
		GameObject gameObject = CreatureSpawnPoint.GetChild(0).gameObject;
		BattleCreatureAnimState componentInChildren = gameObject.GetComponentInChildren<BattleCreatureAnimState>();
		for (int i = 0; i < componentInChildren.orignalMeshes.Count; i++)
		{
			componentInChildren.orignalMeshes[i].material = componentInChildren.originalMats[i];
		}
	}
}
