public class GrantRevVampIfFrdCreatureDestroyed : OnFriendlyDamaged
{
	public override bool OnEnable()
	{
		if (Attacked.HP <= 0)
		{
			ApplyStatus(base.Owner, StatusEnum.Revenge, 1f);
			ApplyStatus(base.Owner, StatusEnum.Vampiric, base.Val1);
			return true;
		}
		return false;
	}
}
