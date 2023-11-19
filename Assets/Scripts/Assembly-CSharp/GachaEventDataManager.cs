using System;
using System.Collections.Generic;
using System.IO;

public class GachaEventDataManager : DataManager<GachaEventData>
{
	public class EventStatus
	{
		public GachaEventData Event;

		public string DisplayText;

		public bool Active;
	}

	private static GachaEventDataManager _instance;

	public static GachaEventDataManager Instance
	{
		get
		{
			if (_instance == null)
			{
				string path = Path.Combine("Blueprints", "db_GachaEvents.json");
				_instance = new GachaEventDataManager(path);
			}
			return _instance;
		}
	}

	public GachaEventDataManager(string path)
	{
		base.FilePath = path;
		AddDependency(GachaWeightDataManager.Instance);
		AddDependency(GachaSlotDataManager.Instance);
	}

	public List<EventStatus> GetCurrentEvents()
	{
		List<EventStatus> list = new List<EventStatus>();
		if (Singleton<TutorialController>.Instance.IsBlockActive("UseGacha"))
		{
			return list;
		}
		DateTime serverTime = TFUtils.ServerTime;
		GachaEventData eventData;
		foreach (GachaEventData item in GetDatabase())
		{
			eventData = item;
			float num = (float)(eventData.StartTime - serverTime).TotalMinutes;
			float num2 = (float)(eventData.EndTime - serverTime).TotalMinutes;
			if (num <= 0f && num2 >= 0f)
			{
				list.RemoveAll(delegate(EventStatus existingEvent)
				{
					if (existingEvent.Event.Slot == eventData.Slot)
					{
						if (existingEvent.Active)
						{
						}
						return true;
					}
					return false;
				});
				EventStatus eventStatus = new EventStatus();
				eventStatus.Event = eventData;
				eventStatus.Active = true;
				eventStatus.DisplayText = string.Format(KFFLocalization.Get("!!EVENT_FOR_TIME"), eventData.Name, PlayerInfoScript.FormatTimeString((int)num2 * 60));
				list.Add(eventStatus);
			}
			else if (num > 0f && num <= (float)(MiscParams.SpecialQuestPreviewDays * 24 * 60) && !list.Contains((EventStatus existingEvent) => existingEvent.Event.Slot == eventData.Slot))
			{
				EventStatus eventStatus2 = new EventStatus();
				eventStatus2.Event = eventData;
				eventStatus2.Active = false;
				eventStatus2.DisplayText = string.Format(KFFLocalization.Get("!!EVENT_IN_TIME"), eventData.Name, PlayerInfoScript.FormatTimeString((int)num * 60));
				list.Add(eventStatus2);
			}
		}
		return list;
	}

	public EventStatus GetCurrentEvent(GachaSlotData slot)
	{
		DateTime serverTime = TFUtils.ServerTime;
		GachaEventData gachaEventData = null;
		bool flag = false;
		int num = 0;
		foreach (GachaEventData item in GetDatabase())
		{
			if (item.Slot == slot)
			{
				float num2 = (float)(item.StartTime - serverTime).TotalMinutes;
				float num3 = (float)(item.EndTime - serverTime).TotalMinutes;
				if (num2 <= 0f && num3 >= 0f)
				{
					gachaEventData = item;
					flag = true;
					num = (int)num3;
				}
				else if (num2 > 0f && num2 <= (float)(MiscParams.SpecialQuestPreviewDays * 24 * 60) && gachaEventData == null)
				{
					gachaEventData = item;
					flag = false;
					num = (int)num3;
				}
			}
		}
		if (gachaEventData != null)
		{
			EventStatus eventStatus = new EventStatus();
			eventStatus.Event = gachaEventData;
			eventStatus.Active = flag;
			if (flag)
			{
				eventStatus.DisplayText = string.Format(KFFLocalization.Get("!!EVENT_FOR_TIME"), gachaEventData.Name, PlayerInfoScript.FormatTimeString(num * 60));
			}
			else
			{
				eventStatus.DisplayText = string.Format(KFFLocalization.Get("!!EVENT_IN_TIME"), gachaEventData.Name, PlayerInfoScript.FormatTimeString(num * 60));
			}
			return eventStatus;
		}
		return null;
	}
}
