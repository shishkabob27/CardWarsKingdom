using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PVPNewSeasonPopupController : Singleton<PVPNewSeasonPopupController>
{
	public GameObject RewardPrefab;

	public UITweenController ShowTween;

	public UITweenController ShowWithRewardsTween;

	public UILabel DurationLabel;

	public UILabel RewardsDurationLabel;

	public UILabel RewardsPlacementLabel;

	public GameObject RewardsTitleLabel;

	public UILabel RewardsSoftCurrencyLabel;

	public UILabel RewardsSocialCurrencyLabel;

	public UILabel RewardsHardCurrencyLabel;

	public UIGrid RewardsLootGrid;

	public UITable RewardsTable;

	public Transform RewardsTableBottom;

	public Transform RewardsContents;

	private float mNormalRewardsBottomPos;

	private bool mWaitingForPopup;

	private string mPlacementTextColorString;

	private void Awake()
	{
		mNormalRewardsBottomPos = RewardsTableBottom.localPosition.y;
		mPlacementTextColorString = "[" + RewardsPlacementLabel.color.ToHexString() + "]";
		RewardsPlacementLabel.color = Color.white;
	}

	public void Show(PvpSeasonData lastSeason, int leaderboardRankInLastSeason)
	{
		PvpSeasonData activePvpSeason = Singleton<PlayerInfoScript>.Instance.SaveData.ActivePvpSeason;
		string text = KFFLocalization.Get("!!LASTS_FOR_X").Replace("<val1>", activePvpSeason.GetDurationString());
		DurationLabel.text = text;
		RewardsDurationLabel.text = text;
		if (lastSeason != null && leaderboardRankInLastSeason != -1)
		{
			StartCoroutine(ShowPopup(lastSeason, leaderboardRankInLastSeason));
		}
		else
		{
			ShowTween.Play();
		}
	}

	private IEnumerator ShowPopup(PvpSeasonData lastSeason, int leaderboardRankInLastSeason)
	{
		ShowWithRewardsTween.Play();
		string placementString = "[FFFFFF]#" + leaderboardRankInLastSeason + mPlacementTextColorString;
		RewardsPlacementLabel.text = mPlacementTextColorString + KFFLocalization.Get("!!YOU_PLACED_X").Replace("<val1>", placementString);
		mWaitingForPopup = true;
		List<GeneralReward> grantedRewards;
		List<UnlockableData> grantedUnlocks;
		Singleton<PlayerInfoScript>.Instance.GrantEndOfSeasonPvpRewards(lastSeason, leaderboardRankInLastSeason, out grantedRewards, out grantedUnlocks);
		PvpBattleResultsController.PopulateRewards(grantedRewards, RewardsTitleLabel, RewardPrefab, RewardsLootGrid);
		yield return null;
		UnityExtensions.SetLocalPositionY(y: (0f - (RewardsTableBottom.localPosition.y - mNormalRewardsBottomPos)) / 2f, trans: RewardsContents.transform);
		while (mWaitingForPopup)
		{
			yield return null;
		}
		foreach (UnlockableData unlockable in grantedUnlocks)
		{
			mWaitingForPopup = true;
			Singleton<UnlocksPopupController>.Instance.ShowUnlock(unlockable, OnRewardsClosed);
			while (mWaitingForPopup)
			{
				yield return null;
			}
		}
		OnClosed();
	}

	public void OnClosed()
	{
		if (!Singleton<TownController>.Instance.IsIntroDone())
		{
			Singleton<TownController>.Instance.AdvanceIntroState();
		}
	}

	public void OnRewardsClosed()
	{
		mWaitingForPopup = false;
	}
}
