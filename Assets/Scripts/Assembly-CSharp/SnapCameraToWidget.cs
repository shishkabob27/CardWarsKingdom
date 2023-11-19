using UnityEngine;

public class SnapCameraToWidget : MonoBehaviour
{
	public UIWidget Widget;

	private void Start()
	{
		Camera uICam = Singleton<TownController>.Instance.GetUICam();
		Camera component = GetComponent<Camera>();
		Vector3[] worldCorners = Widget.worldCorners;
		Vector3 vector = uICam.WorldToScreenPoint(worldCorners[0]);
		Vector3 vector2 = uICam.WorldToScreenPoint(worldCorners[2]);
		vector.x /= Screen.width;
		vector.y /= Screen.height;
		vector2.x /= Screen.width;
		vector2.y /= Screen.height;
		Vector2 vector3 = new Vector2(vector2.x - vector.x, vector2.y - vector.y);
		component.rect = new Rect(vector.x, vector.y, vector3.x, vector3.y);
	}
}
