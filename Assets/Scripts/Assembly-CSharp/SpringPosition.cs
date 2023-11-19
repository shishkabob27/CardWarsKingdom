using UnityEngine;

public class SpringPosition : MonoBehaviour
{
	public Vector3 target;
	public float strength;
	public bool worldSpace;
	public bool ignoreTimeScale;
	public bool updateScrollView;
	[SerializeField]
	private GameObject eventReceiver;
	[SerializeField]
	public string callWhenFinished;
}
