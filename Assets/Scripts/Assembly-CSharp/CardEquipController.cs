using UnityEngine;

public class CardEquipController : Singleton<CardEquipController>
{
	public UITweenController ShowTween;
	public UITweenController ShowPassiveTween;
	public UITweenController HidePassiveTween;
	public GameObject MainPanel;
	public CreatureStatsPanel StatsPanel;
	public UIStreamingGrid CardGrid;
	public UILabel CreatureName;
	public Transform[] EquippedCardSlots;
	public GameObject[] ExtraCardSlotLocks;
	public UILabel SoftCurrencyLabel;
	public UILabel HardCurrencyLabel;
	public Transform SortButton;
	public Transform[] NativeCardSlots;
	public InventoryTile[] mEquippedCardTiles;
	public Transform ZoomPosition;
	public Collider ZoomCollider;
	public float CardScaleInGrid;
	public InventoryBarController inventoryBar;
	public GameObject CardParentToHide;
}
