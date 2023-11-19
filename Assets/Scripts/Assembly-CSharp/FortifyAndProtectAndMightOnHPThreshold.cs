public class FortifyAndProtectAndMightOnHPThreshold : OnHPThreshold
{
	public override bool OnEnable()
	{
		ApplyStatus(base.Owner, StatusEnum.DefenseBoost, base.Val2);
		ApplyStatus(base.Owner, StatusEnum.ResistanceBoost, base.Val2);
		ApplyStatus(base.Owner, StatusEnum.AttackBoost, base.Val2);
		return true;
	}
}
