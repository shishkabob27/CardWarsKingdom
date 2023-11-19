using UnityEngine;

public class Cripple : StatusState
{
	public override int AttackDiscount()
	{
		return Mathf.RoundToInt(0f - base.Intensity * 100f);
	}
}
