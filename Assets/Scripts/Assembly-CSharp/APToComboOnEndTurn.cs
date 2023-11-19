using System;

public class APToComboOnEndTurn : OnEndTurn
{
	public override bool OnEnable()
	{
		if (ActionPoints > 0)
		{
			int amount = Math.Min(base.Val1, ActionPoints);
			base.Owner.Owner.FillAPMeter(amount);
			base.Owner.Owner.CheckFullAPMeter();
			return true;
		}
		return false;
	}
}
