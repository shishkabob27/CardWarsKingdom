using UnityEngine;
using System.Collections.Generic;

public class TownController : Singleton<TownController>
{
	public float StartRotateFactor;
	public float StartHeightFactor;
	public float PanSensitivity;
	public float ZoomSensitivity;
	public float RotateSensitivity;
	public float MinHeight;
	public float MaxHeight;
	public float MinOffset;
	public float MaxOffset;
	public float GachaGateWaitTime;
	public float GachaChestSpacing;
	public float GachaChestFinishTime;
	public float GachaMultiChestFinishTime;
	public MeshCollider CameraMeshCollider;
	public bool UseUpdateCamera;
	public float LockedDesatAmount;
	public float LockedAddAmount;
	public Color LockedColor;
	public bool UseRealtimeLockColorUpdate;
	public GameObject BuildingUnlockEffect;
	public GameObject GachaBackground;
	public Animator GachaAnimator;
	public Transform GachaKeyParent;
	public Transform GachaChestPassByParent;
	public GameObject GachaKeyholeEffect;
	public GameObject GachaOpeningVFX;
	public List<Collider> TownColliders;
	public bool UnlockAtOnceByDebug;
}
