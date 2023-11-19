using UnityEngine;

public class QuestSelectController : Singleton<QuestSelectController>
{
	public float QuestListOffset;
	public GameObject LeaguePrefab;
	public GameObject SpecialLeaguePrefab;
	public GameObject QuestPrefab;
	public GameObject RandomBattlePrefab;
	public UITweenController ShowTween;
	public UITweenController ShowQuestsTween;
	public UITweenController HideQuestsTween;
	public UILabel TitleLabel;
	public UILabel PlayerStamina;
	public UILabel PlayerStaminaTimer;
	public UIStreamingGrid LeagueGrid;
	public UIScrollView LeagueScrollView;
	public UIStreamingGrid QuestGrid;
	public UIScrollView QuestScrollView;
	public GameObject BackButton;
	public GameObject BackgroundMain;
	public GameObject BackgroundSpecial;
	public GameObject ScrollBar;
	public string DailyDungeonLeagueID;
}
