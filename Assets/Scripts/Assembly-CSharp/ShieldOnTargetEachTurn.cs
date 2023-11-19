public class ShieldOnTargetEachTurn : OnStartTurn
{
	public override bool OnEnable()
	{
		CreatureState creatureState = base.Owner.Owner.GetCreatures().Find((CreatureState m) => m.Data.Form.ID == base.ValID);
		if (creatureState != null)
		{
			ApplyStatus(creatureState, StatusEnum.Shield, base.Val1);
			return true;
		}
		return false;
	}
}
