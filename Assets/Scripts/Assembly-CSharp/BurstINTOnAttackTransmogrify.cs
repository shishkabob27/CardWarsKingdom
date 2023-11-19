public class BurstINTOnAttackTransmogrify : OnAttack
{
	public override bool OnEnable()
	{
		if (Target.HasTransmogrify)
		{
			ApplyStatus(base.Owner, StatusEnum.MagicSpike, base.Val1);
			return true;
		}
		return false;
	}
}
