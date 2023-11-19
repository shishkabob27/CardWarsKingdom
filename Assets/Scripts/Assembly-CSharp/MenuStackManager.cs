using System.Collections.Generic;
using UnityEngine;

public class MenuStackManager
{
	private class StackedMenu
	{
		public bool Popup;

		public BackButtonTarget CloseButton;

		public bool Close()
		{
			if (Popup)
			{
				if (Singleton<SimplePopupController>.Instance.CanCloseFromBackButton())
				{
					RemoveTopItemFromStack();
					Singleton<SimplePopupController>.Instance.CloseFromBackButton();
					return true;
				}
				return false;
			}
			if (!CloseButton.gameObject.activeInHierarchy)
			{
				return false;
			}
			RemoveTopItemFromStack();
			mSendingButtonClick = true;
			CloseButton.SendMessage("OnClick");
			mSendingButtonClick = false;
			return true;
		}
	}

	private static List<StackedMenu> mStackedMenus = new List<StackedMenu>();

	private static bool mSendingButtonClick = false;

	public static void RegisterUIPanel(Collider closeButton)
	{
		if (!(closeButton.GetComponent<BackButtonTarget>() != null))
		{
			StackedMenu stackedMenu = new StackedMenu();
			stackedMenu.CloseButton = closeButton.gameObject.AddComponent<BackButtonTarget>();
			mStackedMenus.Add(stackedMenu);
		}
	}

	public static void RegisterPopup()
	{
		StackedMenu stackedMenu = new StackedMenu();
		stackedMenu.Popup = true;
		mStackedMenus.Add(stackedMenu);
	}

	public static bool PopMenuStack()
	{
		if (Singleton<TutorialController>.Instance.IsAnyTutorialActive())
		{
			return false;
		}
		if (mStackedMenus.Count > 0)
		{
			return mStackedMenus[mStackedMenus.Count - 1].Close();
		}
		return false;
	}

	public static void RemoveTopItemFromStack(bool removeTargetComponent = false)
	{
		if (mStackedMenus.Count <= 0)
		{
			return;
		}
		if (removeTargetComponent)
		{
			BackButtonTarget closeButton = mStackedMenus[mStackedMenus.Count - 1].CloseButton;
			if (closeButton != null)
			{
				Object.Destroy(closeButton);
			}
		}
		mStackedMenus.RemoveAt(mStackedMenus.Count - 1);
	}

	public static void ClearStack()
	{
		mStackedMenus.Clear();
	}

	public static bool OnRootMenuLevel()
	{
		return mStackedMenus.Count == 0;
	}

	public static void OnBackButtonTargetClicked(BackButtonTarget target)
	{
		if (!mSendingButtonClick && mStackedMenus.Count != 0)
		{
			if (mStackedMenus[mStackedMenus.Count - 1].CloseButton == target)
			{
				mStackedMenus.RemoveAt(mStackedMenus.Count - 1);
			}
			else if (mStackedMenus.RemoveAll((StackedMenu m) => m.CloseButton == target) != 0)
			{
			}
		}
	}

	public static void RemoveAnyFromStack(UITweenController tween)
	{
		if (!(tween.MenuStackCloseButton == null))
		{
			BackButtonTarget target = tween.MenuStackCloseButton.GetComponent<BackButtonTarget>();
			mStackedMenus.RemoveAll((StackedMenu m) => m.CloseButton == target);
			Object.Destroy(target);
		}
	}
}
