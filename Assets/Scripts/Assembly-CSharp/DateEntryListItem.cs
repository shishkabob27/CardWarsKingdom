using System;
using UnityEngine;

public class DateEntryListItem : MonoBehaviour
{
	public Action<bool> OnPressedAction;

	private void OnPress(bool pressed)
	{
		if (OnPressedAction != null)
		{
			OnPressedAction(pressed);
		}
	}
}
