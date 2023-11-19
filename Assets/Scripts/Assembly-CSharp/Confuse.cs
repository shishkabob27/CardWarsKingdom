public class Confuse : StatusState
{
	public override bool AttackRandom()
	{
		if (base.Count > 0)
		{
			TickCount();
			return true;
		}
		return false;
	}
}
