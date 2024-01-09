using System;
using System.Collections.Generic;
using System.IO;

public class MailDataManager : DataManager<MailData>
{
	private static MailDataManager _instance;

	public Dictionary<string, List<MailData>> GiftData = new Dictionary<string, List<MailData>>();

	private bool mInitialized;

	public static MailDataManager Instance
	{
		get
		{
			if (_instance == null)
			{
				string path = Path.Combine(SQSettings.CDN_URL, "Blueprints", "db_NewsMail.json");
				_instance = new MailDataManager(path);
			}
			return _instance;
		}
	}

	public MailDataManager(string path)
	{
		base.FilePath = path;
		mInitialized = false;
	}

	public bool IsMailApplicable(MailData mail)
	{
		DateTime serverTime = TFUtils.ServerTime;
		if (Singleton<PlayerInfoScript>.Instance.SaveData.HasDeletedMail(mail.ID))
		{
			return false;
		}
		if (!string.IsNullOrEmpty(mail.playerId) && mail.playerId != Singleton<PlayerInfoScript>.Instance.GetPlayerCode())
		{
			return false;
		}
		if (!string.IsNullOrEmpty(mail.Repeatable))
		{
			if (mail.Repeatable == "Monday")
			{
				return serverTime.DayOfWeek == DayOfWeek.Monday;
			}
			if (mail.Repeatable == "Tuesday")
			{
				return serverTime.DayOfWeek == DayOfWeek.Tuesday;
			}
			if (mail.Repeatable == "Wednesday")
			{
				return serverTime.DayOfWeek == DayOfWeek.Wednesday;
			}
			if (mail.Repeatable == "Thursday")
			{
				return serverTime.DayOfWeek == DayOfWeek.Thursday;
			}
			if (mail.Repeatable == "Friday")
			{
				return serverTime.DayOfWeek == DayOfWeek.Friday;
			}
			if (mail.Repeatable == "Saturday")
			{
				return serverTime.DayOfWeek == DayOfWeek.Saturday;
			}
			if (mail.Repeatable == "Sunday")
			{
				return serverTime.DayOfWeek == DayOfWeek.Sunday;
			}
		}
		if (mail.ShowDate != DateTime.MinValue && mail.EndDate != DateTime.MinValue)
		{
			return serverTime >= mail.ShowDate && serverTime <= mail.EndDate;
		}
		if (mail.ShowDate != DateTime.MinValue)
		{
			return serverTime >= mail.ShowDate;
		}
		return false;
	}
}
