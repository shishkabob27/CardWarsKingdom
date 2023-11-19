public class InflictCrippleToEnemyOnAttack : OnAttack
{
	public override bool OnEnable()
	{
		if (!Target.HasCripple)
		{
			ApplyStatus(Target, StatusEnum.Cripple, base.Val1);
			return true;
		}
		return false;
	}
}
