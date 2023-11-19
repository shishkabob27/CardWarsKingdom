using System.Collections;
using System.Collections.Generic;

public class KFFJobManager : Singleton<KFFJobManager>
{
	private List<AbstractJob> _ThreadJobList = new List<AbstractJob>();

	private List<AbstractJob> _ThreadJobCompletedList = new List<AbstractJob>();

	private void Awake()
	{
		StartCoroutine("UpdateJobs");
	}

	public void AddThreadJob(AbstractJob aJob)
	{
		_ThreadJobList.Add(aJob);
	}

	private IEnumerator UpdateJobs()
	{
		while (true)
		{
			foreach (AbstractJob Job in _ThreadJobList)
			{
				if (Job.State == AbstractJob.eState.NOT_RUNNING)
				{
					Job.Spawn();
				}
				else if (Job.State == AbstractJob.eState.FINISH)
				{
					Job.Completed();
					_ThreadJobCompletedList.Add(Job);
				}
			}
			foreach (AbstractJob CompletedJob in _ThreadJobCompletedList)
			{
				_ThreadJobList.Remove(CompletedJob);
			}
			_ThreadJobCompletedList.Clear();
			yield return null;
		}
	}
}
