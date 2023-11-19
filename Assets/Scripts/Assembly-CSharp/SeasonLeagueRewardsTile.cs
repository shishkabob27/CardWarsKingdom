using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeasonLeagueRewardsTile : UIStreamingGridListItem
{
	public UIGrid ItemGrid;

	public LeagueBadge LeagueBadge;

	public UIButton PrevButton;

	public UIButton NextButton;

	public GameObject ItemTemplate;

	public GameObject SelectedVFX;

	[Header("Tweens")]
	public UITweenController ShowTween;

	public UITweenController SelectedStateTween;

	public UITweenController NormalStateTween;

	public TweenWidth TweenWidth;

	private int _RankIndex;

	private int _StartIndex;

	private int _WidgetWidth = 136;

	private int _NumRanks;

	private List<PvpRankData> _RankDataList;

	private List<SeasonLeagueRewardItem> _RewardItems = new List<SeasonLeagueRewardItem>();

	private bool _IsPlayersPVPLevel;

	public override void Populate(object rankDataObj)
	{
		PVPTier pVPTier = rankDataObj as PVPTier;
		_RankDataList = pVPTier.PvpRanks;
		_StartIndex = pVPTier.StartIndex;
		_IsPlayersPVPLevel = pVPTier.IsPlayersPVPLevel;
		_NumRanks = _RankDataList.Count;
		_RankIndex = _NumRanks - 1;
		PrevButton.gameObject.SetActive(_NumRanks > 1);
		NextButton.gameObject.SetActive(_NumRanks > 1);
		ConfigurePrefab(_RankDataList[_RankIndex]);
	}

	private void ConfigurePrefab(PvpRankData inPvpRankData)
	{
		ItemGrid.transform.DestroyAllChildren();
		InventoryTile.ClearDelegates(true);
		PvpSeasonData activePvpSeason = Singleton<PlayerInfoScript>.Instance.SaveData.ActivePvpSeason;
		LeagueBadge.Populate(inPvpRankData);
		if (_IsPlayersPVPLevel)
		{
			NormalStateTween.StopAndReset();
			SelectedStateTween.Play();
			SelectedVFX.SetActive(true);
		}
		else
		{
			SelectedStateTween.StopAndReset();
			NormalStateTween.Play();
			SelectedVFX.SetActive(false);
		}
		int num = _StartIndex + _RankIndex + 1;
		LeagueBadge.RankNumberLabel.text = num.ToString();
		bool isEnabled = _RankIndex < _NumRanks - 1;
		PrevButton.isEnabled = isEnabled;
		bool isEnabled2 = _RankIndex > 0;
		NextButton.isEnabled = isEnabled2;
		List<GeneralReward> rankRewards = activePvpSeason.GetRankRewards(inPvpRankData.Rank);
		foreach (GeneralReward item in rankRewards)
		{
			SeasonLeagueRewardItem component = ItemGrid.transform.InstantiateAsChild(ItemTemplate).GetComponent<SeasonLeagueRewardItem>();
			component.gameObject.SetActive(true);
			component.ClaimedIcon.SetActive(false);
			item.PopulateUI(null, component.Icon, component.CardBackTexture, null, component.InventoryTileParent.transform, component.CountLabel, 4);
			component.CardBackLabel.gameObject.SetActive(item.RewardType == GeneralReward.TypeEnum.CardBack);
			_RewardItems.Add(component);
		}
		ItemGrid.Reposition();
		_WidgetWidth = ((rankRewards.Count != 0) ? (177 + rankRewards.Count * 90) : 136);
		UIWidget component2 = GetComponent<UIWidget>();
		component2.width = _WidgetWidth;
		component2.alpha = 1f;
	}

	public override void Unload()
	{
		ItemGrid.transform.DestroyAllChildren();
	}

	public void HandlePrevArrowPress()
	{
		_RankIndex++;
		_RankIndex %= _NumRanks;
		ConfigurePrefab(_RankDataList[_RankIndex]);
	}

	public void HandleNextArrowPress()
	{
		_RankIndex--;
		_RankIndex += _NumRanks;
		_RankIndex %= _NumRanks;
		ConfigurePrefab(_RankDataList[_RankIndex]);
	}

	public void TweenInWithDelay(float inDelay)
	{
		GetComponent<UIWidget>().alpha = 0f;
		TweenWidth.to = _WidgetWidth;
		GetComponent<UIWidget>().width = 136;
		for (int i = 0; i < _RewardItems.Count; i++)
		{
			_RewardItems[i].gameObject.SetActive(false);
		}
		StartCoroutine(TweenInCo(inDelay));
	}

	private IEnumerator TweenInCo(float inDelay)
	{
		yield return new WaitForSeconds(inDelay);
		ShowTween.Play();
		yield return new WaitForSeconds(0.1f);
		for (int i = 0; i < _RewardItems.Count; i++)
		{
			_RewardItems[i].TweenController.Play();
			yield return new WaitForSeconds(0.07f);
		}
	}
}
