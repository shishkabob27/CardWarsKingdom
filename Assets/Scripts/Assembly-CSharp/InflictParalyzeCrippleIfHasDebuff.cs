public class InflictParalyzeCrippleIfHasDebuff : OnAttack
{
	public override bool OnEnable()
	{
		if (Target.HasDebuff)
		{
			ApplyStatus(Target, StatusEnum.Paralyze, 1f);
			ApplyStatus(Target, StatusEnum.Cripple, base.Val1);
			return true;
		}
		return false;
	}
}
