public class PlagueOnAttackWhenNoPlague : OnAttack
{
	public override bool OnEnable()
	{
		if (!Target.IsPlagued)
		{
			ApplyStatus(Target, StatusEnum.Plague, base.Val1);
			return true;
		}
		return false;
	}
}
