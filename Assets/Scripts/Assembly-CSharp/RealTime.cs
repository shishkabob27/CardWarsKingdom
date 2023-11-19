using UnityEngine;

public class RealTime : MonoBehaviour
{
	public static float time
	{
		get
		{
			return Time.unscaledTime;
		}
	}

	public static float deltaTime
	{
		get
		{
			return Time.unscaledDeltaTime;
		}
	}
}
