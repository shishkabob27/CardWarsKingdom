public class CompleteAllDailies : Mission
{
	public override int ProgressValue
	{
		get
		{
			return base.Progress.DailiesCompleted;
		}
	}
}
