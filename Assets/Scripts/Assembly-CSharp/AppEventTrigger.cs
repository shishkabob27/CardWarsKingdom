using System.Collections.Generic;
using UnityEngine;

public class AppEventTrigger : MonoBehaviour
{
	public List<EventDelegate> onApplicationPause = new List<EventDelegate>();

	public List<EventDelegate> onApplicationResume = new List<EventDelegate>();

	private void OnApplicationPause(bool paused)
	{
		EventDelegate.Execute((!paused) ? onApplicationResume : onApplicationPause);
	}
}
