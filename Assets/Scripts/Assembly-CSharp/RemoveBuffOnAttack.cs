public class RemoveBuffOnAttack : OnAttack
{
	public override bool OnEnable()
	{
		if (base.Val1Chance && Target.HasBuff)
		{
			Target.RemoveStatusEffect(StatusType.Buff);
			return true;
		}
		return false;
	}
}
