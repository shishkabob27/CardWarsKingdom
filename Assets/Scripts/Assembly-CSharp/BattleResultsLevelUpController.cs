using System.Collections;
using UnityEngine;

public class BattleResultsLevelUpController : Singleton<BattleResultsLevelUpController>
{
	public GameObject MainPanel;

	public UITweenController ShowTween;

	public UITweenController HideTween;

	public UILabel Level;

	public UIGrid NumericalsParent;

	public GameObject StaminaGroup;

	public UILabel StaminaLabel;

	public GameObject InventorySpaceGroup;

	public UILabel InventorySpaceLabel;

	public ParticleSystem mRays;

	public bool ShowLevelUp()
	{
		if (Singleton<PlayerInfoScript>.Instance.SaveData.PlayersLastSavedLevel != Singleton<PlayerInfoScript>.Instance.RankXpLevelData.mCurrentLevel)
		{
			StartCoroutine(PauseBeforeAnimating());
			return true;
		}
		return false;
	}

	public void DebugShowLevelUp()
	{
		StartCoroutine(PauseBeforeAnimating());
	}

	public IEnumerator PauseBeforeAnimating()
	{
		UICamera.LockInput();
		while (LoadingScreenController.ShowingLoadingScreen())
		{
			yield return null;
		}
		if (Singleton<ExpeditionStartController>.Instance.Showing)
		{
			Singleton<ExpeditionStartController>.Instance.HideTween.Play();
		}
		int lastLevel = Singleton<PlayerInfoScript>.Instance.SaveData.PlayersLastSavedLevel;
		int newLevel = Singleton<PlayerInfoScript>.Instance.RankXpLevelData.mCurrentLevel;
		Singleton<PlayerInfoScript>.Instance.SaveData.PlayersLastSavedLevel = newLevel;
		ShowTween.Play();
		mRays.loop = true;
		Level.text = newLevel.ToString();
		PlayerRankData newLevelData = PlayerRankDataManager.Instance.GetData(newLevel - 1);
		PlayerRankData lastLevelData = PlayerRankDataManager.Instance.GetData(lastLevel - 1);
		int socialUnlockLevel = PlayerRankDataManager.Instance.UnlockRanks.Find((PlayerRankData m) => m.UnlockId == "TBuilding_Social").Level;
		int inventorySlotsGained = 0;
		for (int gainedLevel = lastLevel + 1; gainedLevel <= newLevel; gainedLevel++)
		{
			switch (gainedLevel)
			{
			case 2:
				Singleton<KFFSocialManager>.Instance.ReportAchievement(KFFSocialManager.AchievementIDs.DW_LVL_2);
				break;
			case 10:
				Singleton<KFFSocialManager>.Instance.ReportAchievement(KFFSocialManager.AchievementIDs.DW_LVL_10);
				break;
			case 30:
				Singleton<KFFSocialManager>.Instance.ReportAchievement(KFFSocialManager.AchievementIDs.DW_LVL_30);
				break;
			case 50:
				Singleton<KFFSocialManager>.Instance.ReportAchievement(KFFSocialManager.AchievementIDs.DW_LVL_50);
				break;
			}
			Singleton<AnalyticsManager>.Instance.logHighestRank(gainedLevel);
			if (gainedLevel == socialUnlockLevel)
			{
				Singleton<PlayerInfoScript>.Instance.SetDefaultHelperCreature();
			}
			PlayerRankData gainedLevelData = PlayerRankDataManager.Instance.GetData(gainedLevel - 1);
			inventorySlotsGained += gainedLevelData.InventorySpace;
		}
		if (inventorySlotsGained > 0)
		{
			Singleton<PlayerInfoScript>.Instance.SaveData.AddEmptyInventorySlots(inventorySlotsGained);
		}
		Singleton<PlayerInfoScript>.Instance.Save();
		int staminaGained = newLevelData.Stamina - lastLevelData.Stamina;
		if (staminaGained > 0)
		{
			StaminaGroup.SetActive(true);
			StaminaLabel.text = "+" + staminaGained;
		}
		else
		{
			StaminaGroup.SetActive(false);
		}
		if (inventorySlotsGained > 0)
		{
			InventorySpaceGroup.SetActive(true);
			InventorySpaceLabel.text = "+" + inventorySlotsGained;
		}
		else
		{
			InventorySpaceGroup.SetActive(false);
		}
		NumericalsParent.Reposition();
		StartCoroutine(PauseThenAnimateOut());
	}

	public IEnumerator PauseThenAnimateOut()
	{
		yield return new WaitForSeconds(2f);
		mRays.loop = false;
		yield return new WaitForSeconds(2f);
		HideLevelUp();
	}

	public void HideLevelUp()
	{
		UICamera.UnlockInput();
		HideTween.Play();
		Singleton<TownController>.Instance.AdvanceIntroState();
	}
}
