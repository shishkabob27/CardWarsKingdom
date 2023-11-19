using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class UpsightMilestoneManager : DataManager<UpsightMilestoneData>
{
	private static UpsightMilestoneManager _instance;

	public static UpsightMilestoneManager Instance
	{
		get
		{
			if (_instance == null)
			{
				string path = Path.Combine("Blueprints", "db_UpsightMilestone.json");
				_instance = new UpsightMilestoneManager(path);
			}
			return _instance;
		}
	}

	public UpsightMilestoneManager(string path)
	{
		base.FilePath = path;
	}

	protected override void ParseRows(List<object> jlist)
	{
		UpsightMilestoneData upsightMilestoneData = null;
		char[] separator = new char[2] { ':', ',' };
		foreach (object item in jlist)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)item;
			string text = TFUtils.LoadString(dictionary, "ID", string.Empty);
			if (text == null)
			{
				continue;
			}
			if (text != "^")
			{
				upsightMilestoneData = new UpsightMilestoneData(text);
				if (!Database.ContainsKey(text))
				{
					Database.Add(text, upsightMilestoneData);
				}
				DatabaseArray.Add(upsightMilestoneData);
			}
			else if (upsightMilestoneData != null)
			{
				UpsightConditionData upsightConditionData = new UpsightConditionData();
				upsightConditionData.Suffix = TFUtils.LoadString(dictionary, "Suffix", string.Empty);
				upsightConditionData.Priority = TFUtils.LoadInt(dictionary, "Priority", 0);
				string text2 = TFUtils.LoadString(dictionary, "Condition", string.Empty);
				string[] array = text2.Split(separator);
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = array[i].Trim();
				}
				if (array.Length > 0)
				{
					upsightConditionData.ConditionType = (UpsightConditionData.UpsightMSConditionType)(int)Enum.Parse(typeof(UpsightConditionData.UpsightMSConditionType), array[0], true);
				}
				if (array.Length > 1)
				{
					upsightConditionData.ConditionValue = array[1];
				}
				upsightMilestoneData.conditionData.Add(upsightConditionData);
				upsightMilestoneData.conditionData.OrderBy((UpsightConditionData x) => x.Priority);
			}
		}
	}

	public List<UpsightConditionData> GetConditionData(string ID)
	{
		UpsightMilestoneData value = null;
		if (Database.TryGetValue(ID, out value))
		{
			return value.conditionData;
		}
		return null;
	}

	public string GetRequestIDWithCondition(string ID)
	{
		string text = ID;
		List<UpsightConditionData> conditionData = GetConditionData(ID);
		if (conditionData != null)
		{
			string text2 = string.Empty;
			bool flag = false;
			string leaderID;
			foreach (UpsightConditionData item in conditionData)
			{
				switch (item.ConditionType)
				{
				case UpsightConditionData.UpsightMSConditionType.BuyHero:
				{
					bool flag2 = false;
					if (!Singleton<PlayerInfoScript>.Instance.CanBuyLeaders())
					{
						flag2 = true;
					}
					leaderID = item.ConditionValue;
					LeaderData leaderData = LeaderDataManager.Instance.GetDatabase().Find((LeaderData m) => m.ID == leaderID);
					if (leaderData != null && Singleton<PlayerInfoScript>.Instance.IsLeaderUnlocked(leaderData))
					{
						flag2 = true;
					}
					if (flag2)
					{
						text2 += item.Suffix;
						flag = true;
					}
					break;
				}
				case UpsightConditionData.UpsightMSConditionType.Feature:
					if (!Singleton<PlayerInfoScript>.Instance.IsFeatureUnlocked(item.ConditionValue))
					{
						flag = true;
						text2 += item.Suffix;
					}
					break;
				}
			}
			if (flag)
			{
				text = text + text2 + "_locked";
			}
		}
		return text;
	}
}
