public class BurstINT : StatusState
{
	protected override void OnEnable()
	{
		float num = (float)base.Target.Data.INT * base.Intensity;
		base.Target.INT += num;
	}

	public override void ProcessMessage(GameMessage Message)
	{
		if (base.Target != null && Message.Action == GameEvent.CREATURE_ATTACKED && (Message.AttackType == AttackBase.INT || Message.AttackType == AttackBase.Both) && Message.Creature == base.Target)
		{
			Disable();
		}
	}

	protected override void OnDisable()
	{
		float num = (float)base.Target.Data.INT * base.Intensity;
		base.Target.INT -= num;
	}
}
