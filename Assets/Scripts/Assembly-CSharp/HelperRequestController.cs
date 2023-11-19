using UnityEngine;

public class HelperRequestController : Singleton<HelperRequestController>
{
	public GameObject HelperTilePrefab;
	public Transform HelperTileParent;
	public UITweenController ShowTween;
	public UITweenController HideTween;
	public UILabel Title;
	public UILabel Body;
	public Transform YesButton;
	public Transform NoButton;
	public Transform OkButton;
	public Transform CloseButton;
	public UILabel YesButtonLabel;
	public UILabel NoButtonLabel;
	public UILabel OkButtonLabel;
	public bool ShouldSendAllyInvite;
}
