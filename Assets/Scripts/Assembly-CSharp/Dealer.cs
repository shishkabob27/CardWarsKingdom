public class Dealer : StatusState
{
	public override int GetExtraCardCount()
	{
		int count = base.Count;
		while (base.Count > 0)
		{
			TickCount();
		}
		return count;
	}
}
