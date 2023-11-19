public class GrantWeakenStrOnAttacked : OnAttacked
{
	public override bool OnEnable()
	{
		bool result = false;
		StatusState statusState = Attacker.StatusEffects.Find((StatusState s) => s is WeakenSTR);
		if (statusState == null)
		{
			ApplyStatus(Attacker, StatusEnum.WeakenSTR, base.Val1);
			result = true;
		}
		return result;
	}
}
