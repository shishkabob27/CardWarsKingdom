public class RemoveOppPosStatusOnAtk : OnAttack
{
	public override bool OnEnable()
	{
		ApplyStatus(Target, StatusEnum.RemoveStatus, 1f, StatusRemovalData.RemoveRandomBuffs());
		return true;
	}
}
