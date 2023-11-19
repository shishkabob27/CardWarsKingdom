using UnityEngine;

public static class CreatureFactionExtensions
{
	public static string IconTexture(this CreatureFaction faction)
	{
		switch (faction)
		{
		case CreatureFaction.Red:
			return "Icon_Faction_Red";
		case CreatureFaction.Green:
			return "Icon_Faction_Green";
		case CreatureFaction.Blue:
			return "Icon_Faction_Blue";
		case CreatureFaction.Dark:
			return "Icon_Faction_Dark";
		case CreatureFaction.Light:
			return "Icon_Faction_Light";
		default:
			return string.Empty;
		}
	}

	public static string CardFrameTexture(this CreatureFaction faction)
	{
		switch (faction)
		{
		case CreatureFaction.Red:
			return "UI/CardFrames/UI_ActionCard_Frame_Corn_Landscape";
		case CreatureFaction.Green:
			return "UI/CardFrames/UI_ActionCard_Frame_Sand_Landscape";
		case CreatureFaction.Blue:
			return "UI/CardFrames/UI_ActionCard_Plains_Landscape";
		case CreatureFaction.Dark:
			return "UI/CardFrames/UI_ActionCard_Frame_Swamp_Landscape";
		case CreatureFaction.Light:
			return "UI/CardFrames/UI_ActionCard_Frame_Nicelands_Landscape";
		case CreatureFaction.Colorless:
			return "UI/CardFrames/UI_HeroCard_Frame";
		default:
			return string.Empty;
		}
	}

	public static string CreatureFrameTexture(this CreatureFaction faction)
	{
		switch (faction)
		{
		case CreatureFaction.Red:
			return "UI/CardFrames/UI_CreatureCard_Frame_Corn";
		case CreatureFaction.Green:
			return "UI/CardFrames/UI_CreatureCard_Frame_Sand";
		case CreatureFaction.Blue:
			return "UI/CardFrames/UI_CreatureCard_Frame_Plains";
		case CreatureFaction.Dark:
			return "UI/CardFrames/UI_CreatureCard_Frame_Swamp";
		case CreatureFaction.Light:
			return "UI/CardFrames/UI_CreatureCard_Frame_Nicelands";
		case CreatureFaction.Colorless:
			return "UI/CardFrames/UI_HeroCard_Frame";
		default:
			return string.Empty;
		}
	}

	public static string CreatureShardFrameSpriteName(this CreatureFaction faction)
	{
		switch (faction)
		{
		case CreatureFaction.Red:
			return "Shard_Frame_Corn";
		case CreatureFaction.Green:
			return "Shard_Frame_Sand";
		case CreatureFaction.Blue:
			return "Shard_Frame_Plains";
		case CreatureFaction.Dark:
			return "Shard_Frame_Swamp";
		case CreatureFaction.Light:
			return "Shard_Frame_Nicelands";
		case CreatureFaction.Colorless:
			return "Frame_Rune";
		default:
			return string.Empty;
		}
	}

	public static string CreaturePortraitFrameSpriteName(this CreatureFaction faction)
	{
		switch (faction)
		{
		case CreatureFaction.Red:
			return "Frame_Creature_Red";
		case CreatureFaction.Green:
			return "Frame_Creature_Green";
		case CreatureFaction.Blue:
			return "Frame_Creature_Blue";
		case CreatureFaction.Dark:
			return "Frame_Creature_Dark";
		case CreatureFaction.Light:
			return "Frame_Creature_Light";
		case CreatureFaction.Colorless:
			return "Frame_Creature_Blue";
		default:
			return string.Empty;
		}
	}

	public static string CakeFrameSpriteName(this CreatureFaction faction)
	{
		switch (faction)
		{
		case CreatureFaction.Red:
			return "UI_CakeCard_Frame_Corn";
		case CreatureFaction.Green:
			return "UI_CakeCard_Frame_Sand";
		case CreatureFaction.Blue:
			return "UI_CakeCard_Frame_Plains";
		case CreatureFaction.Dark:
			return "UI_CakeCard_Frame_Swamp";
		case CreatureFaction.Light:
			return "UI_CakeCard_Frame_Nicelands";
		case CreatureFaction.Colorless:
			return "UI_CakeCard_Frame_Corn";
		default:
			return string.Empty;
		}
	}

	public static Color VFXColor(this CreatureFaction faction)
	{
		return Singleton<DWFactionColor>.Instance.FactionColors[(int)faction];
	}

	public static string ComboPipTexture(this CreatureFaction faction)
	{
		switch (faction)
		{
		case CreatureFaction.Red:
			return "Icon_Faction_Red";
		case CreatureFaction.Green:
			return "Icon_Faction_Green";
		case CreatureFaction.Blue:
			return "Icon_Faction_Blue";
		case CreatureFaction.Dark:
			return "Icon_Faction_Dark";
		case CreatureFaction.Light:
			return "Icon_Faction_Light";
		default:
			return string.Empty;
		}
	}

	public static string GetIcon(this CreatureFaction faction)
	{
		switch (faction)
		{
		case CreatureFaction.Red:
			return "Icon_Card_Red";
		case CreatureFaction.Green:
			return "Icon_Card_Green";
		case CreatureFaction.Blue:
			return "Icon_Card_Blue";
		case CreatureFaction.Dark:
			return "Icon_Card_Dark";
		case CreatureFaction.Light:
			return "Icon_Card_Light";
		default:
			return string.Empty;
		}
	}

	public static string ClassName(this CreatureFaction faction)
	{
		switch (faction)
		{
		case CreatureFaction.Red:
			return "Fighter";
		case CreatureFaction.Green:
			return "Guard";
		case CreatureFaction.Blue:
			return "Mystic";
		case CreatureFaction.Dark:
			return "Utility";
		case CreatureFaction.Light:
			return "Recovery";
		default:
			return string.Empty;
		}
	}

	public static string ClassDisplayName(this CreatureFaction faction)
	{
		switch (faction)
		{
		case CreatureFaction.Red:
			return KFFLocalization.Get("!!FACTION_RED");
		case CreatureFaction.Green:
			return KFFLocalization.Get("!!FACTION_GREEN");
		case CreatureFaction.Blue:
			return KFFLocalization.Get("!!FACTION_BLUE");
		case CreatureFaction.Dark:
			return KFFLocalization.Get("!!FACTION_PURPLE");
		case CreatureFaction.Light:
			return KFFLocalization.Get("!!FACTION_YELLOW");
		default:
			return string.Empty;
		}
	}

	public static CreatureFaction RandomFaction()
	{
		return (CreatureFaction)Random.Range(1, 6);
	}

	public static string CDubsName(this CreatureFaction faction)
	{
		switch (faction)
		{
		case CreatureFaction.Red:
			return "Corn";
		case CreatureFaction.Green:
			return "Sand";
		case CreatureFaction.Blue:
			return "Plains";
		case CreatureFaction.Dark:
			return "Swamp";
		case CreatureFaction.Light:
			return "Nicelands";
		default:
			return string.Empty;
		}
	}
}
