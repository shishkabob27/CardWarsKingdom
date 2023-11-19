public class CardBlock : StatusState
{
	public override bool BlockDraw()
	{
		if (base.Count > 0)
		{
			TickCount();
			return true;
		}
		return false;
	}
}
