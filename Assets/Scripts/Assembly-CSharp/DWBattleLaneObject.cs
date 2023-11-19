using UnityEngine;

public class DWBattleLaneObject : MonoBehaviour
{
	public float LandscapeLiftLerpSpeed;
	public ShadowBlob ShadowBlob;
	public GameObject CreatureObject;
	public GameObject SwappedCreatureObject;
	public CreatureHPBar HealthBar;
	public BoxCollider LaneCollider;
	public bool IsFrozen;
	public bool IsBunny;
	public bool IgnoreBoardTint;
	public bool IsTransfmogrified;
}
