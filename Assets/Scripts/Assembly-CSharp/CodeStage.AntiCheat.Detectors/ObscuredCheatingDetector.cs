using UnityEngine;
using UnityEngine.Events;

namespace CodeStage.AntiCheat.Detectors
{
	[AddComponentMenu("Code Stage/Anti-Cheat Toolkit/Obscured Cheating Detector")]
	public class ObscuredCheatingDetector : ActDetectorBase
	{
		internal const string COMPONENT_NAME = "Obscured Cheating Detector";

		internal const string FINAL_LOG_PREFIX = "[ACTk] Obscured Cheating Detector: ";

		private static int instancesInScene;

		[Tooltip("Max allowed difference between encrypted and fake values in ObscuredFloat. Increase in case of false positives.")]
		public float floatEpsilon = 0.0001f;

		[Tooltip("Max allowed difference between encrypted and fake values in ObscuredVector2. Increase in case of false positives.")]
		public float vector2Epsilon = 0.1f;

		[Tooltip("Max allowed difference between encrypted and fake values in ObscuredVector3. Increase in case of false positives.")]
		public float vector3Epsilon = 0.1f;

		[Tooltip("Max allowed difference between encrypted and fake values in ObscuredQuaternion. Increase in case of false positives.")]
		public float quaternionEpsilon = 0.1f;

		public static ObscuredCheatingDetector Instance { get; private set; }

		private static ObscuredCheatingDetector GetOrCreateInstance
		{
			get
			{
				if (Instance != null)
				{
					return Instance;
				}
				if (ActDetectorBase.detectorsContainer == null)
				{
					ActDetectorBase.detectorsContainer = new GameObject("Anti-Cheat Toolkit Detectors");
				}
				Instance = ActDetectorBase.detectorsContainer.AddComponent<ObscuredCheatingDetector>();
				return Instance;
			}
		}

		internal static bool IsRunning
		{
			get
			{
				return (object)Instance != null && Instance.isRunning;
			}
		}

		private ObscuredCheatingDetector()
		{
		}

		public static void StartDetection()
		{
			if (Instance != null)
			{
				Instance.StartDetectionInternal(null);
			}
		}

		public static void StartDetection(UnityAction callback)
		{
			GetOrCreateInstance.StartDetectionInternal(callback);
		}

		public static void StopDetection()
		{
			if (Instance != null)
			{
				Instance.StopDetectionInternal();
			}
		}

		public static void Dispose()
		{
			if (Instance != null)
			{
				Instance.DisposeInternal();
			}
		}

		private void Awake()
		{
			instancesInScene++;
			if (Init(Instance, "Obscured Cheating Detector"))
			{
				Instance = this;
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			instancesInScene--;
		}

		private void OnLevelWasLoaded(int index)
		{
			if (instancesInScene < 2)
			{
				if (!keepAlive)
				{
					DisposeInternal();
				}
			}
			else if (!keepAlive && Instance != this)
			{
				DisposeInternal();
			}
		}

		private void StartDetectionInternal(UnityAction callback)
		{
			if (!isRunning && base.enabled)
			{
				if (callback == null || detectionEventHasListener)
				{
				}
				if (callback == null && !detectionEventHasListener)
				{
					base.enabled = false;
					return;
				}
				detectionAction = callback;
				started = true;
				isRunning = true;
			}
		}

		protected override void StartDetectionAutomatically()
		{
			StartDetectionInternal(null);
		}

		protected override void PauseDetector()
		{
			isRunning = false;
		}

		protected override void ResumeDetector()
		{
			if (detectionAction != null || detectionEventHasListener)
			{
				isRunning = true;
			}
		}

		protected override void StopDetectionInternal()
		{
			if (started)
			{
				detectionAction = null;
				started = false;
				isRunning = false;
			}
		}

		protected override void DisposeInternal()
		{
			base.DisposeInternal();
			if (Instance == this)
			{
				Instance = null;
			}
		}
	}
}
