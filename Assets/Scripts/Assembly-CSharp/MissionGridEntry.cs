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
			SendKPIData();
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

	private void SendKPIData()
	{
		string upsightEvent = string.Empty;
		string iD = mMission.Data.ID;
		string value = mMission.Data.Type.ToString();
		string value2 = string.Empty;
		string value3 = string.Empty;
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("missionID", iD);
		dictionary.Add("type", value);
		if (mMission.Data.SoftCurrency > 0)
		{
			upsightEvent = "Economy.CoinEnter.Mission";
			value2 = mMission.Data.SoftCurrency.ToString();
			value3 = "Coin";
			dictionary.Add("amount", value2);
		}
		else if (mMission.Data.SocialCurrency > 0)
		{
			upsightEvent = "Economy.WishboneEnter.Mission";
			value3 = "Wishbone";
			value2 = mMission.Data.SocialCurrency.ToString();
			dictionary.Add("amount", value2);
		}
		else if (mMission.Data.HardCurrency > 0)
		{
			upsightEvent = "Economy.GemEnter.Mission";
			value2 = mMission.Data.HardCurrency.ToString();
			value3 = "Gem";
			dictionary.Add("amount", value2);
		}
		upsightEvent = ((mMission.Data.Type != MissionType.Global) ? "Misssion.Daily" : "Misssion.Global");
		dictionary.Clear();
		dictionary.Add("missionID", iD);
		dictionary.Add("rewardQuantity", value2);
		dictionary.Add("rewardType", value3);
		dictionary.Add("status", "Collected");
	}

	private void OnCollectTweenComplete()
	{
		Singleton<TownHudController>.Instance.ClaimCompletedMission(mMission);
		Singleton<SLOTAudioManager>.Instance.PlaySound("ui/SFX_MissionComplete");
	}
}
