using System.Collections.Generic;
using UnityEngine;

public class QuestLoadoutData : ILoadableData
{
	private string _ID;

	private List<QuestLoadoutEntry> _LoadoutEntries = new List<QuestLoadoutEntry>();

	private int _TotalWeight;

	public string ID
	{
		get
		{
			return _ID;
		}
	}

	public IList<QuestLoadoutEntry> Entries
	{
		get
		{
			return _LoadoutEntries.AsReadOnly();
		}
	}

	public int TotalWeight
	{
		get
		{
			return _TotalWeight;
		}
	}

	public QuestLoadoutData(string loadoutID = null)
	{
		_ID = loadoutID;
	}

	public void Populate(Dictionary<string, object> dict)
	{
	}

	public void AddEntry(QuestLoadoutEntry entry)
	{
		_LoadoutEntries.Add(entry);
		if (entry != null)
		{
			_TotalWeight += entry.AppearWeight;
		}
	}

	public QuestLoadoutEntry GetRandomEntry()
	{
		int num = Random.Range(0, _TotalWeight);
		foreach (QuestLoadoutEntry loadoutEntry in _LoadoutEntries)
		{
			num -= loadoutEntry.AppearWeight;
			if (num < 0)
			{
				return loadoutEntry;
			}
		}
		return null;
	}
}
