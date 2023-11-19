using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugLogController : MonoBehaviour
{
	private const float DisplayTime = 10f;

	public bool mOnScreenLogEnabled = true;

	public Transform LogItemPrefab;

	public Transform LogParent;

	private static List<Transform> mLogEntries = new List<Transform>();

	private static float mTotalHeight = 0f;

	private void Awake()
	{
		mOnScreenLogEnabled = false;
	}

	private void Start()
	{
		SLOTGame.logCallbackEvent += OnLog;
	}

	public void OnLog(string logString, string stackTrace, LogType type)
	{
		if (Application.isPlaying && mOnScreenLogEnabled && !MiscParams.ForceDisableDebug && type != LogType.Log)
		{
		}
	}

	private IEnumerator DestroyAfterDelay(Transform trans)
	{
		yield return new WaitForSeconds(10f);
		if (trans != null)
		{
			UILabel itemLabel = trans.GetComponentInChildren<UILabel>();
			mTotalHeight -= itemLabel.height;
			mLogEntries.RemoveAt(0);
			Object.Destroy(trans.gameObject);
		}
	}
}
