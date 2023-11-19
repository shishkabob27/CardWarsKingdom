using UnityEngine;

public class TownHudController : Singleton<TownHudController>
{
	public UITweenController MissionCompleteTween;
	public UITweenController BuildingLockedTween;
	public UILabel PlayerName;
	public UILabel PlayerLevel;
	public UITexture PlayerPortrait;
	public UILabel PlayerXpPercent;
	public UISprite PlayerXpBar;
	public UILabel PlayerStamina;
	public UILabel PlayerStaminaTimer;
	public UILabel PlayerPvpStamina;
	public UILabel PlayerPvpStaminaTimer;
	public UILabel PlayerHardCurrency;
	public UILabel PlayerSoftCurrency;
	public UILabel PlayerPvPCurrency;
	public GameObject PvpStaminaObject;
	public GameObject SoftCurrencyObject;
	public GameObject HardCurrencyObject;
	public GameObject PvpCurrencyObject;
	public UIGrid CurrenciesGrid;
	public GameObject MissionStackObject;
	public UILabel MissionStackCount;
	public UILabel MissionTimerLabel;
	public GameObject GachaButtonsShowHide;
	public GameObject DungeonButtonsShowHide;
	public GameObject MissionsButton;
	public GameObject ExpeditionsButton;
	public UILabel ExpeditionStackValue;
	public UILabel ExpeditionTimerLabel;
	public UILabel DailyChestTimerLabel;
	public UILabel DailyDungeonNameLabel;
	public UILabel DailyDungeonTimerLabel;
	public UITexture DailyDungeonIcon;
	public GameObject BuildingUnlockPanel;
	public UITweenController ShowHUD;
	public UITweenController HideHUD;
	public UITweenController ShowBuildingUnlockBanner;
	public UITweenController ScribbleStartTween;
	public UILabel BuildingUnlockLabel;
	public UILabel BuildingLockedLabel;
	public UISprite[] DeepLinkButtonBgs;
	public UITexture LeaderTex;
	public GameObject promoBannerObject;
	public GameObject UIBar;
}
