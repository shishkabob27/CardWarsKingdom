using UnityEngine;

public class SeasonRewardsScreen : Singleton<SeasonRewardsScreen>
{
	public GameObject SeasonRewardsPrefab;
	public UITweenController ShowTween;
	public UITweenController HideTween;
	public UIScrollView ScrollView;
	public UIStreamingGrid LeaguesGrid;
}
