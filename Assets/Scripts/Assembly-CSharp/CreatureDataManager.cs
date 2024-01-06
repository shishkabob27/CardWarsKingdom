using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CreatureDataManager : DataManager<CreatureData>
{
	protected Dictionary<string, CreatureData> TutorialDatabase = new Dictionary<string, CreatureData>();

	protected List<CreatureData> TutorialDatabaseArray = new List<CreatureData>();

	protected Dictionary<string, CreatureData> NonMuseumDatabase = new Dictionary<string, CreatureData>();

	protected List<CreatureData> NonMuseumDatabaseArray = new List<CreatureData>();

	private static CreatureDataManager _instance;

	public static CreatureDataManager Instance
	{
		get
		{
			if (_instance == null)
			{
				string path = Path.Combine(SQSettings.CDN_URL, "Blueprints", "db_Creatures.json");
				_instance = new CreatureDataManager(path);
			}
			return _instance;
		}
	}

	public CreatureDataManager(string path)
	{
		base.FilePath = path;
		AddDependency(CardDataManager.Instance);
		AddDependency(XPTableDataManager.Instance);
	}

	protected override void ParseRows(List<object> jlist)
	{
		foreach (object item in jlist)
		{
			Dictionary<string, object> dict = (Dictionary<string, object>)item;
			CreatureData creatureData = (CreatureData)Activator.CreateInstance(typeof(CreatureData));
			creatureData.Populate(dict);
			Dictionary<string, CreatureData> dictionary;
			List<CreatureData> list;
			if (creatureData.TutorialOnly)
			{
				dictionary = TutorialDatabase;
				list = TutorialDatabaseArray;
			}
			else if (creatureData.HideInMuseum)
			{
				dictionary = NonMuseumDatabase;
				list = NonMuseumDatabaseArray;
			}
			else
			{
				dictionary = Database;
				list = DatabaseArray;
			}
			if (creatureData.ID != string.Empty && !dictionary.ContainsKey(creatureData.ID))
			{
				dictionary.Add(creatureData.ID, creatureData);
			}
			list.Add(creatureData);
		}
	}

	public CreatureData GetTutorialCreatureData(string ID)
	{
		CreatureData value;
		if (TutorialDatabase.TryGetValue(ID, out value))
		{
			return value;
		}
		return null;
	}

	public CreatureData GetNonMuseumCreatureData(string ID)
	{
		CreatureData value;
		if (NonMuseumDatabase.TryGetValue(ID, out value))
		{
			return value;
		}
		return null;
	}

	public override void Unload()
	{
		base.Unload();
		TutorialDatabase.Clear();
		TutorialDatabaseArray.Clear();
		NonMuseumDatabase.Clear();
		NonMuseumDatabaseArray.Clear();
	}

	protected override void PostLoad()
	{
		DatabaseArray.Sort((CreatureData c1, CreatureData c2) => c1.CreatureNumber.CompareTo(c2.CreatureNumber));
	}

	public InventorySlotItem GenerateRandomCreatue(int level)
	{
		CreatureData creatureData = DatabaseArray[UnityEngine.Random.Range(1, DatabaseArray.Count)];
		CreatureItem creatureItem = new CreatureItem(creatureData);
		creatureItem.Xp = creatureItem.XPTable.GetXpToReachLevel(level);
		if (creatureData.PassiveData != null)
		{
			creatureItem.PassiveSkillLevel = UnityEngine.Random.Range(1, creatureData.PassiveData.MaxLevel);
		}
		return new InventorySlotItem(creatureItem);
	}

	public CreatureData GetRandomDungeonCreature(int rarity)
	{
		int num = 0;
		foreach (CreatureData item in DatabaseArray)
		{
			if (!item.BlockFromRandomDungeons)
			{
				num++;
			}
		}
		if (num == 0)
		{
			return null;
		}
		int num2 = UnityEngine.Random.Range(0, num);
		foreach (CreatureData item2 in DatabaseArray)
		{
			if (!item2.BlockFromRandomDungeons && item2.Rarity == rarity)
			{
				if (num2 == 0)
				{
					return item2;
				}
				num2--;
			}
		}
		return null;
	}
}
