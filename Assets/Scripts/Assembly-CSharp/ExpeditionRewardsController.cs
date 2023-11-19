using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpeditionRewardsController : Singleton<ExpeditionRewardsController>
{
	public float RankXPFillTime;

	public GameObject RewardLinePrefab;

	public UITexture ArtBG;

	public UIGrid RewardsGrid;

	public UISprite RankBar;

	public UILabel RankLevel;

	[Header("Tweens")]
	public UITweenController ShowTween;

	public UITweenController ShowResultTween;

	public UITweenController ShowCloseButtonTween;

	public UITweenController ShowXPBarTween;

	public void ShowRewards(ExpeditionItem expedition)
	{
		StartCoroutine(ShowRewardsCo(expedition));
	}

	private IEnumerator ShowRewardsCo(ExpeditionItem expedition)
	{
		bool showTweenDone = false;
		ShowTween.PlayWithCallback(delegate
		{
			showTweenDone = true;
		});
		Singleton<SLOTResourceManager>.Instance.QueueUITextureLoad(expedition.NameData.Texture, expedition.NameData.TextureAssetBundle, "UI/UI/LoadingPlaceholder", ArtBG);
		int startingXP = Singleton<PlayerInfoScript>.Instance.SaveData.RankXP;
		List<GeneralReward> lootList;
		DetachedSingleton<ExpeditionManager>.Instance.ClaimExpedition(expedition, out lootList);
		bool showingXP = false;
		bool gotDungeonMap = false;
		InventoryTile.ClearDelegates(true);
		foreach (GeneralReward lootItem in lootList)
		{
			SalePopupListEntry newEntry = RewardsGrid.transform.InstantiateAsChild(RewardLinePrefab).GetComponent<SalePopupListEntry>();
			newEntry.gameObject.SetActive(true);
			lootItem.PopulateUI(newEntry.Label, newEntry.Sprite, newEntry.Texture, newEntry.BackgroundSprite, newEntry.TileNode);
		}
		RewardsGrid.Reposition();
		while (!showTweenDone)
		{
			yield return null;
		}
		Singleton<TownHudController>.Instance.PopulatePlayerInfo();
		yield return StartCoroutine(ShowResultTween.PlayAsCoroutine());
		ShowCloseButtonTween.Play();
		if (gotDungeonMap)
		{
			Singleton<TutorialController>.Instance.StartTutorialBlockIfNotComplete("DungeonMap");
		}
	}

	private void UpdateXpBar(int xp)
	{
		XPLevelData xPLevelData = Singleton<PlayerInfoScript>.Instance.RankXpLevelDataAt(xp);
		RankBar.fillAmount = xPLevelData.mPercentThroughCurrentLevel;
		RankLevel.text = KFFLocalization.Get("!!RANK") + " " + xPLevelData.mCurrentLevel;
	}

	public void OnClickClose()
	{
		Singleton<ExpeditionStartController>.Instance.OnRewardsClosed();
	}

	public void Unload()
	{
		RewardsGrid.transform.DestroyAllChildren();
		ArtBG.UnloadTexture();
	}
}
