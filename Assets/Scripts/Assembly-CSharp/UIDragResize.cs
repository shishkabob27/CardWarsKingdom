using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Drag-Resize Widget")]
public class UIDragResize : MonoBehaviour
{
	public UIWidget target;

	public UIWidget.Pivot pivot = UIWidget.Pivot.BottomRight;

	public int minWidth = 100;

	public int minHeight = 100;

	public int maxWidth = 100000;

	public int maxHeight = 100000;

	private Plane mPlane;

	private Vector3 mRayPos;

	private Vector3 mLocalPos;

	private int mWidth;

	private int mHeight;

	private bool mDragging;

	private void OnDragStart()
	{
		if (target != null)
		{
			Vector3[] worldCorners = target.worldCorners;
			mPlane = new Plane(worldCorners[0], worldCorners[1], worldCorners[3]);
			Ray currentRay = UICamera.currentRay;
			float enter;
			if (mPlane.Raycast(currentRay, out enter))
			{
				mRayPos = currentRay.GetPoint(enter);
				mLocalPos = target.cachedTransform.localPosition;
				mWidth = target.width;
				mHeight = target.height;
				mDragging = true;
			}
		}
	}

	private void OnDrag(Vector2 delta)
	{
		if (mDragging && target != null)
		{
			Ray currentRay = UICamera.currentRay;
			float enter;
			if (mPlane.Raycast(currentRay, out enter))
			{
				Transform cachedTransform = target.cachedTransform;
				cachedTransform.localPosition = mLocalPos;
				target.width = mWidth;
				target.height = mHeight;
				Vector3 vector = currentRay.GetPoint(enter) - mRayPos;
				cachedTransform.position += vector;
				Vector3 vector2 = Quaternion.Inverse(cachedTransform.localRotation) * (cachedTransform.localPosition - mLocalPos);
				cachedTransform.localPosition = mLocalPos;
				NGUIMath.ResizeWidget(target, pivot, vector2.x, vector2.y, minWidth, minHeight, maxWidth, maxHeight);
			}
		}
	}

	private void OnDragEnd()
	{
		mDragging = false;
	}
}
