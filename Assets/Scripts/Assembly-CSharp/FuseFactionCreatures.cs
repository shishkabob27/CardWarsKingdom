public class FuseFactionCreatures : Mission
{
	public override int ProgressValue
	{
		get
		{
			return base.Progress.FactionFused[base.Val2];
		}
	}
}
