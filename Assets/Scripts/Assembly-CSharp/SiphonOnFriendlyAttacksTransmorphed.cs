public class SiphonOnFriendlyAttacksTransmorphed : OnFriendlyAttack
{
	public override bool OnEnable()
	{
		if (Target != null && Target.HasTransmogrify)
		{
			ApplyStatus(Attacker, StatusEnum.Siphon, base.Val1);
			return true;
		}
		return false;
	}
}
