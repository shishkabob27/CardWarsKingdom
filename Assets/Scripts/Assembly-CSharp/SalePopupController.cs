using UnityEngine;

public class SalePopupController : Singleton<SalePopupController>
{
	public GameObject MainPanel1;
	public GameObject MainPanel2;
	public GameObject ItemListEntry;
	public GameObject GrantedItemListEntry;
	public UITweenController ShowTween;
	public UITweenController HideTween;
	public UITweenController ShowGrantedTween;
	public UITweenController HideGrantedTween;
	public UITweenController GrantedTapToContinueTween;
	public UILabel SaleName;
	public UILabel SaleDesc;
	public UILabel Price;
	public UILabel PackTitleLabel;
	public UILabel BadgeLabel;
	public UIGrid ItemGrid;
	public UIScrollView ListScrollview;
	public UILabel TimeLeft;
	public UITexture BackgroundTexture;
	public UITexture BadgeTexture;
	public UIGrid GrantedItemGrid;
	public UIScrollView GrantedListScrollview;
	public float GrantRewardDelay;
}
