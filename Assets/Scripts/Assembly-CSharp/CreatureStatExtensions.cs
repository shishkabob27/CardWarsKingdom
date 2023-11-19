public static class CreatureStatExtensions
{
	public static string DisplayName(this CreatureStat stat)
	{
		switch (stat)
		{
		case CreatureStat.STR:
			return KFFLocalization.Get("!!STR");
		case CreatureStat.INT:
			return KFFLocalization.Get("!!INT");
		case CreatureStat.DEX:
			return KFFLocalization.Get("!!DEX");
		case CreatureStat.DEF:
			return KFFLocalization.Get("!!DEF");
		case CreatureStat.RES:
			return KFFLocalization.Get("!!RES");
		case CreatureStat.HP:
			return KFFLocalization.Get("!!HP");
		default:
			return string.Empty;
		}
	}

	public static bool IsPercent(this CreatureStat stat)
	{
		return stat == CreatureStat.DEX || stat == CreatureStat.DEF || stat == CreatureStat.RES;
	}
}
