using UnityEngine;
using System.Collections.Generic;

public class BattleCreatureAnimState : MonoBehaviour
{
	public Animator anim;
	public Renderer secondMaterial;
	public List<Material> originalMats;
	public List<SkinnedMeshRenderer> orignalMeshes;
	public Transform AttachBoneBlindEffect;
	public Vector3 BlindEffectLocalOffset;
	public MeshRenderer PropsRenderer;
	public Material PropsBlackoutMaterial;
	public float FidgetTimerMin;
	public float FidgetTimerMax;
	public bool PrintAnimState;
}
