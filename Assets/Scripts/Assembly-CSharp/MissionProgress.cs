using System.Collections.Generic;
using System.Text;

public class MissionProgress
{
	public int BattlesWon;

	public int PvPBattlesWon;

	public int LeagueBattlesWon;

	public int HighestLeagueBattlesWon;

	public int DungeonBattlesWon;

	public int Criticals;

	public int CreaturesKilled;

	public int[] FactionKills = new int[6];

	public int CardsPlayed;

	public int[] FactionCards = new int[6];

	public int SuperCardsPlayed;

	public int CreaturesLost;

	public int StatusEffectsInflicted;

	public int BuffsGranted;

	public int EffectsStacked;

	public int Attacks;

	public int DragAttacks;

	public int CardAttacks;

	public int MagicAttacks;

	public int DeployedCreatures;

	public int[] DeployedFaction = new int[6];

	public int EnergyUsed;

	public int HealCardsPlayed;

	public int CreaturesFused;

	public int[] FactionFused = new int[6];

	public int CardsCreated;

	public int CreaturesSold;

	public int CardsSold;

	public int RunesSold;

	public int CoinsCollected;

	public int CoinsSpent;

	public int TeethCollected;

	public int TeethSpent;

	public int StaminaUsed;

	public int CreaturesCollected;

	public int RunesCollected;

	public int XPMaterialsCollected;

	public int DailiesCompleted;

	public int StarsEarned;

	public MissionProgress()
	{
	}

	public MissionProgress(Dictionary<string, object> dict)
	{
		Deserialize(dict);
	}

	public void Reset()
	{
		BattlesWon = 0;
		PvPBattlesWon = 0;
		LeagueBattlesWon = 0;
		HighestLeagueBattlesWon = 0;
		DungeonBattlesWon = 0;
		Criticals = 0;
		CreaturesKilled = 0;
		CardsPlayed = 0;
		SuperCardsPlayed = 0;
		CreaturesLost = 0;
		StatusEffectsInflicted = 0;
		BuffsGranted = 0;
		EffectsStacked = 0;
		Attacks = 0;
		DragAttacks = 0;
		CardAttacks = 0;
		MagicAttacks = 0;
		DeployedCreatures = 0;
		EnergyUsed = 0;
		HealCardsPlayed = 0;
		CreaturesFused = 0;
		CardsCreated = 0;
		CreaturesSold = 0;
		CardsSold = 0;
		RunesSold = 0;
		CoinsCollected = 0;
		CoinsSpent = 0;
		TeethCollected = 0;
		TeethSpent = 0;
		StaminaUsed = 0;
		CreaturesCollected = 0;
		RunesCollected = 0;
		XPMaterialsCollected = 0;
		DailiesCompleted = 0;
		StarsEarned = 0;
		for (int i = 0; i < FactionKills.Length; i++)
		{
			FactionKills[i] = 0;
		}
		for (int j = 0; j < FactionCards.Length; j++)
		{
			FactionCards[j] = 0;
		}
		for (int k = 0; k < FactionFused.Length; k++)
		{
			FactionFused[k] = 0;
		}
		for (int l = 0; l < DeployedFaction.Length; l++)
		{
			DeployedFaction[l] = 0;
		}
	}

	private string SerializeIntArray(int[] array)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[");
		for (int i = 0; i < array.Length; i++)
		{
			if (i > 0)
			{
				stringBuilder.Append(",");
			}
			stringBuilder.Append(array[i].ToString());
		}
		stringBuilder.Append("]");
		return stringBuilder.ToString();
	}

	public string Serialize()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("{");
		stringBuilder.Append(PlayerInfoScript.MakeJS("BattlesWon", BattlesWon) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("PvPBattlesWon", PvPBattlesWon) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("LeagueBattlesWon", LeagueBattlesWon) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("HighestLeagueBattlesWon", HighestLeagueBattlesWon) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("DungeonBattlesWon", DungeonBattlesWon) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("Criticals", Criticals) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("CreaturesKilled", CreaturesKilled) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("CardsPlayed", CardsPlayed) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("SuperCardsPlayed", SuperCardsPlayed) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("CreaturesLost", CreaturesLost) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("StatusEffectsInflicted", StatusEffectsInflicted) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("BuffsGranted", BuffsGranted) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("EffectsStacked", EffectsStacked) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("Attacks", Attacks) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("DragAttacks", DragAttacks) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("CardAttacks", CardAttacks) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("MagicAttacks", MagicAttacks) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("DeployedCreatures", DeployedCreatures) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("EnergyUsed", EnergyUsed) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("HealCardsPlayed", HealCardsPlayed) + ",");
		stringBuilder.Append("\"FactionKills\":" + SerializeIntArray(FactionKills) + ",");
		stringBuilder.Append("\"FactionCards\":" + SerializeIntArray(FactionCards) + ",");
		stringBuilder.Append("\"DeployedFaction\":" + SerializeIntArray(DeployedFaction) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("CreaturesFused", CreaturesFused) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("CardsCreated", CardsCreated) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("CreaturesSold", CreaturesSold) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("CardsSold", CardsSold) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("RunesSold", RunesSold) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("CoinsCollected", CoinsCollected) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("CoinsSpent", CoinsSpent) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("TeethCollected", TeethCollected) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("TeethSpent", TeethSpent) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("StaminaUsed", StaminaUsed) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("CreaturesCollected", CreaturesCollected) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("RunesCollected", RunesCollected) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("XPMaterialsCollected", XPMaterialsCollected) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("DailiesCompleted", DailiesCompleted) + ",");
		stringBuilder.Append("\"FactionFused\":" + SerializeIntArray(FactionFused) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("StarsEarned", StarsEarned) + ",");
		stringBuilder.Append("}");
		return stringBuilder.ToString();
	}

	private int GetValue(Dictionary<string, object> dict, string key)
	{
		if (dict.ContainsKey(key))
		{
			return (int)dict[key];
		}
		return 0;
	}

	private void DeserializeIntArray(Dictionary<string, object> dict, string key, ref int[] array)
	{
		if (dict.ContainsKey(key))
		{
			int[] array2 = (int[])dict[key];
			for (int i = 0; i < array2.Length; i++)
			{
				array[i] = array2[i];
			}
		}
	}

	private void Deserialize(Dictionary<string, object> dict)
	{
		BattlesWon = GetValue(dict, "BattlesWon");
		PvPBattlesWon = GetValue(dict, "PvPBattlesWon");
		LeagueBattlesWon = GetValue(dict, "LeagueBattlesWon");
		HighestLeagueBattlesWon = GetValue(dict, "HighestLeagueBattlesWon");
		DungeonBattlesWon = GetValue(dict, "DungeonBattlesWon");
		Criticals = GetValue(dict, "Criticals");
		CreaturesKilled = GetValue(dict, "CreaturesKilled");
		CardsPlayed = GetValue(dict, "CardsPlayed");
		SuperCardsPlayed = GetValue(dict, "SuperCardsPlayed");
		CreaturesLost = GetValue(dict, "CreaturesLost");
		StatusEffectsInflicted = GetValue(dict, "StatusEffectsInflicted");
		BuffsGranted = GetValue(dict, "BuffsGranted");
		EffectsStacked = GetValue(dict, "EffectsStacked");
		Attacks = GetValue(dict, "Attacks");
		DragAttacks = GetValue(dict, "DragAttacks");
		CardAttacks = GetValue(dict, "CardAttacks");
		MagicAttacks = GetValue(dict, "MagicAttacks");
		DeployedCreatures = GetValue(dict, "DeployedCreatures");
		EnergyUsed = GetValue(dict, "EnergyUsed");
		HealCardsPlayed = GetValue(dict, "HealCardsPlayed");
		DeserializeIntArray(dict, "FactionKills", ref FactionKills);
		DeserializeIntArray(dict, "FactionCards", ref FactionCards);
		DeserializeIntArray(dict, "DeployedFaction", ref DeployedFaction);
		CreaturesFused = GetValue(dict, "CreaturesFused");
		CardsCreated = GetValue(dict, "CardsCreated");
		CreaturesSold = GetValue(dict, "CreaturesSold");
		CardsSold = GetValue(dict, "CardsSold");
		RunesSold = GetValue(dict, "RunesSold");
		CoinsCollected = GetValue(dict, "CoinsCollected");
		CoinsSpent = GetValue(dict, "CoinsSpent");
		TeethCollected = GetValue(dict, "TeethCollected");
		TeethSpent = GetValue(dict, "TeethSpent");
		StaminaUsed = GetValue(dict, "StaminaUsed");
		CreaturesCollected = GetValue(dict, "CreaturesCollected");
		RunesCollected = GetValue(dict, "RunesCollected");
		XPMaterialsCollected = GetValue(dict, "XPMaterialsCollected");
		DailiesCompleted = GetValue(dict, "DailiesCompleted");
		StarsEarned = GetValue(dict, "StarsEarned");
		DeserializeIntArray(dict, "FactionFused", ref FactionFused);
	}
}
