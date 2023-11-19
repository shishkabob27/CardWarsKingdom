using UnityEngine;

public class BattleResultsLevelUpController : Singleton<BattleResultsLevelUpController>
{
	public GameObject MainPanel;
	public UITweenController ShowTween;
	public UITweenController HideTween;
	public UILabel Level;
	public UIGrid NumericalsParent;
	public GameObject StaminaGroup;
	public UILabel StaminaLabel;
	public GameObject InventorySpaceGroup;
	public UILabel InventorySpaceLabel;
	public ParticleSystem mRays;
}
