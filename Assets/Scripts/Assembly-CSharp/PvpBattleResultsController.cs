using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PvpBattleResultsController : Singleton<PvpBattleResultsController>
{
	public float BarFillSpeed;

	public float XpFillTime;

	public UILabel RankLabel;

	public UILabel RankChangeLabel;

	public UILabel PointsLabel;

	public UILabel PointsToAdvanceLabel;

	public UISprite RankProgressBar;

	public UISprite XpBar;

	public UILabel XpAmount;

	public UILabel XpLevel;

	public UILabel XpEarned;

	public LeagueBadge LeagueBadge;

	public LeagueBadge RankUpLeagueBadge;

	public UILabel RankChangeRewardsLabel;

	public UILabel RankChangeNoRewardsLabel;

	public UIGrid RankUpRewardsGrid;

	public UIScrollView RewardsScrollView;

	public UIGrid WinRewardsGrid;

	public GameObject RewardPrefab;

	[SerializeField]
	[Header("Victory/Failed Elements")]
	private GameObject _VictoryBanner;

	[SerializeField]
	private GameObject _FailedBanner;

	[SerializeField]
	[Space(10f)]
	private UISprite[] _Stripes = new UISprite[2];

	[SerializeField]
	private Color[] _StripesColors = new Color[2];

	[SerializeField]
	[Space(10f)]
	private UILabel _LeaguePlaceLabel;

	[SerializeField]
	private Color[] _LeaguePlaceLabelOutlineColors = new Color[2];

	[SerializeField]
	[Space(10f)]
	private UISprite _LeagueFrameFill;

	[SerializeField]
	private Color[] _LeagueFrameFillColors = new Color[2];

	[Space(10f)]
	[SerializeField]
	private UISprite[] _LeagueFrameDetails = new UISprite[0];

	[SerializeField]
	private Color[] _LeagueFrameDetailsColors = new Color[2];

	[SerializeField]
	[Space(10f)]
	private UILabel _RewardsLabel;

	[SerializeField]
	private UITexture _GodRaysTexture;

	[SerializeField]
	private Color[] _GodRaysColors = new Color[2];

	[SerializeField]
	[Space(10f)]
	private UISprite[] _FillBarOutlines = new UISprite[0];

	[SerializeField]
	private UISprite[] _FillBarFills = new UISprite[0];

	[Header("Tweens")]
	public UITweenController ResetTween;

	public UITweenController ShowTween;

	public UITweenController ShowWinRewardsTween;

	public UITweenController ShowBannerTween;

	public UITweenController ShowPlaqueTween;

	public UITweenController ShowContentBgTween;

	public UITweenController ShowGodRaysTween;

	public UITweenController ShowSeasonProgressTween;

	public UITweenController ShowLeagueBadgeTween;

	public UITweenController HideSeasonProgress;

	public UITweenController RankChangeTween;

	public UITweenController ShowTapToContinueTween;

	public UITweenController ShowButtonsTween;

	public UITweenController ShowXpBarTween;

	public UITweenController ShrinkXpTween;

	public UITweenController PromotionMatchTween;

	public UITweenController WinStreakTween;

	[SerializeField]
	private AnimationCurve _TweenInRewardsCurve;

	private PvpMatchResultDetails mResults;

	private bool mClosePopupClicked;

	private int mCurrentRankXP;

	private int mRankXPToGive;

	private bool mShowSequenceDone;

	public bool Showing { get; private set; }

	public void RegisterResults(PvpMatchResultDetails resultDetails)
	{
		mCurrentRankXP = Singleton<PlayerInfoScript>.Instance.SaveData.RankXP;
		SetXpData(Singleton<PlayerInfoScript>.Instance.RankXpLevelData);
		mResults = resultDetails;
		if (mResults.Won)
		{
			RewardManager.TransferPvPRewardsToPlayerInfo();
			if (Singleton<PlayerInfoScript>.Instance.PvPData.RankedMode)
			{
				GeneralReward generalReward = mResults.WinRewards.Find((GeneralReward m) => m.RewardType == GeneralReward.TypeEnum.RankXP);
				if (generalReward != null)
				{
					mRankXPToGive = generalReward.Quantity;
				}
			}
			else
			{
				mRankXPToGive = 0;
			}
		}
		else
		{
			mRankXPToGive = 0;
		}
	}

	public void ShowResultsSequence()
	{
		StartCoroutine(ShowSequenceCo());
	}

	private IEnumerator ShowSequenceCo()
	{
		UICamera.ForceUnlockInput();
		mShowSequenceDone = false;
		Showing = true;
		ConfigureElementsForWinOrLose(mResults.Won);
		ResetTween.Play();
		PointsToAdvanceLabel.enabled = false;
		yield return StartCoroutine(PopulateRewards());
		ShowTween.Play();
		ShowBannerTween.Play();
		ShowGodRaysTween.Play();
		yield return StartCoroutine(ShowContentBgTween.PlayAsCoroutine());
		bool isXpGained = mRankXPToGive != 0;
		if (isXpGained)
		{
			yield return StartCoroutine(ShowXpGain());
		}
		if (!Singleton<PlayerInfoScript>.Instance.PvPData.RankedMode)
		{
			ShowButtonsTween.Play();
			mShowSequenceDone = true;
			yield break;
		}
		int numWinRewards = 0;
		if (mResults.WinRewards != null)
		{
			foreach (GeneralReward reward in mResults.WinRewards)
			{
				if (reward.RewardType != GeneralReward.TypeEnum.RankXP)
				{
					SalePopupListEntry rewardEntry = WinRewardsGrid.transform.InstantiateAsChild(RewardPrefab).GetComponent<SalePopupListEntry>();
					rewardEntry.gameObject.SetActive(true);
					rewardEntry.GetComponent<UIWidget>().alpha = 0f;
					rewardEntry.GetComponent<BoxCollider>().enabled = false;
					reward.PopulateUI(rewardEntry.Label, rewardEntry.Sprite, rewardEntry.Texture, rewardEntry.BackgroundSprite, rewardEntry.TileNode);
					numWinRewards++;
				}
			}
			WinRewardsGrid.Reposition();
		}
		int currentRank = mResults.StartingLevel;
		int currentPoints = mResults.StartingPointsInLevel;
		PvpRankData currentRankData = PvpRankDataManager.Instance.GetRank((!mResults.FirstMatchInSeason) ? currentRank : mResults.EndingLevel);
		bool isPromotionMatch = mResults.EndingPointsInLevel == currentRankData.PointsToAdvance;
		PointsToAdvanceLabel.enabled = !isPromotionMatch;
		if (currentRank == 1)
		{
			PointsToAdvanceLabel.enabled = false;
		}
		bool atLeastOneRewardGranted = mResults.Rewards.Count > 0;
		RankUpLeagueBadge.transform.SetLocalPositionX(atLeastOneRewardGranted ? (-195) : 0);
		RankChangeRewardsLabel.enabled = atLeastOneRewardGranted;
		RankChangeNoRewardsLabel.enabled = !atLeastOneRewardGranted;
		bool levelOneAndNotRankingDown = currentRank == 1 && mResults.EndingLevel == 1;
		bool levelTwoAndRankingUp = currentRank == 2 && mResults.EndingLevel == 1;
		if (!levelOneAndNotRankingDown && !levelTwoAndRankingUp && isXpGained)
		{
			yield return StartCoroutine(ShrinkXpTween.PlayAsCoroutine());
		}
		bool hasRankChanged = true;
		if (numWinRewards > 0)
		{
			mClosePopupClicked = false;
			yield return StartCoroutine(ShowWinRewardsTween.PlayAsCoroutine());
			yield return StartCoroutine(TweenInRewardsCo(WinRewardsGrid));
			while (!mClosePopupClicked)
			{
				yield return null;
			}
		}
		if (mResults.FirstMatchInSeason)
		{
			mClosePopupClicked = false;
			string leagueName3 = string.Format(Language.Get("!!DYNAMIC_LEAGUE"), PvpSeasonDataManager.Instance.RankName(mResults.EndingLevel));
			RankChangeLabel.text = KFFLocalization.Get("!!PLACED_INTO_LEAGUE").Replace("<val1>", leagueName3);
			RankUpLeagueBadge.Populate(currentRankData);
			yield return StartCoroutine(RankChangeTween.PlayAsCoroutine());
			yield return new WaitForSeconds(0.5f);
			if (atLeastOneRewardGranted)
			{
				yield return StartCoroutine(TweenInRewardsCo(RankUpRewardsGrid, RewardsScrollView));
			}
			ShowTapToContinueTween.Play();
			while (!mClosePopupClicked)
			{
				yield return null;
			}
			yield return StartCoroutine(ShowUnlocks());
			yield return new WaitForSeconds(0.4f);
			SetRankData(mResults.EndingLevel, mResults.EndingPointsInLevel, currentRankData.PointsToAdvance);
			yield return StartCoroutine(ShowSeasonProgressTween.PlayAsCoroutine());
		}
		else
		{
			if (currentRank > mResults.EndingLevel)
			{
				mClosePopupClicked = false;
				currentRank--;
				if (currentRank == 1)
				{
					PointsToAdvanceLabel.enabled = false;
				}
				PvpRankData newRankData2 = PvpRankDataManager.Instance.GetRank(currentRank);
				string leagueName2 = string.Format(Language.Get("!!DYNAMIC_LEAGUE"), PvpSeasonDataManager.Instance.RankName(currentRank));
				RankChangeLabel.text = KFFLocalization.Get("!!LEAGUE_REACHED").Replace("<val1>", leagueName2 + PvpSeasonDataManager.Instance.RankNumber(currentRank));
				RankUpLeagueBadge.Populate(newRankData2);
				yield return new WaitForSeconds(0.4f);
				RankLabel.text = string.Format(Language.Get("!!DYNAMIC_LEAGUE"), PvpSeasonDataManager.Instance.RankName(currentRank)) + PvpSeasonDataManager.Instance.RankNumber(currentRank);
				yield return StartCoroutine(RankChangeTween.PlayAsCoroutine());
				yield return new WaitForSeconds(0.4f);
				if (atLeastOneRewardGranted)
				{
					yield return StartCoroutine(TweenInRewardsCo(RankUpRewardsGrid, RewardsScrollView));
				}
				ShowTapToContinueTween.Play();
				while (!mClosePopupClicked)
				{
					yield return null;
				}
				yield return new WaitForSeconds(0.4f);
				yield return StartCoroutine(ShowPlaqueTween.PlayAsCoroutine());
				LeagueBadge.Populate(newRankData2);
				yield return StartCoroutine(ShowLeagueBadgeTween.PlayAsCoroutine());
				currentRankData = newRankData2;
				if (currentRank != 1)
				{
					currentPoints = 0;
					SetRankData(currentRank, currentPoints, currentRankData.PointsToAdvance);
					yield return StartCoroutine(ShowSeasonProgressTween.PlayAsCoroutine());
				}
				yield return StartCoroutine(ShowUnlocks());
				yield return new WaitForSeconds(0.3f);
			}
			else if (currentRank < mResults.EndingLevel)
			{
				mClosePopupClicked = false;
				PvpRankData oldRankData = PvpRankDataManager.Instance.GetRank(currentRank);
				string oldLeagueName = string.Format(Language.Get("!!DYNAMIC_LEAGUE"), PvpSeasonDataManager.Instance.RankName(currentRank));
				RankChangeLabel.text = KFFLocalization.Get("!!LEAGUE_LOST").Replace("<val1>", oldLeagueName + PvpSeasonDataManager.Instance.RankNumber(currentRank));
				RankUpLeagueBadge.Populate(oldRankData);
				currentRank++;
				PvpRankData newRankData = PvpRankDataManager.Instance.GetRank(currentRank);
				PointsToAdvanceLabel.enabled = !isPromotionMatch;
				yield return new WaitForSeconds(0.4f);
				RankLabel.text = string.Format(Language.Get("!!DYNAMIC_LEAGUE"), PvpSeasonDataManager.Instance.RankName(currentRank)) + PvpSeasonDataManager.Instance.RankNumber(currentRank);
				yield return StartCoroutine(RankChangeTween.PlayAsCoroutine());
				yield return new WaitForSeconds(0.4f);
				ShowTapToContinueTween.Play();
				while (!mClosePopupClicked)
				{
					yield return null;
				}
				yield return new WaitForSeconds(0.4f);
				yield return StartCoroutine(ShowPlaqueTween.PlayAsCoroutine());
				LeagueBadge.Populate(newRankData);
				yield return StartCoroutine(ShowLeagueBadgeTween.PlayAsCoroutine());
				currentRankData = PvpRankDataManager.Instance.GetRank(currentRank);
				currentPoints = currentRankData.PointsAfterRankDown() + 1;
				SetRankData(currentRank, currentPoints, currentRankData.PointsToAdvance);
				yield return StartCoroutine(ShowSeasonProgressTween.PlayAsCoroutine());
				yield return new WaitForSeconds(0.3f);
			}
			else
			{
				hasRankChanged = false;
				string leagueName = string.Format(Language.Get("!!DYNAMIC_LEAGUE"), PvpSeasonDataManager.Instance.RankName(mResults.EndingLevel));
				RankLabel.text = leagueName + PvpSeasonDataManager.Instance.RankNumber(currentRank);
				yield return StartCoroutine(ShowPlaqueTween.PlayAsCoroutine());
				LeagueBadge.Populate(currentRankData);
				yield return StartCoroutine(ShowLeagueBadgeTween.PlayAsCoroutine());
				if (currentRank != 1 && currentRankData.PointsToAdvance > 1)
				{
					SetRankData(mResults.StartingLevel, mResults.StartingPointsInLevel, currentRankData.PointsToAdvance);
					yield return StartCoroutine(ShowSeasonProgressTween.PlayAsCoroutine());
				}
			}
			bool winStreakPlaying = false;
			if (!hasRankChanged && mResults.EndingPointsInLevel >= mResults.StartingPointsInLevel + 2)
			{
				winStreakPlaying = true;
				yield return new WaitForSeconds(0.3f);
				WinStreakTween.PlayWithCallback(delegate
				{
					winStreakPlaying = false;
				});
			}
			if (currentPoints < mResults.EndingPointsInLevel)
			{
				while (currentPoints < mResults.EndingPointsInLevel)
				{
					currentPoints++;
					PointsLabel.text = currentPoints + " / " + currentRankData.PointsToAdvance;
					yield return StartCoroutine(MoveProgressBar((float)currentPoints / (float)currentRankData.PointsToAdvance));
				}
			}
			else if (currentPoints > mResults.EndingPointsInLevel)
			{
				while (currentPoints > mResults.EndingPointsInLevel)
				{
					currentPoints--;
					PointsLabel.text = currentPoints + " / " + currentRankData.PointsToAdvance;
					yield return StartCoroutine(MoveProgressBar((float)currentPoints / (float)currentRankData.PointsToAdvance));
				}
			}
			while (winStreakPlaying)
			{
				yield return null;
			}
		}
		if (currentRank != 1 && mResults.EndingPointsInLevel == currentRankData.PointsToAdvance)
		{
			yield return StartCoroutine(PromotionMatchTween.PlayAsCoroutine());
		}
		ShowButtonsTween.Play();
		mShowSequenceDone = true;
		Showing = false;
	}

	private IEnumerator TweenInRewardsCo(UIGrid inRewardsGrid, UIScrollView inScrollView = null)
	{
		inRewardsGrid.Reposition();
		if (inScrollView != null)
		{
			inScrollView.ResetPosition();
		}
		yield return new WaitForSeconds(0.2f);
		foreach (Transform reward in inRewardsGrid.transform)
		{
			reward.GetComponent<UIWidget>().alpha = 1f;
			TweenPosition tp = reward.gameObject.AddComponent<TweenPosition>();
			Vector3 locPos = reward.localPosition;
			tp.from = new Vector3(locPos.x + 300f, locPos.y, locPos.z);
			tp.to = locPos;
			tp.duration = 0.3f;
			tp.animationCurve = _TweenInRewardsCurve;
			tp.PlayForward();
			yield return new WaitForSeconds(0.2f);
		}
	}

	private void ConfigureElementsForWinOrLose(bool inIsWin)
	{
		int num = ((!inIsWin) ? 1 : 0);
		_Stripes[0].color = _StripesColors[num];
		_Stripes[1].color = _StripesColors[num];
		_LeaguePlaceLabel.effectColor = _LeaguePlaceLabelOutlineColors[num];
		_LeagueFrameFill.color = _LeagueFrameFillColors[num];
		UISprite[] leagueFrameDetails = _LeagueFrameDetails;
		foreach (UISprite uISprite in leagueFrameDetails)
		{
			uISprite.color = _LeagueFrameDetailsColors[num];
		}
		_GodRaysTexture.color = _GodRaysColors[num];
		_VictoryBanner.SetActive(inIsWin);
		_FailedBanner.SetActive(!inIsWin);
	}

	private IEnumerator ShowXpGain()
	{
		yield return StartCoroutine(ShowXpBarTween.PlayAsCoroutine());
		float xpPerSecond = (float)mRankXPToGive / XpFillTime;
		if (mRankXPToGive > 0)
		{
			yield return new WaitForSeconds(0.3f);
			Singleton<SLOTAudioManager>.Instance.PlaySound("UI_ExpLoop");
			while (mRankXPToGive > 0)
			{
				int thisFrame = (int)(Time.deltaTime * xpPerSecond);
				if (thisFrame < 1)
				{
					thisFrame = 1;
				}
				if (thisFrame > mRankXPToGive)
				{
					thisFrame = mRankXPToGive;
				}
				mRankXPToGive -= thisFrame;
				mCurrentRankXP += thisFrame;
				SetXpData(Singleton<PlayerInfoScript>.Instance.RankXpLevelDataAt(mCurrentRankXP));
				yield return null;
			}
			Singleton<SLOTAudioManager>.Instance.StopSound("UI_ExpLoop");
		}
		yield return new WaitForSeconds(0.4f);
	}

	private void SetXpData(XPLevelData levelData)
	{
		XpBar.fillAmount = levelData.mPercentThroughCurrentLevel;
		XpAmount.text = levelData.mXPEarnedWithinCurrentLevel + " / " + levelData.mTotalXPInCurrentLevel + " " + KFFLocalization.Get("!!EXP");
		XpLevel.text = KFFLocalization.Get("!!RANK") + " " + levelData.mCurrentLevel;
		if (mRankXPToGive > 0)
		{
			XpEarned.text = "+ " + mRankXPToGive + " " + KFFLocalization.Get("!!EXP");
		}
		else
		{
			XpEarned.text = string.Empty;
		}
	}

	private void SetRankData(int currentRank, int currentPoints, int pointsToAdvance)
	{
		RankLabel.text = string.Format(Language.Get("!!DYNAMIC_LEAGUE"), PvpSeasonDataManager.Instance.RankName(currentRank)) + PvpSeasonDataManager.Instance.RankNumber(currentRank);
		PointsToAdvanceLabel.text = KFFLocalization.Get("!!POINTS_TO_ADVANCE").Replace("<val1>", pointsToAdvance.ToString());
		PointsLabel.text = currentPoints + " / " + pointsToAdvance;
		RankProgressBar.fillAmount = (float)currentPoints / (float)pointsToAdvance;
	}

	private IEnumerator MoveProgressBar(float targetPercent)
	{
		while (RankProgressBar.fillAmount != targetPercent)
		{
			RankProgressBar.fillAmount = RankProgressBar.fillAmount.TickTowards(targetPercent, BarFillSpeed);
			yield return null;
		}
	}

	private IEnumerator PopulateRewards()
	{
		PopulateRewards(mResults.Rewards, RankChangeRewardsLabel.gameObject, RewardPrefab, RankUpRewardsGrid);
		yield return null;
	}

	public static void PopulateRewards(List<GeneralReward> rewards, GameObject titleLabel, GameObject rewardPrefab, UIGrid lootGrid)
	{
		if (rewards == null || rewards.Count == 0)
		{
			titleLabel.SetActive(false);
			lootGrid.gameObject.SetActive(false);
			return;
		}
		titleLabel.SetActive(true);
		lootGrid.gameObject.SetActive(true);
		lootGrid.transform.DestroyAllChildren();
		foreach (GeneralReward reward in rewards)
		{
			SalePopupListEntry component = lootGrid.transform.InstantiateAsChild(rewardPrefab).GetComponent<SalePopupListEntry>();
			component.gameObject.SetActive(true);
			reward.PopulateUI(component.Label, component.Sprite, component.Texture, component.BackgroundSprite, component.TileNode);
			component.GetComponent<UIWidget>().alpha = 0f;
			component.GetComponent<BoxCollider>().enabled = false;
		}
		lootGrid.Reposition();
	}

	private IEnumerator ShowUnlocks()
	{
		foreach (UnlockableData unlock in mResults.GrantedUnlocks)
		{
			mClosePopupClicked = false;
			Singleton<UnlocksPopupController>.Instance.ShowUnlock(unlock, OnClickPopup);
			while (!mClosePopupClicked)
			{
				yield return null;
			}
		}
	}

	public void OnClickPopup()
	{
		mClosePopupClicked = true;
	}

	public void OnClickUploadReplay()
	{
	}

	public void OnClickContinue()
	{
		Singleton<SLOTMusic>.Instance.StopMusic(0.5f);
		Singleton<MultiplayerMessageHandler>.Instance.LeaveGame();
		Singleton<ScreenFadeController>.Instance.ShowLoadScreen(delegate
		{
			StartCoroutine(ChangeScene());
		});
	}

	private IEnumerator ChangeScene()
	{
		yield return new WaitForSeconds(0.6f);
		Singleton<DWBattleLane>.Instance.ClearPooledData();
		DetachedSingleton<SceneFlowManager>.Instance.LoadFrontEndScene(SceneFlowManager.ReturnLocation.Pvp);
	}

	public void AndroidBackButtonPressed()
	{
		if (mShowSequenceDone)
		{
			OnClickContinue();
		}
	}
}
