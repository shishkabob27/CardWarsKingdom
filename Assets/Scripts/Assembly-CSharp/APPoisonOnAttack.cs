public class APPoisonOnAttack : OnPercentAttack
{
	public override bool OnEnable()
	{
		ApplyStatus(Target, StatusEnum.EnergyBleed, base.Val2);
		return true;
	}
}
