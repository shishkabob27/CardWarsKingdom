using System;
using System.Collections.Generic;

public class AbilityState
{
	public bool IsActive;

	public int StoredValue;

	public Dictionary<CreatureState, int> StoredCreatures = new Dictionary<CreatureState, int>();

	public IAbilitySource Source { get; set; }

	public CreatureState Owner { get; set; }

	public int Val1
	{
		get
		{
			return Source.Val1;
		}
	}

	public int Val2
	{
		get
		{
			return Source.Val2;
		}
	}

	public int Val3
	{
		get
		{
			return Source.Val3;
		}
	}

	public int Val4
	{
		get
		{
			return Source.Val4;
		}
	}

	public float Val1Pct
	{
		get
		{
			return (float)Val1 / 100f;
		}
	}

	public float Val2Pct
	{
		get
		{
			return (float)Val2 / 100f;
		}
	}

	public float Val3Pct
	{
		get
		{
			return (float)Val3 / 100f;
		}
	}

	public float Val4Pct
	{
		get
		{
			return (float)Val4 / 100f;
		}
	}

	public string ScriptName
	{
		get
		{
			return Source.ScriptName;
		}
	}

	public bool Val1Chance
	{
		get
		{
			return KFFRandom.Percent(Val1);
		}
	}

	public string ValID
	{
		get
		{
			return Source.ValID;
		}
	}

	public static Type GetScriptClass(string ScriptName)
	{
		Type type = Type.GetType(ScriptName);
		if (type == null)
		{
			type = Type.GetType("AbilityState");
		}
		return type;
	}

	public static AbilityState Create(IAbilitySource Source)
	{
		Type scriptClass = GetScriptClass(Source.ScriptName);
		AbilityState abilityState = DetachedSingleton<KFFPoolManager>.Instance.GetObject(scriptClass) as AbilityState;
		abilityState.Init(Source);
		return abilityState;
	}

	private static AbilityState Create(AbilityState Source)
	{
		AbilityState abilityState = Create(Source.Source);
		abilityState.Init(Source);
		return abilityState;
	}

	public static void Destroy(AbilityState State)
	{
		State.Clean();
		DetachedSingleton<KFFPoolManager>.Instance.ReleaseObject(State);
	}

	public void Init(IAbilitySource abilitySource)
	{
		Source = abilitySource;
	}

	public void Init(AbilityState State)
	{
		IsActive = State.IsActive;
		StoredValue = State.StoredValue;
		StoredCreatures = State.StoredCreatures;
	}

	public void Clean()
	{
		Source = null;
		Owner = null;
		IsActive = false;
		StoredValue = 0;
		StoredCreatures.Clear();
	}

	public AbilityState DeepCopy()
	{
		return Create(this);
	}

	protected void ApplyStatus(CreatureState Target, StatusEnum statusName, float varValue, StatusRemovalData removalData = null)
	{
		StatusData data = StatusDataManager.Instance.GetData(statusName.Name());
		if (data != null)
		{
			Target.ApplyStatus(data, varValue, null, this, removalData);
		}
	}

	protected void CancelStatus(CreatureState Target, StatusEnum statusName)
	{
		Target.CancelStatusEffect(statusName.Name());
	}

	public virtual bool OnEnable()
	{
		return false;
	}

	public virtual void OnDisable()
	{
	}

	public virtual bool ProcessMessage(GameMessage Message)
	{
		return false;
	}
}
