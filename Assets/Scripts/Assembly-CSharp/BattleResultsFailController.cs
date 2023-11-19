using UnityEngine;

public class BattleResultsFailController : Singleton<BattleResultsFailController>
{
	public UITweenController ShowTween;
	public UITweenController ShowTipTween;
	public UITweenController ShowSecondaryTween;
	public UITweenController HideTween;
	public UITweenController MultiplayerShowTween;
	public UILabel AvailableStamina;
	public UILabel RetryStaminaCost;
	public UILabel AvailableHardCurrency;
	public UILabel HardCurrencyCost;
	public UILabel RetryMessage;
	public UILabel TipLabel;
	public UIGrid DropsGrid;
	public UIGrid RewardsGrid;
	public UILabel RewardSoftCurrency;
	public UILabel RewardSocialCurrency;
	public UILabel RewardHardCurrency;
	public GameObject TipPanel;
}
