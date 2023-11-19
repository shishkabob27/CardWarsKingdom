using UnityEngine;

public class CreatureDetailsController : Singleton<CreatureDetailsController>
{
	public int NoGemsPanelShrinkage;
	public UITweenController ShowTween;
	public UITweenController HideTween;
	public UITweenController ShowCardsTween;
	public UITweenController ShowCreatureTween;
	public UITweenController HideCreatureTween;
	public UITweenController ShowPassiveTween;
	public UITweenController HidePassiveTween;
	public GameObject MainPanel;
	public CreatureStatsPanel StatsPanel;
	public Transform MainModelParent;
	public Transform SpawnNodeScaler;
	public UIStreamingGrid MainCreatureGrid;
	public Transform SkillCardNodeParent;
	public Collider ZoomCollider;
	public Transform ZoomPosition;
	public GameObject GemsParent;
	public Transform StatsPanelTransform;
	public Transform StatsButtonTransform;
	public Transform WeightButtonTransform;
	public UISprite StatsPanelBackground;
	public GameObject FavoriteParent;
	public GameObject FavoriteCheckmark;
	public GameObject ExCardsParent;
	public Camera CreatureCamera;
	public Transform[] ExtraCardSlots;
	public GameObject[] ExtraCardSlotLocks;
}
