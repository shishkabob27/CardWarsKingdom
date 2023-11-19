using System.Collections.Generic;
using UnityEngine;

public class BattleCreatureAnimState : MonoBehaviour
{
	public Animator anim;

	public Renderer secondMaterial;

	public List<Material> originalMats = new List<Material>();

	public List<SkinnedMeshRenderer> orignalMeshes = new List<SkinnedMeshRenderer>();

	public Transform AttachBoneBlindEffect;

	public Vector3 BlindEffectLocalOffset = new Vector3(-1f, 1f, 0f);

	public MeshRenderer PropsRenderer;

	public Material PropsBlackoutMaterial;

	private int idleIdx = Animator.StringToHash("Base Layer.Idle");

	private int fidgetIdx = Animator.StringToHash("Base Layer.Fidget");

	private int summonIdx = Animator.StringToHash("Base Layer.Summon");

	private int attackIdx = Animator.StringToHash("Base Layer.Attack");

	private int lungeIdx = Animator.StringToHash("Base Layer.Lunge");

	private int chargeIdx = Animator.StringToHash("Base Layer.Charge");

	private int shootIdx = Animator.StringToHash("Base Layer.Shoot");

	private int hitReactionIdx = Animator.StringToHash("Base Layer.HitReaction");

	private int defeatedIdx = Animator.StringToHash("Base Layer.Defeated");

	private bool mInitDone;

	private float fidgetTimer;

	private float rnd = -1f;

	private bool mStopFidget;

	private bool mStopFidgetInBattle;

	public float FidgetTimerMin = 5f;

	public float FidgetTimerMax = 10f;

	private AnimatorStateInfo mCurrentAnimState;

	public bool PrintAnimState;

	private string mCurrentStateName;

	private int mCurrentIdx;

	private void Start()
	{
		Init();
	}

	public void Init()
	{
		if (!mInitDone)
		{
			anim = GetComponent<Animator>();
			SkinnedMeshRenderer[] componentsInChildren = GetComponentsInChildren<SkinnedMeshRenderer>(true);
			SkinnedMeshRenderer[] array = componentsInChildren;
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in array)
			{
				orignalMeshes.Add(skinnedMeshRenderer);
				originalMats.Add(skinnedMeshRenderer.material);
			}
			mInitDone = true;
		}
	}

	private void Update()
	{
		if (rnd == -1f)
		{
			rnd = Random.Range(FidgetTimerMin, FidgetTimerMax);
		}
		fidgetTimer += Time.deltaTime;
		if (anim == null || anim.runtimeAnimatorController == null)
		{
			return;
		}
		mStopFidget = !DetachedSingleton<SceneFlowManager>.Instance.InFrontEnd();
		if (!mStopFidget)
		{
			if (fidgetTimer >= rnd)
			{
				anim.SetTrigger("Summon");
				fidgetTimer = 0f;
				rnd = Random.Range(5f, 10f);
			}
		}
		else if (!mStopFidgetInBattle && fidgetTimer >= rnd)
		{
			anim.SetTrigger("Fidget");
			fidgetTimer = 0f;
			rnd = Random.Range(5f, 10f);
		}
		mCurrentAnimState = anim.GetCurrentAnimatorStateInfo(0);
		mCurrentIdx = mCurrentAnimState.nameHash;
		if (mCurrentIdx == summonIdx)
		{
			anim.SetBool("Summon", false);
		}
		if (mCurrentIdx == attackIdx)
		{
			anim.SetBool("Attack", false);
		}
		if (mCurrentIdx == defeatedIdx)
		{
			anim.SetBool("Defeated", false);
		}
		if (mCurrentIdx == hitReactionIdx)
		{
			anim.SetBool("HitReaction", false);
		}
		if (mCurrentIdx == lungeIdx)
		{
			anim.SetBool("Lunge", false);
		}
		if (mCurrentIdx == chargeIdx)
		{
			anim.SetBool("Charge", false);
		}
		if (mCurrentIdx == shootIdx)
		{
			anim.SetBool("Shoot", false);
		}
		if (mCurrentIdx == idleIdx)
		{
			anim.SetBool("ToIdle", false);
		}
		if (mCurrentIdx == fidgetIdx)
		{
			anim.SetBool("Fidget", false);
		}
		if (PrintAnimState)
		{
			SetCurrentStateName();
		}
	}

	public string GetCurrentStateName()
	{
		return mCurrentStateName;
	}

	public string GetCurrentAnimFrame()
	{
		float num = mCurrentAnimState.normalizedTime % 1f;
		float num2 = mCurrentAnimState.length * num;
		return string.Format("{0:F2}", num2);
	}

	public void SetCurrentStateName()
	{
	}

	public void ResetFigdetTimer()
	{
		fidgetTimer = 0f;
	}
}
