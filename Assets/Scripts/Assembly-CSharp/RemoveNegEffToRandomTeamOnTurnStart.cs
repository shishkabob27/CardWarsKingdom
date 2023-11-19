using UnityEngine;

public class RemoveNegEffToRandomTeamOnTurnStart : OnStartTurn
{
	public override bool OnEnable()
	{
		bool result = false;
		LaneState[] array = new LaneState[base.Owner.Owner.Lanes.Count];
		int num = 0;
		foreach (LaneState lane in base.Owner.Owner.Lanes)
		{
			if (lane.Creature != null && lane.Creature.HasDebuff)
			{
				array[num] = lane;
				num++;
			}
		}
		if (num > 0)
		{
			int num2 = Random.Range(0, num - 1);
			ApplyStatus(array[num2].Creature, StatusEnum.RemoveStatus, base.Val1, StatusRemovalData.RemoveRandomDebuffs());
			result = true;
		}
		return result;
	}
}
