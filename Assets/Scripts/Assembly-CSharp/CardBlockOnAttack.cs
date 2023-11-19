public class CardBlockOnAttack : OnPercentAttack
{
	public override bool OnEnable()
	{
		ApplyStatus(Target, StatusEnum.CardBlock, base.Val2);
		return true;
	}
}
