using UnityEngine;

public class JobManagerUnitTests : MonoBehaviour
{
	public static int TestValue;

	public static object TestObj = new object();

	public static object TestObj2;

	private void Start()
	{
		AbstractJob aJob = new JobTest();
		Singleton<KFFJobManager>.Instance.AddThreadJob(aJob);
	}

	private void Update()
	{
	}
}
