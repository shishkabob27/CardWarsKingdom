using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class ScheduleDataManager : DataManager<ScheduleData>
{
	private static ScheduleDataManager _instance;

	private Dictionary<string, List<ScheduleData>> Schedules;

	private bool mInitialized;

	private static List<ScheduleData> sEmptyList;

	public static ScheduleDataManager Instance
	{
		get
		{
			if (_instance == null)
			{
				string path = Path.Combine(SQSettings.CDN_URL, "Blueprints", "db_Schedule.json");
				_instance = new ScheduleDataManager(path);
			}
			return _instance;
		}
	}

	private static List<ScheduleData> EmptyList
	{
		get
		{
			if (sEmptyList == null)
			{
				sEmptyList = new List<ScheduleData>();
			}
			return sEmptyList;
		}
	}

	public ScheduleDataManager(string path)
	{
		base.FilePath = path;
		Schedules = new Dictionary<string, List<ScheduleData>>();
		mInitialized = false;
	}

	public new IEnumerator Load()
	{
		IEnumerator result = base.Load();
		foreach (ScheduleData item in DatabaseArray)
		{
			string category = item.Category;
			List<ScheduleData> list;
			if (Schedules.ContainsKey(category))
			{
				list = Schedules[category];
			}
			else
			{
				list = new List<ScheduleData>();
				Schedules.Add(category, list);
			}
			list.Add(item);
		}
		return result;
	}

	public void Init()
	{
		if (mInitialized)
		{
			return;
		}
		foreach (ScheduleData item in DatabaseArray)
		{
			string category = item.Category;
			List<ScheduleData> list;
			if (Schedules.ContainsKey(category))
			{
				list = Schedules[category];
			}
			else
			{
				list = new List<ScheduleData>();
				Schedules.Add(category, list);
			}
			list.Add(item);
		}
		mInitialized = true;
	}

	public List<ScheduleData> GetItems(string category)
	{
		if (!Schedules.ContainsKey(category))
		{
			return EmptyList;
		}
		return Schedules[category];
	}

	public ScheduleData GetItem(string category, string itemID)
	{
		//Discarded unreachable code: IL_0030
		try
		{
			return Schedules[category].Find((ScheduleData item) => item.ID == itemID);
		}
		catch (Exception)
		{
		}
		return null;
	}

	public List<ScheduleData> GetItemsAvailable(string category, long timeCurr)
	{
		if (!Schedules.ContainsKey(category))
		{
			return EmptyList;
		}
		List<ScheduleData> list = new List<ScheduleData>();
		foreach (ScheduleData item in Schedules[category])
		{
			if (item.IsAvailable(timeCurr))
			{
				list.Add(item);
			}
		}
		return list;
	}

	public List<ScheduleData> GetItemsAvailableAndUnlocked(string category, long timeCurr)
	{
		if (!Schedules.ContainsKey(category))
		{
			return EmptyList;
		}
		List<ScheduleData> list = new List<ScheduleData>();
		foreach (ScheduleData item in Schedules[category])
		{
			if (item.IsAvailable(timeCurr) && item.GetTimeToUnlock(timeCurr) <= 0)
			{
				list.Add(item);
			}
		}
		return list;
	}

	public ScheduleData GetFirstItemAvailableAndUnlocked(string category, long timeCurr)
	{
		if (!Schedules.ContainsKey(category))
		{
			return null;
		}
		foreach (ScheduleData item in Schedules[category])
		{
			if (!item.IsAvailable(timeCurr) || item.GetTimeToUnlock(timeCurr) > 0)
			{
				continue;
			}
			return item;
		}
		return null;
	}

	public bool IsItemAvailableAndUnlocked(string category, string itemID, long timeCurr)
	{
		if (!Schedules.ContainsKey(category))
		{
			return false;
		}
		ScheduleData scheduleData = Schedules[category].Find((ScheduleData item) => item.ID == itemID);
		if (scheduleData == null)
		{
			return false;
		}
		return scheduleData.IsAvailable(timeCurr) && scheduleData.GetTimeToUnlock(timeCurr) <= 0;
	}
}
