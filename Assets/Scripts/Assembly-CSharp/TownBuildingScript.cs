using UnityEngine;

public class TownBuildingScript : MonoBehaviour
{
	public string BuildingId;
	public MeshRenderer mesh;
	public Transform NameBarAttachPoint;
	public Transform BadgeAttachPoint;
	public Transform ZoomNode;
	public Transform ZoomNodeWhenUnlock;
	public Animation MainBuildingAnimation;
	public Animator MainBuildingAnimator;
	public Animation[] OtherAnimations;
}
