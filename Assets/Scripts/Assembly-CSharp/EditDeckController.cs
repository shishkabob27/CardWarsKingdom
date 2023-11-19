using UnityEngine;

public class EditDeckController : Singleton<EditDeckController>
{
	public UITweenController ShowTween;
	public UITweenController ExceededTeamCostTween;
	public UITweenController UnlockLeaderTween;
	public UITweenController CardLayoutUnlockLeaderTween;
	public UITweenController PulseTutorialButtonsTween;
	public UITexture LeaderImage;
	public GameObject[] CreatureSlots;
	public UIStreamingGrid CreatureGrid;
	public UILabel LeaderSkillName;
	public UILabel LeaderSkillDesc;
	public UILabel LeaderName;
	public UILabel TeamName;
	public UILabel TeamCost;
	public Transform SortButton;
	public UILabel LeaderCost;
	public GameObject LeaderCostObject;
	public UILabel HardCurrencyLabel;
	public GameObject CreatureLayoutParent;
	public GameObject CreatureLayoutLeaderSelectButtons;
	public GameObject CustomizeButton;
	public GameObject CardLayoutParent;
	public GameObject CardLayoutCreatureSlotsParent;
	public GameObject CardLayoutCreatureCardsParent;
	public UITexture CardLayoutLeaderImage;
	public GameObject CardLayoutLeaderSelectButtons;
	public GameObject CardLayoutLeaderCostObject;
	public UILabel CardLayoutLeaderCost;
	public UILabel CardLayoutLeaderName;
	public InventoryBarController inventoryBar;
	public GameObject ButtonTeamNext;
	public GameObject ButtonTeamPrev;
}
