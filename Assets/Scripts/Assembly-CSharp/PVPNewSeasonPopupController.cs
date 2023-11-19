using UnityEngine;

public class PVPNewSeasonPopupController : Singleton<PVPNewSeasonPopupController>
{
	public GameObject RewardPrefab;
	public UITweenController ShowTween;
	public UITweenController ShowWithRewardsTween;
	public UILabel DurationLabel;
	public UILabel RewardsDurationLabel;
	public UILabel RewardsPlacementLabel;
	public GameObject RewardsTitleLabel;
	public UILabel RewardsSoftCurrencyLabel;
	public UILabel RewardsSocialCurrencyLabel;
	public UILabel RewardsHardCurrencyLabel;
	public UIGrid RewardsLootGrid;
	public UITable RewardsTable;
	public Transform RewardsTableBottom;
	public Transform RewardsContents;
}
