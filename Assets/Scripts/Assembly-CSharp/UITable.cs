using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Table")]
public class UITable : UIWidgetContainer
{
	public enum Direction
	{
		Down,
		Up
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

	public int columns;

	public Direction direction;

	public Sorting sorting;

	public bool hideInactive = true;

	public bool keepWithinPanel;

	public Vector2 padding = Vector2.zero;

	public OnReposition onReposition;

	protected UIPanel mPanel;

	protected bool mInitDone;

	protected bool mReposition;

	protected List<Transform> mChildren = new List<Transform>();

	[SerializeField]
	[HideInInspector]
	private bool sorted;

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

	public List<Transform> children
	{
		get
		{
			if (mChildren.Count == 0)
			{
				Transform transform = base.transform;
				for (int i = 0; i < transform.childCount; i++)
				{
					Transform child = transform.GetChild(i);
					if ((bool)child && (bool)child.gameObject && (!hideInactive || NGUITools.GetActive(child.gameObject)))
					{
						mChildren.Add(child);
					}
				}
				if (sorting != 0 || sorted)
				{
					if (sorting == Sorting.Alphabetic)
					{
						mChildren.Sort(UIGrid.SortByName);
					}
					else if (sorting == Sorting.Horizontal)
					{
						mChildren.Sort(UIGrid.SortHorizontal);
					}
					else if (sorting == Sorting.Vertical)
					{
						mChildren.Sort(UIGrid.SortVertical);
					}
					else
					{
						Sort(mChildren);
					}
				}
			}
			return mChildren;
		}
	}

	protected virtual void Sort(List<Transform> list)
	{
		list.Sort(UIGrid.SortByName);
	}

	protected void RepositionVariableSize(List<Transform> children)
	{
		float num = 0f;
		float num2 = 0f;
		int num3 = ((columns <= 0) ? 1 : (children.Count / columns + 1));
		int num4 = ((columns <= 0) ? children.Count : columns);
		Bounds[,] array = new Bounds[num3, num4];
		Bounds[] array2 = new Bounds[num4];
		Bounds[] array3 = new Bounds[num3];
		int num5 = 0;
		int num6 = 0;
		int i = 0;
		for (int count = children.Count; i < count; i++)
		{
			Transform transform = children[i];
			Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(transform, !hideInactive);
			Vector3 localScale = transform.localScale;
			bounds.min = Vector3.Scale(bounds.min, localScale);
			bounds.max = Vector3.Scale(bounds.max, localScale);
			array[num6, num5] = bounds;
			array2[num5].Encapsulate(bounds);
			array3[num6].Encapsulate(bounds);
			if (++num5 >= columns && columns > 0)
			{
				num5 = 0;
				num6++;
			}
		}
		num5 = 0;
		num6 = 0;
		int j = 0;
		for (int count2 = children.Count; j < count2; j++)
		{
			Transform transform2 = children[j];
			Bounds bounds2 = array[num6, num5];
			Bounds bounds3 = array2[num5];
			Bounds bounds4 = array3[num6];
			Vector3 localPosition = transform2.localPosition;
			localPosition.x = num + bounds2.extents.x - bounds2.center.x;
			localPosition.x += bounds2.min.x - bounds3.min.x + padding.x;
			if (direction == Direction.Down)
			{
				localPosition.y = 0f - num2 - bounds2.extents.y - bounds2.center.y;
				localPosition.y += (bounds2.max.y - bounds2.min.y - bounds4.max.y + bounds4.min.y) * 0.5f - padding.y;
			}
			else
			{
				localPosition.y = num2 + (bounds2.extents.y - bounds2.center.y);
				localPosition.y -= (bounds2.max.y - bounds2.min.y - bounds4.max.y + bounds4.min.y) * 0.5f - padding.y;
			}
			num += bounds3.max.x - bounds3.min.x + padding.x * 2f;
			transform2.localPosition = localPosition;
			if (++num5 >= columns && columns > 0)
			{
				num5 = 0;
				num6++;
				num = 0f;
				num2 += bounds4.size.y + padding.y * 2f;
			}
		}
	}

	[ContextMenu("Execute")]
	public virtual void Reposition()
	{
		if (Application.isPlaying && !mInitDone && NGUITools.GetActive(this))
		{
			mReposition = true;
			return;
		}
		if (!mInitDone)
		{
			Init();
		}
		mReposition = false;
		Transform target = base.transform;
		mChildren.Clear();
		List<Transform> list = children;
		if (list.Count > 0)
		{
			RepositionVariableSize(list);
		}
		if (keepWithinPanel && mPanel != null)
		{
			mPanel.ConstrainTargetToBounds(target, true);
			UIScrollView component = mPanel.GetComponent<UIScrollView>();
			if (component != null)
			{
				component.UpdateScrollbars(true);
			}
		}
		if (onReposition != null)
		{
			onReposition();
		}
	}

	protected virtual void Start()
	{
		Init();
		Reposition();
		base.enabled = false;
	}

	protected virtual void Init()
	{
		mInitDone = true;
		mPanel = NGUITools.FindInParents<UIPanel>(base.gameObject);
	}

	protected virtual void LateUpdate()
	{
		if (mReposition)
		{
			Reposition();
		}
		base.enabled = false;
	}
}
