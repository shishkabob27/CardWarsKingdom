public class BurstSTR : StatusState
{
	protected override void OnEnable()
	{
		float num = (float)base.Target.Data.STR * base.Intensity;
		base.Target.STR += num;
	}

	public override void ProcessMessage(GameMessage Message)
	{
		if (base.Target != null && Message.Action == GameEvent.CREATURE_ATTACKED && (Message.AttackType == AttackBase.STR || Message.AttackType == AttackBase.Both) && Message.Creature == base.Target)
		{
			Disable();
		}
	}

	protected override void OnDisable()
	{
		float num = (float)base.Target.Data.STR * base.Intensity;
		base.Target.STR -= num;
	}
}
