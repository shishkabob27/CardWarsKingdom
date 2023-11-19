using UnityEngine;

public class ExpeditionListPrefab : UIStreamingGridListItem
{
	public UIProgressBar ProgressBar;
	public GameObject ContentsParent;
	public GameObject NotStartedParent;
	public GameObject InProgressParent;
	public GameObject BuySlotParent;
	public GameObject CompleteParent;
	public UILabel Name;
	public UILabel InProgress;
	public UILabel Time;
	public UILabel BuyCost;
	public UILabel CreatureCount;
	public UILabel FavoredClass;
	public UILabel Duration;
	public UILabel DifficultyLabel;
	public UISprite TopBarOutline;
	public UISprite PreferredClassBgRect;
	public UISprite Background;
	public UISprite FactionIcon;
	public UISprite FactionIconOutline;
	public UISprite TopBarFill;
	public UISprite MeterArrow;
	public UISprite[] MeterSegments;
	public int NormalWidth;
	public int SelectedWidth;
	public UITweenController SelectedTween;
	public UITweenController SelectedBgTween;
	public UITweenController ResetColorsTween;
	public UITweenController ResetBgColorsTween;
	public UITweenController UnlockTween;
	public UITweenController CompleteTween;
	public UITweener TimerTickTween;
	public TweenColor SelectedTweenColor;
	public TweenColor SelectedBgTweenColor;
	public TweenColor ResetBgTweenColor;
}
