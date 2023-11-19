using UnityEngine;

public class PVPSendMatchRequestController : Singleton<PVPSendMatchRequestController>
{
	public UITweenController ShowWaitingForResponseTween;
	public UITweenController HideWaitingForResponseTween;
	public GameObject RequestCancelButton;
}
