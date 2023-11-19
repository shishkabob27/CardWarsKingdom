using UnityEngine;

public class SimplePopupController : Singleton<SimplePopupController>
{
	public UITweenController ShowTween;
	public UITweenController ShowNameEntryTween;
	public UITweenController ShowDateEntryTween;
	public UITweenController HideTween;
	public GameObject MainPanel;
	public UILabel Title;
	public UILabel Body;
	public UILabel InputLabel;
	public UILabel YesButtonLabel;
	public UILabel NoButtonLabel;
	public UILabel OkButtonLabel;
	public Transform YesButton;
	public Transform NoButton;
	public Transform OkButton;
	public Transform Input;
	public UIInput InputObject;
	public GameObject FacebookLoginButtons;
	public GameObject FacebookLoginIcon;
	public DateEntryItem MonthEntry;
	public DateEntryItem DayEntry;
	public DateEntryItem YearEntry;
	public Transform InventoryGroup;
	public UIGrid InventoryGrid;
	public UILabel InventoryTitle;
	public UILabel InventoryBody;
}
