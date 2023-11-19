public class HealOnStartTurnWhenStealthed : OnStartTurn
{
	public override bool OnEnable()
	{
		if ((bool)base.Owner.HasStealth && !base.Owner.AtFullHealth)
		{
			base.Owner.Heal(base.Val1Pct);
			return true;
		}
		return false;
	}
}
