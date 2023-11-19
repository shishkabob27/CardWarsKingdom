using UnityEngine;

public class QuickMessageController : Singleton<QuickMessageController>
{
	public float Cooldown;
	public GameObject ChatListPrefab;
	public UITweenController ShowButtonTween;
	public UITweenController HideButtonTween;
	public UITweenController ShowPanelTween;
	public UITweenController HidePanelTween;
	public UIStreamingGrid ChatGrid;
}
