public class PvPFinishController : Singleton<PvPFinishController>
{
	private AsyncData<string> mAsyncMatchData = new AsyncData<string>();

	private int mTrophiesTotalPrevious = -1;

	private int mTrophiesIncrement;

	public void Finish(bool win)
	{
	}
}
