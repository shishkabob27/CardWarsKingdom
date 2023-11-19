using UnityEngine;

public class PVPPrepScreenController : Singleton<PVPPrepScreenController>
{
	public float CountdownTime;
	public UILabel HardCurrencyLabel;
	public UILabel SoftCurrencyLabel;
	public UILabel PlayerStamina;
	public UILabel PlayerStaminaTimer;
	public UILabel EnergyCost;
	public LeagueBadge PlayerBadge;
	public LeagueBadge OpponentBadge;
	public UILabel Countdown;
	public GameObject ReadyBlockingCollider;
	public UILabel LevelSetNotificationLabel;
	public UITexture[] PIPIconTargets;
	public Transform[] MyCreatureNodes;
	public UILabel MyPlayerNameLabel;
	public UILabel TeamCost;
	public UILabel TeamName;
	public UILabel TitleLabel;
	public UILabel LeaderName;
	public GameObject CancelButton;
	public GameObject BackButton;
	public GameObject PressToSearchLabel;
	public GameObject HeartButtonDecoration;
	public GameObject CheckmarkButtonDecoration;
	public UITexture BackgroundTexture;
	public bool matchFound;
	public float syncTime;
	public ParticleSystem CountdownVFX;
	public GameObject SwordShieldAnimation;
	public UITweenController ShowTween;
	public UITweenController ShowAllyTween;
	public UITweenController HideTween;
	public UITweenController ExceededTeamCostTween;
	public UITweenController ConsumeTicketTween;
	public UITweenController ShowConnectingTween;
	public UITweenController HideConnectingTween;
	public UITweenController OpponentFoundTween;
	public UITweenController ShowFlagsAndCounterTween;
	public UITweenController FriendMatchStartingTween;
	public UITweenController CountdownPulseTween;
	public UITweenController ReadyTween;
}
