public class GetCriticals : Mission
{
	public override int ProgressValue
	{
		get
		{
			return base.Progress.Criticals;
		}
	}
}
