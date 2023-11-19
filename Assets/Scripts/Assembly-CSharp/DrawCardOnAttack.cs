public class DrawCardOnAttack : OnPercentAttack
{
	public override bool OnEnable()
	{
		base.Owner.DrawCard();
		return true;
	}
}
