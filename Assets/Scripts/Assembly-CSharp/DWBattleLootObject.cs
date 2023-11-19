using System.Collections;
using UnityEngine;

public class DWBattleLootObject : MonoBehaviour
{
	public PlayerType player;

	public int lane;

	public GameObject LootFlyFXPrefab;

	public GameObject LootAddFXPrefab;

	public ParticleSystem MainParticle;

	public UITexture UITex;

	public Transform UITexParent;

	public Texture2D EvoTexture;

	public GameObject AdditionalSpawnVFXPrefab;

	public Transform AdditionalSpawnVFXParent;

	private GameObject mAdditionalVFXObj;

	public bool SpawnAdditionalVFX = true;

	public GameObject ExtraSparkleFX;

	public bool PickUp;

	public float PickedUpScale;

	private void Start()
	{
		if (AdditionalSpawnVFXPrefab != null && SpawnAdditionalVFX)
		{
			if (AdditionalSpawnVFXParent != null)
			{
				mAdditionalVFXObj = AdditionalSpawnVFXParent.InstantiateAsChild(AdditionalSpawnVFXPrefab);
			}
			else
			{
				mAdditionalVFXObj = base.transform.InstantiateAsChild(AdditionalSpawnVFXPrefab);
			}
		}
	}

	private void OnClick()
	{
		GetComponent<Collider>().enabled = false;
		TriggerLootFly();
		RemoveAdditionalVFX();
	}

	public void RemoveAdditionalVFX()
	{
		if (mAdditionalVFXObj != null)
		{
			Object.Destroy(mAdditionalVFXObj);
		}
	}

	public void TriggerLootFly()
	{
		StartCoroutine(LootFlySequence());
	}

	private IEnumerator LootFlySequence()
	{
		Singleton<TutorialController>.Instance.AdvanceIfOnState("Q1_Drop2");
		Vector2 screenPos = Singleton<DWGameCamera>.Instance.MainCam.WorldToScreenPoint(base.transform.position);
		Vector3 startPos2 = Singleton<DWGameCamera>.Instance.BattleUICam.ScreenToWorldPoint(screenPos);
		startPos2.z = 0f;
		base.transform.position = startPos2;
		Transform destination = Singleton<BattleHudController>.Instance.GetNextLootContainer();
		ParticleSystem[] pSystems = GetComponentsInChildren<ParticleSystem>(true);
		if (pSystems.Length != 0)
		{
			ParticleSystem[] array = pSystems;
			foreach (ParticleSystem ps in array)
			{
				ps.gameObject.SetActive(false);
			}
		}
		UITweener tw = GetComponentInChildren<UITweener>();
		if (tw != null)
		{
			tw.ResetToZero();
			Object.Destroy(tw);
		}
		base.transform.localScale = Vector3.zero;
		base.transform.parent = destination;
		base.transform.localEulerAngles = Vector3.zero;
		startPos2 = base.transform.localPosition;
		base.gameObject.ChangeLayer(Singleton<BattleHudController>.Instance.gameObject.layer);
		GameObject trailFx = null;
		if (LootFlyFXPrefab != null)
		{
			trailFx = base.transform.InstantiateAsChild(LootFlyFXPrefab);
		}
		float travelTime = Singleton<BattleHudController>.Instance.LootFlyTime;
		int i = 4;
		float xAmp = Random.Range(Singleton<BattleHudController>.Instance.LootFlyAmplitudeX, 0f - Singleton<BattleHudController>.Instance.LootFlyAmplitudeX);
		Vector3 rot = new Vector3(0f, 0f, 90f);
		Animator anim = GetComponentInChildren<Animator>();
		if (anim != null)
		{
			anim.transform.Rotate(rot);
		}
		iTween.MoveTo(base.gameObject, iTween.Hash("position", Vector3.zero, "islocal", true, "time", travelTime, "lookahead", 0.5f, "easetype", iTween.EaseType.linear));
		iTween.ScaleTo(base.gameObject, new Vector3(PickedUpScale, PickedUpScale, PickedUpScale), travelTime);
		yield return new WaitForSeconds(travelTime);
		Object.Destroy(trailFx);
		if (LootAddFXPrefab != null)
		{
			base.transform.InstantiateAsChild(LootAddFXPrefab);
		}
		RewardManager.CreatureDropsPickedUp++;
		Singleton<BattleHudController>.Instance.UpdateLootCount();
		if (!PickUp)
		{
			yield break;
		}
		Singleton<BattleHudController>.Instance.LootCreatureTween.Play();
		yield return new WaitForSeconds(0.5f);
		Singleton<DWBattleLane>.Instance.CurrentBattleLootInventoryObjects.RemoveAt(0);
		if (Singleton<DWBattleLane>.Instance.CurrentBattleLootInventoryObjects.Count == 0)
		{
			Singleton<BattleHudController>.Instance.HideTapToCollectBanner();
			if (!Singleton<DWGame>.Instance.IsGameOver())
			{
				Singleton<DWBattleLane>.Instance.ResetLaneColliders(true);
			}
		}
		else
		{
			Singleton<DWBattleLane>.Instance.TriggerBattleLootCollect();
		}
	}

	private GameObject GetDebugSphere(Vector3 pos)
	{
		GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		gameObject.transform.position = pos;
		gameObject.transform.localScale = Vector3.one * 0.1f;
		SLOTGame.SetLayerRecursive(gameObject, LayerMask.NameToLayer("GUI"));
		return gameObject;
	}

	private void Update()
	{
	}

	public void AlignTexToCamera(Camera cam)
	{
		if (UITexParent != null)
		{
			UITexParent.transform.LookAt(cam.transform);
		}
	}

	public void DropEvoMaterialForPostMatchIcon()
	{
		UITweener componentInChildren = GetComponentInChildren<UITweener>();
		if (componentInChildren != null)
		{
			Object.Destroy(componentInChildren);
		}
		base.transform.AddLocalPositionY(40f);
		SpawnAdditionalVFX = false;
		AlignTexToCamera(Singleton<DWGameCamera>.Instance.BattleUICam);
	}
}
