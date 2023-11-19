using UnityEngine;

public class PvpBattleResultsController : Singleton<PvpBattleResultsController>
{
	public float BarFillSpeed;
	public float XpFillTime;
	public UILabel RankLabel;
	public UILabel RankChangeLabel;
	public UILabel PointsLabel;
	public UILabel PointsToAdvanceLabel;
	public UISprite RankProgressBar;
	public UISprite XpBar;
	public UILabel XpAmount;
	public UILabel XpLevel;
	public UILabel XpEarned;
	public LeagueBadge LeagueBadge;
	public LeagueBadge RankUpLeagueBadge;
	public UILabel RankChangeRewardsLabel;
	public UILabel RankChangeNoRewardsLabel;
	public UIGrid RankUpRewardsGrid;
	public UIScrollView RewardsScrollView;
	public UIGrid WinRewardsGrid;
	public GameObject RewardPrefab;
	[SerializeField]
	private GameObject _VictoryBanner;
	[SerializeField]
	private GameObject _FailedBanner;
	[SerializeField]
	private UISprite[] _Stripes;
	[SerializeField]
	private Color[] _StripesColors;
	[SerializeField]
	private UILabel _LeaguePlaceLabel;
	[SerializeField]
	private Color[] _LeaguePlaceLabelOutlineColors;
	[SerializeField]
	private UISprite _LeagueFrameFill;
	[SerializeField]
	private Color[] _LeagueFrameFillColors;
	[SerializeField]
	private UISprite[] _LeagueFrameDetails;
	[SerializeField]
	private Color[] _LeagueFrameDetailsColors;
	[SerializeField]
	private UILabel _RewardsLabel;
	[SerializeField]
	private UITexture _GodRaysTexture;
	[SerializeField]
	private Color[] _GodRaysColors;
	[SerializeField]
	private UISprite[] _FillBarOutlines;
	[SerializeField]
	private UISprite[] _FillBarFills;
	public UITweenController ResetTween;
	public UITweenController ShowTween;
	public UITweenController ShowWinRewardsTween;
	public UITweenController ShowBannerTween;
	public UITweenController ShowPlaqueTween;
	public UITweenController ShowContentBgTween;
	public UITweenController ShowGodRaysTween;
	public UITweenController ShowSeasonProgressTween;
	public UITweenController ShowLeagueBadgeTween;
	public UITweenController HideSeasonProgress;
	public UITweenController RankChangeTween;
	public UITweenController ShowTapToContinueTween;
	public UITweenController ShowButtonsTween;
	public UITweenController ShowXpBarTween;
	public UITweenController ShrinkXpTween;
	public UITweenController PromotionMatchTween;
	public UITweenController WinStreakTween;
	[SerializeField]
	private AnimationCurve _TweenInRewardsCurve;
}
