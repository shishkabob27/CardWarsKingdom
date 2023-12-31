using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

public class ScheduleData : ILoadableData
{
	public class SchedulePeriod
	{
		public long DateStart { get; private set; }

		public long DateEnd { get; private set; }

		public SchedulePeriod(DateTime dateStart, DateTime dateEnd)
		{
			DateStart = dateStart.Ticks;
			DateEnd = dateEnd.Ticks;
		}

		public override string ToString()
		{
			return string.Format("[SchedulePeriod: DateStart={0}, DateEnd={1}]", new DateTime(DateStart), new DateTime(DateEnd));
		}
	}

	private const int DAYSOFWEEK_NUM = 7;

	private const string DATE_FORMAT = "dd/MM/yyyy";

	private string _ID;

	private string _Category;

	public string ID
	{
		get
		{
			return _ID;
		}
	}

	public string Category
	{
		get
		{
			return _Category;
		}
	}

	public int DayAvailableBits { get; private set; }

	public List<SchedulePeriod> PeriodsAvailable { get; private set; }

	public ScheduleData(string inID)
	{
		_ID = inID;
		DayAvailableBits = 0;
		PeriodsAvailable = null;
	}

	public ScheduleData()
	{
		DayAvailableBits = 0;
		PeriodsAvailable = null;
	}

	public void Populate(Dictionary<string, object> dict)
	{
		_Category = TFUtils.LoadString(dict, "category");
		_ID = TFUtils.LoadString(dict, "item");
		string text = TFUtils.LoadString(dict, "startDate");
		string text2 = TFUtils.LoadString(dict, "endDate");
		bool flag = string.IsNullOrEmpty(text);
		bool flag2 = string.IsNullOrEmpty(text);
		if (flag2 || flag)
		{
			if (!flag2 || !flag)
			{
				Debug.LogWarning("Half complete schedule data. Ignoring: " + _Category + "(" + _ID + ") - " + text + " -> " + text2);
			}
		}
		else if (!AddPeriod(text, text2))
		{
            Debug.LogWarning("Invalid schedule data. Ignoring: " + _Category + "(" + _ID + ") - " + text + " -> " + text2);
		}
	}

	public bool IsAvailable(long timeCurr)
	{
		if (PeriodsAvailable == null)
		{
			return true;
		}
		foreach (SchedulePeriod item in PeriodsAvailable)
		{
			if (timeCurr >= item.DateStart && timeCurr <= item.DateEnd)
			{
				return true;
			}
		}
		return false;
	}

	public long GetTimeToUnlock(long timeCurr)
	{
		if (DayAvailableBits == 0)
		{
			return 0L;
		}
		DateTime dateTime = new DateTime(timeCurr, DateTimeKind.Utc);
		int dayOfWeek = (int)dateTime.DayOfWeek;
		if ((DayAvailableBits & (1 << dayOfWeek)) != 0)
		{
			return 0L;
		}
		int i;
		for (i = 1; (DayAvailableBits & (1 << (dayOfWeek + i) % 7)) == 0; i++)
		{
		}
		DateTime dateTime2 = dateTime.AddDays(i);
		return new DateTime(dateTime2.Year, dateTime2.Month, dateTime2.Day, 0, 0, 0, DateTimeKind.Utc).Ticks - timeCurr;
	}

	public long GetTimeToLock(long timeCurr)
	{
		if (DayAvailableBits == 0 || GetTimeToUnlock(timeCurr) != 0L)
		{
			return long.MaxValue;
		}
		DateTime dateTime = new DateTime(timeCurr, DateTimeKind.Utc);
		int dayOfWeek = (int)dateTime.DayOfWeek;
		int i;
		for (i = 1; (DayAvailableBits & (1 << (dayOfWeek + i) % 7)) != 0; i++)
		{
		}
		DateTime dateTime2 = dateTime.AddDays(i);
		DateTime dateTime3 = new DateTime(dateTime2.Year, dateTime2.Month, dateTime2.Day, 0, 0, 0, DateTimeKind.Utc);
		if (PeriodsAvailable != null)
		{
			foreach (SchedulePeriod item in PeriodsAvailable)
			{
				if (timeCurr >= item.DateStart && timeCurr <= item.DateEnd && item.DateEnd < dateTime3.Ticks)
				{
					dateTime2 = new DateTime(item.DateEnd, DateTimeKind.Utc);
					dateTime3 = new DateTime(dateTime2.Year, dateTime2.Month, dateTime2.Day, 0, 0, 0, DateTimeKind.Utc);
				}
			}
		}
		return dateTime3.Ticks - timeCurr;
	}

	public bool AddPeriod(string startDateString, string endDateString)
	{
		if (TryAddDayPeriod(startDateString, endDateString))
		{
			return true;
		}
		return TryAddDatePeriod(startDateString, endDateString);
	}

	private bool TryAddDayPeriod(string startDateString, string endDateString)
	{
		//Discarded unreachable code: IL_0061, IL_006e
		try
		{
			int num = (int)Enum.Parse(typeof(DayOfWeek), startDateString, true);
			int num2 = (int)Enum.Parse(typeof(DayOfWeek), endDateString, true);
			int num3 = num;
			do
			{
				DayAvailableBits |= 1 << num3;
				num3++;
				if (num3 >= 7)
				{
					num3 = 0;
				}
			}
			while (num3 != num2);
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	private bool TryAddDatePeriod(string startDateString, string endDateString)
	{
		//Discarded unreachable code: IL_006f, IL_007c
		try
		{
			DateTime dateStart = DateTime.ParseExact(startDateString, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
			DateTime dateEnd = DateTime.ParseExact(endDateString, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
			if (PeriodsAvailable == null)
			{
				PeriodsAvailable = new List<SchedulePeriod>();
			}
			if (dateEnd.Ticks <= dateStart.Ticks)
			{
				return false;
			}
			PeriodsAvailable.Add(new SchedulePeriod(dateStart, dateEnd));
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(string.Format("[ScheduleData: ID={0}", ID));
		foreach (SchedulePeriod item in PeriodsAvailable)
		{
			stringBuilder.AppendLine(item.ToString());
		}
		return stringBuilder.ToString();
	}
}
