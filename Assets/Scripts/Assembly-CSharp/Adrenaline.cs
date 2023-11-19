using UnityEngine;

public class Adrenaline : StatusState
{
	public override int AttackDiscount()
	{
		return Mathf.RoundToInt(base.Intensity * 100f);
	}
}
