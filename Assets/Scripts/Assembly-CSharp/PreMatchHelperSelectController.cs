using UnityEngine;

public class PreMatchHelperSelectController : Singleton<PreMatchHelperSelectController>
{
	public GameObject HelperTilePrefab;
	public UITweenController ShowTween;
	public UITweenController HideTween;
	public Transform SortButton;
	public UIStreamingGrid HelperGrid;
	public InventoryTile SelectedHelperCreatureTile;
	public HelperPrefabScript AssignedHelper;
	public bool IsHelperListDisplayed;
}
