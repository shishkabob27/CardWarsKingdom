using System;
using System.Collections;
using UnityEngine;

public class DateEntryItem : MonoBehaviour
{
	public enum Slot
	{
		Month,
		Day,
		Year
	}

	private const int StartYear = 1950;

	public float SnapSpeed;

	public GameObject TextPrefab;

	private Slot mSlot;

	private int mSelectedIndex;

	public void Populate(Slot slot, int year, int day, int month)
	{
		base.transform.DestroyAllChildren();
		mSlot = slot;
		float width = (float)base.transform.parent.GetComponent<UIWidget>().width * base.transform.parent.localScale.x;
		switch (slot)
		{
		case Slot.Month:
		{
			int num2 = 0;
			for (int j = 1; j <= 12; j++)
			{
				CreateLabel(KFFLocalization.Get("!!MONTH" + j), width);
				if (j == month)
				{
					mSelectedIndex = num2;
				}
				num2++;
			}
			break;
		}
		case Slot.Day:
		{
			int num3 = 0;
			int num4 = DateTime.DaysInMonth(year, month);
			for (int k = 1; k <= num4; k++)
			{
				CreateLabel(k.ToString(), width);
				if (k == day)
				{
					mSelectedIndex = num3;
				}
				num3++;
			}
			break;
		}
		case Slot.Year:
		{
			int num = 0;
			for (int i = 1950; i <= DateTime.Now.Year; i++)
			{
				CreateLabel(i.ToString(), width);
				if (i == year)
				{
					mSelectedIndex = num;
				}
				num++;
			}
			break;
		}
		}
		GetComponent<UIGrid>().Reposition();
		StartCoroutine(SnapToSelectedNextFrame());
	}

	private IEnumerator SnapToSelectedNextFrame()
	{
		UICamera.LockInput();
		yield return null;
		UICamera.UnlockInput();
		SnapToSelected();
	}

	private void CreateLabel(string text, float width)
	{
		UILabel component = base.transform.InstantiateAsChild(TextPrefab).GetComponent<UILabel>();
		component.gameObject.SetActive(true);
		component.text = text;
		BoxCollider component2 = component.GetComponent<BoxCollider>();
		Vector3 size = component2.size;
		size.x = width;
		component2.size = size;
		component.GetComponent<DateEntryListItem>().OnPressedAction = OnPressed;
	}

	private void OnPressed(bool pressed)
	{
		if (pressed)
		{
			SpringPanel component = base.gameObject.GetComponent<SpringPanel>();
			if (component != null)
			{
				component.enabled = false;
			}
		}
		else
		{
			FindNearestIndex();
			Singleton<SimplePopupController>.Instance.OnDateUpdated();
		}
	}

	private void FindNearestIndex()
	{
		float num = float.MaxValue;
		for (int i = 0; i < base.transform.childCount; i++)
		{
			Transform child = base.transform.GetChild(i);
			float num2 = Mathf.Abs(child.position.y - base.transform.parent.position.y);
			if (num2 < num)
			{
				num = num2;
				mSelectedIndex = i;
			}
		}
	}

	private void SnapToSelected()
	{
		if (mSelectedIndex >= base.transform.childCount)
		{
			mSelectedIndex = base.transform.childCount - 1;
		}
		Transform child = base.transform.GetChild(mSelectedIndex);
		Vector3 localPosition = child.localPosition;
		localPosition.y *= -1f;
		SpringPanel.Begin(base.gameObject, localPosition, 1000f);
	}

	public void Clear()
	{
		base.transform.DestroyAllChildren();
	}

	public int GetSelectedValue()
	{
		if (mSlot == Slot.Year)
		{
			return 1950 + mSelectedIndex;
		}
		return 1 + mSelectedIndex;
	}
}
