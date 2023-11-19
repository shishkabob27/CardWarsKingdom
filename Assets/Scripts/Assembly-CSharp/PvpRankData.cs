using System;
using System.Collections.Generic;

public class PvpRankData : ILoadableData
{
	private int _ID;

	private int _Rank;

	private int _PointsToAdvance;

	private bool _NoPointLoss;

	public int RankAfterSeasonReset { get; private set; }

	public string ID
	{
		get
		{
			return _ID.ToString();
		}
	}

	public int Rank
	{
		get
		{
			return _Rank;
		}
	}

	public string RankName { get; private set; }

	public string ShortRankName { get; private set; }

	public int PointsToAdvance
	{
		get
		{
			return _PointsToAdvance;
		}
	}

	public bool NoPointLoss
	{
		get
		{
			return _NoPointLoss;
		}
	}

	public string LeagueTexture { get; private set; }

	public string LeagueTextureNameOnly { get; private set; }

	public List<GeneralReward> WinRewards { get; private set; }

	public void Populate(Dictionary<string, object> dict)
	{
		_ID = TFUtils.LoadInt(dict, "Rank", 0);
		_Rank = Convert.ToInt32(_ID);
		_PointsToAdvance = TFUtils.LoadInt(dict, "PointsToAdvance", 0);
		_NoPointLoss = TFUtils.LoadBool(dict, "NoPointLoss", false);
		RankAfterSeasonReset = TFUtils.LoadInt(dict, "RankAfterSeasonReset", -1);
		RankName = KFFLocalization.Get(TFUtils.LoadString(dict, "RankName", string.Empty));
		ShortRankName = KFFLocalization.Get(TFUtils.LoadString(dict, "ShortRankName", string.Empty));
		LeagueTexture = "UI/Icons_Leagues/" + TFUtils.LoadString(dict, "LeagueTexture", string.Empty);
		LeagueTextureNameOnly = TFUtils.LoadString(dict, "LeagueTexture", string.Empty);
		WinRewards = GeneralReward.ParseFromJson(dict);
	}

	public int PointsAfterRankDown()
	{
		return _PointsToAdvance - 1;
	}
}
