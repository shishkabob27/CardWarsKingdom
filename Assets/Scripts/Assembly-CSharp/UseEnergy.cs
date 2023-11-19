public class UseEnergy : Mission
{
	public override int ProgressValue
	{
		get
		{
			return base.Progress.EnergyUsed;
		}
	}
}
