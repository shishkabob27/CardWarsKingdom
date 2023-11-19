public class DrawCardsWhenDeployed : OnDeploy
{
	public override bool OnEnable()
	{
		for (int i = 0; i < base.Val1; i++)
		{
			base.Owner.DrawCard();
		}
		return true;
	}
}
