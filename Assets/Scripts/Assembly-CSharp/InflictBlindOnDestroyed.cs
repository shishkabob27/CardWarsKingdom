public class InflictBlindOnDestroyed : OnAttacked
{
	public override bool OnEnable()
	{
		if (base.Owner.HP <= 0)
		{
			ApplyStatus(Attacker, StatusEnum.Blind, base.Val1);
			return true;
		}
		return false;
	}
}
