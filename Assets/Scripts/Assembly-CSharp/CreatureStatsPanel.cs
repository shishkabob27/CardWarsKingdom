using UnityEngine;

public class CreatureStatsPanel : MonoBehaviour
{
	public UILabel InfoName;
	public UILabel InfoDetails;
	public UISprite InfoFactionIcon;
	public Transform InfoRarityStarsParent;
	public Transform[] InfoRarityStars;
	public UILabel InfoXPCount;
	public UILabel InfoXPPercent;
	public UISprite InfoXPBar;
	public UILabel InfoLevel;
	public UILabel InfoLevelNumber;
	public UILabel[] InfoStats;
	public UILabel InfoType;
	public UILabel InfoPassiveLevel;
	public UILabel InfoPassiveSkill;
	public Transform PortraitSpawnNode;
	public UILabel TeamCost;
	public UILabel AttackCost;
	public GameObject MaxLevelToggle;
	public UILabel MaxOrCurrentLabel;
	public bool ShouldTweenInRarityStars;
	public AnimationCurve TweenInStarCurve;
	public UITweenController[] PulseStarTweens;
	public UITweenController[] UnPulseStarTweens;
}
