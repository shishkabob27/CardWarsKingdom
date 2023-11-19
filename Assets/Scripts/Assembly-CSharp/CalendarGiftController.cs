using UnityEngine;

public class CalendarGiftController : Singleton<CalendarGiftController>
{
	public GameObject MainPanel;
	public GameObject DayTemplatePrefab;
	public UITweenController RewardFXShow;
	public UITweenController RewardFXHide;
	public UITweenController CalendarShow;
	public UITweenController CalendarHide;
	public UITweenController MonthRewardClaim;
	public UISprite MonthRewardIcon;
	public UITexture MonthRewardTexture;
	public UILabel MonthRewardAmount;
	public UILabel FinaleLabel;
	public UILabel TitleLabel;
	public GameObject CalendarButtonClose;
	public GameObject EffectsObject;
	public Transform DayNodesParent;
	public UITexture Background;
}
