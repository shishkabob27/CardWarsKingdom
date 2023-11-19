using UnityEngine;

public class SpendHealthToEraseWhenFriendlyAttacks : OnFriendlyAttack
{
	public override bool OnEnable()
	{
		if (Target != null && Target.HasBuff)
		{
			ApplyStatus(Target, StatusEnum.RemoveStatus, 1f, StatusRemovalData.RemoveRandomBuffs());
			int num = Mathf.RoundToInt((float)base.Owner.MaxHP * base.Val1Pct);
			if (num == 0)
			{
				num = 1;
			}
			base.Owner.DealDamage(num, AttackBase.None, false, null, null);
			return true;
		}
		return false;
	}
}
