using System.Collections.Generic;
using UnityEngine;

public class GachaWeightTable : ILoadableData
{
	private string _GachaID;

	private List<GachaWeightEntry> _WeightEntries = new List<GachaWeightEntry>();

	private int _TotalWeight;

	public string ID
	{
		get
		{
			return _GachaID;
		}
	}

	public GachaWeightTable(string gachaId)
	{
		_GachaID = gachaId;
	}

	public void Populate(Dictionary<string, object> dict)
	{
	}

	public void AddEntry(string dropId, int starOverride, int weight)
	{
		GachaWeightEntry gachaWeightEntry = new GachaWeightEntry();
		gachaWeightEntry.DropID = dropId;
		gachaWeightEntry.Weight = weight;
		gachaWeightEntry.StarOverride = starOverride;
		_WeightEntries.Add(gachaWeightEntry);
		_TotalWeight += weight;
	}

	public GachaWeightEntry Spin()
	{
		int num = Random.Range(0, _TotalWeight);
		foreach (GachaWeightEntry weightEntry in _WeightEntries)
		{
			num -= weightEntry.Weight;
			if (num < 0)
			{
				return weightEntry;
			}
		}
		return null;
	}

	public GachaWeightEntry TutorialSpin()
	{
		int num = Random.Range(0, _TotalWeight);
		CreatureFaction chosenTeamCreatureFaction = Singleton<TutorialController>.Instance.getChosenTeamCreatureFaction();
		foreach (GachaWeightEntry weightEntry in _WeightEntries)
		{
			CreatureData data = CreatureDataManager.Instance.GetData(weightEntry.DropID);
			if (data != null && data.Faction == chosenTeamCreatureFaction)
			{
				return weightEntry;
			}
		}
		GachaWeightEntry defaultTutCreature = GetDefaultTutCreature(chosenTeamCreatureFaction);
		if (defaultTutCreature != null)
		{
			return defaultTutCreature;
		}
		return Spin();
	}

	public GachaWeightEntry GetDefaultTutCreature(CreatureFaction f)
	{
		GachaWeightEntry gachaWeightEntry = null;
		string defaultId = string.Empty;
		switch (f)
		{
		case CreatureFaction.Blue:
			defaultId = MiscParams.defaultTutCreature_Plains;
			break;
		case CreatureFaction.Red:
			defaultId = MiscParams.defaultTutCreature_Corn;
			break;
		case CreatureFaction.Light:
			defaultId = MiscParams.defaultTutCreature_Nice;
			break;
		}
		gachaWeightEntry = _WeightEntries.Find((GachaWeightEntry m) => m.DropID == defaultId);
		if (gachaWeightEntry == null)
		{
		}
		return gachaWeightEntry;
	}
}
