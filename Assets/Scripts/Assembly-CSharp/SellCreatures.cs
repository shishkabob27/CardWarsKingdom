public class SellCreatures : Mission
{
	public override int ProgressValue
	{
		get
		{
			return base.Progress.CreaturesSold;
		}
	}
}
