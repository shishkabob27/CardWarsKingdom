using System.Collections.Generic;
using UnityEngine;

public class MissionGridEntry : UIStreamingGridListItem
{
	public UITweenController CollectTween;

	public UITweenController GlobalCollectTween;

	public UILabel Name;

	public UILabel Description;

	public UILabel ProgressLabel;

	public UILabel RewardAmount;

	public UISprite RewardIcon;

	public UISprite RewardIconShadow;

	public UISprite ProgressSprite;

	public GameObject CompletedMark;

	public GameObject CompletedGlow;

	public GameObject ClaimedGroup;

	public GameObject RewardGroup;

	private Mission mMission;

	public override void Populate(object dataObj)
	{
		mMission = dataObj as Mission;
		Name.text = mMission.Data.Name;
		Description.text = mMission.Data.Description;
		int num = mMission.ProgressValue;
		if (num > mMission.Data.Val1)
		{
			num = mMission.Data.Val1;
		}
		ProgressLabel.text = num + " / " + mMission.Data.Val1;
		ProgressSprite.fillAmount = mMission.ProgressPct;
		if (mMission.Claimed)
		{
			ClaimedGroup.SetActive(true);
			RewardGroup.SetActive(false);
			CompletedMark.SetActive(false);
			CompletedGlow.SetActive(false);
			return;
		}
		ClaimedGroup.SetActive(false);
		RewardGroup.SetActive(true);
		CompletedMark.SetActive(mMission.Completed);
		CompletedGlow.SetActive(mMission.Completed);
		int rewardAmount;
		string rewardIcon;
		mMission.Data.GetRewardInfo(out rewardAmount, out rewardIcon);
		RewardAmount.text = rewardAmount.ToString();
		RewardIcon.spriteName = rewardIcon;
		RewardIconShadow.spriteName = rewardIcon;
	}

	public void OnClicked()
	{
		if (mMission.Completed && !mMission.Claimed)
		{
			if (mMission.Data.Type == MissionType.Global)
			{
				GlobalCollectTween.PlayWithCallback(OnCollectTweenComplete);
			}
			else
			{
				CollectTween.PlayWithCallback(OnCollectTweenComplete);
			}
		}
	}

	private void OnCollectTweenComplete()
	{
		Singleton<TownHudController>.Instance.ClaimCompletedMission(mMission);
		Singleton<SLOTAudioManager>.Instance.PlaySound("SFX_MissionComplete");
	}
}
