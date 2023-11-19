public class StackStatus : Mission
{
	public override int ProgressValue
	{
		get
		{
			return base.Progress.EffectsStacked;
		}
	}
}
