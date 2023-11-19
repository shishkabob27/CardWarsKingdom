using UnityEngine;

public class SocialController : Singleton<SocialController>
{
	public UILabel TitleLabel;
	public UIToggle InviteFriendsToggle;
	public UIToggle MailToggle;
	public UIToggle AllyBoxToggle;
	public UIToggle SetMyHelperToggle;
	public GameObject MailStackObject;
	public UILabel MailStackLabel;
	public GameObject FBInviteRewardVFX;
	public UITweenController ShowTween;
	public UITweenController HideTween;
	public FriendInviteRewardsController InviteRewards;
	public FriendList Friends;
	public GameObject FacebookNotConnectGroup;
	public GameObject FacebookConnectGroup;
	public UIGrid ButtonsGrid;
	public GameObject InviteRewardsButton;
	public Transform SendParent;
	public GameObject RedeemParent;
	public Transform RedeemCreatureTileNode;
	public UILabel InviteCodeLabel;
	public string HelperTitleText;
	public string AlliesTitleText;
	public string MailTitleText;
	public string GiftCodesTitleText;
}
