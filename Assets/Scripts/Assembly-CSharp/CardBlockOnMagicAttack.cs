public class CardBlockOnMagicAttack : OnMagicAttack
{
	public override bool OnEnable()
	{
		if (base.Val1Chance)
		{
			ApplyStatus(Target, StatusEnum.CardBlock, base.Val2);
			return true;
		}
		return false;
	}
}
