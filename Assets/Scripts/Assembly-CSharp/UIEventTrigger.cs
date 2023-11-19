using UnityEngine;
using System.Collections.Generic;

public class UIEventTrigger : MonoBehaviour
{
	public List<EventDelegate> onHoverOver;
	public List<EventDelegate> onHoverOut;
	public List<EventDelegate> onPress;
	public List<EventDelegate> onRelease;
	public List<EventDelegate> onSelect;
	public List<EventDelegate> onDeselect;
	public List<EventDelegate> onClick;
	public List<EventDelegate> onDoubleClick;
	public List<EventDelegate> onDragOver;
	public List<EventDelegate> onDragOut;
}
