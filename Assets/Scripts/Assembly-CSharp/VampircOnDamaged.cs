public class VampircOnDamaged : OnDamaged
{
	public override bool OnEnable()
	{
		if (base.Val1Chance)
		{
			ApplyStatus(base.Owner, StatusEnum.Vampiric, base.Val2);
			return true;
		}
		return false;
	}
}
