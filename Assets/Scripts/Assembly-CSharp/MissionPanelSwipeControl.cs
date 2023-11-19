using System.Collections.Generic;
using UnityEngine;

public class MissionPanelSwipeControl : MonoBehaviour
{
	private Vector3 fp;

	private Vector3 lp;

	private float dragDistance;

	private List<Vector3> touchPositions = new List<Vector3>();

	private void Start()
	{
		dragDistance = Screen.width * 20 / 100;
	}

	private void Update()
	{
		Touch[] touches = Input.touches;
		for (int i = 0; i < touches.Length; i++)
		{
			Touch touch = touches[i];
			if (touch.phase == TouchPhase.Moved)
			{
				touchPositions.Add(touch.position);
			}
			if (touch.phase != TouchPhase.Ended || touchPositions.Count <= 0)
			{
				continue;
			}
			fp = touchPositions[0];
			lp = touchPositions[touchPositions.Count - 1];
			if ((Mathf.Abs(lp.x - fp.x) > dragDistance || Mathf.Abs(lp.y - fp.y) > dragDistance) && Mathf.Abs(lp.x - fp.x) > Mathf.Abs(lp.y - fp.y))
			{
				if (lp.x > fp.x)
				{
					Singleton<MissionListController>.Instance.OnSwipeRight();
				}
				else
				{
					Singleton<MissionListController>.Instance.OnSwipeLeft();
				}
			}
			touchPositions.Clear();
			break;
		}
	}
}
