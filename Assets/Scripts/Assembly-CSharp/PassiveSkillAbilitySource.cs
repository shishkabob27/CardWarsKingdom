using CodeStage.AntiCheat.ObscuredTypes;

public class PassiveSkillAbilitySource : IAbilitySource
{
	private string _ScriptName;

	private ObscuredInt[] _Values;

	public string ScriptName
	{
		get
		{
			return _ScriptName;
		}
	}

	public AbilitySourceType AbilitySourceType
	{
		get
		{
			return AbilitySourceType.Creature;
		}
	}

	public int Val1
	{
		get
		{
			return _Values[0];
		}
	}

	public int Val2
	{
		get
		{
			return _Values[1];
		}
	}

	public int Val3
	{
		get
		{
			return _Values[2];
		}
	}

	public int Val4
	{
		get
		{
			return _Values[3];
		}
	}

	public string ValID { get; private set; }

	public PassiveSkillAbilitySource(CreaturePassiveData effectData, int level)
	{
		_ScriptName = effectData.ScriptName;
		_Values = effectData.GetValuesAtLevel(level);
		ValID = effectData.ValID;
	}
}
