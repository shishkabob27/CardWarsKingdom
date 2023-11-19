using UnityEngine;

public class FuseStatsPanelController : Singleton<FuseStatsPanelController>
{
	public UITweenController ShowTween;
	public UITweenController HideTween;
	public UITweenController ShowAfterTween;
	public UITweenController HideAfterTween;
	public UITweenController ShowAfterPassiveTween;
	public UILabel CreatureName;
	public UISprite[] RarityStars;
	public UISprite FactionIcon;
	public UISprite ExpBarFill;
	public UILabel ExpPct;
	public UILabel ExpInLevel;
	public UILabel ExpReceived;
	public UILabel CreatureLevel;
	public UILabel CreatureLevelBefore;
	public UILabel[] GemNames;
	public UILabel[] GemDescriptions;
	public UILabel DEF;
	public UILabel DEX;
	public UILabel HP;
	public UILabel INT;
	public UILabel RES;
	public UILabel STR;
	public UILabel PassiveSkill;
	public UILabel PassiveLevel;
	public UILabel PassiveSkillScale;
	public UILabel PassiveLevelScale;
	public UILabel DEFBefore;
	public UILabel DEXBefore;
	public UILabel HPBefore;
	public UILabel INTBefore;
	public UILabel RESBefore;
	public UILabel STRBefore;
	public UILabel PassiveSkillBefore;
	public UILabel PassiveLevelBefore;
	public GameObject PassiveLevelUpArrow;
	public int EarnedXP;
	public float ExpTickTime;
}
