using System.Collections;
using UnityEngine;

public class FrontEndPIPController : Singleton<FrontEndPIPController>
{
	public LeaderData[] Leaders = new LeaderData[2];

	public Transform[] CharacterSpawnPoints = new Transform[2];

	public FrontEndPIP[] PIPs = new FrontEndPIP[2];

	private GameObject[] mLeaderModelInstances = new GameObject[2];

	public void ShowModelPortraits(LeaderData[] leaders, UIWidget[] iconPortraits)
	{
		StartCoroutine(LoadModelsCo(leaders, iconPortraits));
	}

	public void HideModelPortrait()
	{
		FrontEndPIP[] pIPs = PIPs;
		foreach (FrontEndPIP frontEndPIP in pIPs)
		{
			frontEndPIP.ShowPortrait(false);
		}
		UnloadModels();
	}

	private IEnumerator LoadModelsCo(LeaderData[] leaders, UIWidget[] iconPortraits)
	{
		LeaderData[] leaders2 = default(LeaderData[]);
		UIWidget[] iconPortraits2 = default(UIWidget[]);
		for (int i = 0; i < leaders.Length; i++)
		{
			if (Leaders[i] == leaders[i])
			{
				continue;
			}
			UnloadModel(i);
			Resources.UnloadUnusedAssets();
			if (leaders[i] == null)
			{
				PIPs[i].ShowPortrait(false);
				continue;
			}
			yield return StartCoroutine(Singleton<SLOTResourceManager>.Instance.LoadLeaderResources(leaders[i], delegate(Object loadedObjData)
			{
				mLeaderModelInstances[i] = CharacterSpawnPoints[i].InstantiateAsChild(loadedObjData as GameObject);
				mLeaderModelInstances[i].ChangeLayer(CharacterSpawnPoints[i].gameObject.layer);
				BattleCharacterAnimState component = mLeaderModelInstances[i].GetComponent<BattleCharacterAnimState>();
				component.player = i;
				component.Init(leaders2[i]);
				PIPs[i].Init(i, leaders2[i], CharacterSpawnPoints[i].gameObject, iconPortraits2[i]);
				PIPs[i].ShowPortrait(true);
				Leaders[i] = leaders2[i];
			}));
		}
	}

	public void UnloadModels(bool clearResource = true)
	{
		for (int i = 0; i < mLeaderModelInstances.Length; i++)
		{
			UnloadModel(i);
		}
		if (clearResource)
		{
			Resources.UnloadUnusedAssets();
		}
	}

	private void UnloadModel(int n)
	{
		if (mLeaderModelInstances[n] != null)
		{
			Object.Destroy(mLeaderModelInstances[n]);
			mLeaderModelInstances[n] = null;
			Leaders[n] = null;
		}
	}

	public void PlayBattleStartAnim()
	{
		for (int i = 0; i < mLeaderModelInstances.Length; i++)
		{
			if (mLeaderModelInstances[i] != null)
			{
				Animator component = mLeaderModelInstances[i].GetComponent<Animator>();
				component.SetTrigger(CharAnimType.NoCards_MenuAdvance.ToString());
				PlayFaceAnimation(i, CharAnimType.NoCards_MenuAdvance.ToString());
			}
		}
	}

	private void PlayFaceAnimation(int player, string faceAnimBaseName)
	{
		BattleCharacterAnimState component = mLeaderModelInstances[player].GetComponent<BattleCharacterAnimState>();
		component.PlayFaceAnimation(faceAnimBaseName);
	}
}
