using UnityEngine;

public class FX_AlphaPulsing : MonoBehaviour
{
	public float blinkingSpeed = 2f;

	private void Awake()
	{
	}

	private void Start()
	{
	}

	private void Update()
	{
		AlphaChangingSkinRender();
	}

	private void AlphaChangingSkinRender()
	{
		Color color = GetComponent<SkinnedMeshRenderer>().material.GetColor("_TintColor");
		float a = color.a;
		a = (color.a = Mathf.PingPong(Time.time * blinkingSpeed, 0.5f));
		GetComponent<SkinnedMeshRenderer>().material.SetColor("_TintColor", color);
	}
}
