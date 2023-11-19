using UnityEngine;

public class EvoStatsPanelController : Singleton<EvoStatsPanelController>
{
	public UILabel CreatureName;
	public UILabel CreatureLevel;
	public UILabel CreatureLevelBefore;
	public UILabel CreatureMaxLevel;
	public UILabel CreatureMaxLevelBefore;
	public UILabel[] GemNames;
	public UILabel[] GemDescriptions;
	public UILabel[] StatsBefore;
	public UILabel[] StatsAfter;
	public UILabel PassiveSkill;
	public UILabel PassiveLevel;
	public UILabel PassiveSkillBefore;
	public UILabel PassiveLevelBefore;
	public Transform SkillCardNodeParent;
	public Transform ZoomPosition;
}
