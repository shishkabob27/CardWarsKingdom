using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using MiniJSON;
using UnityEngine;

public class VersionChecker : Singleton<VersionChecker>
{
	public float TimeBetweenChecks;

	private bool mChecking;

	private float mCheckTimer = -1f;

	private void Update()
	{
		if (!Singleton<TownEnvironmentHolder>.Instance.Loaded || !Singleton<TownController>.Instance.IsIntroDone() || mChecking)
		{
			return;
		}
		if (mCheckTimer <= 0f)
		{
			if (!Singleton<MouseOrbitCamera>.Instance.IsZoomedInToBuilding())
			{
				CheckVersion();
			}
		}
		else
		{
			mCheckTimer -= Time.deltaTime;
		}
	}

	public void CheckVersion(Action callbackWhenUpToDate = null)
	{
		StartCoroutine(CheckVersionCo(callbackWhenUpToDate));
	}

	private IEnumerator CheckVersionCo(Action callbackWhenUpToDate)
	{
        callbackWhenUpToDate();
        mChecking = true;
		Singleton<BusyIconPanelController>.Instance.Show();
		int result = -1;
		Dictionary<string, object> data = null;

		using (TFWebClient client = new TFWebClient(null))
		{
			client.DownloadDataCompleted += delegate(object sender, DownloadDataCompletedEventArgs e)
			{
				string json = SQContentPatcher.decodeZippedData(e.Result);
				data = (Dictionary<string, object>)Json.Deserialize(json);
				result = 1;
			};
			client.NetworkError += delegate
			{
				result = 0;
			};
			client.DownloadDataAsync(new Uri(SQSettings.MANIFEST_URL), SQSettings.MANIFEST_URL);
		}
		while (result == -1)
		{
			yield return null;
		}
		Singleton<BusyIconPanelController>.Instance.Hide();
		if (result == 0 || data == null || !data.ContainsKey("version"))
		{
			OnError(callbackWhenUpToDate);
			yield break;
		}
		long serverVersion = (long)data["version"];
		if (0 == 0 && serverVersion > SQContentPatcher.CurrentManifestVersion)
		{
			Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!NEW_GAME_DATA"), RestartGame);
			yield break;
		}
		mCheckTimer = TimeBetweenChecks;
		mChecking = false;
		if (callbackWhenUpToDate != null)
		{
			callbackWhenUpToDate();
		}
	}

	private void OnError(Action callbackWhenUpToDate)
	{
		Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!SERVER_ERROR_MESSAGE"), delegate
		{
			CheckVersion(callbackWhenUpToDate);
		});
	}

	private void RestartGame()
	{
		Singleton<ChatManager>.Instance.EndChat();
		SessionManager.Instance.theSession.ReloadGame();
	}
}
