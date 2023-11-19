public class TransmogrifyOppLowestHPOnDeploy : OnDeploy
{
	public override bool OnEnable()
	{
		CreatureState creatureState = null;
		foreach (LaneState lane in base.Owner.Owner.Opponent.Lanes)
		{
			if (lane.Creature != null && (creatureState == null || lane.Creature.HP < creatureState.HP))
			{
				creatureState = lane.Creature;
			}
		}
		if (creatureState != null)
		{
			ApplyStatus(creatureState, StatusEnum.Transmogrify, base.Val1);
			return true;
		}
		return false;
	}
}
