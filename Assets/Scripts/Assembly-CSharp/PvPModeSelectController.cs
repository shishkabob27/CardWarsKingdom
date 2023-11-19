using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using Multiplayer;
using UnityEngine;

public class PvPModeSelectController : Singleton<PvPModeSelectController>
{
	public delegate void DummyLeaderboardsDelegate(List<LeaderboardData> dataList, ResponseFlag result);

	private const int LeaderboardPageSize = 50;

	[Header("Tab to Click on Init")]
	public GameObject InitTab;

	public UILabel NameLabel;

	public UILabel RankNumberLabel;

	public UILabel CurrentLeagueLabel;

	public LeagueBadge Badge;

	public UILabel SeasonLabel;

	public UILabel SeasonName;

	public UITexture SeasonBanner;

	public SeasonProgressBar SeasonProgressBar;

	public GameObject RankedParent;

	public GameObject NotRankedParent;

	public GameObject PromotionMatchLabel;

	public UIStreamingGrid LeaderboardGrid;

	public GameObject LeaderboardListPrefab;

	public SeasonLabelColorPallette[] SeasonLabelColors = new SeasonLabelColorPallette[0];

	[Header("Tweens")]
	public UITweenController ShowTween;

	public UITweenController HideTween;

	public UITweenController PromotionMatchTween;

	public UITweenController FetchingLeaderboardsTween;

	public UITweenController HideFetchingLeaderboardsTween;

	public UITweenController HideElementsTween;

	public UITweenController ShowElementsTween;

	private UIStreamingGridDataSource<LeaderboardData> mLeaderboardGridDataSource = new UIStreamingGridDataSource<LeaderboardData>();

	private bool mInPromotionMatch;

	private bool mShowing;

	private PvpSeasonData _ActiveSeason;

	public void Populate()
	{
		Singleton<PlayerInfoScript>.Instance.CheckPvpSeasonStatus();
		PlayerSaveData saveData = Singleton<PlayerInfoScript>.Instance.SaveData;
		PvpSeasonData pvpSeasonData = (_ActiveSeason = Singleton<PlayerInfoScript>.Instance.SaveData.ActivePvpSeason);
		if (pvpSeasonData.IsExpired())
		{
			SeasonLabel.text = string.Empty;
		}
		else
		{
			SeasonLabel.text = string.Format(Language.Get("!!SEASON_ENDS"), pvpSeasonData.GetDurationString());
		}
		SeasonName.text = pvpSeasonData.Name;
		ColorSeasonNameLabel(pvpSeasonData.BannerTextureNameOnly);
		Singleton<SLOTResourceManager>.Instance.QueueResourceLoad(pvpSeasonData.BannerTexture, "GeneralBundle", delegate(Object loadedResouce)
		{
			SeasonBanner.UnloadTexture();
			SeasonBanner.mainTexture = loadedResouce as Texture;
			ShowTween.PlayWithCallback(ShowTweenDone);
		});
		if (saveData.PlayedFirstBattleInPvpSeason)
		{
			RankedParent.SetActive(true);
			NotRankedParent.SetActive(false);
			string arg = PvpSeasonDataManager.Instance.RankName(saveData.MultiplayerLevel);
			CurrentLeagueLabel.text = string.Format(Language.Get("!!DYNAMIC_LEAGUE"), arg);
			RankNumberLabel.text = saveData.MultiplayerLevel.ToString();
			Badge.PopulateMyData();
			PvpRankData rank = PvpRankDataManager.Instance.GetRank(saveData.MultiplayerLevel);
			ObscuredInt pointsInMultiplayerLevel = saveData.PointsInMultiplayerLevel;
			int pointsToAdvance = rank.PointsToAdvance;
			SeasonProgressBar.AnimateTo(pointsInMultiplayerLevel, pointsInMultiplayerLevel, pointsToAdvance);
			SeasonProgressBar.gameObject.SetActive(rank.Rank > 1);
			PromotionMatchLabel.SetActive(false);
			mInPromotionMatch = saveData.MultiplayerLevel != 1 && (int)pointsInMultiplayerLevel == pointsToAdvance;
		}
		else
		{
			RankedParent.SetActive(false);
			NotRankedParent.SetActive(true);
			NameLabel.text = Singleton<PlayerInfoScript>.Instance.SaveData.MultiplayerPlayerName;
		}
		mShowing = true;
		Singleton<TutorialController>.Instance.AdvanceIfTargetingBuilding("TBuilding_PVP");
		if (!Singleton<TutorialController>.Instance.IsAnyTutorialActive())
		{
			UpsightRequester.RequestContent("enter_pvp");
		}
		Invoke("GoToInitTab", 0.1f);
	}

	public void Hide()
	{
		HideTween.Play();
	}

	private void GoToInitTab()
	{
		ColorSeasonNameLabel(_ActiveSeason.BannerTextureNameOnly);
		if (InitTab != null)
		{
			InitTab.SendMessage("OnClick");
		}
		InitTab = null;
	}

	private void ShowTweenDone()
	{
		if (mInPromotionMatch)
		{
			PromotionMatchTween.Play();
		}
	}

	private IEnumerator PopulateLeaderboard()
	{
		List<LeaderboardData> fetchedEntries = null;
		bool done = false;
		FetchingLeaderboardsTween.Play();
		FetchDummyLeaderboards(0, 50, delegate(List<LeaderboardData> leaderboardEntries, ResponseFlag result)
		{
			if (result == ResponseFlag.Success)
			{
				fetchedEntries = leaderboardEntries;
			}
			done = true;
		});
		while (!done)
		{
			yield return null;
		}
		HideFetchingLeaderboardsTween.Play();
		if (mShowing)
		{
			mLeaderboardGridDataSource.Init(LeaderboardGrid, LeaderboardListPrefab, fetchedEntries);
		}
	}

	private void FetchDummyLeaderboards(int startIndex, int count, DummyLeaderboardsDelegate dataCallback)
	{
		StartCoroutine(FetchDummyLeaderboardsCo(startIndex, count, dataCallback));
	}

	private IEnumerator FetchDummyLeaderboardsCo(int startIndex, int count, DummyLeaderboardsDelegate callback)
	{
		yield return new WaitForSeconds(Random.Range(0.1f, 2f));
		List<LeaderboardData> results = new List<LeaderboardData>();
		for (int i = startIndex; i < startIndex + count; i++)
		{
			LeaderboardData newData = new LeaderboardData();
			newData.rank = i + 1;
			newData.name = "Player " + newData.rank;
			newData.trophies = 1000 - i;
			results.Add(newData);
		}
		callback(results, ResponseFlag.Success);
	}

	public void OnClickRankedMatch()
	{
		if (MiscParams.DisablePvp)
		{
			Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!PVP_DISABLED"));
		}
		else
		{
			Singleton<PVPPrepScreenController>.Instance.Show(PvpMode.Ranked, null, true);
		}
	}

	public void OnClickUnrankedMatch()
	{
		if (MiscParams.DisablePvp)
		{
			Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!PVP_DISABLED"));
		}
		else
		{
			Singleton<PVPPrepScreenController>.Instance.Show(PvpMode.Unranked, null);
		}
	}

	public void OnClickAllyMatch()
	{
		if (MiscParams.DisablePvp)
		{
			Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!PVP_DISABLED"));
			return;
		}
		HideElementsTween.Play();
		Singleton<PvPAllySelectController>.Instance.Populate();
	}

	public void OnClickBattleHistory()
	{
		HideElementsTween.Play();
		BattleHistoryController.Instance.Show();
	}

	public void OnClickRewards()
	{
		HideElementsTween.Play();
		Singleton<SeasonRewardsScreen>.Instance.Show();
	}

	public void OnClickClose()
	{
		mShowing = false;
	}

	public void Unload()
	{
		SeasonBanner.UnloadTexture();
		mLeaderboardGridDataSource.Clear();
		Singleton<TownHudController>.Instance.ReturnToTownView();
	}

	public void ShowElements()
	{
		ShowElementsTween.Play();
	}

	private void ColorSeasonNameLabel(string inBannerTextureNameOnly)
	{
		int num = 0;
		switch (inBannerTextureNameOnly)
		{
		case "PVP_Season_Cinnamon":
			num = 1;
			break;
		case "PVP_Season_Mint":
			num = 2;
			break;
		case "PVP_Season_Nutmeg":
			num = 3;
			break;
		case "PVP_Season_ToeSalt":
			num = 4;
			break;
		case "PVP_Season_SkullKing":
			num = 5;
			break;
		}
		SeasonLabelColorPallette seasonLabelColorPallette = SeasonLabelColors[num];
		SeasonName.color = seasonLabelColorPallette.TextColor;
		SeasonName.effectColor = seasonLabelColorPallette.OutlineColor;
		if (SeasonName.LabelShadow != null)
		{
			SeasonName.LabelShadow.ShadowTextColor = seasonLabelColorPallette.ShadowTextColor;
			SeasonName.LabelShadow.ShadowEffectColor = seasonLabelColorPallette.ShadowOutlineColor;
			SeasonName.LabelShadow.RefreshShadowLabel();
		}
	}
}
