using System.Threading;

public abstract class AbstractJob
{
	public enum eState
	{
		NOT_RUNNING,
		RUNNING,
		FINISH
	}

	protected eState _State;

	public eState State
	{
		get
		{
			return _State;
		}
		set
		{
			_State = value;
		}
	}

	public void Spawn()
	{
		_State = eState.RUNNING;
		ThreadPool.QueueUserWorkItem(delegate
		{
			Run();
		});
	}

	public abstract void Run();

	public abstract void Completed();
}
