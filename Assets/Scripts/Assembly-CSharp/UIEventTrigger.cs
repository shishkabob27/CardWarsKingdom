using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Event Trigger")]
public class UIEventTrigger : MonoBehaviour
{
	public static UIEventTrigger current;

	public List<EventDelegate> onHoverOver = new List<EventDelegate>();

	public List<EventDelegate> onHoverOut = new List<EventDelegate>();

	public List<EventDelegate> onPress = new List<EventDelegate>();

	public List<EventDelegate> onRelease = new List<EventDelegate>();

	public List<EventDelegate> onSelect = new List<EventDelegate>();

	public List<EventDelegate> onDeselect = new List<EventDelegate>();

	public List<EventDelegate> onClick = new List<EventDelegate>();

	public List<EventDelegate> onDoubleClick = new List<EventDelegate>();

	public List<EventDelegate> onDragOver = new List<EventDelegate>();

	public List<EventDelegate> onDragOut = new List<EventDelegate>();

	private void OnHover(bool isOver)
	{
		if (!(current != null))
		{
			current = this;
			if (isOver)
			{
				EventDelegate.Execute(onHoverOver);
			}
			else
			{
				EventDelegate.Execute(onHoverOut);
			}
			current = null;
		}
	}

	private void OnPress(bool pressed)
	{
		if (!(current != null))
		{
			current = this;
			if (pressed)
			{
				EventDelegate.Execute(onPress);
			}
			else
			{
				EventDelegate.Execute(onRelease);
			}
			current = null;
		}
	}

	private void OnSelect(bool selected)
	{
		if (!(current != null))
		{
			current = this;
			if (selected)
			{
				EventDelegate.Execute(onSelect);
			}
			else
			{
				EventDelegate.Execute(onDeselect);
			}
			current = null;
		}
	}

	private void OnClick()
	{
		if (!(current != null))
		{
			current = this;
			EventDelegate.Execute(onClick);
			current = null;
		}
	}

	private void OnDoubleClick()
	{
		if (!(current != null))
		{
			current = this;
			EventDelegate.Execute(onDoubleClick);
			current = null;
		}
	}

	private void OnDragOver(GameObject go)
	{
		if (!(current != null))
		{
			current = this;
			EventDelegate.Execute(onDragOver);
			current = null;
		}
	}

	private void OnDragOut(GameObject go)
	{
		if (!(current != null))
		{
			current = this;
			EventDelegate.Execute(onDragOut);
			current = null;
		}
	}
}
