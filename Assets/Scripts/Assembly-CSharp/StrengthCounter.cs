public class StrengthCounter : CounterAttack
{
	public override void ProcessMessage(GameMessage Message)
	{
		Attack(Message, AttackBase.STR);
	}
}
