using System.Collections;
using System.Collections.Generic;
using Allies;
using UnityEngine;

public class PreMatchHelperSelectController : Singleton<PreMatchHelperSelectController>
{
	public GameObject HelperTilePrefab;

	public UITweenController ShowTween;

	public UITweenController HideTween;

	public Transform SortButton;

	public UIStreamingGrid HelperGrid;

	private UIStreamingGridDataSource<HelperItem> mHelperGridDataSource = new UIStreamingGridDataSource<HelperItem>();

	public InventoryTile SelectedHelperCreatureTile;

	private List<AllyData> mFetchedAllies = new List<AllyData>();

	private List<HelperItem> mHelperItemList = new List<HelperItem>();

	public HelperPrefabScript AssignedHelper;

	public bool IsHelperListDisplayed;

	public void Show()
	{
		StartCoroutine(RetrieveHelperList());
	}

	public IEnumerator RetrieveHelperList()
	{
		List<string> exlist = Singleton<PlayerInfoScript>.Instance.StateData.UsedHelperList;
		if (exlist == null)
		{
			exlist = new List<string>();
		}
		mFetchedAllies.Clear();
		bool waiting = true;
		Singleton<BusyIconPanelController>.Instance.Show();
		Ally.GetHelperList(SessionManager.Instance.theSession, exlist, delegate(List<AllyData> alliesList, ResponseFlag flag)
		{
			if (flag == ResponseFlag.Success)
			{
				mFetchedAllies = alliesList;
			}
			waiting = false;
		});
		while (waiting)
		{
			yield return null;
		}
		Singleton<BusyIconPanelController>.Instance.Hide();
		PopulateHelperGrid();
		Singleton<TutorialController>.Instance.AdvanceIfNextStateIs("SC_DragHelper");
	}

	private void OnCancelClicked()
	{
		OnCloseClicked();
	}

	public bool IsThisTileAssigned(InventoryTile tile)
	{
		if (SelectedHelperCreatureTile == null)
		{
			return false;
		}
		if (tile.ParentHelper == null)
		{
			return false;
		}
		return tile.ParentHelper.HelperID == SelectedHelperCreatureTile.ParentHelper.HelperID;
	}

	public void PopulateHelperGrid()
	{
		int averageCreatureLevel = Singleton<PlayerInfoScript>.Instance.GetCurrentLoadout().GetAverageCreatureLevel();
		IsHelperListDisplayed = true;
		mHelperItemList.Clear();
		if (mFetchedAllies.Count > 0)
		{
			foreach (AllyData mFetchedAlly in mFetchedAllies)
			{
				HelperItem helperItem = new HelperItem(mFetchedAlly);
				if (helperItem.HelperCreature != null && helperItem.HelperCreature.Creature != null)
				{
					if (averageCreatureLevel >= helperItem.HelperCreature.Creature.MaxLevel)
					{
						helperItem.HelperCreature.Creature.Xp = helperItem.HelperCreature.Creature.XPTable.GetXpToReachLevel(helperItem.HelperCreature.Creature.MaxLevel);
					}
					else
					{
						helperItem.HelperCreature.Creature.Xp = helperItem.HelperCreature.Creature.XPTable.GetXpToReachLevel(averageCreatureLevel);
					}
					mHelperItemList.Add(helperItem);
				}
			}
		}
		else
		{
			HelperItem helperItem2 = new HelperItem(KFFLocalization.Get("!!FRIENDLY_HELPER"), null);
			helperItem2.Fake = true;
			helperItem2.HelperRank = Singleton<PlayerInfoScript>.Instance.RankXpLevelData.mCurrentLevel;
			List<CreatureData> list = CreatureDataManager.Instance.GetDatabase().FindAll((CreatureData m) => m.AlreadyCollected);
			CreatureData data = list[Random.Range(0, list.Count)];
			CreatureItem creatureItem = new CreatureItem(data);
			creatureItem.FromOtherPlayer = true;
			helperItem2.HelperCreature = new InventorySlotItem(creatureItem);
			if (averageCreatureLevel >= helperItem2.HelperCreature.Creature.MaxLevel)
			{
				helperItem2.HelperCreature.Creature.Xp = helperItem2.HelperCreature.Creature.XPTable.GetXpToReachLevel(helperItem2.HelperCreature.Creature.MaxLevel);
			}
			else
			{
				helperItem2.HelperCreature.Creature.Xp = helperItem2.HelperCreature.Creature.XPTable.GetXpToReachLevel(averageCreatureLevel);
			}
			mHelperItemList.Add(helperItem2);
		}
		SortHelpers();
		mHelperGridDataSource.Init(HelperGrid, HelperTilePrefab, mHelperItemList);
		HelperPrefabScript[] componentsInChildren = HelperGrid.GetComponentsInChildren<HelperPrefabScript>();
		HelperPrefabScript[] array = componentsInChildren;
		foreach (HelperPrefabScript helperPrefabScript in array)
		{
			helperPrefabScript.RefreshOverlay();
		}
		InventoryTile[] componentsInChildren2 = HelperGrid.GetComponentsInChildren<InventoryTile>(true);
		InventoryTile[] array2 = componentsInChildren2;
		foreach (InventoryTile inventoryTile in array2)
		{
			inventoryTile.CurrentSorts = Singleton<PlayerInfoScript>.Instance.SaveData.HelperSorts;
			inventoryTile.StartBlinkStats();
		}
		ShowTween.PlayWithCallback(DelayEnableInput);
	}

	private void DelayEnableInput()
	{
		UICamera.UnlockInput();
	}

	public void OnCloseClicked()
	{
		IsHelperListDisplayed = false;
		Unload();
		UICamera.ForceUnlockInput();
	}

	public void Unload()
	{
		mHelperGridDataSource.Clear();
		mHelperItemList.Clear();
		mFetchedAllies.Clear();
	}

	public void SetHelperToLoadout(HelperItem helper)
	{
		if (Singleton<PlayerInfoScript>.Instance.StateData.SelectedHelper != null)
		{
			KPIHelper(true, Singleton<PlayerInfoScript>.Instance.StateData.SelectedHelper);
		}
		if (helper != null)
		{
			KPIHelper(false, helper);
		}
		Singleton<PlayerInfoScript>.Instance.StateData.SelectedHelper = helper;
		Singleton<PreMatchController>.Instance.OnHelperChanged();
		Singleton<TutorialController>.Instance.AdvanceIfDraggingTile();
	}

	private void KPIHelper(bool remove, HelperItem helper)
	{
		string text = "Helper.";
		text = ((!remove) ? (text + "Selected") : (text + "Unselected"));
		string iD = Singleton<PlayerInfoScript>.Instance.StateData.CurrentActiveQuest.SetLoadout.ID;
		string iD2 = Singleton<PlayerInfoScript>.Instance.GetCurrentLoadout().Leader.SelectedSkin.ID;
		string text2 = string.Empty;
		int num = 5;
		for (int i = 0; i < num; i++)
		{
			InventorySlotItem inventorySlotItem = Singleton<PlayerInfoScript>.Instance.GetCurrentLoadout().CreatureSet[i];
			if (Singleton<PlayerInfoScript>.Instance.GetCurrentLoadout().CreatureSet[i] != null)
			{
				text2 += inventorySlotItem.Creature.ToString();
				if (i < num - 1)
				{
					text2 += ", ";
				}
			}
		}
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("questID", iD);
		dictionary.Add("helperID", helper.HelperCreature.Creature.ToString());
		dictionary.Add("helperStars", helper.HelperCreature.Creature.StarRating.ToString());
		dictionary.Add("helperLevel", helper.HelperCreature.Creature.Level.ToString());
		dictionary.Add("isAlly", helper.IsAlly.ToString());
		dictionary.Add("leader", iD2);
		dictionary.Add("team", text2);
		Singleton<KFFUpsightVGController>.Instance.RecordCustomEvent(text, dictionary);
	}

	public void OnClickSort()
	{
		Singleton<SortPopupController>.Instance.Show(SortPopupController.Category.Helpers, SortButton, PopulateHelperGrid);
	}

	public void SortHelpers()
	{
		HelperComparer comparer = new HelperComparer(Singleton<PlayerInfoScript>.Instance.SaveData.HelperSorts);
		mHelperItemList.Sort(comparer);
	}

	public HelperPrefabScript GetHelperPrefab(HelperItem helper)
	{
		HelperPrefabScript[] componentsInChildren = HelperGrid.GetComponentsInChildren<HelperPrefabScript>();
		return componentsInChildren.Find((HelperPrefabScript m) => m.Helper == helper);
	}

	public GameObject GetHelperCreatureObject()
	{
		if (HelperGrid.transform.childCount > 0)
		{
			HelperPrefabScript component = HelperGrid.transform.GetChild(0).GetComponent<HelperPrefabScript>();
			return component.TileScript.gameObject;
		}
		return null;
	}
}
