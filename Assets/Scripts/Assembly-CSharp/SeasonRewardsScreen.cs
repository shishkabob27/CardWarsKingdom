using System.Collections.Generic;
using UnityEngine;

public class SeasonRewardsScreen : Singleton<SeasonRewardsScreen>
{
	public GameObject SeasonRewardsPrefab;

	public UITweenController ShowTween;

	public UITweenController HideTween;

	public UIScrollView ScrollView;

	public UIStreamingGrid LeaguesGrid;

	private UIStreamingGridDataSource<PVPTier> mLeaguesGridDataSource = new UIStreamingGridDataSource<PVPTier>();

	public void Show()
	{
		ShowTween.Play();
		List<PvpRankData> list = PvpRankDataManager.Instance.GetDatabase().Copy();
		string text = "Skippy is Awesome!!!";
		PVPTier pVPTier = null;
		List<PVPTier> list2 = new List<PVPTier>();
		for (int i = 0; i < list.Count; i++)
		{
			PvpRankData pvpRankData = list[i];
			if (text != pvpRankData.LeagueTextureNameOnly)
			{
				bool inIsPlayersPVPLevel = i == Singleton<PlayerInfoScript>.Instance.SaveData.MultiplayerLevel - 1;
				pVPTier = new PVPTier(i, pvpRankData, inIsPlayersPVPLevel);
				list2.Add(pVPTier);
			}
			else
			{
				pVPTier.PvpRanks.Add(pvpRankData);
			}
		}
		mLeaguesGridDataSource.Init(LeaguesGrid, SeasonRewardsPrefab, list2, true, true);
		ScrollView.ResetPosition();
		SeasonLeagueRewardsTile[] componentsInChildren = LeaguesGrid.transform.GetComponentsInChildren<SeasonLeagueRewardsTile>();
		for (int j = 0; j < componentsInChildren.Length; j++)
		{
			SeasonLeagueRewardsTile seasonLeagueRewardsTile = componentsInChildren[j];
			float inDelay = 0.3f + (float)j * 0.17f;
			seasonLeagueRewardsTile.TweenInWithDelay(inDelay);
		}
	}

	public void Hide()
	{
		Singleton<PvPModeSelectController>.Instance.ShowElements();
		HideTween.PlayWithCallback(Unload);
	}

	public void Unload()
	{
		mLeaguesGridDataSource.Clear();
	}

	public void OnCloseButtonClick()
	{
		HideTween.PlayWithCallback(Unload);
		Singleton<PvPModeSelectController>.Instance.Hide();
	}
}
