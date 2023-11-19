using UnityEngine;

public class RemovePlagueNDirectDamageOnAtk : OnAttack
{
	public override bool OnEnable()
	{
		StatusState statusState = Target.StatusEffects.Find((StatusState s) => s is Plague);
		if (statusState != null)
		{
			CancelStatus(Target, StatusEnum.Plague);
			int num = Mathf.RoundToInt((float)Target.MaxHP * base.Val1Pct);
			if (num == 0)
			{
				num = 1;
			}
			Target.DealDamage(num, AttackBase.None, false, null, null);
			return true;
		}
		return false;
	}
}
