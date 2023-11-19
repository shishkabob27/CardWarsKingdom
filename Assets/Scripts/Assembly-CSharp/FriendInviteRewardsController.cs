using System.Collections.Generic;
using UnityEngine;

public class FriendInviteRewardsController : MonoBehaviour
{
	public GameObject FriendInviteItemTemplate;

	private UIStreamingGridDataSource<InviteReward> mRewardsGridDataSource = new UIStreamingGridDataSource<InviteReward>();

	private List<InviteReward> mInviteRewards = new List<InviteReward>();

	public void ShowInviteRewards()
	{
		if (mInviteRewards.Count <= 0)
		{
			mInviteRewards = InviteRewardsManager.Instance.GetDatabase();
		}
		UIStreamingGrid component = base.gameObject.GetComponent<UIStreamingGrid>();
		mRewardsGridDataSource.Init(component, FriendInviteItemTemplate, mInviteRewards);
	}

	public void Clear()
	{
		mRewardsGridDataSource.Clear();
	}

	private void ClearList(UIStreamingGrid list)
	{
		if (list != null)
		{
			Transform transform = list.transform;
			for (int num = transform.childCount - 1; num >= 0; num--)
			{
				GameObject gameObject = transform.GetChild(num).gameObject;
				gameObject.SetActive(false);
				Object.Destroy(gameObject);
			}
		}
	}

	public void PopulateAllRewardItems(InviteReward reward = null)
	{
		FriendInviteRewardItem[] componentsInChildren = base.gameObject.GetComponentsInChildren<FriendInviteRewardItem>(true);
		FriendInviteRewardItem[] array = componentsInChildren;
		foreach (FriendInviteRewardItem friendInviteRewardItem in array)
		{
			friendInviteRewardItem.ShouldPlayFX = friendInviteRewardItem.Reward == reward;
			friendInviteRewardItem.Populate(null);
		}
	}
}
