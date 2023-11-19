using UnityEngine;

public class FrontEndPIP : MonoBehaviour
{
	public int player;

	public LeaderData CurrentLeader;

	public GameObject leaderObj;

	private Camera PIP_Camera;

	private Vector2 Size = default(Vector2);

	private Rect VPRect = default(Rect);

	private GameObject mCamAlignObj;

	private UIWidget mIconPortrait;

	private Camera mUICam;

	private bool mInitDone;

	public bool mShowPIP;

	public void Init(int n, LeaderData leader, GameObject thisLeaderObj, UIWidget iconPortrait)
	{
		CurrentLeader = leader;
		leaderObj = thisLeaderObj;
		player = n;
		PIP_Camera = GetComponent<Camera>();
		mUICam = Singleton<TownController>.Instance.GetUICam();
		string text = ((n != 0) ? CurrentLeader.PIPTransformP2 : CurrentLeader.PIPTransformP1);
		mCamAlignObj = Singleton<FrontEndPIPController>.Instance.gameObject.FindInChildren(text);
		PIP_Camera.transform.position = mCamAlignObj.transform.position;
		mIconPortrait = iconPortrait;
		mInitDone = true;
	}

	public void ShowPortrait(bool show)
	{
		mShowPIP = show;
	}

	private void Update()
	{
		if (!mInitDone)
		{
			return;
		}
		PIP_Camera.GetComponent<Camera>().enabled = mShowPIP;
		if (!mShowPIP)
		{
			return;
		}
		Vector3[] worldCorners = mIconPortrait.worldCorners;
		Vector3 vector = mUICam.WorldToScreenPoint(worldCorners[0]);
		Vector3 vector2 = mUICam.WorldToScreenPoint(worldCorners[2]);
		vector.x /= Screen.width;
		vector.y /= Screen.height;
		vector2.x /= Screen.width;
		vector2.y /= Screen.height;
		Size.x = vector2.x - vector.x;
		Size.y = vector2.y - vector.y;
		VPRect.Set(vector.x, vector.y, Size.x, Size.y);
		PIP_Camera.rect = VPRect;
		Renderer componentInChildren = leaderObj.GetComponentInChildren<Renderer>();
		if (componentInChildren != null)
		{
			if (!componentInChildren.isVisible)
			{
				PIP_Camera.fieldOfView = 40f;
			}
			else
			{
				PIP_Camera.fieldOfView = 30f;
			}
		}
	}
}
