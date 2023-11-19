using UnityEngine;

public class ParticleAttractor : MonoBehaviour
{
	public float Speed = 1f;

	public float InherentVelocityReduction = 1f;

	public Vector3 TargetPositionOffset = Vector3.zero;

	private Transform mTarget;

	private Vector3 mTargetPos;

	private ParticleSystem mPS;

	private ParticleSystem.Particle[] mParticles;

	private void Awake()
	{
		mPS = GetComponent<ParticleSystem>();
		mParticles = new ParticleSystem.Particle[mPS.maxParticles];
		mTarget = new GameObject().transform;
		mTarget.name = "ParticleAttactor Target";
		mTarget.SetParent(base.transform);
		mTarget.localPosition = Vector3.zero;
		mTargetPos = mTarget.position + TargetPositionOffset;
	}

	public void SetTarget(Transform target)
	{
		mTarget.SetParent(target);
		mTarget.transform.localPosition = Vector3.zero;
	}

	private void Update()
	{
		if (mTarget != null)
		{
			mTargetPos = mTarget.position + TargetPositionOffset;
		}
		int particles = mPS.GetParticles(mParticles);
		for (int i = 0; i < particles; i++)
		{
			Vector3 vector = mTargetPos - mParticles[i].position;
			Vector3 vector2 = vector.normalized * Speed * Time.deltaTime;
			if (vector2.sqrMagnitude >= vector.sqrMagnitude)
			{
				mParticles[i].remainingLifetime = 0f;
			}
			else
			{
				mParticles[i].position += vector2;
			}
			mParticles[i].velocity = Vector3.Lerp(mParticles[i].velocity, Vector3.zero, InherentVelocityReduction * Time.deltaTime);
		}
		mPS.SetParticles(mParticles, particles);
	}

	private void OnDestroy()
	{
		if (mTarget != null)
		{
			Object.Destroy(mTarget.gameObject);
		}
	}
}
