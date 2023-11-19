using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class UIStreamingGridDataSource<T>
{
	private UIStreamingGrid mGridObject;

	private GameObject mPrefab;

	private UIScrollView mScrollView;

	private UIPanel mPanel;

	private ReadOnlyCollection<T> mDataList;

	private UIStreamingGridListItem[,] mContents;

	private bool mInitialized;

	private int mFirstRowIndex;

	private bool mIsHorizontal;

	private Vector2 mPanelSize;

	private float mCellSize;

	private int mRowSize;

	private int mNumRows;

	private float mMoveDistance;

	public bool Initialized
	{
		get
		{
			return mInitialized;
		}
	}

	public int FirstVisibleIndex
	{
		get
		{
			return mFirstRowIndex * mRowSize;
		}
	}

	public int LastVisibleIndex
	{
		get
		{
			return FirstVisibleIndex + mNumRows * mRowSize;
		}
	}

	public void Init(UIStreamingGrid gridObject, GameObject prefab, List<T> dataList, bool resetPosition = false, bool activateItems = false)
	{
		Init(gridObject, prefab, dataList.AsReadOnly(), resetPosition, activateItems);
	}

	public void Init(UIStreamingGrid gridObject, GameObject prefab, ReadOnlyCollection<T> dataList, bool resetPosition = false, bool activateItems = false)
	{
		Clear();
		Transform parent = gridObject.transform.parent;
		mGridObject = gridObject;
		mPrefab = prefab;
		mScrollView = NGUITools.FindInParents<UIScrollView>(parent);
		mPanel = NGUITools.FindInParents<UIPanel>(parent);
		mDataList = dataList;
		if (!(mScrollView.bounds.extents.x < gridObject.cellWidth) || mPanel.clipping == UIDrawCall.Clipping.SoftClip)
		{
		}
		Vector3 localPosition = mScrollView.transform.localPosition;
		SetScrollPos(Vector2.zero);
		mPanel.ResetAndUpdateAnchors();
		UIAnchor component = mGridObject.GetComponent<UIAnchor>();
		if (component != null)
		{
			component.ManualUpdate();
			component.enabled = false;
		}
		SetScrollPos(localPosition);
		mIsHorizontal = mScrollView.movement == UIScrollView.Movement.Vertical;
		mGridObject.SetFunctions(Update, SetScrollPos);
		if (resetPosition)
		{
			SetScrollPos(new Vector2(0f, 0f));
		}
		mPanelSize = mPanel.GetViewSize();
		int num2;
		int num;
		if (mIsHorizontal)
		{
			num = (int)(mPanelSize.x / mGridObject.cellWidth);
			num2 = (int)Math.Ceiling(mPanelSize.y / mGridObject.cellHeight);
			num2++;
		}
		else
		{
			num = (int)Math.Ceiling(mPanelSize.x / mGridObject.cellWidth);
			num2 = (int)(mPanelSize.y / mGridObject.cellHeight);
			num++;
		}
		if (mIsHorizontal)
		{
			mCellSize = mGridObject.cellHeight;
			mRowSize = num;
			mNumRows = num2;
			mMoveDistance = (float)(-num2) * mGridObject.cellHeight;
		}
		else
		{
			mCellSize = mGridObject.cellWidth;
			mRowSize = num2;
			mNumRows = num;
			mMoveDistance = (float)num * mGridObject.cellWidth;
		}
		SetScrollViewBounds();
		float num3 = CurrentScrollDistance();
		mFirstRowIndex = (int)((0f - num3) / mCellSize);
		mContents = new UIStreamingGridListItem[num, num2];
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num2; j++)
			{
				GameObject gameObject = mGridObject.transform.InstantiateAsChild(prefab);
				gameObject.ChangeLayer(mGridObject.gameObject.layer);
				UIStreamingGridListItem component2 = gameObject.GetComponent<UIStreamingGridListItem>();
				if (component2 == null)
				{
					return;
				}
				mContents[i, j] = component2;
				component2.ParentGrid = mGridObject;
				int x;
				int y;
				if (mIsHorizontal)
				{
					x = i;
					y = mFirstRowIndex + PosMod(j - mFirstRowIndex, mNumRows);
				}
				else
				{
					x = mFirstRowIndex + PosMod(i - mFirstRowIndex, mNumRows);
					y = j;
				}
				SetPrefabPosition(gameObject.transform, x, y);
				T dataItem;
				if (DataAtGridPosition(x, y, out dataItem))
				{
					if (activateItems)
					{
						gameObject.SetActive(true);
						component2.enabled = true;
					}
					component2.Populate(dataItem);
				}
				else
				{
					gameObject.gameObject.SetActive(false);
				}
			}
		}
		mScrollView.UpdateScrollbars();
		mInitialized = true;
	}

	private void SetScrollViewBounds()
	{
		int num = mRowSize;
		if (mRowSize == 0)
		{
			num = 1;
		}
		float width;
		float num2;
		if (mIsHorizontal)
		{
			width = mGridObject.cellWidth * (float)mRowSize;
			num2 = mGridObject.cellHeight * (float)(1 + (mDataList.Count - 1) / num);
		}
		else
		{
			width = mGridObject.cellWidth * (float)(1 + (mDataList.Count - 1) / num);
			num2 = mGridObject.cellHeight * (float)mRowSize;
		}
		num2 += mGridObject.EndPadding;
		mScrollView.OverrideBounds(width, num2);
	}

	public void RepopulateObjects()
	{
		for (int i = 0; i < mContents.GetLength(0); i++)
		{
			for (int j = 0; j < mContents.GetLength(1); j++)
			{
				int x;
				int y;
				if (mIsHorizontal)
				{
					x = i;
					y = mFirstRowIndex + PosMod(j - mFirstRowIndex, mNumRows);
				}
				else
				{
					x = mFirstRowIndex + PosMod(i - mFirstRowIndex, mNumRows);
					y = j;
				}
				T dataItem;
				if (DataAtGridPosition(x, y, out dataItem))
				{
					mContents[i, j].Populate(dataItem);
				}
			}
		}
	}

	private int PosMod(int x, int mod)
	{
		return (x % mod + mod) % mod;
	}

	private UIStreamingGridListItem PrefabAtListPosition(int listIndex)
	{
		int num = mContents.GetLength(0) * mContents.GetLength(1);
		if (mFirstRowIndex + 1 > listIndex || mFirstRowIndex + num - 2 <= listIndex)
		{
			int num2 = listIndex - mNumRows / 2;
			if (num2 < 0)
			{
				num2 = 0;
			}
			int num3 = mRowSize;
			if (mRowSize == 0)
			{
				num3 = 1;
			}
			if (mIsHorizontal)
			{
				SetScrollPos(new Vector2(0f, mCellSize * ((float)num2 - 0.5f) / (float)num3));
			}
			else
			{
				SetScrollPos(new Vector2((0f - mCellSize) * ((float)num2 - 0.5f) / (float)num3, 0f));
			}
		}
		int num4 = listIndex % num;
		if (mIsHorizontal)
		{
			int length = mContents.GetLength(0);
			int num5 = num4 % length;
			int num6 = num4 / length;
			return mContents[num5, num6];
		}
		int length2 = mContents.GetLength(1);
		int num7 = num4 / length2;
		int num8 = num4 % length2;
		return mContents[num7, num8];
	}

	private bool DataAtGridPosition(int x, int y, out T dataItem)
	{
		int num = ((!mIsHorizontal) ? (x * mContents.GetLength(1) + y) : (y * mContents.GetLength(0) + x));
		if (num < 0 || num >= mDataList.Count)
		{
			dataItem = default(T);
			return false;
		}
		dataItem = mDataList[num];
		return true;
	}

	private void SetPrefabPosition(Transform prefab, int x, int y)
	{
		prefab.localPosition = new Vector3((float)x * mGridObject.cellWidth, (float)(-y) * mGridObject.cellHeight, 0f);
	}

	public void Clear()
	{
		if (mGridObject != null)
		{
			for (int i = 0; i < mGridObject.transform.childCount; i++)
			{
				Transform child = mGridObject.transform.GetChild(i);
				UIStreamingGridListItem component = child.GetComponent<UIStreamingGridListItem>();
				if (component != null)
				{
					component.Unload();
				}
			}
			mGridObject.transform.DestroyAllChildren();
			mGridObject.SetFunctions(null, null);
		}
		mGridObject = null;
		mScrollView = null;
		mPanel = null;
		mDataList = null;
		mContents = null;
		mPrefab = null;
		mFirstRowIndex = 0;
		mInitialized = false;
	}

	public float CurrentScrollDistance()
	{
		if (mScrollView == null)
		{
			return 0f;
		}
		if (mIsHorizontal)
		{
			return 0f - mScrollView.transform.localPosition.y;
		}
		return mScrollView.transform.localPosition.x;
	}

	private void Update()
	{
		if (!mInitialized)
		{
			return;
		}
		float num = CurrentScrollDistance();
		while (true)
		{
			float num2 = (float)mFirstRowIndex * mCellSize + num;
			if (num2 < 0f - mCellSize)
			{
				int num3 = PosMod(mFirstRowIndex, mNumRows);
				for (int i = 0; i < mRowSize; i++)
				{
					UIStreamingGridListItem uIStreamingGridListItem = ((!mIsHorizontal) ? mContents[num3, i] : mContents[i, num3]);
					uIStreamingGridListItem.Unload();
					Vector3 localPosition = uIStreamingGridListItem.transform.localPosition;
					if (mIsHorizontal)
					{
						localPosition.y += mMoveDistance;
					}
					else
					{
						localPosition.x += mMoveDistance;
					}
					uIStreamingGridListItem.transform.localPosition = localPosition;
					int x;
					int y;
					if (mIsHorizontal)
					{
						x = i;
						y = mFirstRowIndex + mNumRows;
					}
					else
					{
						x = mFirstRowIndex + mNumRows;
						y = i;
					}
					T dataItem;
					if (DataAtGridPosition(x, y, out dataItem))
					{
						uIStreamingGridListItem.gameObject.SetActive(true);
						uIStreamingGridListItem.Populate(dataItem);
					}
					else
					{
						uIStreamingGridListItem.gameObject.SetActive(false);
					}
				}
				mFirstRowIndex++;
				continue;
			}
			if (!(num2 > 0f))
			{
				break;
			}
			int num4 = PosMod(mFirstRowIndex - 1, mNumRows);
			for (int j = 0; j < mRowSize; j++)
			{
				UIStreamingGridListItem uIStreamingGridListItem2 = ((!mIsHorizontal) ? mContents[num4, j] : mContents[j, num4]);
				uIStreamingGridListItem2.Unload();
				Vector3 localPosition2 = uIStreamingGridListItem2.transform.localPosition;
				if (mIsHorizontal)
				{
					localPosition2.y -= mMoveDistance;
				}
				else
				{
					localPosition2.x -= mMoveDistance;
				}
				uIStreamingGridListItem2.transform.localPosition = localPosition2;
				int x2;
				int y2;
				if (mIsHorizontal)
				{
					x2 = j;
					y2 = mFirstRowIndex - 1;
				}
				else
				{
					x2 = mFirstRowIndex - 1;
					y2 = j;
				}
				T dataItem2;
				if (DataAtGridPosition(x2, y2, out dataItem2))
				{
					uIStreamingGridListItem2.gameObject.SetActive(true);
					uIStreamingGridListItem2.Populate(dataItem2);
				}
				else
				{
					uIStreamingGridListItem2.gameObject.SetActive(false);
				}
			}
			mFirstRowIndex--;
		}
	}

	private void SetScrollPos(int rowIndex)
	{
		Vector3 localPosition = mScrollView.transform.localPosition;
		if (mIsHorizontal)
		{
			localPosition.y = (float)rowIndex * mCellSize + mPanelSize.y / 2f - mCellSize / 2f;
			if (localPosition.y < 0f)
			{
				localPosition.x = 0f;
			}
		}
		else
		{
			localPosition.x = (float)(-rowIndex) * mCellSize + mPanelSize.x / 2f - mCellSize / 2f;
			if (localPosition.x > 0f)
			{
				localPosition.x = 0f;
			}
		}
		mScrollView.transform.localPosition = localPosition;
	}

	public void SetScrollPos(Vector2 pos, UIStreamingGrid gridObject = null)
	{
		if (gridObject != null)
		{
			Transform parent = gridObject.transform.parent;
			mScrollView = NGUITools.FindInParents<UIScrollView>(parent);
			mPanel = NGUITools.FindInParents<UIPanel>(parent);
		}
		Vector2 zero = Vector2.zero;
		mPanel.clipOffset = -pos + zero;
		mScrollView.transform.localPosition = pos - zero;
		mScrollView.UpdateScrollbars();
	}

	public GameObject FindPrefab(T dataItem)
	{
		if (mDataList == null)
		{
			return null;
		}
		int num = mDataList.IndexOf(dataItem);
		if (num == -1)
		{
			return null;
		}
		UIStreamingGridListItem uIStreamingGridListItem = PrefabAtListPosition(num);
		return (!(uIStreamingGridListItem != null)) ? null : uIStreamingGridListItem.gameObject;
	}
}
