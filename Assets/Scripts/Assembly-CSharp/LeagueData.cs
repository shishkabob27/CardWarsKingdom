using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class LeagueData : ILoadableData
{
	public class StartEndTime
	{
		public DateTime StartTime;

		public DateTime EndTime;
	}

	public class QuestBonus
	{
		public QuestBonusType BonusType;

		public DayOfWeek StartDay;

		public TimeSpan StartTime;

		public DayOfWeek EndDay;

		public TimeSpan EndTime;

		public DateTime StartDate;

		public DateTime EndDate;
	}

	private string _ID;

	private string _Name;

	private QuestLineEnum _QuestLine;

	private QuestAvailabilityType _AvailabilityType = QuestAvailabilityType.Unset;

	private List<StartEndTime> _AvailabilityTimes = new List<StartEndTime>();

	private DayOfWeek _WeekdayStartTime;

	private string _BackgroundTexture;

	private string _FrameTexture;

	private string _LeagueIconTexture;

	private List<QuestBonus> _QuestBonuses = new List<QuestBonus>();

	private string HideUntilPassedString;

	public List<QuestData> Quests = new List<QuestData>();

	private static LeagueData mLastPopulatedLeague;

	public string ID
	{
		get
		{
			return _ID;
		}
	}

	public string Name
	{
		get
		{
			return _Name;
		}
	}

	public QuestLineEnum QuestLine
	{
		get
		{
			return _QuestLine;
		}
	}

	public QuestAvailabilityType AvailabilityType
	{
		get
		{
			return _AvailabilityType;
		}
	}

	public List<StartEndTime> AvailabilityTimes
	{
		get
		{
			return _AvailabilityTimes;
		}
	}

	public string BackgroundTexture
	{
		get
		{
			return _BackgroundTexture;
		}
	}

	public string FrameTexture
	{
		get
		{
			return _FrameTexture;
		}
	}

	public string LeagueIconTexture
	{
		get
		{
			return _LeagueIconTexture;
		}
	}

	public bool IsFinalMainLineLeague { get; private set; }

	public LeagueData HideUntilPassed { get; private set; }

	public RandomLeagueInterval RandomInterval { get; private set; }

	public int RandomCount { get; private set; }

	public int RandomDuration { get; private set; }

	public bool RandomizeToHours { get; private set; }

	public int PreviewMinutes { get; private set; }

	public bool NotificationOn { get; private set; }

	public bool LinearDungeon { get; private set; }

	public void Populate(Dictionary<string, object> dict)
	{
		//Discarded unreachable code: IL_02fa
		_ID = TFUtils.LoadString(dict, "ID", string.Empty);
		_Name = TFUtils.LoadLocalizedString(dict, "Name", string.Empty);
		_QuestLine = (QuestLineEnum)(int)Enum.Parse(typeof(QuestLineEnum), TFUtils.LoadString(dict, "QuestLine", string.Empty), true);
		_BackgroundTexture = "UI/Textures/PlayerBackdrops/" + TFUtils.LoadString(dict, "BackgroundTexture", string.Empty);
		_FrameTexture = "UI/Icons_Leagues/" + TFUtils.LoadString(dict, "FrameTexture", string.Empty);
		_LeagueIconTexture = "UI/Icons_DailyDungeons/" + TFUtils.LoadString(dict, "LeagueIconTexture", string.Empty);
		HideUntilPassedString = TFUtils.LoadString(dict, "HideUntil", null);
		LinearDungeon = TFUtils.LoadBool(dict, "LinearDungeon", false);
		RandomCount = TFUtils.LoadInt(dict, "RandomCount", 1);
		string text = TFUtils.LoadString(dict, "RandomInterval", null);
		if (text != null)
		{
			RandomInterval = (RandomLeagueInterval)(int)Enum.Parse(typeof(RandomLeagueInterval), text, true);
		}
		string text2 = TFUtils.LoadString(dict, "RandomDuration", null);
		if (text2 != null && text2.Length > 1)
		{
			if (text2.EndsWith("h"))
			{
				RandomDuration = 3600 * Convert.ToInt32(text2.Substring(0, text2.Length - 1));
				RandomizeToHours = true;
			}
			else if (text2.EndsWith("d"))
			{
				RandomDuration = 86400 * Convert.ToInt32(text2.Substring(0, text2.Length - 1));
				RandomizeToHours = false;
			}
		}
		NotificationOn = TFUtils.LoadBool(dict, "NotificationOn", false);
		string text3 = TFUtils.LoadString(dict, "LeaguePreview", null);
		if (text3 != null)
		{
			if (text3.EndsWith("h"))
			{
				PreviewMinutes = 60 * Convert.ToInt32(text3.Substring(0, text3.Length - 1));
			}
			else if (text3.EndsWith("d"))
			{
				PreviewMinutes = 1440 * Convert.ToInt32(text3.Substring(0, text3.Length - 1));
			}
		}
		if (mLastPopulatedLeague != null && mLastPopulatedLeague.QuestLine == QuestLineEnum.Main && QuestLine != 0)
		{
			mLastPopulatedLeague.IsFinalMainLineLeague = true;
		}
		mLastPopulatedLeague = this;
		int num = 1;
		while (true)
		{
			string text4 = TFUtils.LoadString(dict, "StartTime" + num, null);
			if (text4 == null)
			{
				if (num == 1)
				{
					_AvailabilityType = QuestAvailabilityType.Always;
				}
				break;
			}
			if (text4.ToLower() == "weekend")
			{
				_AvailabilityType = QuestAvailabilityType.Weekend;
				break;
			}
			try
			{
				_WeekdayStartTime = (DayOfWeek)(int)Enum.Parse(typeof(DayOfWeek), text4, true);
				_AvailabilityType = QuestAvailabilityType.Weekday;
			}
			catch
			{
				try
				{
					string s = TFUtils.LoadString(dict, "EndTime" + num, null);
					StartEndTime startEndTime = new StartEndTime();
					startEndTime.StartTime = DateTime.ParseExact(text4, "M/d/yyyy", CultureInfo.InvariantCulture);
					startEndTime.EndTime = DateTime.ParseExact(s, "M/d/yyyy", CultureInfo.InvariantCulture);
					_AvailabilityTimes.Add(startEndTime);
					_AvailabilityType = QuestAvailabilityType.Date;
				}
				catch
				{
				}
				goto IL_0361;
			}
			break;
			IL_0361:
			num++;
		}
		int num2 = 1;
		while (true)
		{
			string text5 = TFUtils.LoadString(dict, "Bonus" + num2, null);
			if (text5 == null)
			{
				break;
			}
			QuestBonus questBonus = new QuestBonus();
			questBonus.BonusType = (QuestBonusType)(int)Enum.Parse(typeof(QuestBonusType), text5, false);
			string text6 = TFUtils.LoadString(dict, "BonusStart" + num2, string.Empty);
			string text7 = TFUtils.LoadString(dict, "BonusEnd" + num2, string.Empty);
			string[] array = text6.Split(',');
			if (array.Length > 1)
			{
				questBonus.StartDate = DateTime.MinValue;
				questBonus.StartDay = (DayOfWeek)(int)Enum.Parse(typeof(DayOfWeek), array[0].Trim(), true);
				questBonus.StartTime = DateTime.Parse(array[1]).TimeOfDay;
				string[] array2 = text7.Split(',');
				questBonus.EndDay = (DayOfWeek)(int)Enum.Parse(typeof(DayOfWeek), array2[0].Trim(), true);
				questBonus.EndTime = DateTime.Parse(array2[1]).TimeOfDay;
			}
			else
			{
				questBonus.StartDate = DateTime.ParseExact(text6, "M/d/yyyy", CultureInfo.InvariantCulture);
				questBonus.EndDate = DateTime.ParseExact(text7, "M/d/yyyy", CultureInfo.InvariantCulture);
			}
			_QuestBonuses.Add(questBonus);
			num2++;
		}
	}

	public void PostLoad()
	{
		if (HideUntilPassedString != null)
		{
			HideUntilPassed = LeagueDataManager.Instance.GetData(HideUntilPassedString);
			HideUntilPassedString = null;
		}
	}

	public void GetTimeStatus(out bool isVisible, out bool isClickable, out bool expired, out string timeText, out string fullTimeText, out DateTime startTime, out DateTime endTime)
	{
		timeText = string.Empty;
		fullTimeText = string.Empty;
		isClickable = true;
		isVisible = true;
		expired = false;
		startTime = (endTime = DateTime.MinValue);
		if (RandomInterval == RandomLeagueInterval.None && (_AvailabilityType == QuestAvailabilityType.Always || _AvailabilityType == QuestAvailabilityType.Unset))
		{
			return;
		}
		DateTime serverTime = TFUtils.ServerTime;
		if (RandomInterval != 0)
		{
			int num = ID.GetHashCode();
			switch (RandomInterval)
			{
			case RandomLeagueInterval.Daily:
				num += serverTime.DayOfYear + 365 * serverTime.Year;
				break;
			case RandomLeagueInterval.Weekly:
				num += serverTime.DayOfYear / 7;
				break;
			case RandomLeagueInterval.Monthly:
				num += serverTime.Month + 12 * serverTime.Year;
				break;
			case RandomLeagueInterval.Yearly:
				num += serverTime.Year;
				break;
			}
			FasterRandom fasterRandom = new FasterRandom(num);
			float[] array = new float[RandomCount];
			for (int i = 0; i < RandomCount; i++)
			{
				array[i] = (float)fasterRandom.NextDouble();
			}
			float num2 = 0f;
			int num3 = 0;
			if (RandomInterval == RandomLeagueInterval.Daily)
			{
				num3 = 86400;
				num2 = (float)(serverTime.TimeOfDay.TotalSeconds / (double)num3);
			}
			else if (RandomInterval == RandomLeagueInterval.Weekly)
			{
				num3 = 604800;
				double num4 = (double)((int)serverTime.DayOfWeek * 86400) + serverTime.TimeOfDay.TotalSeconds;
				num2 = (float)(num4 / (double)num3);
			}
			else if (RandomInterval == RandomLeagueInterval.Monthly)
			{
				num3 = DateTime.DaysInMonth(serverTime.Year, serverTime.Month) * 86400;
				double num5 = (double)(serverTime.Day * 86400) + serverTime.TimeOfDay.TotalSeconds;
				num2 = (float)(num5 / (double)num3);
			}
			else if (RandomInterval == RandomLeagueInterval.Yearly)
			{
				int num6 = ((!DateTime.IsLeapYear(serverTime.Year)) ? 365 : 366);
				num3 = num6 * 86400;
				double num7 = (double)(serverTime.DayOfYear * 86400) + serverTime.TimeOfDay.TotalSeconds;
				num2 = (float)(num7 / (double)num3);
			}
			int value = (int)((float)RandomCount * num2);
			value = Mathf.Clamp(value, 0, RandomCount - 1);
			float num8 = array[value];
			float num9 = num3 / RandomCount;
			if ((float)RandomDuration > num9)
			{
				num9 = RandomDuration;
			}
			float num10 = num8 * (num9 - (float)RandomDuration);
			float num11 = num10 + num9 * (float)value;
			num11 = ((!RandomizeToHours) ? (num11 - num11 % 86400f) : (num11 - num11 % 3600f));
			switch (RandomInterval)
			{
			case RandomLeagueInterval.Daily:
				startTime = serverTime;
				break;
			case RandomLeagueInterval.Weekly:
				startTime = serverTime.AddDays(0 - serverTime.DayOfWeek);
				break;
			case RandomLeagueInterval.Monthly:
				startTime = serverTime.AddDays(-serverTime.Day);
				break;
			case RandomLeagueInterval.Yearly:
				startTime = serverTime.AddDays(-serverTime.DayOfYear);
				break;
			}
			startTime = startTime.AddSeconds(0.0 - serverTime.TimeOfDay.TotalSeconds);
			startTime = startTime.AddSeconds(num11);
			endTime = startTime.AddSeconds(RandomDuration);
		}
		else if (_AvailabilityType == QuestAvailabilityType.Weekday)
		{
			int num12 = (_WeekdayStartTime - serverTime.DayOfWeek + 7) % 7;
			startTime = serverTime.AddDays(num12) - serverTime.TimeOfDay;
			endTime = startTime.AddDays(1.0);
		}
		else if (_AvailabilityType == QuestAvailabilityType.Weekend)
		{
			int num13 = (int)(6 - serverTime.DayOfWeek + 7) % 7;
			if (num13 == 6)
			{
				num13 -= 7;
			}
			startTime = serverTime.AddDays(num13) - serverTime.TimeOfDay;
			endTime = startTime.AddDays(2.0);
		}
		else
		{
			StartEndTime startEndTime = null;
			float f = 0f;
			foreach (StartEndTime availabilityTime in _AvailabilityTimes)
			{
				float num14 = (float)(availabilityTime.StartTime - serverTime).TotalMinutes;
				float num15 = (float)(availabilityTime.EndTime - serverTime).TotalMinutes;
				if (!(num14 > (float)PreviewMinutes) && !(num15 < 0f) && (startEndTime == null || Mathf.Abs(num14) < Mathf.Abs(f)))
				{
					startEndTime = availabilityTime;
					f = num14;
				}
			}
			if (startEndTime == null)
			{
				isClickable = false;
				isVisible = false;
				return;
			}
			startTime = startEndTime.StartTime;
			endTime = startEndTime.EndTime;
		}
		float num16 = (float)(startTime - serverTime).TotalMinutes;
		float num17 = (float)(endTime - serverTime).TotalMinutes;
		if (num16 > 0f)
		{
			if (num16 > (float)PreviewMinutes)
			{
				isClickable = false;
				isVisible = false;
				return;
			}
			timeText = PlayerInfoScript.FormatTimeString((int)num16 * 60);
			fullTimeText = KFFLocalization.Get("!!STARTS_IN_X").Replace("<val1>", timeText);
			isClickable = false;
			isVisible = true;
		}
		else if (num17 < 0f)
		{
			isClickable = false;
			isVisible = false;
		}
		else
		{
			timeText = PlayerInfoScript.FormatTimeString((int)num17 * 60);
			fullTimeText = KFFLocalization.Get("!!TIME_VAL_LEFT").Replace("<val1>", timeText);
			isClickable = true;
			isVisible = true;
		}
	}

	public void GetBonusStatus(out bool bonusActive, out QuestBonusType bonusType, out string timeText)
	{
		if (QuestLine != QuestLineEnum.Special && !Singleton<PlayerInfoScript>.Instance.IsLeagueComplete(this))
		{
			bonusActive = false;
			bonusType = QuestBonusType.None;
			timeText = string.Empty;
			return;
		}
		DateTime serverTime = TFUtils.ServerTime;
		QuestBonus questBonus = null;
		float num = 0f;
		foreach (QuestBonus questBonuse in _QuestBonuses)
		{
			float num2;
			float num3;
			if (questBonuse.StartDate != DateTime.MinValue)
			{
				num2 = (float)(questBonuse.StartDate - serverTime).TotalMinutes;
				num3 = (float)(questBonuse.EndDate - serverTime).TotalMinutes;
			}
			else
			{
				int num4 = (questBonuse.StartDay - serverTime.DayOfWeek + 7) % 7;
				DateTime dateTime = serverTime.AddDays(num4) + questBonuse.StartTime - serverTime.TimeOfDay;
				int num5 = (questBonuse.EndDay - serverTime.DayOfWeek + 7) % 7;
				DateTime dateTime2 = serverTime.AddDays(num5) + questBonuse.EndTime - serverTime.TimeOfDay;
				num2 = (float)(dateTime - serverTime).TotalMinutes;
				num3 = (float)(dateTime2 - serverTime).TotalMinutes;
			}
			if (num2 > 0f)
			{
				if (questBonus == null || num2 < num)
				{
					questBonus = questBonuse;
					num = num2;
				}
			}
			else if (!(num3 < 0f))
			{
				bonusActive = true;
				bonusType = questBonuse.BonusType;
				timeText = string.Format(KFFLocalization.Get("!!EVENT_FOR_TIME"), BonusDisplayString(bonusType), PlayerInfoScript.FormatTimeString((int)num3 * 60));
				return;
			}
		}
		bonusActive = false;
		if (questBonus == null || num > (float)(MiscParams.LeagueBonusPreviewHours * 60))
		{
			bonusType = QuestBonusType.None;
			timeText = string.Empty;
		}
		else
		{
			bonusType = questBonus.BonusType;
			timeText = string.Format(KFFLocalization.Get("!!EVENT_IN_TIME"), BonusDisplayString(bonusType), PlayerInfoScript.FormatTimeString((int)num * 60));
		}
	}

	private string BonusDisplayString(QuestBonusType bonusType)
	{
		switch (bonusType)
		{
		case QuestBonusType.ReducedStamina:
			return KFFLocalization.Get("!!QB_STAMINA");
		case QuestBonusType.BonusXp:
			return KFFLocalization.Get("!!QB_RANKXP");
		case QuestBonusType.BonusSoftCurrencyQuantity:
			return KFFLocalization.Get("!!QB_COINS");
		case QuestBonusType.BonusCreatureDrop:
			return KFFLocalization.Get("!!QB_CREATURES");
		case QuestBonusType.BonusEvoMaterialDrop:
			return KFFLocalization.Get("!!QB_RUNES");
		case QuestBonusType.BonusCardDrop:
			return KFFLocalization.Get("!!QB_CARDS");
		case QuestBonusType.BonusSocialCurrencyDrop:
			return KFFLocalization.Get("!!QB_FANGS");
		default:
			return string.Empty;
		}
	}

	public bool HasOnlyCompletedOneTimeQuests()
	{
		foreach (QuestData quest in Quests)
		{
			if (!quest.IsCompletedOneTimeQuest())
			{
				return false;
			}
		}
		return true;
	}
}
