using UnityEngine;

public class FriendInviteRewardItem : UIStreamingGridListItem
{
	public UILabel Description;

	public UILabel InvitesRequired;

	public UILabel RewardQunatity;

	public UISprite Icon;

	public UISprite Fill;

	public Transform RewardCompleteMarkGroup;

	private InviteReward mReward;

	public bool ShouldPlayFX;

	public InviteReward Reward
	{
		get
		{
			return mReward;
		}
	}

	public override void Populate(object dataObj)
	{
		mReward = ((dataObj == null) ? mReward : (dataObj as InviteReward));
		if (mReward == null)
		{
			return;
		}
		int friendInvites = Singleton<PlayerInfoScript>.Instance.SaveData.FriendInvites;
		if (friendInvites < mReward.InvitesRequired)
		{
			InvitesRequired.text = friendInvites + "/" + mReward.InvitesRequired;
			Fill.fillAmount = (float)friendInvites / (float)mReward.InvitesRequired;
		}
		else
		{
			Fill.fillAmount = 1f;
			InvitesRequired.text = KFFLocalization.Get("!!CLAIMED");
			if (ShouldPlayFX)
			{
				base.transform.InstantiateAsChild(Singleton<SocialController>.Instance.FBInviteRewardVFX);
			}
		}
		RewardCompleteMarkGroup.gameObject.SetActive(friendInvites >= mReward.InvitesRequired);
		Description.text = KFFLocalization.Get(mReward.Name);
		RewardQunatity.text = "x" + mReward.RewardQuantity;
		Icon.spriteName = mReward.Icon;
	}
}
