using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DarkTonic.MasterAudio;
using JsonFx.Json;
using UnityEngine;

public class BattleCharacterAnimState : MonoBehaviour
{
	public Animator anim;

	public int player;

	private LeaderData mThisCharacter;

	private string mCharacterFaceAnimPrefx = string.Empty;

	public float HasCard;

	private float mPrevHasCard;

	private int p1IntroIdx = Animator.StringToHash("Base Layer.P1Intro");

	private int p2IntroIdx = Animator.StringToHash("Base Layer.P2Intro");

	private int winIdx = Animator.StringToHash("Base Layer.Win");

	private int loseIdx = Animator.StringToHash("Base Layer.Lose");

	private int idleIdx = Animator.StringToHash("Base Layer.Idle");

	private int fThinkIdx = Animator.StringToHash("Base Layer.Fidget_Think");

	private int fThink2Idx = Animator.StringToHash("Base Layer.Fidget_Think2");

	private int fPropIdx = Animator.StringToHash("Base Layer.Fidget_Prop");

	private int fBoredIdx = Animator.StringToHash("Base Layer.Fidget_Bored");

	private int fTauntIdx = Animator.StringToHash("Base Layer.Fidget_Taunt");

	private int bigActionIdx = Animator.StringToHash("Base Layer.BigActionReact");

	private int hr1Idx = Animator.StringToHash("Base Layer.HR1");

	private int hr2Idx = Animator.StringToHash("Base Layer.HR2");

	private int playCardIdx = Animator.StringToHash("Base Layer.PlayCard");

	private int playBigCardIdx = Animator.StringToHash("Base Layer.PlayBigCard");

	private int menuAdvanceIdx = Animator.StringToHash("Base Layer.NoCards_MenuAdvance");

	private int heroCard0Idx = Animator.StringToHash("Base Layer.HeroCard_00");

	private int heroCard1Idx = Animator.StringToHash("Base Layer.HeroCard_01");

	private int reviveIdx = Animator.StringToHash("Base Layer.Revive");

	private Dictionary<CharAnimType, int> mAnimHashDic = new Dictionary<CharAnimType, int>();

	private float fidgetTimer;

	private float rnd = -1f;

	private bool mStopFidget;

	private bool mResetTimer;

	private bool mThinkingAI;

	private bool mThinkingAIPrevState;

	private float mBlendSpeed = 0.3f;

	private float mBlendTimer;

	private CharAnimType mCurrentAnimType;

	private int mCurrentIdx;

	private int mPrevIdx;

	private int mPlayedVOIdx;

	private AnimatorStateInfo mCurrentClipState;

	public string mCurrentStateName;

	public string mCurrentFacialName;

	public string mCurrentFacialFrame;

	public void Init(LeaderData leaderData)
	{
		anim = GetComponent<Animator>();
		mThisCharacter = leaderData;
		PreloadVOForCharacter();
		PlayPersistentLeaderVFX();
		mCharacterFaceAnimPrefx = anim.runtimeAnimatorController.name;
		SQUtils.ReadJSONData(mCharacterFaceAnimPrefx + "_HR1.json", "CharacterFaceAnim");
		SQUtils.ReadJSONData(mCharacterFaceAnimPrefx + "_HR2.json", "CharacterFaceAnim");
		SQUtils.ReadJSONData(mCharacterFaceAnimPrefx + "_BigActionReact.json", "CharacterFaceAnim");
		SQUtils.ReadJSONData(mCharacterFaceAnimPrefx + "_PlayCard.json", "CharacterFaceAnim");
		SQUtils.ReadJSONData(mCharacterFaceAnimPrefx + "_PlayBigCard.json", "CharacterFaceAnim");
		SQUtils.ReadJSONData(mCharacterFaceAnimPrefx + "_HeroCard_00.json", "CharacterFaceAnim");
		SQUtils.ReadJSONData(mCharacterFaceAnimPrefx + "_HeroCard_01.json", "CharacterFaceAnim");
		SQUtils.ReadJSONData(mCharacterFaceAnimPrefx + "_NoCards_MenuAdvance.json", "CharacterFaceAnim");
		SQUtils.ReadJSONData(mCharacterFaceAnimPrefx + "_P1Intro.json", "CharacterFaceAnim");
		SQUtils.ReadJSONData(mCharacterFaceAnimPrefx + "_P2Intro.json", "CharacterFaceAnim");
		SQUtils.ReadJSONData(mCharacterFaceAnimPrefx + "_Idle.json", "CharacterFaceAnim");
		SQUtils.ReadJSONData(mCharacterFaceAnimPrefx + "_Fidget_Think.json", "CharacterFaceAnim");
		SQUtils.ReadJSONData(mCharacterFaceAnimPrefx + "_Fidget_Think2.json", "CharacterFaceAnim");
		SQUtils.ReadJSONData(mCharacterFaceAnimPrefx + "_Fidget_Prop.json", "CharacterFaceAnim");
		SQUtils.ReadJSONData(mCharacterFaceAnimPrefx + "_Fidget_Bored.json", "CharacterFaceAnim");
		SQUtils.ReadJSONData(mCharacterFaceAnimPrefx + "_Fidget_Taunt.json", "CharacterFaceAnim");
	}

	private void PreloadVOForCharacter()
	{
		string text = anim.runtimeAnimatorController.name;
		if (text.Contains("Gen"))
		{
			text = text.Replace("Gen", string.Empty);
		}
		CharAnimType[] array = Enum.GetValues(typeof(CharAnimType)) as CharAnimType[];
		CharAnimType[] array2 = array;
		foreach (CharAnimType charAnimType in array2)
		{
			if (charAnimType == CharAnimType.P1Intro || charAnimType == CharAnimType.P2Intro || charAnimType == CharAnimType.Lose || charAnimType == CharAnimType.Win)
			{
				string text2 = "VO_" + text + "_" + charAnimType;
				string path = Path.Combine(Application.dataPath, "Resources/VO/" + text + "/" + text2 + ".wav");
				if (File.Exists(path))
				{
					string clipName = "VO/" + text + "/" + text2;
					AudioResourceOptimizer.PopulateSourcesWithResourceClip(clipName, null);
				}
			}
		}
	}

	public void StopFidget(bool enable)
	{
		mStopFidget = enable;
	}

	public void Reset()
	{
		mStopFidget = false;
		mResetTimer = true;
	}

	private void Update()
	{
		if (rnd == -1f)
		{
			rnd = UnityEngine.Random.Range(5f, 10f);
		}
		if (mResetTimer)
		{
			fidgetTimer = 0f;
			mResetTimer = false;
		}
		if (!mStopFidget)
		{
			fidgetTimer += Time.deltaTime;
		}
		else
		{
			fidgetTimer = 0f;
		}
		if (anim == null || anim.runtimeAnimatorController == null)
		{
			return;
		}
		mCurrentClipState = anim.GetCurrentAnimatorStateInfo(0);
		mCurrentIdx = mCurrentClipState.nameHash;
		if (mCurrentIdx == p1IntroIdx)
		{
			mCurrentAnimType = CharAnimType.P1Intro;
		}
		if (mCurrentIdx == p2IntroIdx)
		{
			mCurrentAnimType = CharAnimType.P2Intro;
		}
		if (mCurrentIdx == winIdx)
		{
			mCurrentAnimType = CharAnimType.Win;
		}
		if (mCurrentIdx == loseIdx)
		{
			mCurrentAnimType = CharAnimType.Lose;
		}
		if (mCurrentIdx == idleIdx)
		{
			mCurrentAnimType = CharAnimType.Idle;
			mResetTimer = false;
		}
		if (mCurrentIdx == menuAdvanceIdx)
		{
			mCurrentAnimType = CharAnimType.NoCards_MenuAdvance;
		}
		if (mCurrentIdx == fThinkIdx)
		{
			mCurrentAnimType = CharAnimType.Fidget_Think;
			mResetTimer = true;
		}
		if (mCurrentIdx == fThink2Idx)
		{
			mCurrentAnimType = CharAnimType.Fidget_Think2;
			mResetTimer = true;
		}
		if (mCurrentIdx == fPropIdx)
		{
			mCurrentAnimType = CharAnimType.Fidget_Prop;
		}
		if (mCurrentIdx == fBoredIdx)
		{
			mCurrentAnimType = CharAnimType.Fidget_Bored;
		}
		if (mCurrentIdx == fBoredIdx)
		{
			mCurrentAnimType = CharAnimType.Fidget_Bored;
		}
		if (mCurrentIdx == fTauntIdx)
		{
			mCurrentAnimType = CharAnimType.Fidget_Taunt;
		}
		if (mCurrentIdx == bigActionIdx)
		{
			mCurrentAnimType = CharAnimType.BigActionReact;
			mResetTimer = true;
		}
		if (mCurrentIdx == hr1Idx)
		{
			mCurrentAnimType = CharAnimType.HR1;
			mResetTimer = true;
		}
		if (mCurrentIdx == hr2Idx)
		{
			mCurrentAnimType = CharAnimType.HR2;
			mResetTimer = true;
		}
		if (mCurrentIdx == playCardIdx)
		{
			mCurrentAnimType = CharAnimType.PlayCard;
			mResetTimer = true;
		}
		if (mCurrentIdx == playBigCardIdx)
		{
			mCurrentAnimType = CharAnimType.PlayBigCard;
			mResetTimer = true;
		}
		if (mCurrentIdx == heroCard0Idx)
		{
			mCurrentAnimType = CharAnimType.HeroCard_00;
			mResetTimer = true;
		}
		if (mCurrentIdx == heroCard1Idx)
		{
			mCurrentAnimType = CharAnimType.HeroCard_01;
			mResetTimer = true;
		}
		if (mCurrentIdx == reviveIdx)
		{
			mCurrentAnimType = CharAnimType.Revive;
			mResetTimer = true;
		}
		ResetAnimTrigger(mCurrentAnimType);
		if (DetachedSingleton<SceneFlowManager>.Instance.GetCurrentScene() != SceneFlowManager.Scene.Battle)
		{
			return;
		}
		HasCard = ((Singleton<CharacterAnimController>.Instance.GetNumberOfHands(player) > 0) ? 1f : 0f);
		if (HasCard != mPrevHasCard)
		{
			try
			{
				anim.logWarnings = false;
				mBlendTimer += Time.deltaTime;
				float num = mBlendTimer / mBlendSpeed;
				if (num <= 1f)
				{
					float value = Mathf.Lerp(mPrevHasCard, HasCard, num);
					anim.SetFloat("HasCard", value);
				}
				else
				{
					anim.SetFloat("HasCard", HasCard);
					mBlendTimer = 0f;
					mPrevHasCard = HasCard;
				}
			}
			catch
			{
			}
		}
		GameState currentGameState = Singleton<DWGame>.Instance.GetCurrentGameState();
		if (mStopFidget)
		{
			return;
		}
		List<CharAnimType> list = new List<CharAnimType>();
		if (fidgetTimer >= rnd)
		{
			bool flag = true;
			list.Add(CharAnimType.Fidget_Think);
			list.Add(CharAnimType.Fidget_Think2);
			list.Add(CharAnimType.Fidget_Prop);
			if ((flag && player == (int)PlayerType.User) || (!flag && player == (int)PlayerType.Opponent))
			{
				list.Add(CharAnimType.Fidget_Taunt);
			}
			if ((currentGameState.IsP2Turn() && player == (int)PlayerType.User) || (currentGameState.IsP1Turn() && player == (int)PlayerType.Opponent))
			{
				list.Add(CharAnimType.Fidget_Bored);
			}
			int index = UnityEngine.Random.Range(0, list.Count);
			Singleton<CharacterAnimController>.Instance.PlayHeroAnim(player, list[index]);
			fidgetTimer = 0f;
			rnd = UnityEngine.Random.Range(5f, 10f);
		}
	}

	public void SetCurrentStateName()
	{
	}

	public string GetCurrentStateName()
	{
		mCurrentStateName = mCurrentAnimType.ToString();
		return mCurrentStateName;
	}

	public CharAnimType GetCurrentAnimType()
	{
		return mCurrentAnimType;
	}

	public string GetCurrentAnimTime()
	{
		float num = mCurrentClipState.normalizedTime % 1f;
		float num2 = mCurrentClipState.length * num;
		return string.Format("{0:F2}", num2);
	}

	public bool IsCurrentAnimDone()
	{
		return mCurrentClipState.normalizedTime >= 0.97f;
	}

	public string GetCurrentAnimFrame()
	{
		float num = mCurrentClipState.normalizedTime % 1f;
		float num2 = mCurrentClipState.length * num;
		float num3 = num2 * 30f;
		return string.Format("{0:F2}", num3);
	}

	public string GetCurrentFaceAnimName()
	{
		return mCurrentFacialName;
	}

	public string GetCurrentFaceFrame()
	{
		return mCurrentFacialFrame;
	}

	public void PlayFaceAnimation(string animName)
	{
		StopCoroutine("IPlayFaceAnimation");
		StartCoroutine(IPlayFaceAnimation(animName, "CNT_MOUTH"));
		StartCoroutine(IPlayFaceAnimation(animName, "CNT_EYES"));
	}

	private IEnumerator IPlayFaceAnimation(string animName, string facialPart)
	{
		float totalTime = 0f;
		mCharacterFaceAnimPrefx = anim.runtimeAnimatorController.name;
		CharAnimType startAnimType = CharAnimType.Idle;
		mCurrentFacialFrame = "0.0";
		string filePath = "CharacterFaceAnim/" + mCharacterFaceAnimPrefx + "_" + animName + ".json";
		string appliedFilePath = SessionManager.Instance.GetStreamingAssetsPath(filePath);
		WWW www = new WWW(appliedFilePath);
		while (!www.isDone)
		{
			yield return null;
		}
		string json = www.text;
		if (!string.IsNullOrEmpty(json))
		{
			Dictionary<string, object>[] data = JsonReader.Deserialize<Dictionary<string, object>[]>(json);
			if (data != null || data.Count() > 0)
			{
				yield return null;
			}
			float startTime = float.Parse((string)data[0]["time"]);
			float prevTime = startTime;
			mCurrentFacialName = mCharacterFaceAnimPrefx + "_" + animName;
			Dictionary<string, object>[] array = data;
			foreach (Dictionary<string, object> dt in array)
			{
				string controller = (string)dt["CNT"];
				if (controller != facialPart)
				{
					continue;
				}
				GameObject targetMesh = FindInChildren(childName: controller.Replace("CNT_", "M_"), obj: base.gameObject);
				if (targetMesh != null)
				{
					float time = float.Parse((string)dt["time"]) * 2f - prevTime;
					yield return new WaitForSeconds(time);
					float num;
					totalTime = (num = totalTime + time);
					prevTime = num;
					if (startAnimType == CharAnimType.Idle)
					{
						startAnimType = mCurrentAnimType;
					}
					mCurrentFacialFrame = prevTime.ToString();
					float x = float.Parse((string)dt["x"]);
					float y = float.Parse((string)dt["y"]);
					Renderer targetRenderer = targetMesh.GetComponent<Renderer>();
					targetRenderer.material.SetTextureOffset("_MainTex", new Vector2(x, y));
				}
			}
		}
		if (startAnimType == mCurrentAnimType)
		{
			float stopDelay = mCurrentClipState.length - totalTime;
			yield return new WaitForSeconds(stopDelay);
		}
		string faceAnimName = mCharacterFaceAnimPrefx + "_Idle";
		if (animName == "Win")
		{
			faceAnimName = mCharacterFaceAnimPrefx + "_WinIdle";
		}
		else if (animName == "Lose")
		{
			faceAnimName = mCharacterFaceAnimPrefx + "_LoseIdle";
		}
		mCurrentFacialFrame = "-";
		if (animName != "NoCards_MenuAdvance")
		{
			SetFaceIdle(faceAnimName);
		}
	}

	private void SetFaceIdle(string faceAnimName)
	{
		mCurrentFacialName = faceAnimName;
		string path = Path.Combine(Application.streamingAssetsPath, "CharacterFaceAnim/" + faceAnimName + ".json");
		if (!File.Exists(path))
		{
			return;
		}
		Dictionary<string, object>[] array = SQUtils.ReadJSONData(faceAnimName + ".json", "CharacterFaceAnim");
		bool flag = false;
		bool flag2 = false;
		Dictionary<string, object>[] array2 = array;
		foreach (Dictionary<string, object> dictionary in array2)
		{
			string text = (string)dictionary["CNT"];
			if (text == "CNT_EYES" && !flag)
			{
				GameObject gameObject = FindInChildren(base.gameObject, "M_EYES");
				float x = float.Parse((string)dictionary["x"]);
				float y = float.Parse((string)dictionary["y"]);
				Renderer component = gameObject.GetComponent<Renderer>();
				component.material.SetTextureOffset("_MainTex", new Vector2(x, y));
				flag = true;
			}
			else if (text == "CNT_MOUTH" && !flag2)
			{
				GameObject gameObject2 = FindInChildren(base.gameObject, "M_MOUTH");
				float x2 = float.Parse((string)dictionary["x"]);
				float y2 = float.Parse((string)dictionary["y"]);
				Renderer component2 = gameObject2.GetComponent<Renderer>();
				component2.material.SetTextureOffset("_MainTex", new Vector2(x2, y2));
				flag2 = true;
			}
		}
	}

	private GameObject FindInChildren(GameObject obj, string childName)
	{
		Transform[] componentsInChildren = obj.GetComponentsInChildren<Transform>();
		Transform[] array = componentsInChildren;
		foreach (Transform transform in array)
		{
			if (transform.name.ToLower() == childName.ToLower())
			{
				return transform.gameObject;
			}
		}
		return null;
	}

	public bool PlayingBigCard()
	{
		return mCurrentIdx == playBigCardIdx;
	}

	public void ResetAnimTrigger(CharAnimType animType)
	{
		anim.SetBool(animType.ToString(), false);
	}

	private void PlayPersistentLeaderVFX()
	{
		if (mThisCharacter.LeaderVFX != null)
		{
			List<LeaderVFXEntry> list = mThisCharacter.LeaderVFX.Entries.ToList();
			LeaderVFXEntry leaderVFXEntry = list.Find((LeaderVFXEntry m) => m.AnimStateName == "Persistent");
			if (leaderVFXEntry != null)
			{
				AttachLeaderVFX(leaderVFXEntry);
			}
		}
	}

	public void PlayHeroVFX(CharAnimType animType)
	{
		if (mThisCharacter.LeaderVFX == null)
		{
			return;
		}
		List<LeaderVFXEntry> list = mThisCharacter.LeaderVFX.Entries.ToList();
		List<LeaderVFXEntry> list2 = list.FindAll((LeaderVFXEntry m) => m.AnimStateName == animType.ToString());
		if (list2 == null)
		{
			return;
		}
		foreach (LeaderVFXEntry item in list2)
		{
			AttachLeaderVFX(item);
		}
	}

	private void AttachLeaderVFX(LeaderVFXEntry vfx)
	{
		GameObject gameObject = FindInChildren(base.gameObject, vfx.AttachNode);
		if (gameObject != null)
		{
			GameObject gameObject2 = Singleton<SLOTResourceManager>.Instance.LoadResource("VFX/Leader/" + vfx.VFXName) as GameObject;
			if (gameObject2 != null)
			{
				StartCoroutine(PlayLeaderVFX(gameObject2, gameObject, vfx.StartOffsetTime));
			}
		}
	}

	private IEnumerator PlayLeaderVFX(GameObject prefab, GameObject attachNode, float delay)
	{
		float currentTime = 0f;
		while (currentTime < delay)
		{
			currentTime += Time.deltaTime;
			yield return null;
		}
		GameObject fxObj = attachNode.InstantiateAsChild(prefab);
		fxObj.ChangeLayer(attachNode.layer);
	}

	private void EmitParticle(GameObject fxObj, bool emit)
	{
		ParticleSystem[] componentsInChildren = fxObj.GetComponentsInChildren<ParticleSystem>(true);
		ParticleSystem[] array = componentsInChildren;
		foreach (ParticleSystem particleSystem in array)
		{
			particleSystem.enableEmission = emit;
			if (emit)
			{
				particleSystem.Play();
			}
			else
			{
				particleSystem.Stop();
			}
		}
	}
}
