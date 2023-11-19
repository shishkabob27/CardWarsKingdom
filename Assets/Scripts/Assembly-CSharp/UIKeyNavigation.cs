using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Key Navigation")]
public class UIKeyNavigation : MonoBehaviour
{
	public enum Constraint
	{
		None,
		Vertical,
		Horizontal,
		Explicit
	}

	public static BetterList<UIKeyNavigation> list = new BetterList<UIKeyNavigation>();

	public Constraint constraint;

	public GameObject onUp;

	public GameObject onDown;

	public GameObject onLeft;

	public GameObject onRight;

	public GameObject onClick;

	public bool startsSelected;

	protected virtual void OnEnable()
	{
		list.Add(this);
		if (startsSelected && (UICamera.selectedObject == null || !NGUITools.GetActive(UICamera.selectedObject)))
		{
			UICamera.currentScheme = UICamera.ControlScheme.Controller;
			UICamera.selectedObject = base.gameObject;
		}
	}

	protected virtual void OnDisable()
	{
		list.Remove(this);
	}

	protected GameObject GetLeft()
	{
		if (NGUITools.GetActive(onLeft))
		{
			return onLeft;
		}
		if (constraint == Constraint.Vertical || constraint == Constraint.Explicit)
		{
			return null;
		}
		return Get(Vector3.left, true);
	}

	private GameObject GetRight()
	{
		if (NGUITools.GetActive(onRight))
		{
			return onRight;
		}
		if (constraint == Constraint.Vertical || constraint == Constraint.Explicit)
		{
			return null;
		}
		return Get(Vector3.right, true);
	}

	protected GameObject GetUp()
	{
		if (NGUITools.GetActive(onUp))
		{
			return onUp;
		}
		if (constraint == Constraint.Horizontal || constraint == Constraint.Explicit)
		{
			return null;
		}
		return Get(Vector3.up, false);
	}

	protected GameObject GetDown()
	{
		if (NGUITools.GetActive(onDown))
		{
			return onDown;
		}
		if (constraint == Constraint.Horizontal || constraint == Constraint.Explicit)
		{
			return null;
		}
		return Get(Vector3.down, false);
	}

	protected GameObject Get(Vector3 myDir, bool horizontal)
	{
		Transform transform = base.transform;
		myDir = transform.TransformDirection(myDir);
		Vector3 center = GetCenter(base.gameObject);
		float num = float.MaxValue;
		GameObject result = null;
		for (int i = 0; i < list.size; i++)
		{
			UIKeyNavigation uIKeyNavigation = list[i];
			if (uIKeyNavigation == this)
			{
				continue;
			}
			UIButton component = uIKeyNavigation.GetComponent<UIButton>();
			if (component != null && !component.isEnabled)
			{
				continue;
			}
			Vector3 direction = GetCenter(uIKeyNavigation.gameObject) - center;
			float num2 = Vector3.Dot(myDir, direction.normalized);
			if (!(num2 < 0.707f))
			{
				direction = transform.InverseTransformDirection(direction);
				if (horizontal)
				{
					direction.y *= 2f;
				}
				else
				{
					direction.x *= 2f;
				}
				float sqrMagnitude = direction.sqrMagnitude;
				if (!(sqrMagnitude > num))
				{
					result = uIKeyNavigation.gameObject;
					num = sqrMagnitude;
				}
			}
		}
		return result;
	}

	protected static Vector3 GetCenter(GameObject go)
	{
		UIWidget component = go.GetComponent<UIWidget>();
		if (component != null)
		{
			Vector3[] worldCorners = component.worldCorners;
			return (worldCorners[0] + worldCorners[2]) * 0.5f;
		}
		return go.transform.position;
	}

	protected virtual void OnKey(KeyCode key)
	{
		if (!NGUITools.GetActive(this))
		{
			return;
		}
		GameObject gameObject = null;
		switch (key)
		{
		case KeyCode.LeftArrow:
			gameObject = GetLeft();
			break;
		case KeyCode.RightArrow:
			gameObject = GetRight();
			break;
		case KeyCode.UpArrow:
			gameObject = GetUp();
			break;
		case KeyCode.DownArrow:
			gameObject = GetDown();
			break;
		case KeyCode.Tab:
			if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
			{
				gameObject = GetLeft();
				if (gameObject == null)
				{
					gameObject = GetUp();
				}
				if (gameObject == null)
				{
					gameObject = GetDown();
				}
				if (gameObject == null)
				{
					gameObject = GetRight();
				}
			}
			else
			{
				gameObject = GetRight();
				if (gameObject == null)
				{
					gameObject = GetDown();
				}
				if (gameObject == null)
				{
					gameObject = GetUp();
				}
				if (gameObject == null)
				{
					gameObject = GetLeft();
				}
			}
			break;
		}
		if (gameObject != null)
		{
			UICamera.selectedObject = gameObject;
		}
	}

	protected virtual void OnClick()
	{
		if (NGUITools.GetActive(this) && NGUITools.GetActive(onClick))
		{
			UICamera.selectedObject = onClick;
		}
	}
}
