using UnityEngine;

public class ComScoreController : MonoBehaviour
{
	private const string TURNER_C2 = "6035750";

	private const string TURNER_PUBLISH_SECRET = "6bba25a9ff38cd173c1c93842c768e28";

	public string appName;

	private static AndroidJavaClass comScore;

	private static AndroidJavaObject unityActivity;

	private bool init;
}
