using UnityEngine;

public class PreMatchController : Singleton<PreMatchController>
{
	public Color[] TutorialTeamColors;
	public UITweenController ShowTween;
	public UITweenController ExceededTeamCostTween;
	public UITweenController ConsumeTicketTween;
	public UITweenController ShowHelperBeingDraggedTween;
	public UITweenController HideHelperBeingDraggedTween;
	public UITweenController BattleStartTween;
	public UITweenController PulseTutorialButtonsTween;
	public UITweenController FadeInHelperButtonTween;
	public UITweenController FadeOutHelperButtonTween;
	public UITexture[] PIPIconTargets;
	public UITexture[] PlayerBackgroundTextures;
	public Transform[] MyCreatureNodes;
	public Transform[] EnemyCreatureNodes;
	public UILabel StaminaCost;
	public UILabel PlayerStamina;
	public UILabel PlayerStaminaTimer;
	public GameObject EditDeckButton;
	public UILabel QuestName;
	public UISprite IconVS;
	public UILabel TeamCost;
	public UILabel TeamName;
	public UILabel[] LeaderNames;
	public GameObject AddHelperButton;
	public GameObject RemoveHelperButton;
	public GameObject HelperBlockingSlot;
	public GameObject StandardQuestParent;
	public GameObject RandomQuestParent;
	public UILabel RandomQuestFloor;
	public UILabel RandomQuestReward;
	public UILabel RandomQuestBattles;
	public GameObject StartButton;
	public GameObject RequiredCreatureParent;
	public GameObject RequiredCreatureSpawnNode;
	public GameObject CloseButton;
	public UILabel TeamDescription;
	public UISprite TeamDescriptionBox;
	public BoxCollider HelperSlot;
	public bool mDeclineSlotPurchase;
}
