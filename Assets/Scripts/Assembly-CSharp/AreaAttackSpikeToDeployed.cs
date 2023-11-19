public class AreaAttackSpikeToDeployed : OnDeployed
{
	public override bool OnEnable()
	{
		foreach (LaneState thisAndAdjacentLane in DeployedCreature.Lane.ThisAndAdjacentLanes)
		{
			if (thisAndAdjacentLane.Creature != null)
			{
				ApplyStatus(thisAndAdjacentLane.Creature, StatusEnum.AttackSpike, base.Val1);
			}
		}
		return true;
	}
}
