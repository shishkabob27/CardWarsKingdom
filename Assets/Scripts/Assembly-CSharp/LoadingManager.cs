using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingManager : Singleton<LoadingManager>
{
	public delegate void OnFinishedDelegate();

	private static float lastYieldTime;

	private OnFinishedDelegate OnFinishedCallback;

	private List<IDataManager> LoadableList = new List<IDataManager>();

	private void Start()
	{
		SQSettings.Init();
		Add(TownScheduleDataManager.Instance);
		Add(SpeedUpDataManager.Instance);
		Add(ChatCountryBlacklistDataManager.Instance);
		Add(XPMaterialDataManager.Instance);
		Add(RandomDungeonFloorDataManager.Instance);
		Add(RandomDungeonRewardDataManager.Instance);
		Add(ExpeditionSlotCostDataManager.Instance);
		Add(CustomAIDataManager.Instance);
		Add(PlayerPortraitDataManager.Instance);
		Add(QuickChatDataManager.Instance);
		Add(PlayerTitleDataManager.Instance);
		Add(PlayerBadgeDataManager.Instance);
		Add(CardBackDataManager.Instance);
		Add(PvpRankDataManager.Instance);
		Add(LeagueDataManager.Instance);
		//Add(TapMinigameParams.Instance);
		Add(TutorialBoardDataManager.Instance);
		Add(TutorialDataManager.Instance);
		Add(TutorialCardOverridesDataManager.Instance);
		Add(PlayerRankDataManager.Instance);
		Add(CurrencyPackageDataManager.Instance);
		Add(EvoMaterialDataManager.Instance);
		Add(XPTableDataManager.Instance);
		Add(CreatureStarRatingDataManager.Instance);
		Add(GameEventFXDataManager.Instance);
		Add(StatusDataManager.Instance);
		Add(CardDataManager.Instance);
		Add(LeaderVFXDataManager.Instance);
		Add(LeaderDataManager.Instance);
		Add(CreaturePassiveDataManager.Instance);
		Add(CreatureDataManager.Instance);
		Add(GachaWeightDataManager.Instance);
		Add(GachaSlotDataManager.Instance);
		Add(GachaEventDataManager.Instance);
		Add(SpecialSaleDataManager.Instance);
		Add(CalendarGiftDataManager.Instance);
		Add(MiscParams.Instance);
		Add(QuestLoadoutDataManager.Instance);
		Add(QuestDataManager.Instance);
		Add(TownBuildingDataManager.Instance);
		Add(KeyWordDataManager.Instance);
		Add(AchievementDataManager.Instance);
		Add(ScheduleDataManager.Instance);
		Add(DailyRouletteGiftDataManager.Instance);
		Add(InviteRewardsManager.Instance);
		Add(MailDataManager.Instance);
		Add(HelpDataManager.Instance);
		Add(TipsDataManager.Instance);
		Add(MissionDataManager.Instance);
		Add(NotificationDataManager.Instance);
		Add(VirtualGoodsDataManager.Instance);
		Add(PvpSeasonDataManager.Instance);
		Add(ExpeditionParams.Instance);
		Add(ExpeditionNameDataManager.Instance);
		Add(ExpeditionDifficultyDataManager.Instance);
		Add(ProfanityFilterDataManager.Instance);
		Add(EventTemplateDataManager.Instance);
		Add(EventBannersDataManager.Instance);
		Add(UpsightMilestoneManager.Instance);
	}

	public static bool ShouldYield()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		bool flag = realtimeSinceStartup - lastYieldTime > 0.03f;
		if (flag)
		{
			lastYieldTime = realtimeSinceStartup;
		}
		return flag;
	}

	public void Add(IDataManager item)
	{
		LoadableList.Add(item);
	}

	public void LoadAll(OnFinishedDelegate callback)
	{
		OnFinishedCallback = callback;
		StartCoroutine(LoadAllCo());
	}

	private IEnumerator LoadAllCo()
	{
		foreach (IDataManager ldr in LoadableList)
		{
			if (ldr != null)
			{
				Debug.Log("LoadingManager -- Loading " + ldr.ToString());
				yield return StartCoroutine(ldr.Load());
			}
		}
		if (OnFinishedCallback != null)
		{
			OnFinishedCallback();
		}
	}

	public void UnloadAll()
	{
		foreach (IDataManager loadable in LoadableList)
		{
			if (loadable != null)
			{
				loadable.Unload();
			}
		}
	}
}
