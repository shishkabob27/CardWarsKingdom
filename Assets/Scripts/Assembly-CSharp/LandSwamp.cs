using UnityEngine;

public class LandSwamp : StatusState
{
	public override void ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.CREATURE_ATTACKED && Message.SecondCreature == base.Target && !Message.IsMiss)
		{
			Message.Creature.ApplyStatus(StatusDataManager.Instance.GetData(StatusEnum.Blind.Name()), 1f, null);
			Message.Creature.ApplyStatus(StatusDataManager.Instance.GetData(StatusEnum.Plague.Name()), base.Intensity * 100f, null);
		}
	}

	public override int AttackDiscount()
	{
		return Mathf.RoundToInt((0f - base.Intensity2) * 100f);
	}
}
