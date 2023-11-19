public class FactionAbilitySource : IAbilitySource
{
	public string ScriptName { get; set; }

	public AbilitySourceType AbilitySourceType
	{
		get
		{
			return AbilitySourceType.Faction;
		}
	}

	public int Val1
	{
		get
		{
			return 0;
		}
	}

	public int Val2
	{
		get
		{
			return 0;
		}
	}

	public int Val3
	{
		get
		{
			return 0;
		}
	}

	public int Val4
	{
		get
		{
			return 0;
		}
	}

	public string ValID
	{
		get
		{
			return null;
		}
	}
}
