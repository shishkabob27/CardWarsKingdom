using UnityEngine;

public class QuestSelectQuest : UIStreamingGridListItem
{
	public UITweenController UnlockTween;
	public UITweenController ResetTween;
	public UILabel Name;
	public UILabel Index;
	public UILabel Stamina;
	public UILabel MapCount;
	public UITexture OpponentPortrait;
	public Transform CreatureNodeParent;
	public GameObject Lock;
	public GameObject HideWhenLocked;
	public Transform LeaderPortraitParent;
	public GameObject BossBanner;
	public GameObject NewLabel;
	public GameObject[] Stars;
	public UILabel RandomRewardType;
	public float DelayToShowUnlockedElements;
}
