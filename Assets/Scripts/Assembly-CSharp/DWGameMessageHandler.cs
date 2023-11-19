using UnityEngine;

public class DWGameMessageHandler : Singleton<DWGameMessageHandler>
{
	public Color StatusTextBuffColor;
	public Color StatusTextDebuffColor;
	public Color PassiveTextColor;
	public float HitFlashMatDuration;
	public float TimeBetweenTeamAttacks;
	public float RangedAttackFinishTime;
	public float RotateToAttackTime;
	public Material HitFlashMaterial;
	public Material FreezeDesatMaterial;
	public Material StealthMaterial;
	public GameObject TransmogrifyCreature;
	public float CritCamYOffset;
	public float CritCamZOffset;
	public float CritCamLookAtYOffset;
	public float NormalCameraShake;
	public float NormalCameraShakeTime;
	public float CritCameraShake;
	public float CritCameraShakeTime;
	public GameObject NonLaneActionStartFXPrefab;
	public GameObject EffectNameDisplayPrefab;
	public GameObject ShieldHitFX;
	public bool PrintDebugActionName;
	public Transform ProjectileFollower;
	public Transform BoardCenterPos;
	public Transform PlayerLaneCenterPos;
	public Transform OpponentLaneCenterPos;
}
