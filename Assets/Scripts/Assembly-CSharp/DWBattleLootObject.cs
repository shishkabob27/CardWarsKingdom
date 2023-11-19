using UnityEngine;

public class DWBattleLootObject : MonoBehaviour
{
	public int lane;
	public GameObject LootFlyFXPrefab;
	public GameObject LootAddFXPrefab;
	public ParticleSystem MainParticle;
	public UITexture UITex;
	public Transform UITexParent;
	public Texture2D EvoTexture;
	public GameObject AdditionalSpawnVFXPrefab;
	public Transform AdditionalSpawnVFXParent;
	public bool SpawnAdditionalVFX;
	public GameObject ExtraSparkleFX;
	public bool PickUp;
	public float PickedUpScale;
}
