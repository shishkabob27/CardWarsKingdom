using UnityEngine;

public class MissionGridEntry : UIStreamingGridListItem
{
	public UITweenController CollectTween;
	public UITweenController GlobalCollectTween;
	public UILabel Name;
	public UILabel Description;
	public UILabel ProgressLabel;
	public UILabel RewardAmount;
	public UISprite RewardIcon;
	public UISprite RewardIconShadow;
	public UISprite ProgressSprite;
	public GameObject CompletedMark;
	public GameObject CompletedGlow;
	public GameObject ClaimedGroup;
	public GameObject RewardGroup;
}
