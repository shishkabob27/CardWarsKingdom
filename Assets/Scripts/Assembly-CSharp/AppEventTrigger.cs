using UnityEngine;
using System.Collections.Generic;

public class AppEventTrigger : MonoBehaviour
{
	public List<EventDelegate> onApplicationPause;
	public List<EventDelegate> onApplicationResume;
}
