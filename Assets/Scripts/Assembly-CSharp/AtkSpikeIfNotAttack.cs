public class AtkSpikeIfNotAttack : OnAttackOREndTurn
{
	public override bool OnEnable()
	{
		if (!isEndTurn)
		{
			StoredValue++;
		}
		else
		{
			if (StoredValue <= 0)
			{
				ApplyStatus(base.Owner, StatusEnum.AttackSpike, base.Val1);
				return true;
			}
			StoredValue = 0;
		}
		return false;
	}
}
