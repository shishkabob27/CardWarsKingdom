using UnityEngine;

public class SeasonLeagueRewardsTile : UIStreamingGridListItem
{
	public UIGrid ItemGrid;
	public LeagueBadge LeagueBadge;
	public UIButton PrevButton;
	public UIButton NextButton;
	public GameObject ItemTemplate;
	public GameObject SelectedVFX;
	public UITweenController ShowTween;
	public UITweenController SelectedStateTween;
	public UITweenController NormalStateTween;
	public TweenWidth TweenWidth;
}
