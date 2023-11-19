using UnityEngine;

public class AuthDialogController : MonoBehaviour
{
	private bool mKeepShowingDataPrompt;

	private void Awake()
	{
		Object.DontDestroyOnLoad(base.transform.gameObject);
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (SessionManager.Instance.theSession != null && SessionManager.Instance.theSession.TheGame != null)
		{
			if (SessionManager.Instance.theSession.TheGame.needsReloadErrorDialog)
			{
				SessionManager.Instance.theSession.TheGame.needsReloadErrorDialog = false;
				mKeepShowingDataPrompt = true;
			}
			if (mKeepShowingDataPrompt && !LoadingScreenController.ShowingLoadingScreen())
			{
				Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!RELOAD_DATA_PROMPT"), ReloadFromServer, SimplePopupController.PopupPriority.ServerError);
			}
		}
	}

	private void ShowDataPrompt()
	{
	}

	public void PauseGame()
	{
	}

	private void ReloadFromServer()
	{
		mKeepShowingDataPrompt = false;
		SessionManager.Instance.theSession.WebFileServer.DeleteETagFile();
		SessionManager.Instance.theSession.ReloadGame();
	}
}
