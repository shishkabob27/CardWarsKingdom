public class JobTest : AbstractJob
{
	public override void Run()
	{
		if (JobManagerUnitTests.TestObj != null)
		{
			JobManagerUnitTests.TestValue = 1;
			JobManagerUnitTests.TestObj2 = new object();
			base.State = eState.FINISH;
		}
	}

	public override void Completed()
	{
	}
}
