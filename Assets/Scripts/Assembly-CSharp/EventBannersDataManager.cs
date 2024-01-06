using System;
using System.IO;

public class EventBannersDataManager : DataManager<EventBannersData>
{
	private static EventBannersDataManager _instance;

	public static EventBannersDataManager Instance
	{
		get
		{
			if (_instance == null)
			{
				string path = Path.Combine(SQSettings.CDN_URL, "Blueprints", "db_EventBanners.json");
				_instance = new EventBannersDataManager(path);
			}
			return _instance;
		}
	}

	public EventBannersDataManager(string path)
	{
		base.FilePath = path;
	}

	public bool CheckBannerDates(EventBannersData data)
	{
		bool flag = false;
		string[] array = data.StartDate.Split('/');
		if (array.Length < 3)
		{
			flag = true;
		}
		string[] array2 = data.EndDate.Split('/');
		if (array2.Length < 3)
		{
			flag = true;
		}
		bool flag2 = false;
		if (flag)
		{
			return flag2;
		}
		DayOfWeek dayOfWeek = DateTime.Today.DayOfWeek;
		string[] array3 = data.SpecificDayRepeats.Split(',');
		for (int i = 0; i < array3.Length; i++)
		{
			switch (int.Parse(array3[i]))
			{
			case 1:
				if (dayOfWeek == DayOfWeek.Sunday)
				{
					flag2 = true;
				}
				break;
			case 2:
				if (dayOfWeek == DayOfWeek.Monday)
				{
					flag2 = true;
				}
				break;
			case 3:
				if (dayOfWeek == DayOfWeek.Tuesday)
				{
					flag2 = true;
				}
				break;
			case 4:
				if (dayOfWeek == DayOfWeek.Wednesday)
				{
					flag2 = true;
				}
				break;
			case 5:
				if (dayOfWeek == DayOfWeek.Thursday)
				{
					flag2 = true;
				}
				break;
			case 6:
				if (dayOfWeek == DayOfWeek.Friday)
				{
					flag2 = true;
				}
				break;
			case 7:
				if (dayOfWeek == DayOfWeek.Saturday)
				{
					flag2 = true;
				}
				break;
			}
		}
		if (!flag2 && array3.Length > 0 && (array3.Length > 1 || int.Parse(array3[0]) != 0))
		{
			return false;
		}
		int month = int.Parse(array[0]);
		int day = int.Parse(array[1]);
		int year = int.Parse(array[2]);
		DateTime dateTime = new DateTime(year, month, day);
		month = int.Parse(array2[0]);
		day = int.Parse(array2[1]);
		year = int.Parse(array2[2]);
		DateTime dateTime2 = new DateTime(year, month, day);
		if (dateTime.CompareTo(DateTime.Today) <= 0 && dateTime2.CompareTo(DateTime.Today) >= 0)
		{
			flag2 = true;
		}
		return flag2;
	}
}
