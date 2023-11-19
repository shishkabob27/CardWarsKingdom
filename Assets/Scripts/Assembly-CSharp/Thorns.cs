using UnityEngine;

public class Thorns : StatusState
{
	public override void ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.CREATURE_ATTACKED && Message.SecondCreature == base.Target && (Message.AttackType == AttackBase.STR || Message.AttackType == AttackBase.Both) && !Message.IsMiss)
		{
			float num = Message.RawAmount * base.Intensity;
			if (!Message.Creature.BreakDEF)
			{
				num *= 1f - Message.Creature.DEF / 100f;
			}
			int num2 = Mathf.CeilToInt(num);
			if (num2 == 0 && Message.RawAmount > 0f)
			{
				num2 = 1;
			}
			CreatureState target = base.Target;
			base.Target = Message.Creature;
			ReportStatusAction(GameEvent.STATUS_ACTION_THORNS, num2);
			base.Target = target;
			Message.Creature.DealDamage(num2, AttackBase.None, false, null, this);
		}
	}
}
