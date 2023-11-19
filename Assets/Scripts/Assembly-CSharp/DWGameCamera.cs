using System;
using UnityEngine;

public class DWGameCamera : Singleton<DWGameCamera>
{
	[Serializable]
	public class CameraDests
	{
		public Transform DeployCam;
		public Transform P1Setup;
		public Transform P2Setup;
		public Transform TapAttack;
		public Transform TapDefense;
		public Transform SelectPlayerLanes;
		public Transform SelectOpponentLanes;
		public Transform P1DefeatedCam;
		public Transform P2DefeatedCam;
		public Transform VictoryCam;
		public Transform LoserCam;
		public Transform FixedAttackCam;
		public Transform P1SetupVariant;
	}

	[Serializable]
	public class CameraLookAtDests
	{
		public Transform DeployCam;
		public Transform P1Setup;
		public Transform P2Setup;
		public Transform TapAttack;
		public Transform TapDefense;
		public Transform SelectPlayerLanes;
		public Transform SelectOpponentLanes;
		public Transform P1DefeatedCam;
		public Transform P2DefeatedCam;
		public Transform VictoryCam;
		public Transform LoserCam;
		public Transform FixedAttackCam;
		public Transform P1SetupVariant;
	}

	public Camera MainCam;
	public BattleCameraLookAt MainCamLookAt;
	public GameObject MainCamParent;
	public Camera BattleUICam;
	public Camera Battle3DUICam;
	public Camera CreatureDetailCam;
	public Portrait_PIP[] PIPPortraits;
	public Transform PIPCameraTransformParent;
	public BattleCameraLookAt CreatureDetailCamLookAt;
	public bool UseDetailCam;
	public Vector3 CreatureDetailCamOffsetPos;
	public Vector3 CreatureSummonCamOffsetPos;
	public float CeatureDetailCamLookAtOffsetZ;
	public float CeatureSummonCamLookAtOffsetZ;
	public float CreatureWidthZoomOutFactor;
	public float CreatureSummonZoomInFactor;
	public float CreatureDetailMoveCamDuration;
	public float CreatureSummonMoveCamDuration;
	public bool CameraConstraint;
	public float DefaultDuration;
	public bool DebugCubeForCameraBound;
	public CameraDests CamDestinations;
	public CameraLookAtDests CamLookAtDestinations;
	public Vector3 AttackSequenceOffset;
	public float OpponentWidthMultiplier;
	public float P1DistanceOffset;
	public float P1DistanceMultiplier;
	public float DebugHeightAdd;
	public float DebugWidthAdd;
	public float FovAfterCards;
	public float NearestCreaturePos;
	public float HeightPadding;
	public float BaseCameraAngle;
	public float AddCameraAnglePerHeight;
}
