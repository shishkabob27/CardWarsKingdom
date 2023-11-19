using UnityEngine;

public class MissionListController : Singleton<MissionListController>
{
	public GameObject MissionGridPrefab;
	public UITweenController ShowTween;
	public UITweenController ShowGlobalTween;
	public UITweenController ShowDailyTween;
	public UIPanel DailyPanel;
	public UIPanel DailyScrollPanel;
	public UIPanel GlobalPanel;
	public UIPanel GlobalScrollPanel;
	public UIStreamingGrid MissionGrid;
	public UILabel MissionTimeLeft;
	public GameObject NoMissionsLabel;
	public UIStreamingGrid GlobalMissionGrid;
	public GameObject ShowDailyCollider;
	public GameObject ShowGlobalCollider;
}
