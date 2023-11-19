public class Marked : StatusState
{
	public override int AdjustCritChance(int chance)
	{
		return (int)((float)chance + (float)chance * base.Intensity);
	}
}
