public class InflictDebuffs : Mission
{
	public override int ProgressValue
	{
		get
		{
			return base.Progress.StatusEffectsInflicted;
		}
	}
}
