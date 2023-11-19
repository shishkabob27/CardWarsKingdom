public class MagicCounter : CounterAttack
{
	public override void ProcessMessage(GameMessage Message)
	{
		Attack(Message, AttackBase.INT);
	}
}
