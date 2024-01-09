using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestSelectController : Singleton<QuestSelectController>
{
	public class SpecialLeagueEntry
	{
		public LeagueData League;

		public string TextString;

		public string FullTextString;

		public bool Clickable;

		public DateTime StartTime;

		public DateTime EndTime;

		public bool Expired;

		public SpecialLeagueEntry(LeagueData league, string textString, string fullTextString, bool clickable, bool expired, DateTime startTime, DateTime endTime)
		{
			League = league;
			TextString = textString;
			FullTextString = fullTextString;
			Clickable = clickable;
			StartTime = startTime;
			EndTime = endTime;
			Expired = expired;
		}
	}

	public const string DungeonMapLeague = "DungeonMaps";

	public const string DailyRandomLeague = "DailyRandom";

	public float QuestListOffset = 808f;

	public GameObject LeaguePrefab;

	public GameObject SpecialLeaguePrefab;

	public GameObject QuestPrefab;

	public GameObject RandomBattlePrefab;

	public UITweenController ShowTween;

	public UITweenController ShowQuestsTween;

	public UITweenController HideQuestsTween;

	public UILabel TitleLabel;

	public UILabel PlayerStamina;

	public UILabel PlayerStaminaTimer;

	public UIStreamingGrid LeagueGrid;

	public UIScrollView LeagueScrollView;

	private UIStreamingGridDataSource<LeagueData> mLeagueGridDataSource = new UIStreamingGridDataSource<LeagueData>();

	private UIStreamingGridDataSource<SpecialLeagueEntry> mSpecialLeagueGridDataSource = new UIStreamingGridDataSource<SpecialLeagueEntry>();

	public UIStreamingGrid QuestGrid;

	public UIScrollView QuestScrollView;

	private UIStreamingGridDataSource<QuestData> mQuestGridDataSource = new UIStreamingGridDataSource<QuestData>();

	public GameObject BackButton;

	private QuestSelectLeague mShowingLeague;

	private List<SpecialLeagueEntry> mSpecialLeagueEntries = new List<SpecialLeagueEntry>();

	private bool mShowingSpecialLeagues;

	public GameObject BackgroundMain;

	public GameObject BackgroundSpecial;

	public GameObject ScrollBar;

	public string DailyDungeonLeagueID = string.Empty;

	private QuestSelectQuest selectedQuest;

	public void Populate(string param)
	{
		if (param == "Main")
		{
			TitleLabel.text = KFFLocalization.Get("!!BUILDING_QUESTS");
			mShowingSpecialLeagues = false;
			bool flag = false;
			List<LeagueData> leagues = LeagueDataManager.Instance.GetLeagues(QuestLineEnum.Main);
			mLeagueGridDataSource.SetScrollPos(new Vector2(Singleton<PlayerInfoScript>.Instance.SaveData.QuestSelectScrollPos, 0f), LeagueGrid);
			mLeagueGridDataSource.Init(LeagueGrid, LeaguePrefab, leagues);
			LeagueData leagueData = null;
			LeagueData leagueData2 = null;
			LeagueData leagueData3 = null;
			foreach (LeagueData item in leagues)
			{
				if (Singleton<PlayerInfoScript>.Instance.IsLeagueUnlocking(item))
				{
					leagueData3 = item;
				}
				else if (Singleton<PlayerInfoScript>.Instance.IsLeagueUnlocked(item))
				{
					leagueData2 = item;
				}
			}
			if (leagueData3 != null)
			{
				GameObject gameObject = mLeagueGridDataSource.FindPrefab(leagueData3);
				QuestSelectLeague component = gameObject.GetComponent<QuestSelectLeague>();
				if (component.Lock != null)
				{
					component.Lock.SetActive(true);
				}
				if (component.HideWhenLocked != null)
				{
					component.HideWhenLocked.SetActive(false);
				}
				GameObject gameObject2 = mLeagueGridDataSource.FindPrefab(leagueData2);
				QuestSelectLeague component2 = gameObject2.GetComponent<QuestSelectLeague>();
				component2.CompleteTween.PlayWithCallback(component.DelayedShowUnlockedElements);
			}
			else
			{
				QuestData questData = leagueData2.Quests.Find((QuestData m) => Singleton<PlayerInfoScript>.Instance.IsQuestUnlocking(m));
				if (questData != null)
				{
					flag = true;
					GameObject gameObject3 = mLeagueGridDataSource.FindPrefab(leagueData2);
					OnLeagueClicked(gameObject3.GetComponent<QuestSelectLeague>());
					GameObject gameObject4 = mQuestGridDataSource.FindPrefab(questData);
					QuestSelectQuest component3 = gameObject4.GetComponent<QuestSelectQuest>();
					component3.DelayedShowUnlockedElements();
				}
			}
			Singleton<PlayerInfoScript>.Instance.SaveData.TopShownCompletedQuestId = Singleton<PlayerInfoScript>.Instance.SaveData.TopCompletedQuestId;
			BackgroundMain.SetActive(true);
			BackgroundSpecial.SetActive(false);
			BackButton.SetActive(false);
			Singleton<TutorialController>.Instance.AdvanceIfTargetingBuilding("TBuilding_Quests");
			if (flag)
			{
				Singleton<TutorialController>.Instance.AdvanceIfOnState("SC_TapLeague");
				Singleton<TutorialController>.Instance.AdvanceIfOnState("Q3_LeagueList");
				Singleton<TutorialController>.Instance.AdvanceIfOnState("Q2_LeagueList");
			}
		}
		else if (param == "Special")
		{
			TitleLabel.text = KFFLocalization.Get("!!BUILDING_DUNGEON");
			DetachedSingleton<DailyRandomDungeonManager>.Instance.BuildDailyRandomBattles();
			mShowingSpecialLeagues = true;
			mSpecialLeagueEntries = BuildSpecialLeagueList();
			mSpecialLeagueGridDataSource.Init(LeagueGrid, SpecialLeaguePrefab, mSpecialLeagueEntries, true);
			BackgroundMain.SetActive(false);
			BackgroundSpecial.SetActive(true);
			BackButton.SetActive(false);
			Singleton<TutorialController>.Instance.AdvanceIfTargetingBuilding("TBuilding_Dungeon");
			if (!Singleton<TutorialController>.Instance.IsAnyTutorialActive())
			{
				UpsightRequester.RequestContent("enter_dungeon");
			}
			if (DailyDungeonLeagueID != string.Empty)
			{
				foreach (SpecialLeagueEntry mSpecialLeagueEntry in mSpecialLeagueEntries)
				{
					if (mSpecialLeagueEntry.League.ID == DailyDungeonLeagueID)
					{
						DailyDungeonLeagueID = string.Empty;
						GameObject gameObject5 = mSpecialLeagueGridDataSource.FindPrefab(mSpecialLeagueEntry);
						OnLeagueClicked(gameObject5.GetComponent<QuestSelectLeague>());
						break;
					}
				}
			}
		}
		UpdateTimers();
	}

	public void Unload()
	{
		Singleton<TownHudController>.Instance.ReturnToTownView();
		mLeagueGridDataSource.Clear();
		mQuestGridDataSource.Clear();
		mSpecialLeagueGridDataSource.Clear();
		mShowingLeague = null;
	}

	public void Show(string param)
	{
		ShowTween.Play();
		Populate(param);
	}

	private void Update()
	{
		UpdateTimers();
		if (LeagueGrid.gameObject.activeInHierarchy)
		{
			if (mShowingSpecialLeagues)
			{
				CheckForSpecialLeagueChanges();
			}
			else
			{
				Singleton<PlayerInfoScript>.Instance.SaveData.QuestSelectScrollPos = mLeagueGridDataSource.CurrentScrollDistance();
			}
			CheckLeagueBonuses();
		}
	}

	private void CheckForSpecialLeagueChanges()
	{
		List<SpecialLeagueEntry> list = BuildSpecialLeagueList();
		bool flag = list.Count == mSpecialLeagueEntries.Count;
		if (flag)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].League != mSpecialLeagueEntries[i].League)
				{
					flag = true;
					break;
				}
				mSpecialLeagueEntries[i].Clickable = list[i].Clickable;
				mSpecialLeagueEntries[i].TextString = list[i].TextString;
				mSpecialLeagueEntries[i].FullTextString = list[i].FullTextString;
			}
		}
		if (!flag)
		{
			if (mShowingLeague != null)
			{
				mShowingLeague = null;
				HideQuestsTween.Play();
			}
			mSpecialLeagueEntries = list;
			mSpecialLeagueGridDataSource.Init(LeagueGrid, SpecialLeaguePrefab, mSpecialLeagueEntries);
			return;
		}
		for (int j = 0; j < LeagueGrid.transform.childCount; j++)
		{
			Transform child = LeagueGrid.transform.GetChild(j);
			if (child.gameObject.activeInHierarchy)
			{
				QuestSelectLeague component = child.GetComponent<QuestSelectLeague>();
				component.PopulateSpecialLeague();
			}
		}
	}

	public static List<SpecialLeagueEntry> BuildSpecialLeagueList()
	{
		List<SpecialLeagueEntry> list = new List<SpecialLeagueEntry>();
		List<LeagueData> leagues = LeagueDataManager.Instance.GetLeagues(QuestLineEnum.Special);
		foreach (LeagueData item in leagues)
		{
			if (item.ID == "DungeonMaps")
			{
				if (Singleton<PlayerInfoScript>.Instance.SaveData.DungeonMaps.Count > 0)
				{
					list.Add(new SpecialLeagueEntry(item, string.Empty, string.Empty, true, false, DateTime.MinValue, DateTime.MaxValue));
				}
			}
			else if (item.ID == "DailyRandom")
			{
				list.Add(new SpecialLeagueEntry(item, string.Empty, string.Empty, true, false, DateTime.MinValue, DateTime.MaxValue));
			}
			else if (!item.HasOnlyCompletedOneTimeQuests())
			{
				bool isVisible;
				bool isClickable;
				bool expired;
				string timeText;
				string fullTimeText;
				DateTime startTime;
				DateTime endTime;
				item.GetTimeStatus(out isVisible, out isClickable, out expired, out timeText, out fullTimeText, out startTime, out endTime);
				if (isVisible)
				{
					list.Add(new SpecialLeagueEntry(item, timeText, fullTimeText, isClickable, expired, startTime, endTime));
				}
			}
		}
		list.Sort(delegate(SpecialLeagueEntry a, SpecialLeagueEntry b)
		{
			if (a.Clickable != b.Clickable)
			{
				return b.Clickable.CompareTo(a.Clickable);
			}
			if (a.League.AvailabilityType != b.League.AvailabilityType)
			{
				return a.League.AvailabilityType.CompareTo(b.League.AvailabilityType);
			}
			if (a.Expired != b.Expired)
			{
				return a.Expired.CompareTo(b.Expired);
			}
			return a.Clickable ? a.EndTime.CompareTo(b.EndTime) : a.StartTime.CompareTo(b.StartTime);
		});
		return list;
	}

	private void CheckLeagueBonuses()
	{
		bool stamina = false;
		for (int i = 0; i < LeagueGrid.transform.childCount; i++)
		{
			Transform child = LeagueGrid.transform.GetChild(i);
			if (!child.gameObject.activeInHierarchy)
			{
				continue;
			}
			QuestSelectLeague component = child.GetComponent<QuestSelectLeague>();
			bool bonusActive;
			QuestBonusType bonusType;
			string timeText;
			component.League.GetBonusStatus(out bonusActive, out bonusType, out timeText);
			if (component == mShowingLeague && bonusActive && bonusType == QuestBonusType.ReducedStamina)
			{
				stamina = true;
			}
			component.BonusText.text = timeText;
			if (bonusActive)
			{
				if (!component.FlashBonus.AnyTweenPlaying())
				{
					component.FlashBonus.Play();
				}
			}
			else
			{
				component.FlashBonus.StopAndReset();
			}
		}
		if (!(mShowingLeague != null))
		{
			return;
		}
		for (int j = 0; j < QuestGrid.transform.childCount; j++)
		{
			QuestSelectQuest component2 = QuestGrid.transform.GetChild(j).GetComponent<QuestSelectQuest>();
			if (component2.gameObject.activeInHierarchy)
			{
				component2.SetStamina(stamina);
			}
		}
	}

	public void OnLeagueClicked(QuestSelectLeague clickedLeague)
	{
		if (mShowingLeague == null)
		{
			clickedLeague.EnterExitLabel.text = KFFLocalization.Get(clickedLeague.ExitText);
			ExpandLeague(clickedLeague);
		}
		else
		{
			clickedLeague.EnterExitLabel.text = KFFLocalization.Get(clickedLeague.EnterText);
			CollapseLeague();
		}
	}

	private void ExpandLeague(QuestSelectLeague league)
	{
		mShowingLeague = league;
		Vector3 localPosition = mShowingLeague.transform.localPosition;
		float x = LeagueScrollView.transform.localPosition.x;
		float num = localPosition.x + x;
		ScrollBar.SetActive(false);
		for (int i = 0; i < LeagueGrid.transform.childCount; i++)
		{
			QuestSelectLeague component = LeagueGrid.transform.GetChild(i).GetComponent<QuestSelectLeague>();
			if (component.gameObject.activeInHierarchy)
			{
				Vector3 vector = (component.StoredPosition = component.transform.localPosition);
				TweenPosition component2 = component.MoveTween.GetComponent<TweenPosition>();
				component2.target = component.gameObject.GetComponent<UIWidget>();
				component2.from = vector;
				component2.to = vector;
				component2.to.x -= num;
				if (vector.x > localPosition.x)
				{
					component2.to.x += QuestListOffset;
				}
				component.MoveTween.Play();
			}
		}
		ShowQuestsTween.Play();
		if (league.League.ID == "DungeonMaps")
		{
			List<QuestData> list = new List<QuestData>();
			foreach (string key in Singleton<PlayerInfoScript>.Instance.SaveData.DungeonMaps.Keys)
			{
				QuestData data = QuestDataManager.Instance.GetData(key);
				if (data != null)
				{
					list.Add(data);
				}
			}
			mQuestGridDataSource.Init(QuestGrid, QuestPrefab, list, true);
		}
		else if (league.League.QuestLine == QuestLineEnum.Special)
		{
			List<QuestData> dataList = mShowingLeague.League.Quests.FindAll((QuestData m) => !m.IsCompletedOneTimeQuest());
			if (league.League.ID == "DailyRandom")
			{
				mQuestGridDataSource.Init(QuestGrid, RandomBattlePrefab, dataList, true);
			}
			else
			{
				mQuestGridDataSource.Init(QuestGrid, QuestPrefab, dataList, true);
			}
		}
		else
		{
			mQuestGridDataSource.Init(QuestGrid, QuestPrefab, mShowingLeague.League.Quests, true);
		}
		BackButton.SetActive(true);
	}

	public void CollapseLeague()
	{
		ScrollBar.SetActive(true);
		for (int i = 0; i < LeagueGrid.transform.childCount; i++)
		{
			QuestSelectLeague component = LeagueGrid.transform.GetChild(i).GetComponent<QuestSelectLeague>();
			if (component.gameObject.activeInHierarchy)
			{
				Vector3 localPosition = component.transform.localPosition;
				TweenPosition component2 = component.MoveTween.GetComponent<TweenPosition>();
				component2.from = component.transform.localPosition;
				component2.to = component.StoredPosition;
				component.MoveTween.Play();
				if (component != mShowingLeague && component.MoveAlphaTween != null)
				{
					component.MoveAlphaTween.PlayReverse();
				}
				mShowingLeague = null;
			}
		}
		HideQuestsTween.Play();
		BackButton.SetActive(false);
		ResetAllLeagueButtons();
	}

	public void OnQuestClicked(QuestSelectQuest questObject)
	{
		if (Singleton<PlayerInfoScript>.Instance.IsQuestUnlocked(questObject.Quest))
		{
			Singleton<PlayerInfoScript>.Instance.StateData.CurrentActiveQuest = questObject.Quest;
			Singleton<PlayerInfoScript>.Instance.StateData.DungeonMapQuest = mShowingLeague.League.ID == "DungeonMaps";
			selectedQuest = questObject;
			Singleton<PreMatchController>.Instance.Show();
		}
	}

	private void UpdateTimers()
	{
		if (PlayerStamina.gameObject.activeInHierarchy)
		{
			int currentStamina;
			int maxStamina;
			int secondsUntilNextStamina;
			DetachedSingleton<StaminaManager>.Instance.GetStaminaInfo(StaminaType.Quests, out currentStamina, out maxStamina, out secondsUntilNextStamina);
			PlayerStamina.text = currentStamina + " / " + maxStamina;
			PlayerStaminaTimer.text = ((secondsUntilNextStamina <= 0) ? string.Empty : PlayerInfoScript.BuildTimerString(secondsUntilNextStamina));
		}
	}

	public GameObject GetLeagueObject(string leagueId)
	{
		for (int i = 0; i < LeagueGrid.transform.childCount; i++)
		{
			QuestSelectLeague component = LeagueGrid.transform.GetChild(i).GetComponent<QuestSelectLeague>();
			if (component.League.ID == leagueId)
			{
				return component.gameObject;
			}
		}
		return null;
	}

	public GameObject GetQuestObject(string questId)
	{
		for (int i = 0; i < QuestGrid.transform.childCount; i++)
		{
			QuestSelectQuest component = QuestGrid.transform.GetChild(i).GetComponent<QuestSelectQuest>();
			if (component.Quest.ID == questId)
			{
				GameObject gameObject = component.gameObject.FindInChildren("Button");
				if (gameObject != null)
				{
					return gameObject;
				}
				return component.gameObject;
			}
		}
		return null;
	}

	public void OnLeagueDrag()
	{
		if (mShowingLeague != null)
		{
			CollapseLeague();
			ResetAllLeagueButtons();
		}
	}

	private void ResetAllLeagueButtons()
	{
		for (int i = 0; i < LeagueGrid.transform.childCount; i++)
		{
			QuestSelectLeague component = LeagueGrid.transform.GetChild(i).GetComponent<QuestSelectLeague>();
			component.EnterExitLabel.text = KFFLocalization.Get(component.EnterText);
			ButtonDepressEffect component2 = LeagueGrid.transform.GetChild(i).GetComponent<ButtonDepressEffect>();
			component2.ResetDepression();
		}
	}

	public void OnReturnShowBossFX()
	{
		if (selectedQuest != null)
		{
			selectedQuest.ShowFX();
		}
	}

	public GameObject GetHighestLeagueObject()
	{
		for (int i = 0; i < LeagueGrid.transform.childCount; i++)
		{
			QuestSelectLeague component = LeagueGrid.transform.GetChild(i).GetComponent<QuestSelectLeague>();
			if (component.gameObject.activeInHierarchy && !Singleton<PlayerInfoScript>.Instance.IsLeagueUnlocked(component.League))
			{
				return LeagueGrid.transform.GetChild(i - 1).gameObject;
			}
		}
		return null;
	}

	public GameObject GetHighestQuestObject()
	{
		GameObject result = null;
		for (int i = 0; i < QuestGrid.transform.childCount; i++)
		{
			QuestSelectQuest component = QuestGrid.transform.GetChild(i).GetComponent<QuestSelectQuest>();
			if (!component.gameObject.activeInHierarchy)
			{
				continue;
			}
			if (!Singleton<PlayerInfoScript>.Instance.IsQuestUnlocked(component.Quest))
			{
				GameObject gameObject = QuestGrid.transform.GetChild(i - 1).gameObject.FindInChildren("Button");
				if (gameObject != null)
				{
					return gameObject;
				}
				return QuestGrid.transform.GetChild(i - 1).gameObject;
			}
			GameObject gameObject2 = QuestGrid.transform.GetChild(i).gameObject.FindInChildren("Button");
			result = ((!(gameObject2 != null)) ? QuestGrid.transform.GetChild(i).gameObject : gameObject2);
		}
		return result;
	}

	public bool ShowingDungeonMapsLeague()
	{
		return mShowingLeague != null && mShowingLeague.League.QuestLine == QuestLineEnum.Special && mShowingLeague.League.ID == "DungeonMaps";
	}
}
