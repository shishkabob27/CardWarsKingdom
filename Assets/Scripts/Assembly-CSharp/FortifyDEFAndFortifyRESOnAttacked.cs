public class FortifyDEFAndFortifyRESOnAttacked : OnAttacked
{
	public override bool OnEnable()
	{
		if (base.Val1Chance)
		{
			ApplyStatus(base.Owner, StatusEnum.DefenseBoost, base.Val2);
			ApplyStatus(base.Owner, StatusEnum.ResistanceBoost, base.Val2);
			return true;
		}
		return false;
	}
}
