public class PoisonOnAttack : OnPercentAttack
{
	public override bool OnEnable()
	{
		ApplyStatus(Target, StatusEnum.Poison, base.Val2);
		return true;
	}
}
