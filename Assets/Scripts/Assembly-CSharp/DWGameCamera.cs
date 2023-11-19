using System;
using System.Collections;
using System.Collections.Generic;
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

	public float DefaultDuration = 1f;

	private Transform mCurrentTransform;

	private Transform mCurrentLookAtTransform;

	public bool DebugCubeForCameraBound;

	public CameraDests CamDestinations;

	public CameraLookAtDests CamLookAtDestinations;

	public Vector3 AttackSequenceOffset;

	private bool mUseObjectLock;

	private Transform mLockObjectToCamera;

	private Vector3 mLockObjectPos;

	private Vector3 mLockTargetPos;

	private Vector3 mLockTargetLookAtPos;

	private Vector3 mCalculatedP1SetupPos;

	private Vector3 mCalculatedP1SetupLookAtPos;

	private Vector3 mCalculatedP1SetupVariantPos;

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

	private float mGizmoWidth;

	private float mGizmoHeight;

	private void Awake()
	{
		if (CameraConstraint)
		{
			CameraConstraint = false;
		}
	}

	public void MoveCameraToDeployCreature()
	{
		MainCamLookAt.SetFollowCam(true);
		MoveCameraTo(CamDestinations.DeployCam.transform, DefaultDuration);
		MoveCameraTargetTo(CamLookAtDestinations.DeployCam.transform, DefaultDuration);
	}

	public void MoveCameraToP1Setup(bool instant = false)
	{
		Singleton<BattleHudController>.Instance.EnemyThinkingObject.SetActive(false);
		if (instant)
		{
			MainCam.transform.position = mCalculatedP1SetupPos;
			MainCamLookAt.transform.position = mCalculatedP1SetupLookAtPos;
		}
		else
		{
			MoveCameraTo(mCalculatedP1SetupPos, 1f);
			MoveCameraTargetTo(mCalculatedP1SetupLookAtPos, 1f);
		}
	}

	public void MoveCameraToP1SetupVariant(bool instant = false)
	{
		Singleton<BattleHudController>.Instance.EnemyThinkingObject.SetActive(false);
		if (instant)
		{
			MainCam.transform.position = mCalculatedP1SetupVariantPos;
			MainCamLookAt.transform.position = CamLookAtDestinations.P1SetupVariant.transform.position;
		}
		else
		{
			MoveCameraTo(mCalculatedP1SetupVariantPos, 1f);
			MoveCameraTargetTo(CamLookAtDestinations.P1SetupVariant.transform, 1f);
		}
	}

	public void MoveCameraToP2Setup(bool instant = false)
	{
		LeaderData character = Singleton<DWGame>.Instance.GetCharacter(PlayerType.Opponent);
		Vector3 position = CamDestinations.P2Setup.transform.position;
		position.x -= character.HeightAdjust;
		if (instant)
		{
			MainCam.transform.position = position;
			MainCamLookAt.transform.position = CamLookAtDestinations.P2Setup.transform.position;
		}
		else
		{
			MoveCameraTo(position, 1f);
			MoveCameraTargetTo(CamLookAtDestinations.P2Setup.transform, 1f);
		}
	}

	public void MoveCameraToP1SelectLane()
	{
		MoveCameraTo(CamDestinations.SelectPlayerLanes.transform, 1f);
		MoveCameraTargetTo(CamLookAtDestinations.SelectPlayerLanes.transform, 1f);
	}

	public void MoveCameraToTapAttack()
	{
		MoveCameraTo(CamDestinations.TapAttack.transform, 0.5f);
		MoveCameraTargetTo(CamLookAtDestinations.TapAttack.transform, 0.5f);
	}

	public void MoveCameraToTapDefense()
	{
		MoveCameraTo(CamDestinations.TapDefense.transform, 0.5f);
		MoveCameraTargetTo(CamLookAtDestinations.TapDefense.transform, 0.5f);
	}

	public void MoveCameraToFixedAttackCam()
	{
		MoveCameraTo(CamDestinations.FixedAttackCam.transform, 0.5f);
		MoveCameraTargetTo(CamLookAtDestinations.FixedAttackCam.transform, 0.5f);
	}

	public void MoveCameraToAction(CreatureState creature)
	{
		Vector3 position = Singleton<DWBattleLane>.Instance.GetCreatureObject(creature).transform.position;
		Vector3 dest = position;
		dest.x -= 15f;
		dest.y += 15f;
		MoveCameraTo(dest, 0.2f);
		MoveCameraTargetTo(position, 0.2f);
	}

	public void MoveCameraToAction(List<GameMessage> UniqueList)
	{
		MainCamLookAt.SetFollowCam(true);
		List<Transform> list = new List<Transform>();
		foreach (GameMessage Unique in UniqueList)
		{
			Transform spawnPointFromMessage = GetSpawnPointFromMessage(Unique);
			if (spawnPointFromMessage != null && !list.Contains(spawnPointFromMessage))
			{
				list.Add(spawnPointFromMessage);
			}
		}
		if (list.Count != 0)
		{
			Bounds boundsRecursive = SLOTGame.GetBoundsRecursive(list);
			Vector3 center = boundsRecursive.center;
			if (DebugCubeForCameraBound)
			{
				GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
				gameObject.transform.localScale = boundsRecursive.size;
				gameObject.transform.position = center;
			}
			float num = boundsRecursive.size.z / 2f + 15f;
			Vector3 dest = new Vector3(center.x - num, center.y + num * 0.75f, 0f);
			MoveCameraTo(dest, 0.3f);
			MoveCameraTargetTo(center, 0.3f);
		}
	}

	private Transform GetSpawnPointFromMessage(GameMessage ms)
	{
		return Singleton<DWBattleLane>.Instance.GetCreatureObject(ms.Creature).transform;
	}

	private Vector3 FindCenterOfBounds(Bounds bounds)
	{
		Vector3 zero = Vector3.zero;
		zero.x = (bounds.min.x + bounds.max.x) / 2f;
		zero.y = (bounds.min.y + bounds.max.y) / 2f;
		zero.z = (bounds.min.z + bounds.max.z) / 2f;
		return zero;
	}

	public void MoveCameraToP2Defeated()
	{
		Singleton<BattleHudController>.Instance.EnemyThinkingObject.SetActive(false);
		MainCamLookAt.SetFollowCam(true);
		MoveCameraTo(CamDestinations.P2DefeatedCam.transform, 1f);
		MoveCameraTargetTo(CamLookAtDestinations.P2DefeatedCam.transform, 1f);
		StartCoroutine(SetGameStateWithDelay(GameState.PlayerVictory, 3f));
	}

	public void MoveCameraToP1Defeated()
	{
		Singleton<BattleHudController>.Instance.EnemyThinkingObject.SetActive(false);
		MainCamLookAt.SetFollowCam(true);
		MoveCameraTo(CamDestinations.P1DefeatedCam.transform, 1f);
		MoveCameraTargetTo(CamLookAtDestinations.P1DefeatedCam.transform, 1f);
		StartCoroutine(SetGameStateWithDelay(GameState.EnemyVictory, 3f));
	}

	public void MoveCameraToP1Winner()
	{
		Singleton<BattleHudController>.Instance.EnemyThinkingObject.SetActive(false);
		MainCamLookAt.SetFollowCam(true);
		MoveCameraTo(CamDestinations.VictoryCam.transform, 1f);
		MoveCameraTargetTo(CamLookAtDestinations.VictoryCam.transform, 1f);
	}

	public void MoveCameraToP2Winner()
	{
		Singleton<BattleHudController>.Instance.EnemyThinkingObject.SetActive(false);
		MainCamLookAt.SetFollowCam(true);
		MoveCameraTo(CamDestinations.LoserCam.transform, 1f);
		MoveCameraTargetTo(CamLookAtDestinations.LoserCam.transform, 1f);
	}

	public void SetCreatureDetailCam(bool enable, CreatureState creature = null)
	{
		CreatureDetailCam.enabled = enable;
		if (creature != null)
		{
			GameObject creatureObject = Singleton<DWBattleLane>.Instance.GetCreatureObject(creature);
			Bounds uIBoundsRecursive = SLOTGame.GetUIBoundsRecursive(creatureObject.transform);
			Vector3 position = creatureObject.transform.position;
			position.y = uIBoundsRecursive.size.y / 2f;
			CreatureDetailCamLookAt.transform.position = position;
			CreatureDetailCamOffsetPos.x = ((creature.Owner.Type != PlayerType.User) ? (0f - Mathf.Abs(CreatureDetailCamOffsetPos.x)) : Mathf.Abs(CreatureDetailCamOffsetPos.x));
			Vector3 position2 = position + CreatureDetailCamOffsetPos;
			CreatureDetailCam.transform.position = position2;
		}
	}

	public void MoveCameraToCreatureDetail(CreatureState creature, bool instant = false)
	{
		MainCamLookAt.SetFollowCam(true);
		PlayerType type = creature.Owner.Type;
		GameObject creatureObject = Singleton<DWBattleLane>.Instance.GetCreatureObject(creature);
		Bounds uIBoundsRecursive = SLOTGame.GetUIBoundsRecursive(creatureObject.transform);
		Vector3 position = creatureObject.transform.position;
		position.y += uIBoundsRecursive.size.y / 2f;
		position.z = ((type != PlayerType.User) ? (position.z - CeatureDetailCamLookAtOffsetZ) : (position.z + CeatureDetailCamLookAtOffsetZ));
		if (instant)
		{
			MainCamLookAt.transform.position = position;
		}
		else
		{
			MoveCameraTargetTo(position, CreatureDetailMoveCamDuration);
		}
		Vector3 position2 = creatureObject.transform.position;
		CreatureDetailCamOffsetPos.x = ((type != PlayerType.User) ? (0f - Mathf.Abs(CreatureDetailCamOffsetPos.x)) : Mathf.Abs(CreatureDetailCamOffsetPos.x));
		CreatureDetailCamOffsetPos.z = ((type != PlayerType.User) ? (0f - Mathf.Abs(CreatureDetailCamOffsetPos.z)) : Mathf.Abs(CreatureDetailCamOffsetPos.z));
		position2 += CreatureDetailCamOffsetPos;
		position2.y = position.y;
		float num = creature.Data.Form.Width * CreatureWidthZoomOutFactor;
		position2 += (position2 - creatureObject.transform.position) * num;
		if (instant)
		{
			MainCam.transform.position = position2;
		}
		else
		{
			MoveCameraTo(position2, CreatureDetailMoveCamDuration);
		}
	}

	public void MoveCameraToCreatureSummon(CreatureState creature, bool instant = false)
	{
		MainCamLookAt.SetFollowCam(true);
		PlayerType type = creature.Owner.Type;
		GameObject creatureObject = Singleton<DWBattleLane>.Instance.GetCreatureObject(creature);
		Bounds uIBoundsRecursive = SLOTGame.GetUIBoundsRecursive(creatureObject.transform);
		Vector3 position = creatureObject.transform.position;
		position.y += uIBoundsRecursive.size.y / 2f;
		position.z = ((type != PlayerType.User) ? (position.z + CeatureSummonCamLookAtOffsetZ) : (position.z + CeatureSummonCamLookAtOffsetZ));
		if (instant)
		{
			MainCamLookAt.transform.position = position;
		}
		else
		{
			MoveCameraTargetTo(position, CreatureSummonMoveCamDuration);
		}
		Vector3 position2 = creatureObject.transform.position;
		CreatureSummonCamOffsetPos.x = ((type != PlayerType.User) ? (0f - Mathf.Abs(CreatureSummonCamOffsetPos.x + 6f)) : Mathf.Abs(CreatureSummonCamOffsetPos.x));
		CreatureSummonCamOffsetPos.z = ((type != PlayerType.User) ? (0f - Mathf.Abs(CreatureSummonCamOffsetPos.z)) : (0f - Mathf.Abs(CreatureSummonCamOffsetPos.z)));
		position2 += CreatureSummonCamOffsetPos;
		position2.y = position.y + CreatureSummonCamOffsetPos.y;
		float num = creature.Data.Form.Width * CreatureSummonZoomInFactor;
		position2 += (position2 - creatureObject.transform.position) * num;
		if (instant)
		{
			MainCam.transform.position = position2;
		}
		else
		{
			MoveCameraTo(position2, CreatureSummonMoveCamDuration);
		}
	}

	public void MoveCameraToLootCollect(DWBattleLootObject lootObj)
	{
		Vector3 position = lootObj.transform.position;
		position.y += 2f;
		MoveCameraTargetTo(position, 0.7f);
		Vector3 dest = new Vector3(position.x - 10f, position.y + 7f, position.z);
		MoveCameraTo(dest, 0.7f);
	}

	public void MoveCameraTo(Transform dest, float time)
	{
		mCurrentTransform = dest;
		MoveTo(MainCam.gameObject, dest, time);
	}

	public void MoveCameraTo(Vector3 dest, float time)
	{
		MoveTo(MainCam.gameObject, dest, time);
	}

	public void MoveCameraTargetTo(Transform dest, float time)
	{
		mCurrentLookAtTransform = dest;
		MoveTo(MainCamLookAt.gameObject, dest, time);
	}

	public void MoveCameraTargetTo(Vector3 dest, float time)
	{
		MoveTo(MainCamLookAt.gameObject, dest, time);
	}

	public void MoveTo(GameObject obj, Transform dest, float time)
	{
		MoveTo(obj, dest.transform.position, time);
	}

	public void MoveTo(GameObject obj, Vector3 dest, float time)
	{
		if (time == 0f)
		{
			obj.transform.position = dest;
		}
		iTween.MoveTo(obj, iTween.Hash("position", dest, "time", time));
	}

	private IEnumerator SetGameStateWithDelay(GameState state, float delay)
	{
		yield return new WaitForSeconds(delay);
		Singleton<DWGame>.Instance.SetGameState(state);
	}

	public void SetObjectLock(Transform obj)
	{
		if (obj != null)
		{
			if (mLockObjectToCamera == null)
			{
				mLockObjectPos = obj.position;
				mLockTargetPos = MainCam.transform.position;
				mLockTargetLookAtPos = MainCamLookAt.transform.position;
			}
			mUseObjectLock = true;
		}
		else
		{
			mUseObjectLock = false;
		}
		mLockObjectToCamera = obj;
	}

	private void Update()
	{
		if (CameraConstraint)
		{
			MainCam.transform.position = mCurrentTransform.transform.position;
			MainCamLookAt.transform.position = mCurrentLookAtTransform.transform.position;
		}
		if (mUseObjectLock)
		{
			float t = 10f * Time.deltaTime;
			if (mLockObjectToCamera != null)
			{
				Vector3 position = mLockObjectToCamera.position;
				Vector3 vector = position - mLockObjectPos;
				mLockTargetPos += vector;
				mLockTargetLookAtPos += vector;
				mLockObjectPos = position;
			}
			MainCam.transform.position = Vector3.Lerp(MainCam.transform.position, mLockTargetPos, t);
			MainCamLookAt.transform.position = Vector3.Lerp(MainCamLookAt.transform.position, mLockTargetLookAtPos, t);
		}
	}

	public Transform GetPIPTarget(string str)
	{
		Transform transform = null;
		Transform[] componentsInChildren = PIPCameraTransformParent.GetComponentsInChildren<Transform>(true);
		Transform[] array = componentsInChildren;
		foreach (Transform transform2 in array)
		{
			if (transform2.name == str)
			{
				return transform2;
			}
		}
		if (componentsInChildren.Length > 0)
		{
			return componentsInChildren[0];
		}
		return null;
	}

	public void InitPIPCams()
	{
		Portrait_PIP[] pIPPortraits = PIPPortraits;
		foreach (Portrait_PIP portrait_PIP in pIPPortraits)
		{
			portrait_PIP.Init();
		}
	}

	public void UpdateSetupCamPositions(Vector3 tallestEnemyTop, float totalUserCreatureWidth, float totalOpponentCreatureWidth)
	{
		mGizmoHeight = tallestEnemyTop.y + DebugHeightAdd;
		tallestEnemyTop.y += HeightPadding + DebugHeightAdd;
		float num = BaseCameraAngle + tallestEnemyTop.y * AddCameraAnglePerHeight;
		float num2 = num + MainCam.fieldOfView / 2f;
		float num3 = num2 - FovAfterCards;
		float num4 = Mathf.Tan(num2 * ((float)Math.PI / 180f));
		float num5 = Mathf.Tan(num3 * ((float)Math.PI / 180f));
		mCalculatedP1SetupPos.x = (num4 * tallestEnemyTop.x - num5 * NearestCreaturePos - tallestEnemyTop.y) / (num4 - num5);
		mCalculatedP1SetupPos.y = num5 * (mCalculatedP1SetupPos.x - NearestCreaturePos);
		Ray ray = new Ray(mCalculatedP1SetupPos, new Vector3(Mathf.Cos(num * ((float)Math.PI / 180f)), Mathf.Sin(num * ((float)Math.PI / 180f))));
		float enter;
		if (new Plane(Vector3.up, Vector3.zero).Raycast(ray, out enter))
		{
			mCalculatedP1SetupLookAtPos = ray.GetPoint(enter);
			totalOpponentCreatureWidth *= OpponentWidthMultiplier;
			float num6 = Math.Max(totalUserCreatureWidth, totalOpponentCreatureWidth);
			num6 = (mGizmoWidth = num6 + DebugWidthAdd);
			float num7 = P1DistanceOffset + P1DistanceMultiplier * num6 / MainCam.aspect;
			if (num7 > enter)
			{
				float num8 = num7 - enter;
				mCalculatedP1SetupPos -= ray.direction.normalized * num8;
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (Application.isPlaying)
		{
			Gizmos.color = Color.yellow;
			float num = mGizmoWidth / 2f;
			Gizmos.DrawLine(new Vector3(100f, 0f, num), new Vector3(-100f, 0f, num));
			Gizmos.DrawLine(new Vector3(100f, 0f, 0f - num), new Vector3(-100f, 0f, 0f - num));
			Gizmos.DrawLine(new Vector3(7.5f, mGizmoHeight, -100f), new Vector3(7.5f, mGizmoHeight, 100f));
		}
	}

	public void RenderP1Character(bool render)
	{
		int num = 1 << LayerMask.NameToLayer("PIP");
		if (render)
		{
			MainCam.cullingMask |= num;
		}
		else
		{
			MainCam.cullingMask &= ~num;
		}
	}

	public void CameraShake(float amount, float time)
	{
		iTween.ShakePosition(MainCamParent, iTween.Hash("amount", new Vector3(amount, amount, amount), "time", time));
	}
}
