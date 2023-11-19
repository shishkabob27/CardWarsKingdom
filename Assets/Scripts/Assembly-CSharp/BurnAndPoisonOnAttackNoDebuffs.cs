public class BurnAndPoisonOnAttackNoDebuffs : OnAttack
{
	public override bool OnEnable()
	{
		if (!Target.HasDebuff)
		{
			ApplyStatus(Target, StatusEnum.Burn, base.Val1);
			ApplyStatus(Target, StatusEnum.Poison, base.Val2);
			return true;
		}
		return false;
	}
}
