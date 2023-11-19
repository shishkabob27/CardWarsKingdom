using System.Collections.Generic;

public class MenuStackController : Singleton<MenuStackController>
{
	private List<MenuStackItem> mStack = new List<MenuStackItem>();

	private UITweenController mWaitingForShowTween;

	private bool mEnabling;

	public bool ClearMenuStackInProgress { get; private set; }

	public void OnStackItemEnabled(MenuStackItem item)
	{
		if (!mEnabling)
		{
			if (!mStack.Contains(item))
			{
				mStack.Add(item);
			}
			mWaitingForShowTween = item.ShowTween;
		}
	}

	public void OnStackItemDisabled(MenuStackItem item)
	{
		if (!mEnabling)
		{
			if (!mStack.Remove(item))
			{
			}
			if (mStack.Count > 0)
			{
				mEnabling = true;
				mStack[mStack.Count - 1].gameObject.SetActive(true);
				mEnabling = false;
			}
		}
	}

	public void ClearMenuStack()
	{
		MenuStackItem menuStackItem = ((mStack.Count <= 0) ? null : mStack[mStack.Count - 1]);
		mEnabling = true;
		for (int i = 0; i < mStack.Count - 1; i++)
		{
			if (mStack[i].UnloadFunction != null)
			{
				mStack[i].UnloadFunction.Execute();
			}
			mStack[i].gameObject.SetActive(false);
		}
		mEnabling = false;
		mStack.Clear();
		if (menuStackItem != null)
		{
			mStack.Add(menuStackItem);
			ClearMenuStackInProgress = true;
			menuStackItem.HideTween.PlayWithCallback(ClearStackDone);
		}
	}

	private void ClearStackDone()
	{
		ClearMenuStackInProgress = false;
	}

	private void Update()
	{
		if (mWaitingForShowTween != null && !mWaitingForShowTween.AnyTweenPlaying())
		{
			mWaitingForShowTween = null;
			if (mStack.Count >= 2)
			{
				mEnabling = true;
				mStack[mStack.Count - 2].gameObject.SetActive(false);
				mEnabling = false;
			}
		}
		if (mStack.Count >= 2 && mStack[mStack.Count - 1].HideTween.AnyTweenPlaying())
		{
			mEnabling = true;
			mStack[mStack.Count - 2].gameObject.SetActive(true);
			mEnabling = false;
		}
	}
}
