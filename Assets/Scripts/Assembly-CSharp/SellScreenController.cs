using UnityEngine;

public class SellScreenController : Singleton<SellScreenController>
{
	public UITweenController SellItemShowTween;
	public UITweenController SellItemCoinsVFXTween;
	public UITweenController SellItemGemsVFXTween;
	public UITweenController InventoryFullTween;
	public UIStreamingGrid ItemGrid;
	public BoxCollider[] ContainerColliders;
	public UILabel SoftCurrencyLabel;
	public UILabel HardCurrencyLabel;
	public UILabel SoftSellPriceLabel;
	public UILabel HardSellPriceLabel;
	public GameObject SellGroup;
	public Transform SortButton;
	public UILabel InventorySlotsLabel;
	public InventoryBarController inventoryBar;
}
