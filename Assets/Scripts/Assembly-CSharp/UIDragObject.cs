using UnityEngine;

public class UIDragObject : MonoBehaviour
{
	public enum DragEffect
	{
		None = 0,
		Momentum = 1,
		MomentumAndSpring = 2,
	}

	public Transform target;
	public Vector3 scrollMomentum;
	public bool restrictWithinPanel;
	public UIRect contentRect;
	public DragEffect dragEffect;
	public float momentumAmount;
	[SerializeField]
	protected Vector3 scale;
	[SerializeField]
	private float scrollWheelFactor;
}
