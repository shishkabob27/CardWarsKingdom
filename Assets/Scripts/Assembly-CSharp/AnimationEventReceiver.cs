using UnityEngine;

public class AnimationEventReceiver : MonoBehaviour
{
	public string Name;

	public GameObject Effect;

	public GameObject SpawnOnObject;

	public Vector3 SpawnOffset;

	public bool ParentToTarget;

	private void NewEvent(string param)
	{
		if (!(param != Name))
		{
			Transform parent = ((!(SpawnOnObject != null)) ? base.transform : SpawnOnObject.transform);
			if (ParentToTarget)
			{
				GameObject gameObject = parent.InstantiateAsChild(Effect);
				gameObject.ChangeLayerToParent();
				gameObject.transform.localPosition = SpawnOffset;
			}
			else
			{
				GameObject gameObject = SLOTGame.InstantiateFX(Effect, SpawnOnObject.transform.position + SpawnOffset, SpawnOnObject.transform.rotation) as GameObject;
			}
		}
	}
}
