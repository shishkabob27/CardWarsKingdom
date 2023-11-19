using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Wrap Content")]
public class UIWrapContent : MonoBehaviour
{
	public delegate void OnInitializeItem(GameObject go, int wrapIndex, int realIndex);

	public int itemSize = 100;

	public bool cullContent = true;

	public int minIndex;

	public int maxIndex;

	public OnInitializeItem onInitializeItem;

	private Transform mTrans;

	private UIPanel mPanel;

	private UIScrollView mScroll;

	private bool mHorizontal;

	private bool mFirstTime = true;

	private List<Transform> mChildren = new List<Transform>();

	protected virtual void Start()
	{
		SortBasedOnScrollMovement();
		WrapContent();
		if (mScroll != null)
		{
			mScroll.GetComponent<UIPanel>().onClipMove = OnMove;
		}
		mFirstTime = false;
	}

	protected virtual void OnMove(UIPanel panel)
	{
		WrapContent();
	}

	[ContextMenu("Sort Based on Scroll Movement")]
	public void SortBasedOnScrollMovement()
	{
		if (CacheScrollView())
		{
			mChildren.Clear();
			for (int i = 0; i < mTrans.childCount; i++)
			{
				mChildren.Add(mTrans.GetChild(i));
			}
			if (mHorizontal)
			{
				mChildren.Sort(UIGrid.SortHorizontal);
			}
			else
			{
				mChildren.Sort(UIGrid.SortVertical);
			}
			ResetChildPositions();
		}
	}

	[ContextMenu("Sort Alphabetically")]
	public void SortAlphabetically()
	{
		if (CacheScrollView())
		{
			mChildren.Clear();
			for (int i = 0; i < mTrans.childCount; i++)
			{
				mChildren.Add(mTrans.GetChild(i));
			}
			mChildren.Sort(UIGrid.SortByName);
			ResetChildPositions();
		}
	}

	protected bool CacheScrollView()
	{
		mTrans = base.transform;
		mPanel = NGUITools.FindInParents<UIPanel>(base.gameObject);
		mScroll = mPanel.GetComponent<UIScrollView>();
		if (mScroll == null)
		{
			return false;
		}
		if (mScroll.movement == UIScrollView.Movement.Horizontal)
		{
			mHorizontal = true;
		}
		else
		{
			if (mScroll.movement != UIScrollView.Movement.Vertical)
			{
				return false;
			}
			mHorizontal = false;
		}
		return true;
	}

	private void ResetChildPositions()
	{
		int i = 0;
		for (int count = mChildren.Count; i < count; i++)
		{
			Transform transform = mChildren[i];
			transform.localPosition = ((!mHorizontal) ? new Vector3(0f, -i * itemSize, 0f) : new Vector3(i * itemSize, 0f, 0f));
		}
	}

	public void WrapContent()
	{
		float num = (float)(itemSize * mChildren.Count) * 0.5f;
		Vector3[] worldCorners = mPanel.worldCorners;
		for (int i = 0; i < 4; i++)
		{
			Vector3 position = worldCorners[i];
			position = mTrans.InverseTransformPoint(position);
			worldCorners[i] = position;
		}
		Vector3 vector = Vector3.Lerp(worldCorners[0], worldCorners[2], 0.5f);
		bool flag = true;
		float num2 = num * 2f;
		if (mHorizontal)
		{
			float num3 = worldCorners[0].x - (float)itemSize;
			float num4 = worldCorners[2].x + (float)itemSize;
			int j = 0;
			for (int count = mChildren.Count; j < count; j++)
			{
				Transform transform = mChildren[j];
				float num5 = transform.localPosition.x - vector.x;
				if (num5 < 0f - num)
				{
					Vector3 localPosition = transform.localPosition;
					localPosition.x += num2;
					num5 = localPosition.x - vector.x;
					int num6 = Mathf.RoundToInt(localPosition.x / (float)itemSize);
					if (minIndex == maxIndex || (minIndex <= num6 && num6 <= maxIndex))
					{
						transform.localPosition = localPosition;
						UpdateItem(transform, j);
						transform.name = num6.ToString();
					}
					else
					{
						flag = false;
					}
				}
				else if (num5 > num)
				{
					Vector3 localPosition2 = transform.localPosition;
					localPosition2.x -= num2;
					num5 = localPosition2.x - vector.x;
					int num7 = Mathf.RoundToInt(localPosition2.x / (float)itemSize);
					if (minIndex == maxIndex || (minIndex <= num7 && num7 <= maxIndex))
					{
						transform.localPosition = localPosition2;
						UpdateItem(transform, j);
						transform.name = num7.ToString();
					}
					else
					{
						flag = false;
					}
				}
				else if (mFirstTime)
				{
					UpdateItem(transform, j);
				}
				if (cullContent)
				{
					num5 += mPanel.clipOffset.x - mTrans.localPosition.x;
					if (!UICamera.IsPressed(transform.gameObject))
					{
						NGUITools.SetActive(transform.gameObject, num5 > num3 && num5 < num4, false);
					}
				}
			}
			return;
		}
		float num8 = worldCorners[0].y - (float)itemSize;
		float num9 = worldCorners[2].y + (float)itemSize;
		int k = 0;
		for (int count2 = mChildren.Count; k < count2; k++)
		{
			Transform transform2 = mChildren[k];
			float num10 = transform2.localPosition.y - vector.y;
			if (num10 < 0f - num)
			{
				Vector3 localPosition3 = transform2.localPosition;
				localPosition3.y += num2;
				num10 = localPosition3.y - vector.y;
				int num11 = Mathf.RoundToInt(localPosition3.y / (float)itemSize);
				if (minIndex == maxIndex || (minIndex <= num11 && num11 <= maxIndex))
				{
					transform2.localPosition = localPosition3;
					UpdateItem(transform2, k);
					transform2.name = num11.ToString();
				}
				else
				{
					flag = false;
				}
			}
			else if (num10 > num)
			{
				Vector3 localPosition4 = transform2.localPosition;
				localPosition4.y -= num2;
				num10 = localPosition4.y - vector.y;
				int num12 = Mathf.RoundToInt(localPosition4.y / (float)itemSize);
				if (minIndex == maxIndex || (minIndex <= num12 && num12 <= maxIndex))
				{
					transform2.localPosition = localPosition4;
					UpdateItem(transform2, k);
					transform2.name = num12.ToString();
				}
				else
				{
					flag = false;
				}
			}
			else if (mFirstTime)
			{
				UpdateItem(transform2, k);
			}
			if (cullContent)
			{
				num10 += mPanel.clipOffset.y - mTrans.localPosition.y;
				if (!UICamera.IsPressed(transform2.gameObject))
				{
					NGUITools.SetActive(transform2.gameObject, num10 > num8 && num10 < num9, false);
				}
			}
		}
		mScroll.restrictWithinPanel = !flag;
	}

	private void OnValidate()
	{
		if (maxIndex < minIndex)
		{
			maxIndex = minIndex;
		}
		if (minIndex > maxIndex)
		{
			maxIndex = minIndex;
		}
	}

	protected virtual void UpdateItem(Transform item, int index)
	{
		if (onInitializeItem != null)
		{
			int realIndex = ((mScroll.movement != UIScrollView.Movement.Vertical) ? Mathf.RoundToInt(item.localPosition.x / (float)itemSize) : Mathf.RoundToInt(item.localPosition.y / (float)itemSize));
			onInitializeItem(item.gameObject, index, realIndex);
		}
	}
}
