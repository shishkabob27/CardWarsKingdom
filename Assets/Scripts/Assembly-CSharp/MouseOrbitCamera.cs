using UnityEngine;

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
	public GameObject CameraTransform;
	public bool UseOrbitCam;
	public bool FollowCamTarget;
	public float ZoomSpeed;
	public float ZoomSpeedWhenUnlock;
	public Vector3 ZoomCamOffset;
	public Vector3 ZoomCamRot;
	public TownBuildingScript CurrentBuilding;
}
