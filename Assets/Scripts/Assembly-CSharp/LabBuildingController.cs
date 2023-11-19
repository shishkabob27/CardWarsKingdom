using UnityEngine;

public class LabBuildingController : Singleton<LabBuildingController>
{
	public UITweenController HideTween;
	public GameObject CreatureEnhanceLock;
	public Collider CreatureEnhanceCol;
	public GameObject CreatureEnhanceToggleScript;
	public GameObject CreatureEvoLock;
	public Collider CreatureEvoCol;
	public GameObject CreatureEvoToggleScript;
	public GameObject EvoStackParent;
	public UILabel EvoStackLabel;
	public UILabel TitleLabel;
	public GameObject AwakenTab;
	public string TabToJumpTo;
	public string PowerUpText;
	public string EnhanceText;
	public string AwakenText;
}
