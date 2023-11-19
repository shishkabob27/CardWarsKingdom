using System.Collections;
using UnityEngine;

[AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
public class MouseOrbitCamera : Singleton<MouseOrbitCamera>
{
	public Transform target;

	public float HorizontalTilt;

	public float VerticalTilt;

	public float TiltRestoreSpeed;

	public float TiltSnapSpeed;

	public float LookAtDistance;

	public float MaxOffset;

	public Vector3 DebugRotate;

	private Vector3 mDefaultPos;

	private Vector3 mDefaultTargetPos;

	private Vector3 mDefaultRot;

	private Vector3 mCalculatedLookAt;

	public GameObject CameraTransform;

	private float mLastHorizontalTilt;

	private float mLastVerticalTilt;

	private Vector3 mTiltTarget = Vector3.zero;

	private bool mZoomingOut;

	private float mTiltDelayTime = -1f;

	public bool UseOrbitCam;

	public bool FollowCamTarget;

	public float ZoomSpeed = 1f;

	public float ZoomSpeedWhenUnlock = 1.5f;

	public Vector3 ZoomCamOffset = new Vector3(3f, 3f, 0f);

	public Vector3 ZoomCamRot = new Vector3(15f, -20f, 0f);

	public TownBuildingScript CurrentBuilding;

	private void Awake()
	{
		mDefaultPos = base.transform.position;
		mDefaultRot = CameraTransform.transform.rotation.eulerAngles;
		mDefaultTargetPos = target.position;
		mCalculatedLookAt = CameraTransform.transform.position + CameraTransform.transform.forward * LookAtDistance;
	}

	public void AdjustLookAtDistance(float amount)
	{
		CameraTransform.transform.localPosition = Vector3.zero;
		CameraTransform.transform.LookAt(mCalculatedLookAt);
		LookAtDistance += amount;
		mCalculatedLookAt = CameraTransform.transform.position + CameraTransform.transform.forward * LookAtDistance;
	}

	public void AdjustGyroRate(float amount)
	{
	}

	private void Start()
	{
		if ((bool)GetComponent<Rigidbody>())
		{
			GetComponent<Rigidbody>().freezeRotation = true;
		}
	}

	public void EnableTiltCam(bool enabled)
	{
		Singleton<PlayerInfoScript>.Instance.SaveData.TownTiltCam = enabled;
		Input.gyro.enabled = enabled;
		if (enabled)
		{
			mTiltDelayTime = 0.2f;
		}
	}

	public void CheckTiltCamSettingBeforeTutorial()
	{
		if (PlayerPrefs.HasKey("EnableTiltCam"))
		{
			Singleton<PlayerInfoScript>.Instance.SaveData.TownTiltCam = PlayerPrefs.GetInt("EnableTiltCam") == 1;
		}
		EnableTiltCam(Singleton<PlayerInfoScript>.Instance.SaveData.TownTiltCam);
	}

	private void Update()
	{
		float num = TiltRestoreSpeed;
		if (Singleton<PlayerInfoScript>.Instance.SaveData.TownTiltCam && CurrentBuilding == null && !mZoomingOut)
		{
			Vector3 rotationRateUnbiased = Input.gyro.rotationRateUnbiased;
			rotationRateUnbiased += DebugRotate;
			if (mTiltDelayTime == -1f)
			{
				mTiltTarget.x -= rotationRateUnbiased.y * HorizontalTilt;
				mTiltTarget.y += rotationRateUnbiased.x * VerticalTilt;
				float num2 = mTiltTarget.magnitude / MaxOffset;
				if (num2 > 1f)
				{
					mTiltTarget /= num2;
				}
			}
			else
			{
				mTiltDelayTime -= Time.deltaTime;
				if (mTiltDelayTime <= 0f)
				{
					mTiltDelayTime = -1f;
				}
			}
		}
		else
		{
			num *= 10f;
		}
		CameraTransform.transform.localPosition = Vector3.Lerp(CameraTransform.transform.localPosition, mTiltTarget, TiltSnapSpeed);
		mTiltTarget = Vector3.Lerp(mTiltTarget, Vector3.zero, num);
		if (CurrentBuilding == null && !mZoomingOut)
		{
			CameraTransform.transform.LookAt(mCalculatedLookAt);
		}
	}

	private void LateUpdate()
	{
		if (FollowCamTarget)
		{
			base.transform.LookAt(target);
		}
	}

	public void ZoomToButton(TownBuildingScript building, bool unlockBuilding = false)
	{
		UICamera.LockInput();
		CurrentBuilding = building;
		Input.gyro.enabled = false;
		Transform transform = ((!unlockBuilding) ? CurrentBuilding.ZoomNode : CurrentBuilding.ZoomNodeWhenUnlock);
		if (transform == null)
		{
			transform = CurrentBuilding.ZoomNode;
		}
		Vector3 position = transform.position;
		Vector3 eulerAngles = transform.rotation.eulerAngles;
		float num = ((!unlockBuilding) ? ZoomSpeed : ZoomSpeedWhenUnlock);
		iTween.MoveTo(base.gameObject, iTween.Hash("position", position, "time", num, "easetype", iTween.EaseType.easeOutQuad));
		if (!FollowCamTarget)
		{
			iTween.RotateTo(CameraTransform, iTween.Hash("rotation", eulerAngles, "time", num, "easetype", iTween.EaseType.easeOutQuad));
		}
		else
		{
			iTween.MoveTo(target.gameObject, iTween.Hash("position", CurrentBuilding.transform, "time", num, "easetype", iTween.EaseType.easeOutQuad));
		}
		StartCoroutine(DelayAfterZoom(unlockBuilding));
	}

	private IEnumerator DelayAfterZoom(bool unlockBuilding)
	{
		float zoomTime = ((!unlockBuilding) ? ZoomSpeed : ZoomSpeedWhenUnlock);
		yield return new WaitForSeconds(zoomTime);
		UICamera.UnlockInput();
	}

	public bool IsZoomedInToBuilding()
	{
		return CurrentBuilding != null;
	}

	public void UnZoom()
	{
		CurrentBuilding = null;
		mZoomingOut = true;
		UICamera.LockInput();
		iTween.MoveTo(base.gameObject, iTween.Hash("position", mDefaultPos, "time", ZoomSpeed, "easetype", iTween.EaseType.easeOutQuad));
		if (!FollowCamTarget)
		{
			iTween.RotateTo(CameraTransform, iTween.Hash("rotation", mDefaultRot, "time", ZoomSpeed, "easetype", iTween.EaseType.easeOutQuad));
		}
		else
		{
			iTween.MoveTo(target.gameObject, iTween.Hash("position", mDefaultTargetPos, "time", ZoomSpeed, "easetype", iTween.EaseType.easeOutQuad));
		}
		StartCoroutine(DelayAfterUnZoom());
		bool flag = Singleton<PlayerInfoScript>.Instance.IsFeatureUnlocked("TBuilding_Gacha");
	}

	private IEnumerator DelayAfterUnZoom()
	{
		yield return new WaitForSeconds(ZoomSpeed);
		UseOrbitCam = true;
		mZoomingOut = false;
		mTiltTarget = Vector3.zero;
		UICamera.UnlockInput();
		EnableTiltCam(Singleton<PlayerInfoScript>.Instance.SaveData.TownTiltCam);
	}

	public static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360f)
		{
			angle += 360f;
		}
		if (angle > 360f)
		{
			angle -= 360f;
		}
		return Mathf.Clamp(angle, min, max);
	}
}
