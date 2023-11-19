public class RemoveBuffsOnAttack : OnAttack
{
	public override bool OnEnable()
	{
		bool result = false;
		if (base.Val1Chance)
		{
			for (int i = 0; i < base.Val2; i++)
			{
				if (Target.HasBuff)
				{
					Target.RemoveStatusEffect(StatusType.Buff);
					result = true;
				}
			}
		}
		return result;
	}
}
