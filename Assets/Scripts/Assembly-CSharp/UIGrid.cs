using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Grid")]
public class UIGrid : UIWidgetContainer
{
	public enum Arrangement
	{
		Horizontal,
		Vertical
	}

	public enum Sorting
	{
		None,
		Alphabetic,
		Horizontal,
		Vertical,
		Custom
	}

	public delegate void OnReposition();

	public Arrangement arrangement;

	public Sorting sorting;

	public UIWidget.Pivot pivot;

	public int maxPerLine;

	public float cellWidth = 200f;

	public float cellHeight = 200f;

	public float sizeLimit;

	public bool animateSmoothly;

	public bool hideInactive = true;

	public bool keepWithinPanel;

	public OnReposition onReposition;

	public Comparison<Transform> onCustomSort;

	[SerializeField]
	[HideInInspector]
	private bool sorted;

	protected bool mReposition;

	protected UIPanel mPanel;

	protected bool mInitDone;

	public bool repositionNow
	{
		set
		{
			if (value)
			{
				mReposition = true;
				base.enabled = true;
			}
		}
	}

	public List<Transform> GetChildList()
	{
		Transform transform = base.transform;
		List<Transform> list = new List<Transform>();
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			if (!hideInactive || ((bool)child && NGUITools.GetActive(child.gameObject)))
			{
				list.Add(child);
			}
		}
		if (sorting != 0)
		{
			if (sorting == Sorting.Alphabetic)
			{
				list.Sort(SortByName);
			}
			else if (sorting == Sorting.Horizontal)
			{
				list.Sort(SortHorizontal);
			}
			else if (sorting == Sorting.Vertical)
			{
				list.Sort(SortVertical);
			}
			else if (onCustomSort != null)
			{
				list.Sort(onCustomSort);
			}
			else
			{
				Sort(list);
			}
		}
		return list;
	}

	public Transform GetChild(int index)
	{
		List<Transform> childList = GetChildList();
		return (index >= childList.Count) ? null : childList[index];
	}

	public int GetIndex(Transform trans)
	{
		return GetChildList().IndexOf(trans);
	}

	public void AddChild(Transform trans)
	{
		AddChild(trans, true);
	}

	public void AddChild(Transform trans, bool sort)
	{
		if (trans != null)
		{
			trans.parent = base.transform;
			ResetPosition(GetChildList());
		}
	}

	public bool RemoveChild(Transform t)
	{
		List<Transform> childList = GetChildList();
		if (childList.Remove(t))
		{
			ResetPosition(childList);
			return true;
		}
		return false;
	}

	protected virtual void Init()
	{
		mInitDone = true;
		mPanel = NGUITools.FindInParents<UIPanel>(base.gameObject);
	}

	protected virtual void Start()
	{
		if (!mInitDone)
		{
			Init();
		}
		bool flag = animateSmoothly;
		animateSmoothly = false;
		Reposition();
		animateSmoothly = flag;
		base.enabled = false;
	}

	protected virtual void Update()
	{
		if (mReposition)
		{
			Reposition();
		}
		base.enabled = false;
	}

	public static int SortByName(Transform a, Transform b)
	{
		return string.Compare(a.name, b.name);
	}

	public static int SortHorizontal(Transform a, Transform b)
	{
		return a.localPosition.x.CompareTo(b.localPosition.x);
	}

	public static int SortVertical(Transform a, Transform b)
	{
		return b.localPosition.y.CompareTo(a.localPosition.y);
	}

	protected virtual void Sort(List<Transform> list)
	{
	}

	[ContextMenu("Execute")]
	public virtual void Reposition()
	{
		if (Application.isPlaying && !mInitDone && NGUITools.GetActive(this))
		{
			mReposition = true;
			return;
		}
		if (sorted)
		{
			sorted = false;
			if (sorting == Sorting.None)
			{
				sorting = Sorting.Alphabetic;
			}
			NGUITools.SetDirty(this);
		}
		if (!mInitDone)
		{
			Init();
		}
		List<Transform> childList = GetChildList();
		ResetPosition(childList);
		if (keepWithinPanel)
		{
			ConstrainWithinPanel();
		}
		if (onReposition != null)
		{
			onReposition();
		}
	}

	public void ConstrainWithinPanel()
	{
		if (mPanel != null)
		{
			mPanel.ConstrainTargetToBounds(base.transform, true);
		}
	}

	protected void ResetPosition(List<Transform> list)
	{
		mReposition = false;
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		Transform transform = base.transform;
		float num5 = cellWidth;
		float num6 = cellHeight;
		if (sizeLimit > 0f && maxPerLine == 0)
		{
			if (arrangement == Arrangement.Horizontal)
			{
				float num7 = (float)list.Count * cellWidth;
				if (num7 > sizeLimit)
				{
					num5 *= sizeLimit / num7;
				}
			}
			else
			{
				float num8 = (float)list.Count * cellHeight;
				if (num8 > sizeLimit)
				{
					num6 *= sizeLimit / num8;
				}
			}
		}
		int i = 0;
		for (int count = list.Count; i < count; i++)
		{
			Transform transform2 = list[i];
			float z = transform2.localPosition.z;
			Vector3 vector = ((arrangement != 0) ? new Vector3(num5 * (float)num2, (0f - num6) * (float)num, z) : new Vector3(num5 * (float)num, (0f - num6) * (float)num2, z));
			if (animateSmoothly && Application.isPlaying)
			{
				SpringPosition springPosition = SpringPosition.Begin(transform2.gameObject, vector, 15f);
				springPosition.updateScrollView = true;
				springPosition.ignoreTimeScale = true;
			}
			else
			{
				transform2.localPosition = vector;
			}
			num3 = Mathf.Max(num3, num);
			num4 = Mathf.Max(num4, num2);
			if (++num >= maxPerLine && maxPerLine > 0)
			{
				num = 0;
				num2++;
			}
		}
		if (pivot == UIWidget.Pivot.TopLeft)
		{
			return;
		}
		Vector2 pivotOffset = NGUIMath.GetPivotOffset(pivot);
		float num9;
		float num10;
		if (arrangement == Arrangement.Horizontal)
		{
			num9 = Mathf.Lerp(0f, (float)num3 * num5, pivotOffset.x);
			num10 = Mathf.Lerp((float)(-num4) * num6, 0f, pivotOffset.y);
		}
		else
		{
			num9 = Mathf.Lerp(0f, (float)num4 * num5, pivotOffset.x);
			num10 = Mathf.Lerp((float)(-num3) * num6, 0f, pivotOffset.y);
		}
		for (int j = 0; j < transform.childCount; j++)
		{
			Transform child = transform.GetChild(j);
			SpringPosition component = child.GetComponent<SpringPosition>();
			if (component != null)
			{
				component.target.x -= num9;
				component.target.y -= num10;
				continue;
			}
			Vector3 localPosition = child.localPosition;
			localPosition.x -= num9;
			localPosition.y -= num10;
			child.localPosition = localPosition;
		}
	}
}
