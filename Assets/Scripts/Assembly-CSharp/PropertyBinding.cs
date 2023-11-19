using UnityEngine;

public class PropertyBinding : MonoBehaviour
{
	public enum Direction
	{
		SourceUpdatesTarget = 0,
		TargetUpdatesSource = 1,
		BiDirectional = 2,
	}

	public enum UpdateCondition
	{
		OnStart = 0,
		OnUpdate = 1,
		OnLateUpdate = 2,
		OnFixedUpdate = 3,
	}

	public PropertyReference source;
	public PropertyReference target;
	public Direction direction;
	public UpdateCondition update;
	public bool editMode;
}
