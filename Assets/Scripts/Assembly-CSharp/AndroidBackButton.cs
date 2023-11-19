using UnityEngine;

public class AndroidBackButton : MonoBehaviour
{
	private void Update()
	{
		if (!Input.GetKeyDown(KeyCode.Escape))
		{
			return;
		}
		bool flag = UICamera.IsInputLocked();
		if (DetachedSingleton<SceneFlowManager>.Instance.InBattleScene() && Singleton<PauseController>.Instance.Paused)
		{
			flag = false;
		}
		if (flag)
		{
			return;
		}
		if (Singleton<AndroidQuitPopupController>.Instance.Showing)
		{
			Singleton<AndroidQuitPopupController>.Instance.OnClickNo();
		}
		else
		{
			if (MenuStackManager.PopMenuStack() || !MenuStackManager.OnRootMenuLevel())
			{
				return;
			}
			if (DetachedSingleton<SceneFlowManager>.Instance.InBattleScene())
			{
				if (Singleton<PvpBattleResultsController>.Instance.Showing)
				{
					Singleton<PvpBattleResultsController>.Instance.AndroidBackButtonPressed();
				}
				else if (Singleton<BattleResultsController>.Instance.Showing)
				{
					Singleton<BattleResultsController>.Instance.AndroidBackButtonPressed();
				}
				else if (Singleton<PauseController>.Instance.ButtonShowing)
				{
					Singleton<PauseController>.Instance.OnClickPause();
				}
			}
			else
			{
				Singleton<AndroidQuitPopupController>.Instance.Show();
			}
		}
	}
}
