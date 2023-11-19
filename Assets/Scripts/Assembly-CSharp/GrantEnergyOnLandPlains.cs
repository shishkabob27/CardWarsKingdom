public class GrantEnergyOnLandPlains : OnLandPlains
{
	public override bool OnEnable()
	{
		int num = base.Owner.Owner.Hand.Count * base.Val1;
		if (num > base.Val2)
		{
			num = base.Val2;
		}
		base.Owner.Owner.AddActionPoints(num);
		return true;
	}
}
