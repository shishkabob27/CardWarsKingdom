public class Berserk : StatusState
{
	public override void ProcessMessage(GameMessage Message)
	{
		if (base.Target != null && base.Target.HP > 0 && Message.Action == GameEvent.START_TURN && Message.WhichPlayer == base.Target.Owner && base.Count > 0 && base.Target.AttackCost <= base.Target.Owner.ActionPoints)
		{
			CreatureState randomTarget = base.Target.Owner.GetRandomTarget(base.Target);
			if (randomTarget != null)
			{
				base.Target.Owner.AttackAndDraw(base.Target.Lane.Index, randomTarget.Lane.Index, AttackCause.Berserk);
			}
			TickCount();
		}
	}
}
