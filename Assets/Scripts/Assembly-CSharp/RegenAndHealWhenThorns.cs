public class RegenAndHealWhenThorns : OnFriendlyThorns
{
	public override bool OnEnable()
	{
		ApplyStatus(Target, StatusEnum.Regen, base.Val1);
		Target.Heal(base.Val2Pct);
		return true;
	}
}
