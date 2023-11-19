using UnityEngine;

public class IgnoreLocalRotation : MonoBehaviour
{
	private Transform mTrToFollow;

	private void Start()
	{
		mTrToFollow = base.transform.parent;
	}

	private void Update()
	{
		if (mTrToFollow == null)
		{
			mTrToFollow = base.transform.parent;
			return;
		}
		base.transform.parent = null;
		base.transform.position = mTrToFollow.position;
	}
}
