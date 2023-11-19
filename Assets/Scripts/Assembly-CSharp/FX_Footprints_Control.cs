using UnityEngine;

public class FX_Footprints_Control : MonoBehaviour
{
	public Transform[] waypoint;

	private int currentPoint;

	public float rotationDamper = 6f;

	public float accelerateSpeed = 1.8f;

	public Color targetColor;

	private float currentTime;

	private void Start()
	{
	}

	private void Update()
	{
		if (currentPoint == waypoint.Length)
		{
			currentPoint = 0;
		}
		else
		{
			continueMoving();
		}
		if (!(Time.deltaTime - currentTime > 3f))
		{
		}
	}

	private void continueMoving()
	{
		Quaternion b = Quaternion.LookRotation(waypoint[currentPoint].position - base.transform.position);
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, b, Time.deltaTime * rotationDamper);
		float num = Vector3.Dot((waypoint[currentPoint].position - base.transform.position).normalized, base.transform.forward);
		float num2 = accelerateSpeed * num;
		base.transform.Translate(0f, 0f, Time.deltaTime * num2);
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (collider.tag == "waypoint")
		{
			currentPoint++;
		}
	}
}
