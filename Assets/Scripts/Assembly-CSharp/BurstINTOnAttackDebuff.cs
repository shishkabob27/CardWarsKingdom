public class BurstINTOnAttackDebuff : OnAttack
{
	public override bool OnEnable()
	{
		if (Target.HasDebuff)
		{
			ApplyStatus(base.Owner, StatusEnum.MagicSpike, base.Val1);
			return true;
		}
		return false;
	}
}
