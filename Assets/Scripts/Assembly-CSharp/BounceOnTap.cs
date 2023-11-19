using System.Collections;
using UnityEngine;

public class BounceOnTap : MonoBehaviour
{
	private CreatureData mCreature;

	private Vector3 mBaseScale = Vector3.zero;

	private BattleCreatureAnimState mAnimState;

	public void Init(CreatureData creature)
	{
		mCreature = creature;
	}

	private void OnClick()
	{
		StartCoroutine(Bounce());
	}

	private IEnumerator Bounce()
	{
		BattleCreatureAnimState animState = GetComponentInChildren<BattleCreatureAnimState>();
		Transform scaleObj = ((!(animState != null)) ? base.transform : animState.transform);
		if (mBaseScale.x == 0f && mBaseScale.y == 0f && mBaseScale.z == 0f)
		{
			mBaseScale = scaleObj.localScale;
		}
		if (mCreature != null && mCreature.ZoomSound != null)
		{
			Singleton<SLOTAudioManager>.Instance.PlaySound(mCreature.ZoomSound);
		}
		Transform tweenObject = Singleton<PrefabReferences>.Instance.BounceTween.transform;
		bool done = false;
		Singleton<PrefabReferences>.Instance.BounceTween.PlayWithCallback(delegate
		{
			done = true;
		});
		while (!done)
		{
			scaleObj.localScale = new Vector3(mBaseScale.x * tweenObject.localScale.x, mBaseScale.y * tweenObject.localScale.y, mBaseScale.z * tweenObject.localScale.z);
			yield return null;
		}
		scaleObj.localScale = mBaseScale;
		if (animState != null)
		{
			animState.anim.SetTrigger("Fidget");
			animState.ResetFigdetTimer();
		}
	}
}
