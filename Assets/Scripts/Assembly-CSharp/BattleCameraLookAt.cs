using UnityEngine;

public class BattleCameraLookAt : MonoBehaviour
{
	public Camera mCameraToFollow;

	public bool mFollowFlag = true;

	public void SetFollowCam(bool enable)
	{
		mFollowFlag = enable;
	}

	private void LateUpdate()
	{
		if (mFollowFlag)
		{
			mCameraToFollow.transform.LookAt(base.transform);
		}
	}
}
