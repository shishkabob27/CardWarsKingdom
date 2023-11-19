public class InflictPlagueIfNoOnAtk : OnAttack
{
	public override bool OnEnable()
	{
		StatusState statusState = Target.StatusEffects.Find((StatusState s) => s is Plague);
		if (statusState == null)
		{
			ApplyStatus(Target, StatusEnum.Plague, base.Val1);
			return true;
		}
		return false;
	}
}
