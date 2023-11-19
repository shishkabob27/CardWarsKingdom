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
	public BoxCollider HelperSlot;
	public GameObject CreatureInfoGroup;
	public UILabel HelperCreatureName;
	public UISprite[] HelperCreatureRarityStars;
	public UIGrid RarityGrid;
	public Transform SortButton;
	public Transform CreatureSpawnPoint;
	public GameObject CreaturePedestal;
	public GameObject HelperSlotSprite;
	public InventoryBarController inventoryBar;
}
