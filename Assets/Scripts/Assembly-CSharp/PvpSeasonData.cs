using System;
using System.Collections.Generic;
using System.Globalization;

public class PvpSeasonData : ILoadableData
{
	public class RankRewards
	{
		public List<GeneralReward> Rewards;
	}

	public class LeaderboardRewards
	{
		public List<GeneralReward> Rewards;

		public int Position;
	}

	private List<RankRewards> mRankRewards;

	private List<LeaderboardRewards> mLeaderboardRewards = new List<LeaderboardRewards>();

	public string ID { get; private set; }

	public DateTime EndDate { get; private set; }

	public PvpSeasonData PreviousSeason { get; set; }

	public string Name { get; private set; }

	public string BannerTexture { get; private set; }

	public string BannerTextureNameOnly { get; private set; }

	public void Populate(Dictionary<string, object> dict)
	{
		ID = TFUtils.LoadString(dict, "Season", string.Empty);
		EndDate = DateTime.ParseExact(TFUtils.LoadString(dict, "EndDate", string.Empty), "M/d/yyyy", CultureInfo.InvariantCulture);
		int count = PvpRankDataManager.Instance.GetDatabase().Count;
		mRankRewards = new List<RankRewards>(count);
		for (int i = 0; i < count; i++)
		{
			mRankRewards.Add(null);
		}
		Name = TFUtils.LoadLocalizedString(dict, "Name", string.Empty);
		BannerTexture = "UI/GeneralBundle/SeasonBanners/" + TFUtils.LoadString(dict, "BannerTexture", string.Empty);
		BannerTextureNameOnly = TFUtils.LoadString(dict, "BannerTexture", string.Empty);
		AddRewardRow(dict);
	}

	public void AddRewardRow(Dictionary<string, object> dict)
	{
		int num = TFUtils.LoadInt(dict, "LeaderboardPos", -1);
		if (num != -1)
		{
			LeaderboardRewards leaderboardRewards = new LeaderboardRewards();
			leaderboardRewards.Rewards = GeneralReward.ParseFromJson(dict);
			leaderboardRewards.Position = num;
			mLeaderboardRewards.Add(leaderboardRewards);
			return;
		}
		int num2 = TFUtils.LoadInt(dict, "Rank", 0);
		if (num2 >= 1 && num2 <= mRankRewards.Count)
		{
			if (mRankRewards[num2 - 1] != null)
			{
			}
			mRankRewards[num2 - 1] = new RankRewards();
			mRankRewards[num2 - 1].Rewards = GeneralReward.ParseFromJson(dict);
		}
	}

	public List<GeneralReward> GetRankRewards(int rank)
	{
		if (mRankRewards[rank - 1] == null)
		{
			return new List<GeneralReward>();
		}
		return mRankRewards[rank - 1].Rewards;
	}

	public List<GeneralReward> GetLeaderboardRewardsForPlacement(int placement)
	{
		List<GeneralReward> list = new List<GeneralReward>();
		foreach (LeaderboardRewards mLeaderboardReward in mLeaderboardRewards)
		{
			if (placement <= mLeaderboardReward.Position)
			{
				list.AddRange(mLeaderboardReward.Rewards);
			}
		}
		return list;
	}

	public string GetEndDateString()
	{
		return EndDate.ToShortDateString();
	}

	public bool IsExpired()
	{
		return TFUtils.ServerTime > EndDate;
	}

	public string GetDurationString()
	{
		return PlayerInfoScript.FormatTimeString((int)(EndDate - TFUtils.ServerTime).TotalSeconds);
	}
}
