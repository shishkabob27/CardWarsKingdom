using System.Collections;
using UnityEngine;

public class PostMatchSkip : MonoBehaviour
{
	private void Start()
	{
	}

	private void OnClick()
	{
		Singleton<BattleResultsController>.Instance.SkipRewardStates();
		StartCoroutine(DelayInput());
	}

	private IEnumerator DelayInput()
	{
		UICamera.LockInput();
		yield return new WaitForSeconds(1f);
		UICamera.UnlockInput();
	}

	private void Update()
	{
	}
}
