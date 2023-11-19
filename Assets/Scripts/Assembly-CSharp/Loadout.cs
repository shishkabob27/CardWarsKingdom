using System;
using System.Collections.Generic;
using System.Text;

public class Loadout
{
	public LeaderItem Leader;

	public List<InventorySlotItem> CreatureSet = new List<InventorySlotItem>();

	public Loadout()
	{
	}

	public Loadout(Dictionary<string, object> dict)
	{
		Deserialize(dict);
	}

	public bool IsUsable()
	{
		return CreatureSet.Find((InventorySlotItem m) => m != null) != null;
	}

	public int CreatureCount()
	{
		return CreatureSet.FindAll((InventorySlotItem m) => m != null).Count;
	}

	public string Serialize()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("{");
		if (Leader != null)
		{
			stringBuilder.Append(PlayerInfoScript.MakeJS("LeaderID", Leader.Form.ID) + ",");
		}
		for (int i = 0; i < CreatureSet.Count; i++)
		{
			if (CreatureSet[i] != null)
			{
				stringBuilder.Append(PlayerInfoScript.MakeJS("CreatureID" + i, CreatureSet[i].Creature.UniqueId) + ",");
			}
		}
		stringBuilder.Append("}");
		return stringBuilder.ToString();
	}

	public void Deserialize(Dictionary<string, object> dict, List<CreatureItem> _Creatures = null)
	{
		if (dict.ContainsKey("LeaderID"))
		{
			string leaderId = (string)dict["LeaderID"];
			Leader = Singleton<PlayerInfoScript>.Instance.GetLeaderItem(leaderId);
		}
		else
		{
			Leader = null;
		}
		CreatureSet.Clear();
		for (int i = 0; i < MiscParams.CreaturesOnTeam; i++)
		{
			string key = "CreatureID" + i;
			int uniqueId = TFUtils.LoadInt(dict, key, 0);
			if (uniqueId != 0)
			{
				if (_Creatures != null)
				{
					CreatureSet.Add(new InventorySlotItem(_Creatures.Find((CreatureItem match) => match.UniqueId == uniqueId)));
				}
				else
				{
					CreatureSet.Add(Singleton<PlayerInfoScript>.Instance.SaveData.GetCreatureItem(uniqueId));
				}
			}
			else
			{
				CreatureSet.Add(null);
			}
		}
	}

	public bool ContainsCreature(CreatureItem creature)
	{
		foreach (InventorySlotItem item in CreatureSet)
		{
			if (item != null && item.Creature == creature)
			{
				return true;
			}
		}
		return false;
	}

	public int GetTeamCost(bool ignoreLastSlot = false)
	{
		int num = 0;
		for (int i = 0; i < CreatureSet.Count; i++)
		{
			if ((!ignoreLastSlot || i != CreatureSet.Count - 1) && CreatureSet[i] != null)
			{
				num += CreatureSet[i].Creature.currentTeamCost;
			}
		}
		return num;
	}

	public Loadout Copy()
	{
		Loadout loadout = new Loadout();
		loadout.Leader = Leader;
		foreach (InventorySlotItem item in CreatureSet)
		{
			loadout.CreatureSet.Add(item);
		}
		return loadout;
	}

	public int GetAverageCreatureLevel()
	{
		int num = 0;
		int num2 = 0;
		foreach (InventorySlotItem item in CreatureSet)
		{
			if (item != null)
			{
				num2++;
				num += item.Creature.Level;
			}
		}
		if (num2 == 0 || num < num2)
		{
			return 1;
		}
		return (int)Math.Ceiling((float)num / (float)num2);
	}

	public int GetTotalCreatureLevel()
	{
		int num = 0;
		foreach (InventorySlotItem item in CreatureSet)
		{
			if (item != null)
			{
				num += item.Creature.Level;
			}
		}
		return num;
	}

	public bool ContainsCreatureOrEvo(CreatureData creature, bool ignoreLastSlot)
	{
		for (int i = 0; i < CreatureSet.Count; i++)
		{
			if (i == MiscParams.CreaturesOnTeam - 1 && ignoreLastSlot)
			{
				continue;
			}
			InventorySlotItem inventorySlotItem = CreatureSet[i];
			if (inventorySlotItem != null)
			{
				if (inventorySlotItem.Creature.Form == creature)
				{
					return true;
				}
				if (creature.EvolvesTo != null && inventorySlotItem.Creature.Form.ID == creature.EvolvesTo)
				{
					return true;
				}
			}
		}
		return false;
	}
}
