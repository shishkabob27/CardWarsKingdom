public class ChilledOnAttack : OnPercentAttack
{
	public override bool OnEnable()
	{
		ApplyStatus(base.Owner, StatusEnum.FrostArmor, base.Val2);
		return true;
	}
}
