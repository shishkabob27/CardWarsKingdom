using System;
using System.Collections.Generic;
using System.Text;

public class Mission
{
	public MissionData Data { get; set; }

	public bool Completed { get; set; }

	public bool Claimed { get; set; }

	public MissionProgress Progress
	{
		get
		{
			if (Data.Type == MissionType.Global)
			{
				return DetachedSingleton<MissionManager>.Instance.GlobalProgress;
			}
			if (Data.SingleBattle)
			{
				return DetachedSingleton<MissionManager>.Instance.BattleProgress;
			}
			return DetachedSingleton<MissionManager>.Instance.DailyProgress;
		}
	}

	public virtual int ProgressValue
	{
		get
		{
			return 0;
		}
	}

	public float ProgressPct
	{
		get
		{
			return (float)ProgressValue / (float)Data.Val1;
		}
	}

	protected int Val2
	{
		get
		{
			return Data.Val2;
		}
	}

	public static Type GetScriptClass(string ScriptName)
	{
		Type type = Type.GetType(ScriptName);
		if (type == null)
		{
			type = Type.GetType("Mission");
		}
		return type;
	}

	public static Mission Instantiate(MissionData missionData)
	{
		Type scriptClass = GetScriptClass(missionData.ScriptName);
		Mission mission = Activator.CreateInstance(scriptClass) as Mission;
		mission.Data = missionData;
		return mission;
	}

	public string Serialize()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("{");
		stringBuilder.Append(PlayerInfoScript.MakeJS("MissionID", Data.ID) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("Completed", Completed) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("Claimed", Claimed) + ",");
		stringBuilder.Append("}");
		return stringBuilder.ToString();
	}

	public static Mission Deserialize(Dictionary<string, object> dict)
	{
		string iD = (string)dict["MissionID"];
		MissionData data = MissionDataManager.Instance.GetData(iD);
		if (data == null)
		{
			return null;
		}
		Mission mission = Instantiate(data);
		mission.Completed = TFUtils.LoadBool(dict, "Completed", false);
		mission.Claimed = TFUtils.LoadBool(dict, "Claimed", false);
		return mission;
	}

	public virtual bool IsComplete()
	{
		if (ProgressValue >= Data.Val1)
		{
			return true;
		}
		return false;
	}
}
