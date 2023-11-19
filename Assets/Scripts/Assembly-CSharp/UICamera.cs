using UnityEngine;

public class UICamera : MonoBehaviour
{
	public enum EventType
	{
		World_3D = 0,
		UI_3D = 1,
		World_2D = 2,
		UI_2D = 3,
	}

	public EventType eventType;
	public LayerMask eventReceiverMask;
	public bool debug;
	public bool useMouse;
	public bool useTouch;
	public bool allowMultiTouch;
	public bool useKeyboard;
	public bool useController;
	public bool stickyTooltip;
	public float tooltipDelay;
	public float mouseDragThreshold;
	public float mouseClickThreshold;
	public float touchDragThreshold;
	public float touchClickThreshold;
	public float rangeDistance;
	public string scrollAxisName;
	public string verticalAxisName;
	public string horizontalAxisName;
	public KeyCode submitKey0;
	public KeyCode submitKey1;
	public KeyCode cancelKey0;
	public KeyCode cancelKey1;
}
