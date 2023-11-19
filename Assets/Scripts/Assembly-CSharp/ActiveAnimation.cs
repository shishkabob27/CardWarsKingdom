using UnityEngine;
using System.Collections.Generic;

public class ActiveAnimation : MonoBehaviour
{
	public List<EventDelegate> onFinished;
	public GameObject eventReceiver;
	public string callWhenFinished;
}
