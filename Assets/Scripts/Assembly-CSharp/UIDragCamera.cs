using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Drag Camera")]
[ExecuteInEditMode]
public class UIDragCamera : MonoBehaviour
{
	public UIDraggableCamera draggableCamera;

	private void Awake()
	{
		if (draggableCamera == null)
		{
			draggableCamera = NGUITools.FindInParents<UIDraggableCamera>(base.gameObject);
		}
	}

	private void OnPress(bool isPressed)
	{
		if (base.enabled && NGUITools.GetActive(base.gameObject) && draggableCamera != null)
		{
			draggableCamera.Press(isPressed);
		}
	}

	private void OnDrag(Vector2 delta)
	{
		if (base.enabled && NGUITools.GetActive(base.gameObject) && draggableCamera != null)
		{
			draggableCamera.Drag(delta);
		}
	}

	private void OnScroll(float delta)
	{
		if (base.enabled && NGUITools.GetActive(base.gameObject) && draggableCamera != null)
		{
			draggableCamera.Scroll(delta);
		}
	}
}
