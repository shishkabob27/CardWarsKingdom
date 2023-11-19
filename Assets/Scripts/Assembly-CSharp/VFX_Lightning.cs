using UnityEngine;

public class VFX_Lightning : MonoBehaviour
{
	public GameObject targetObject;

	private float magnitudeX1;

	public float magnitudeX = 0.5f;

	private float magnitudeY1;

	public float magnitudeY = 0.5f;

	private LineRenderer lineRenderer;

	private void Start()
	{
		magnitudeX1 = magnitudeX * -1f;
		magnitudeY1 = magnitudeY * -1f;
		lineRenderer = GetComponent<LineRenderer>();
	}

	private void Update()
	{
		lineRenderer.SetPosition(0, base.transform.localPosition);
		for (int i = 1; i < 3; i++)
		{
			Vector3 position = Vector3.Lerp(base.transform.localPosition, targetObject.transform.localPosition, (float)i / 3f);
			position.x += Random.Range(magnitudeX1, magnitudeX);
			position.y += Random.Range(magnitudeY1, magnitudeY);
			lineRenderer.SetPosition(i, position);
		}
		lineRenderer.SetPosition(3, targetObject.transform.localPosition);
	}
}
