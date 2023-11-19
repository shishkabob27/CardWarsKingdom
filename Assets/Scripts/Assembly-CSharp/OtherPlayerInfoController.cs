using UnityEngine;

public class OtherPlayerInfoController : Singleton<OtherPlayerInfoController>
{
	public UITweenController CloseTween;
	public UILabel PlayerName;
	public UILabel PlayerLevel;
	public LeagueBadge PlayerLeagueBadge;
	public UITexture Flag;
	public UIWidget InviteButton;
	public UILabel SentInvite;
	public UIWidget AlreadyAlly;
	public GameObject DuelButton;
	public GameObject BlockButton;
	public UITweenController ShowTween;
}
