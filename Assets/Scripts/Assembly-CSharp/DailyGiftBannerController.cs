using UnityEngine;

public class DailyGiftBannerController : Singleton<DailyGiftBannerController>
{
	public UITweenController ShowBanner;
	public UITweenController HideBanner;
	public UILabel RewardLabel;
	public UISprite RewardIcon;
	public Transform RewardSlotNode;
	public UITexture CardBackTexture;
}
