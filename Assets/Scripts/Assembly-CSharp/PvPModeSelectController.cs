using UnityEngine;

public class PvPModeSelectController : Singleton<PvPModeSelectController>
{
	public GameObject InitTab;
	public UILabel NameLabel;
	public UILabel RankNumberLabel;
	public UILabel CurrentLeagueLabel;
	public LeagueBadge Badge;
	public UILabel SeasonLabel;
	public UILabel SeasonName;
	public UITexture SeasonBanner;
	public SeasonProgressBar SeasonProgressBar;
	public GameObject RankedParent;
	public GameObject NotRankedParent;
	public GameObject PromotionMatchLabel;
	public UIStreamingGrid LeaderboardGrid;
	public GameObject LeaderboardListPrefab;
	public SeasonLabelColorPallette[] SeasonLabelColors;
	public UITweenController ShowTween;
	public UITweenController HideTween;
	public UITweenController PromotionMatchTween;
	public UITweenController FetchingLeaderboardsTween;
	public UITweenController HideFetchingLeaderboardsTween;
	public UITweenController HideElementsTween;
	public UITweenController ShowElementsTween;
}
