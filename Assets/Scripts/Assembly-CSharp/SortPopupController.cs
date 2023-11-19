using System.Collections.Generic;
using UnityEngine;

public class SortPopupController : Singleton<SortPopupController>
{
	public enum Category
	{
		Creatures,
		Cards,
		Helpers,
		Count
	}

	public delegate void SortedCallback();

	public int MaxCreatureSorts = 7;

	public int MaxCardSorts = 5;

	public float ListBorder = 30f;

	public float TypesListBorder = 15f;

	public GameObject PopupEntryPrefab;

	public GameObject SortTypePrefab;

	public UITweenController ShowTween;

	public UITweenController HideTween;

	public UITweenController ShowSortTypesTween;

	public UITweenController HideSortTypesTween;

	public Transform MainPanel;

	public UIGrid EntryGrid;

	public UISprite ListBackground;

	public Transform SortTypesGroup;

	public UIGrid SortTypesGrid;

	public UISprite SortTypesBackground;

	public GameObject ToggleGroup;

	public UILabel ToggleLabel;

	private Vector3 mGridPos;

	private Vector3 mSortTypesGridPos;

	private Vector3 mSortTypesBGPos;

	private Category mCategory;

	private Category mToggledCategory;

	private SortPopupEntry mChangingSortEntry;

	private SortedCallback mSortedCallback;

	private void Awake()
	{
		mGridPos = EntryGrid.transform.localPosition;
		mSortTypesGridPos = SortTypesGrid.transform.localPosition;
		mSortTypesBGPos = SortTypesBackground.transform.localPosition;
	}

	public void Show(Category category, Transform sortButton, SortedCallback sortedCallback, bool showCategoryToggle = false)
	{
		ShowTween.Play();
		ToggleGroup.SetActive(showCategoryToggle);
		if (showCategoryToggle)
		{
			UpdateToggleText();
			mCategory = mToggledCategory;
		}
		else
		{
			mCategory = category;
		}
		mSortedCallback = sortedCallback;
		MainPanel.position = sortButton.position;
		PopulateEntries();
	}

	private void UpdateToggleText()
	{
		if (mToggledCategory == Category.Creatures)
		{
			ToggleLabel.text = KFFLocalization.Get("!!SORT_CREATURES");
		}
		else
		{
			ToggleLabel.text = KFFLocalization.Get("!!SORT_CARDS");
		}
	}

	private void PopulateEntries()
	{
		EntryGrid.transform.DestroyAllChildren();
		int num = 0;
		int num2 = 0;
		bool showDelete = Singleton<PlayerInfoScript>.Instance.SaveData.CreatureSorts.Count > 1;
		if (mCategory == Category.Creatures)
		{
			num2 = MaxCreatureSorts;
			foreach (SortEntry creatureSort in Singleton<PlayerInfoScript>.Instance.SaveData.CreatureSorts)
			{
				GameObject gameObject = EntryGrid.transform.InstantiateAsChild(PopupEntryPrefab);
				gameObject.ChangeLayer(base.gameObject.layer);
				SortPopupEntry component = gameObject.GetComponent<SortPopupEntry>();
				component.Populate(creatureSort, num, showDelete);
				num++;
			}
		}
		else if (mCategory == Category.Cards)
		{
			num2 = MaxCardSorts;
			List<SortEntry> exCardSorts = Singleton<PlayerInfoScript>.Instance.SaveData.ExCardSorts;
			foreach (SortEntry item in exCardSorts)
			{
				GameObject gameObject2 = EntryGrid.transform.InstantiateAsChild(PopupEntryPrefab);
				gameObject2.ChangeLayer(base.gameObject.layer);
				SortPopupEntry component2 = gameObject2.GetComponent<SortPopupEntry>();
				component2.Populate(item, num, showDelete);
				num++;
			}
		}
		else if (mCategory == Category.Helpers)
		{
			num2 = MaxCreatureSorts;
			foreach (SortEntry helperSort in Singleton<PlayerInfoScript>.Instance.SaveData.HelperSorts)
			{
				GameObject gameObject3 = EntryGrid.transform.InstantiateAsChild(PopupEntryPrefab);
				gameObject3.ChangeLayer(base.gameObject.layer);
				SortPopupEntry component3 = gameObject3.GetComponent<SortPopupEntry>();
				component3.Populate(helperSort, num, showDelete);
				num++;
			}
		}
		if (num < num2)
		{
			GameObject gameObject4 = EntryGrid.transform.InstantiateAsChild(PopupEntryPrefab);
			gameObject4.ChangeLayer(base.gameObject.layer);
			SortPopupEntry component4 = gameObject4.GetComponent<SortPopupEntry>();
			component4.Populate(null, num, false);
			num++;
		}
		Vector3 localPosition = mGridPos;
		localPosition.y += (float)(num - 1) * EntryGrid.cellHeight;
		EntryGrid.transform.localPosition = localPosition;
		EntryGrid.Reposition();
		ListBackground.height = Mathf.RoundToInt((ListBorder + (float)num * EntryGrid.cellHeight) / ListBackground.transform.localScale.y) + 20;
	}

	public void Unload()
	{
		EntryGrid.transform.DestroyAllChildren();
		SortTypesGrid.transform.DestroyAllChildren();
	}

	public void OnClickName(SortPopupEntry entry)
	{
		if (mChangingSortEntry == entry)
		{
			mChangingSortEntry = null;
			HideSortTypesTween.Play();
			return;
		}
		ShowSortTypesTween.Play();
		mChangingSortEntry = entry;
		SortTypesGroup.position = entry.transform.position;
		SortTypesGrid.transform.DestroyAllChildren();
		SortTypesGrid.maxPerLine = 3;
		List<SortTypeEnum> sortTypes = GetSortTypes();
		foreach (SortTypeEnum item in sortTypes)
		{
			GameObject gameObject = SortTypesGrid.transform.InstantiateAsChild(SortTypePrefab);
			gameObject.ChangeLayer(base.gameObject.layer);
			SortTypePopupEntry component = gameObject.GetComponent<SortTypePopupEntry>();
			component.Populate(item);
		}
		SortTypesGrid.Reposition();
		int num = (sortTypes.Count + SortTypesGrid.maxPerLine - 1) / SortTypesGrid.maxPerLine;
		Vector3 localPosition = mSortTypesGridPos;
		localPosition.y += (float)(num - 1) * SortTypesGrid.cellHeight;
		SortTypesGrid.transform.localPosition = localPosition;
		SortTypesBackground.height = (int)(TypesListBorder + (float)num * SortTypesGrid.cellHeight);
		SortTypesBackground.width = (int)(TypesListBorder + (float)SortTypesGrid.maxPerLine * SortTypesGrid.cellWidth);
		if (entry.transform.position.y >= 0f)
		{
			Vector3 localPosition2 = mSortTypesBGPos;
			localPosition2.y = 0f - localPosition2.y - (float)SortTypesBackground.height;
			SortTypesBackground.transform.localPosition = localPosition2;
		}
		else
		{
			SortTypesBackground.transform.localPosition = mSortTypesBGPos;
		}
	}

	private List<SortTypeEnum> GetSortTypes()
	{
		List<SortTypeEnum> list = new List<SortTypeEnum>();
		if (mCategory == Category.Creatures)
		{
			list.Add(SortTypeEnum.Newest);
			list.Add(SortTypeEnum.Alphabetical);
			list.Add(SortTypeEnum.Level);
			list.Add(SortTypeEnum.STR);
			list.Add(SortTypeEnum.INT);
			list.Add(SortTypeEnum.DEX);
			list.Add(SortTypeEnum.HP);
			list.Add(SortTypeEnum.TeamCost);
			list.Add(SortTypeEnum.Favorites);
			list.Add(SortTypeEnum.InUse);
			list.Add(SortTypeEnum.Faction);
			list.Add(SortTypeEnum.Rarity);
		}
		else if (mCategory == Category.Cards)
		{
			list.Add(SortTypeEnum.Newest);
			list.Add(SortTypeEnum.Alphabetical);
			list.Add(SortTypeEnum.InUse);
			list.Add(SortTypeEnum.Faction);
			list.Add(SortTypeEnum.CardCost);
		}
		else if (mCategory == Category.Helpers)
		{
			list.Add(SortTypeEnum.Level);
			list.Add(SortTypeEnum.STR);
			list.Add(SortTypeEnum.INT);
			list.Add(SortTypeEnum.DEX);
			list.Add(SortTypeEnum.HP);
			list.Add(SortTypeEnum.TeamCost);
			list.Add(SortTypeEnum.Faction);
			list.Add(SortTypeEnum.Rarity);
		}
		return list;
	}

	public void OnClickSortType(SortTypePopupEntry clickedEntry)
	{
		HideSortTypesTween.Play();
		List<SortEntry> list = null;
		if (mCategory == Category.Creatures)
		{
			list = Singleton<PlayerInfoScript>.Instance.SaveData.CreatureSorts;
		}
		else if (mCategory == Category.Cards)
		{
			list = Singleton<PlayerInfoScript>.Instance.SaveData.ExCardSorts;
		}
		else if (mCategory == Category.Helpers)
		{
			list = Singleton<PlayerInfoScript>.Instance.SaveData.HelperSorts;
		}
		int num = list.FindIndex((SortEntry m) => m.SortType == clickedEntry.SortType);
		if (num != -1)
		{
			if (mChangingSortEntry.Entry != null)
			{
				SortEntry sortEntry = list[num];
				SortTypeEnum sortType = sortEntry.SortType;
				bool reversed = sortEntry.Reversed;
				sortEntry.SortType = mChangingSortEntry.Entry.SortType;
				sortEntry.Reversed = mChangingSortEntry.Entry.Reversed;
				mChangingSortEntry.Entry.SortType = sortType;
				mChangingSortEntry.Entry.Reversed = reversed;
			}
		}
		else if (mChangingSortEntry.Entry != null)
		{
			mChangingSortEntry.Entry.SortType = clickedEntry.SortType;
			mChangingSortEntry.Entry.Reversed = false;
		}
		else
		{
			SortEntry item = new SortEntry(clickedEntry.SortType);
			list.Add(item);
		}
		mChangingSortEntry = null;
		ApplySorts();
	}

	public void OnClickDirection(SortPopupEntry clickedEntry)
	{
		if (mChangingSortEntry != null)
		{
			HideSortTypesTween.Play();
			mChangingSortEntry = null;
		}
		clickedEntry.Entry.Reversed = !clickedEntry.Entry.Reversed;
		ApplySorts();
	}

	public void OnClickDelete(SortPopupEntry clickedEntry)
	{
		if (mChangingSortEntry != null)
		{
			HideSortTypesTween.Play();
			mChangingSortEntry = null;
		}
		List<SortEntry> creatureSorts = Singleton<PlayerInfoScript>.Instance.SaveData.CreatureSorts;
		creatureSorts.Remove(clickedEntry.Entry);
		ApplySorts();
	}

	private void ApplySorts()
	{
		mSortedCallback();
		PopulateEntries();
	}

	public void OnClickClose()
	{
		if (mChangingSortEntry != null)
		{
			HideSortTypesTween.Play();
			mChangingSortEntry = null;
		}
		HideTween.Play();
	}

	public void OnClickToggle()
	{
		if (mToggledCategory == Category.Creatures)
		{
			mToggledCategory = Category.Cards;
		}
		else
		{
			mToggledCategory = Category.Creatures;
		}
		mCategory = mToggledCategory;
		UpdateToggleText();
		PopulateEntries();
	}
}
