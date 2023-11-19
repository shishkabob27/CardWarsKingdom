public class AreaAttackSpikeResistanceOnFriendlyDeployed : OnDeployed
{
	public override bool OnEnable()
	{
		foreach (LaneState thisAndAdjacentLane in DeployedCreature.Lane.ThisAndAdjacentLanes)
		{
			if (thisAndAdjacentLane.Creature != null)
			{
				ApplyStatus(thisAndAdjacentLane.Creature, StatusEnum.AttackSpike, base.Val1);
				ApplyStatus(thisAndAdjacentLane.Creature, StatusEnum.ResistanceBoost, base.Val2);
			}
		}
		return true;
	}
}
