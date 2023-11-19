using UnityEngine;

public class ExpeditionRewardsController : Singleton<ExpeditionRewardsController>
{
	public float RankXPFillTime;
	public GameObject RewardLinePrefab;
	public UITexture ArtBG;
	public UIGrid RewardsGrid;
	public UISprite RankBar;
	public UILabel RankLevel;
	public UITweenController ShowTween;
	public UITweenController ShowResultTween;
	public UITweenController ShowCloseButtonTween;
	public UITweenController ShowXPBarTween;
}
