public class GrantEnergyOnDied : OnDied
{
	public override bool OnEnable()
	{
		int num = 0;
		bool result = false;
		foreach (LaneState lane in base.Owner.Owner.Lanes)
		{
			if (lane.Creature != null)
			{
				num++;
				result = true;
			}
		}
		num *= base.Val1;
		if (num > base.Val2)
		{
			num = base.Val2;
		}
		base.Owner.Owner.AddActionPoints(num);
		return result;
	}
}
