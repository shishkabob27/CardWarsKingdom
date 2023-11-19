using System.Collections.Generic;
using UnityEngine;

public class TownNotificationController : Singleton<TownNotificationController>
{
	private enum NotificationType
	{
		DailyMissionTime,
		DungeonTime,
		PvpSeasonTime,
		GachaEvent,
		HeroesToBuy,
		UnreadMail,
		CreaturesToEvo,
		NextBattle,
		AvailableExpeditions,
		Count
	}

	public float InitialTime;

	public float TimeBetweenNotifications;

	public float NotificationTypeCooldown;

	public float BuildingYOffset;

	public float PopupXOffset;

	public UITweenController ShowPopup;

	public UILabel PopupText;

	public Transform PopupObject;

	public GameObject LeftPointingGroup;

	public GameObject RightPointingGroup;

	public UIWidget ContentsParent;

	private float mGlobalTimer = -1f;

	private float[] mNotificationTimers = new float[9];

	private Transform mAttachedTransform;

	private int mTownLayer;

	private void Start()
	{
		mTownLayer = LayerMask.NameToLayer("3DFrontEnd");
	}

	private void Update()
	{
		if (!SessionManager.Instance.IsLoadDataDone() || !Singleton<PlayerInfoScript>.Instance.IsInitialized || !Singleton<TownController>.Instance.IsIntroDone() || Singleton<TutorialController>.Instance.IsAnyTutorialActive())
		{
			return;
		}
		bool flag = true;
		if (ChatWindowController.Instance.Expanded)
		{
			flag = false;
		}
		if (Singleton<MouseOrbitCamera>.Instance.IsZoomedInToBuilding())
		{
			flag = false;
		}
		if (flag)
		{
			if (mGlobalTimer == -1f)
			{
				mGlobalTimer = InitialTime;
			}
			for (int i = 0; i < mNotificationTimers.Length; i++)
			{
				mNotificationTimers[i] -= Time.deltaTime;
				if (mNotificationTimers[i] < 0f)
				{
					mNotificationTimers[i] = 0f;
				}
			}
			mGlobalTimer -= Time.deltaTime;
			if (mGlobalTimer <= 0f)
			{
				mGlobalTimer = TimeBetweenNotifications;
				ShowNextNotification();
			}
		}
		else
		{
			mGlobalTimer = -1f;
		}
		if (mAttachedTransform != null)
		{
			UpdatePopupPosition();
		}
	}

	private void UpdatePopupPosition()
	{
		Vector3 position;
		if (mAttachedTransform.gameObject.layer == mTownLayer)
		{
			Vector2 vector = Singleton<TownController>.Instance.GetTownCam().WorldToScreenPoint(mAttachedTransform.position);
			Vector3 vector2 = Singleton<TownController>.Instance.GetUICam().ScreenToWorldPoint(vector);
			vector2.y += BuildingYOffset;
			position = vector2;
		}
		else
		{
			position = mAttachedTransform.position;
		}
		if (ContentsParent.pivot == UIWidget.Pivot.Right)
		{
			position.x += PopupXOffset;
		}
		else
		{
			position.x -= PopupXOffset;
		}
		position.z = 0f;
		PopupObject.position = position;
	}

	private void ShowNextNotification()
	{
		List<NotificationType> list = new List<NotificationType>();
		AddIfOffCooldown(list, NotificationType.DailyMissionTime);
		if (Singleton<PlayerInfoScript>.Instance.IsFeatureUnlocked("TBuilding_Dungeon"))
		{
			AddIfOffCooldown(list, NotificationType.DungeonTime);
		}
		if (Singleton<PlayerInfoScript>.Instance.CanPvp() && Singleton<PlayerInfoScript>.Instance.SaveData.ActivePvpSeason != null && !Singleton<PlayerInfoScript>.Instance.SaveData.ActivePvpSeason.IsExpired())
		{
			AddIfOffCooldown(list, NotificationType.PvpSeasonTime);
		}
		List<GachaEventDataManager.EventStatus> currentEvents = GachaEventDataManager.Instance.GetCurrentEvents();
		GachaEventDataManager.EventStatus eventStatus = null;
		if (currentEvents.Count > 0)
		{
			eventStatus = currentEvents.RandomElement();
		}
		if (eventStatus != null)
		{
			AddIfOffCooldown(list, NotificationType.GachaEvent);
		}
		int num = 0;
		if (Singleton<PlayerInfoScript>.Instance.CanBuyLeaders())
		{
			num = LeaderDataManager.Instance.GetDatabase().FindAll((LeaderData m) => m.ShowInSaleList() && !Singleton<PlayerInfoScript>.Instance.IsLeaderUnlocked(m)).Count;
			if (num > 0)
			{
				AddIfOffCooldown(list, NotificationType.HeroesToBuy);
			}
		}
		int num2 = 0;
		if (Singleton<PlayerInfoScript>.Instance.IsFeatureUnlocked("TBuilding_Social"))
		{
			num2 = Singleton<PlayerInfoScript>.Instance.StateData.BadgeCounts[0];
			if (num2 > 0)
			{
				AddIfOffCooldown(list, NotificationType.UnreadMail);
			}
		}
		int num3 = 0;
		if (Singleton<PlayerInfoScript>.Instance.IsFeatureUnlocked("Lab_Evo"))
		{
			num3 = Singleton<PlayerInfoScript>.Instance.StateData.BadgeCounts[1];
			if (num3 > 0)
			{
				AddIfOffCooldown(list, NotificationType.CreaturesToEvo);
			}
		}
		if (!Singleton<PlayerInfoScript>.Instance.IsQuestComplete(QuestData.HighestMainLineQuest))
		{
			AddIfOffCooldown(list, NotificationType.NextBattle);
		}
		int num4 = 0;
		if (num4 > 0)
		{
			AddIfOffCooldown(list, NotificationType.AvailableExpeditions);
		}
		if (list.Count == 0)
		{
			return;
		}
		ShowPopup.Play();
		Singleton<SLOTAudioManager>.Instance.PlaySound("SFX_NotificationShow");
		NotificationType notificationType = list[Random.Range(0, list.Count)];
		mNotificationTimers[(int)notificationType] = NotificationTypeCooldown;
		switch (notificationType)
		{
		case NotificationType.DailyMissionTime:
		{
			string newValue = PlayerInfoScript.FormatTimeString((int)DetachedSingleton<MissionManager>.Instance.GetMinutesUntilQuestRefresh() * 60);
			PopupText.text = KFFLocalization.Get("!!POPUP_DAILY_MISSIONS").Replace("<val1>", newValue);
			mAttachedTransform = Singleton<TownHudController>.Instance.MissionsButton.transform;
			break;
		}
		case NotificationType.DungeonTime:
		{
			List<QuestSelectController.SpecialLeagueEntry> list2 = QuestSelectController.BuildSpecialLeagueList().FindAll((QuestSelectController.SpecialLeagueEntry m) => m.Clickable);
			QuestSelectController.SpecialLeagueEntry specialLeagueEntry = list2[Random.Range(0, list2.Count)];
			PopupText.text = KFFLocalization.Get("!!POPUP_DUNGEON").Replace("<val1>", specialLeagueEntry.League.Name).Replace("<val2>", specialLeagueEntry.TextString);
			mAttachedTransform = Singleton<TownController>.Instance.GetBuildingScript("TBuilding_Dungeon").NameBarAttachPoint.transform;
			break;
		}
		case NotificationType.PvpSeasonTime:
		{
			string durationString = Singleton<PlayerInfoScript>.Instance.SaveData.ActivePvpSeason.GetDurationString();
			PopupText.text = KFFLocalization.Get("!!POPUP_PVP_SEASON").Replace("<val1>", durationString);
			mAttachedTransform = Singleton<TownController>.Instance.GetBuildingScript("TBuilding_PVP").NameBarAttachPoint.transform;
			break;
		}
		case NotificationType.GachaEvent:
			PopupText.text = eventStatus.DisplayText;
			mAttachedTransform = Singleton<TownController>.Instance.GetBuildingScript("TBuilding_Gacha").NameBarAttachPoint.transform;
			break;
		case NotificationType.HeroesToBuy:
			PopupText.text = KFFLocalization.Get("!!POPUP_HEROES").Replace("<val1>", num.ToString());
			mAttachedTransform = Singleton<TownController>.Instance.GetBuildingScript("TBuilding_Store").NameBarAttachPoint.transform;
			break;
		case NotificationType.UnreadMail:
			PopupText.text = KFFLocalization.Get("!!POPUP_UNREAD_MAIL").Replace("<val1>", num2.ToString());
			mAttachedTransform = Singleton<TownController>.Instance.GetBuildingScript("TBuilding_Social").NameBarAttachPoint.transform;
			break;
		case NotificationType.CreaturesToEvo:
			PopupText.text = KFFLocalization.Get("!!POPUP_EVO_CREATURES").Replace("<val1>", num3.ToString());
			mAttachedTransform = Singleton<TownController>.Instance.GetBuildingScript("TBuilding_Lab").NameBarAttachPoint.transform;
			break;
		case NotificationType.NextBattle:
		{
			QuestData highestUnlockedMainLineQuest = Singleton<PlayerInfoScript>.Instance.GetHighestUnlockedMainLineQuest();
			PopupText.text = KFFLocalization.Get("!!POPUP_NEXT_BATTLE").Replace("<val1>", highestUnlockedMainLineQuest.LevelName);
			mAttachedTransform = Singleton<TownController>.Instance.GetBuildingScript("TBuilding_Quests").NameBarAttachPoint.transform;
			break;
		}
		case NotificationType.AvailableExpeditions:
			PopupText.text = KFFLocalization.Get("!!POPUP_AVAILABLE_EXPEDITIONS").Replace("<val1>", num4.ToString());
			mAttachedTransform = Singleton<TownHudController>.Instance.ExpeditionsButton.transform;
			break;
		}
		UpdatePopupPosition();
		if (PopupObject.localPosition.x < 0f)
		{
			RightPointingGroup.SetActive(false);
			LeftPointingGroup.SetActive(true);
			ContentsParent.pivot = UIWidget.Pivot.Right;
		}
		else
		{
			RightPointingGroup.SetActive(true);
			LeftPointingGroup.SetActive(false);
			ContentsParent.pivot = UIWidget.Pivot.Left;
		}
	}

	private void AddIfOffCooldown(List<NotificationType> list, NotificationType item)
	{
		if (mNotificationTimers[(int)item] == 0f)
		{
			list.Add(item);
		}
	}

	public void OnPopupClosed()
	{
		mAttachedTransform = null;
	}

	public void HidePopup()
	{
		mAttachedTransform = null;
		ShowPopup.StopAndReset();
		PopupObject.gameObject.SetActive(false);
	}
}
