using UnityEngine;

public class LandCorn : StatusState
{
	public override int BonusDragAttacks()
	{
		return Mathf.RoundToInt(base.Intensity * 100f) - 1;
	}

	public override bool BlockDraw()
	{
		return true;
	}
}
