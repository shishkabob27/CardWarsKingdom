public class FortifyDEFOnPhysicalDamage : OnPhysicalDamage
{
	public override bool OnEnable()
	{
		if (base.Val1Chance)
		{
			ApplyStatus(base.Owner, StatusEnum.DefenseBoost, base.Val2);
			return true;
		}
		return false;
	}
}
