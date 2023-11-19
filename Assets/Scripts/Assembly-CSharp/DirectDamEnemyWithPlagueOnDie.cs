using UnityEngine;

public class DirectDamEnemyWithPlagueOnDie : OnDied
{
	public override bool OnEnable()
	{
		bool result = false;
		foreach (LaneState lane in base.Owner.Owner.Opponent.Lanes)
		{
			if (lane.Creature != null && lane.Creature.IsPlagued)
			{
				int hP = lane.Creature.HP - Mathf.RoundToInt(Mathf.Ceil((float)lane.Creature.MaxHP * base.Val1Pct));
				lane.Creature.SetHP(hP);
				result = true;
			}
		}
		return result;
	}
}
