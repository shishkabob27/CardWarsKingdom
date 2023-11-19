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
	public float CardScaleInGrid;
	public UIStreamingGrid InventoryGrid;
	public UIStreamingGrid CollectionGrid;
	public string InventoryTitleText;
	public string CreaturesTitleText;
	public string GemsTitleText;
	public string RunesTitleText;
	public string CardsTitleText;
}
