using UnityEngine;

public class SortPopupController : Singleton<SortPopupController>
{
	public int MaxCreatureSorts;
	public int MaxCardSorts;
	public float ListBorder;
	public float TypesListBorder;
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
}
