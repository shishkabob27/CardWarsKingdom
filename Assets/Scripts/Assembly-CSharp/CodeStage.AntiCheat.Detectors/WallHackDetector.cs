using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace CodeStage.AntiCheat.Detectors
{
	[AddComponentMenu("Code Stage/Anti-Cheat Toolkit/WallHack Detector")]
	public class WallHackDetector : ActDetectorBase
	{
		internal const string COMPONENT_NAME = "WallHack Detector";

		internal const string FINAL_LOG_PREFIX = "[ACTk] WallHack Detector: ";

		private const string SERVICE_CONTAINER_NAME = "[WH Detector Service]";

		private const string WIREFRAME_SHADER_NAME = "Hidden/ACTk/WallHackTexture";

		private const int SHADER_TEXTURE_SIZE = 4;

		private const int RENDER_TEXTURE_SIZE = 4;

		private readonly Vector3 rigidPlayerVelocity = new Vector3(0f, 0f, 1f);

		private static int instancesInScene;

		private WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

		[SerializeField]
		[Tooltip("Check for the \"walk through the walls\" kind of cheats made via Rigidbody hacks?")]
		private bool checkRigidbody = true;

		[SerializeField]
		[Tooltip("Check for the \"walk through the walls\" kind of cheats made via Character Controller hacks?")]
		private bool checkController = true;

		[SerializeField]
		[Tooltip("Check for the \"see through the walls\" kind of cheats made via shader or driver hacks (wireframe, color alpha, etc.)?")]
		private bool checkWireframe = true;

		[Tooltip("Check for the \"shoot through the walls\" kind of cheats made via Raycast hacks?")]
		[SerializeField]
		private bool checkRaycast = true;

		[Range(1f, 60f)]
		[Tooltip("Delay between Wireframe module checks, from 1 up to 60 secs.")]
		public int wireframeDelay = 10;

		[Tooltip("Delay between Raycast module checks, from 1 up to 60 secs.")]
		[Range(1f, 60f)]
		public int raycastDelay = 10;

		[Tooltip("World position of the container for service objects within 3x3x3 cube (drawn as red wire cube in scene).")]
		public Vector3 spawnPosition;

		[Tooltip("Maximum false positives in a row for each detection module before registering a wall hack.")]
		public byte maxFalsePositives = 3;

		private GameObject serviceContainer;

		private GameObject solidWall;

		private GameObject thinWall;

		private Camera wfCamera;

		private MeshRenderer foregroundRenderer;

		private MeshRenderer backgroundRenderer;

		private Color wfColor1 = Color.black;

		private Color wfColor2 = Color.black;

		private Shader wfShader;

		private Material wfMaterial;

		private Texture2D shaderTexture;

		private Texture2D targetTexture;

		private RenderTexture renderTexture;

		private int whLayer = -1;

		private int raycastMask = -1;

		private Rigidbody rigidPlayer;

		private CharacterController charControllerPlayer;

		private float charControllerVelocity;

		private byte rigidbodyDetections;

		private byte controllerDetections;

		private byte wireframeDetections;

		private byte raycastDetections;

		private bool wireframeDetected;

		public bool CheckRigidbody
		{
			get
			{
				return checkRigidbody;
			}
			set
			{
				if (checkRigidbody == value || !Application.isPlaying || !base.enabled || !base.gameObject.activeSelf)
				{
					return;
				}
				checkRigidbody = value;
				if (started)
				{
					UpdateServiceContainer();
					if (checkRigidbody)
					{
						StartRigidModule();
					}
					else
					{
						StopRigidModule();
					}
				}
			}
		}

		public bool CheckController
		{
			get
			{
				return checkController;
			}
			set
			{
				if (checkController == value || !Application.isPlaying || !base.enabled || !base.gameObject.activeSelf)
				{
					return;
				}
				checkController = value;
				if (started)
				{
					UpdateServiceContainer();
					if (checkController)
					{
						StartControllerModule();
					}
					else
					{
						StopControllerModule();
					}
				}
			}
		}

		public bool CheckWireframe
		{
			get
			{
				return checkWireframe;
			}
			set
			{
				if (checkWireframe == value || !Application.isPlaying || !base.enabled || !base.gameObject.activeSelf)
				{
					return;
				}
				checkWireframe = value;
				if (started)
				{
					UpdateServiceContainer();
					if (checkWireframe)
					{
						StartWireframeModule();
					}
					else
					{
						StopWireframeModule();
					}
				}
			}
		}

		public bool CheckRaycast
		{
			get
			{
				return checkRaycast;
			}
			set
			{
				if (checkRaycast == value || !Application.isPlaying || !base.enabled || !base.gameObject.activeSelf)
				{
					return;
				}
				checkRaycast = value;
				if (started)
				{
					UpdateServiceContainer();
					if (checkRaycast)
					{
						StartRaycastModule();
					}
					else
					{
						StopRaycastModule();
					}
				}
			}
		}

		public static WallHackDetector Instance { get; private set; }

		private static WallHackDetector GetOrCreateInstance
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
				Instance = ActDetectorBase.detectorsContainer.AddComponent<WallHackDetector>();
				return Instance;
			}
		}

		private WallHackDetector()
		{
		}

		public static void StartDetection()
		{
			if (Instance != null)
			{
				Instance.StartDetectionInternal(null, Instance.spawnPosition, Instance.maxFalsePositives);
			}
		}

		public static void StartDetection(UnityAction callback)
		{
			StartDetection(callback, GetOrCreateInstance.spawnPosition);
		}

		public static void StartDetection(UnityAction callback, Vector3 spawnPosition)
		{
			StartDetection(callback, spawnPosition, GetOrCreateInstance.maxFalsePositives);
		}

		public static void StartDetection(UnityAction callback, Vector3 spawnPosition, byte maxFalsePositives)
		{
			GetOrCreateInstance.StartDetectionInternal(callback, spawnPosition, maxFalsePositives);
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
			if (Init(Instance, "WallHack Detector"))
			{
				Instance = this;
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			StopAllCoroutines();
			if (serviceContainer != null)
			{
				UnityEngine.Object.Destroy(serviceContainer);
			}
			if (wfMaterial != null)
			{
				wfMaterial.mainTexture = null;
				wfMaterial.shader = null;
				wfMaterial = null;
				wfShader = null;
				shaderTexture = null;
				targetTexture = null;
				renderTexture.DiscardContents();
				renderTexture.Release();
				renderTexture = null;
			}
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

		private void FixedUpdate()
		{
			if (isRunning && checkRigidbody && !(rigidPlayer == null) && rigidPlayer.transform.localPosition.z > 1f)
			{
				rigidbodyDetections++;
				if (!Detect())
				{
					StopRigidModule();
					StartRigidModule();
				}
			}
		}

		private void Update()
		{
			if (!isRunning || !checkController || charControllerPlayer == null || !(charControllerVelocity > 0f))
			{
				return;
			}
			charControllerPlayer.Move(new Vector3(UnityEngine.Random.Range(-0.002f, 0.002f), 0f, charControllerVelocity));
			if (charControllerPlayer.transform.localPosition.z > 1f)
			{
				controllerDetections++;
				if (!Detect())
				{
					StopControllerModule();
					StartControllerModule();
				}
			}
		}

		private void StartDetectionInternal(UnityAction callback, Vector3 servicePosition, byte falsePositivesInRow)
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
				spawnPosition = servicePosition;
				maxFalsePositives = falsePositivesInRow;
				rigidbodyDetections = 0;
				controllerDetections = 0;
				wireframeDetections = 0;
				raycastDetections = 0;
				StartCoroutine(InitDetector());
				started = true;
				isRunning = true;
			}
		}

		protected override void StartDetectionAutomatically()
		{
			StartDetectionInternal(null, spawnPosition, maxFalsePositives);
		}

		protected override void PauseDetector()
		{
			if (isRunning)
			{
				isRunning = false;
				StopRigidModule();
				StopControllerModule();
				StopWireframeModule();
				StopRaycastModule();
			}
		}

		protected override void ResumeDetector()
		{
			if (detectionAction != null || detectionEventHasListener)
			{
				isRunning = true;
				if (checkRigidbody)
				{
					StartRigidModule();
				}
				if (checkController)
				{
					StartControllerModule();
				}
				if (checkWireframe)
				{
					StartWireframeModule();
				}
				if (checkRaycast)
				{
					StartRaycastModule();
				}
			}
		}

		protected override void StopDetectionInternal()
		{
			if (started)
			{
				PauseDetector();
				detectionAction = null;
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

		private void UpdateServiceContainer()
		{
			if (base.enabled && base.gameObject.activeSelf)
			{
				if (whLayer == -1)
				{
					whLayer = LayerMask.NameToLayer("Ignore Raycast");
				}
				if (raycastMask == -1)
				{
					raycastMask = LayerMask.GetMask("Ignore Raycast");
				}
				if (serviceContainer == null)
				{
					serviceContainer = new GameObject("[WH Detector Service]");
					serviceContainer.layer = whLayer;
					serviceContainer.transform.position = spawnPosition;
					UnityEngine.Object.DontDestroyOnLoad(serviceContainer);
				}
				if ((checkRigidbody || checkController) && solidWall == null)
				{
					solidWall = new GameObject("SolidWall");
					solidWall.AddComponent<BoxCollider>();
					solidWall.layer = whLayer;
					solidWall.transform.parent = serviceContainer.transform;
					solidWall.transform.localScale = new Vector3(3f, 3f, 0.5f);
					solidWall.transform.localPosition = Vector3.zero;
				}
				else if (!checkRigidbody && !checkController && solidWall != null)
				{
					UnityEngine.Object.Destroy(solidWall);
				}
				if (checkWireframe && wfCamera == null)
				{
					if (wfShader == null)
					{
						wfShader = Shader.Find("Hidden/ACTk/WallHackTexture");
					}
					if (wfShader == null)
					{
						checkWireframe = false;
					}
					else if (!wfShader.isSupported)
					{
						checkWireframe = false;
					}
					else
					{
						if (wfColor1 == Color.black)
						{
							wfColor1 = GenerateColor();
							do
							{
								wfColor2 = GenerateColor();
							}
							while (ColorsSimilar(wfColor1, wfColor2, 10));
						}
						if (shaderTexture == null)
						{
							shaderTexture = new Texture2D(4, 4, TextureFormat.RGB24, false);
							shaderTexture.filterMode = FilterMode.Point;
							Color[] array = new Color[16];
							for (int i = 0; i < 16; i++)
							{
								if (i < 8)
								{
									array[i] = wfColor1;
								}
								else
								{
									array[i] = wfColor2;
								}
							}
							shaderTexture.SetPixels(array, 0);
							shaderTexture.Apply();
						}
						if (renderTexture == null)
						{
							renderTexture = new RenderTexture(4, 4, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
							renderTexture.generateMips = false;
							renderTexture.filterMode = FilterMode.Point;
							renderTexture.Create();
						}
						if (targetTexture == null)
						{
							targetTexture = new Texture2D(4, 4, TextureFormat.RGB24, false);
							targetTexture.filterMode = FilterMode.Point;
						}
						if (wfMaterial == null)
						{
							wfMaterial = new Material(wfShader);
							wfMaterial.mainTexture = shaderTexture;
						}
						if (foregroundRenderer == null)
						{
							GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
							UnityEngine.Object.Destroy(gameObject.GetComponent<BoxCollider>());
							gameObject.name = "WireframeFore";
							gameObject.layer = whLayer;
							gameObject.transform.parent = serviceContainer.transform;
							gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
							foregroundRenderer = gameObject.GetComponent<MeshRenderer>();
							foregroundRenderer.sharedMaterial = wfMaterial;
							foregroundRenderer.shadowCastingMode = ShadowCastingMode.Off;
							foregroundRenderer.receiveShadows = false;
							foregroundRenderer.enabled = false;
						}
						if (backgroundRenderer == null)
						{
							GameObject gameObject2 = GameObject.CreatePrimitive(PrimitiveType.Quad);
							UnityEngine.Object.Destroy(gameObject2.GetComponent<MeshCollider>());
							gameObject2.name = "WireframeBack";
							gameObject2.layer = whLayer;
							gameObject2.transform.parent = serviceContainer.transform;
							gameObject2.transform.localPosition = new Vector3(0f, 0f, 1f);
							gameObject2.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
							backgroundRenderer = gameObject2.GetComponent<MeshRenderer>();
							backgroundRenderer.sharedMaterial = wfMaterial;
							backgroundRenderer.shadowCastingMode = ShadowCastingMode.Off;
							backgroundRenderer.receiveShadows = false;
							backgroundRenderer.enabled = false;
						}
						if (wfCamera == null)
						{
							wfCamera = new GameObject("WireframeCamera").AddComponent<Camera>();
							wfCamera.gameObject.layer = whLayer;
							wfCamera.transform.parent = serviceContainer.transform;
							wfCamera.transform.localPosition = new Vector3(0f, 0f, -1f);
							wfCamera.clearFlags = CameraClearFlags.Color;
							wfCamera.backgroundColor = Color.black;
							wfCamera.orthographic = true;
							wfCamera.orthographicSize = 0.5f;
							wfCamera.nearClipPlane = 0.01f;
							wfCamera.farClipPlane = 2.1f;
							wfCamera.depth = 0f;
							wfCamera.renderingPath = RenderingPath.Forward;
							wfCamera.useOcclusionCulling = false;
							wfCamera.hdr = false;
							wfCamera.targetTexture = renderTexture;
							wfCamera.enabled = false;
						}
					}
				}
				else if (!checkWireframe && wfCamera != null)
				{
					UnityEngine.Object.Destroy(foregroundRenderer.gameObject);
					UnityEngine.Object.Destroy(backgroundRenderer.gameObject);
					wfCamera.targetTexture = null;
					UnityEngine.Object.Destroy(wfCamera.gameObject);
				}
				if (checkRaycast && thinWall == null)
				{
					thinWall = GameObject.CreatePrimitive(PrimitiveType.Plane);
					thinWall.name = "ThinWall";
					thinWall.layer = whLayer;
					thinWall.transform.parent = serviceContainer.transform;
					thinWall.transform.localScale = new Vector3(0.2f, 1f, 0.2f);
					thinWall.transform.localRotation = Quaternion.Euler(270f, 0f, 0f);
					thinWall.transform.localPosition = new Vector3(0f, 0f, 1.4f);
					UnityEngine.Object.Destroy(thinWall.GetComponent<Renderer>());
					UnityEngine.Object.Destroy(thinWall.GetComponent<MeshFilter>());
				}
				else if (!checkRaycast && thinWall != null)
				{
					UnityEngine.Object.Destroy(thinWall);
				}
			}
			else if (serviceContainer != null)
			{
				UnityEngine.Object.Destroy(serviceContainer);
			}
		}

		private IEnumerator InitDetector()
		{
			yield return waitForEndOfFrame;
			UpdateServiceContainer();
			if (checkRigidbody)
			{
				StartRigidModule();
			}
			if (checkController)
			{
				StartControllerModule();
			}
			if (checkWireframe)
			{
				StartWireframeModule();
			}
			if (checkRaycast)
			{
				StartRaycastModule();
			}
		}

		private void StartRigidModule()
		{
			if (!checkRigidbody)
			{
				StopRigidModule();
				UninitRigidModule();
				UpdateServiceContainer();
				return;
			}
			if (!rigidPlayer)
			{
				InitRigidModule();
			}
			if (rigidPlayer.transform.localPosition.z <= 1f && rigidbodyDetections > 0)
			{
				rigidbodyDetections = 0;
			}
			rigidPlayer.rotation = Quaternion.identity;
			rigidPlayer.angularVelocity = Vector3.zero;
			rigidPlayer.transform.localPosition = new Vector3(0.75f, 0f, -1f);
			rigidPlayer.velocity = rigidPlayerVelocity;
			Invoke("StartRigidModule", 4f);
		}

		private void StartControllerModule()
		{
			if (!checkController)
			{
				StopControllerModule();
				UninitControllerModule();
				UpdateServiceContainer();
				return;
			}
			if (!charControllerPlayer)
			{
				InitControllerModule();
			}
			if (charControllerPlayer.transform.localPosition.z <= 1f && controllerDetections > 0)
			{
				controllerDetections = 0;
			}
			charControllerPlayer.transform.localPosition = new Vector3(-0.75f, 0f, -1f);
			charControllerVelocity = 0.01f;
			Invoke("StartControllerModule", 4f);
		}

		private void StartWireframeModule()
		{
			if (!checkWireframe)
			{
				StopWireframeModule();
				UpdateServiceContainer();
			}
			else if (!wireframeDetected)
			{
				Invoke("ShootWireframeModule", wireframeDelay);
			}
		}

		private void ShootWireframeModule()
		{
			StartCoroutine(CaptureFrame());
			Invoke("ShootWireframeModule", wireframeDelay);
		}

		private IEnumerator CaptureFrame()
		{
			wfCamera.enabled = true;
			yield return waitForEndOfFrame;
			foregroundRenderer.enabled = true;
			backgroundRenderer.enabled = true;
			RenderTexture previousActive = RenderTexture.active;
			RenderTexture.active = renderTexture;
			wfCamera.Render();
			foregroundRenderer.enabled = false;
			backgroundRenderer.enabled = false;
			while (!renderTexture.IsCreated())
			{
				yield return waitForEndOfFrame;
			}
			targetTexture.ReadPixels(new Rect(0f, 0f, 4f, 4f), 0, 0, false);
			targetTexture.Apply();
			RenderTexture.active = previousActive;
			if (wfCamera == null)
			{
				yield return null;
			}
			wfCamera.enabled = false;
			if (!(targetTexture.GetPixel(0, 3) != wfColor1) && !(targetTexture.GetPixel(0, 1) != wfColor2) && !(targetTexture.GetPixel(3, 3) != wfColor1) && !(targetTexture.GetPixel(3, 1) != wfColor2) && !(targetTexture.GetPixel(1, 3) != wfColor1) && !(targetTexture.GetPixel(2, 3) != wfColor1) && !(targetTexture.GetPixel(1, 1) != wfColor2) && !(targetTexture.GetPixel(2, 1) != wfColor2))
			{
				if (wireframeDetections > 0)
				{
					wireframeDetections = 0;
				}
			}
			else
			{
				wireframeDetections++;
				wireframeDetected = Detect();
			}
			yield return null;
		}

		private void StartRaycastModule()
		{
			if (!checkRaycast)
			{
				StopRaycastModule();
				UpdateServiceContainer();
			}
			else
			{
				Invoke("ShootRaycastModule", raycastDelay);
			}
		}

		private void ShootRaycastModule()
		{
			if (Physics.Raycast(serviceContainer.transform.position, serviceContainer.transform.TransformDirection(Vector3.forward), 1.5f, raycastMask))
			{
				if (raycastDetections > 0)
				{
					raycastDetections = 0;
				}
			}
			else
			{
				raycastDetections++;
				if (Detect())
				{
					return;
				}
			}
			Invoke("ShootRaycastModule", raycastDelay);
		}

		private void StopRigidModule()
		{
			if ((bool)rigidPlayer)
			{
				rigidPlayer.velocity = Vector3.zero;
			}
			CancelInvoke("StartRigidModule");
		}

		private void StopControllerModule()
		{
			if ((bool)charControllerPlayer)
			{
				charControllerVelocity = 0f;
			}
			CancelInvoke("StartControllerModule");
		}

		private void StopWireframeModule()
		{
			CancelInvoke("ShootWireframeModule");
		}

		private void StopRaycastModule()
		{
			CancelInvoke("ShootRaycastModule");
		}

		private void InitRigidModule()
		{
			GameObject gameObject = new GameObject("RigidPlayer");
			gameObject.AddComponent<CapsuleCollider>().height = 2f;
			gameObject.layer = whLayer;
			gameObject.transform.parent = serviceContainer.transform;
			gameObject.transform.localPosition = new Vector3(0.75f, 0f, -1f);
			rigidPlayer = gameObject.AddComponent<Rigidbody>();
			rigidPlayer.useGravity = false;
		}

		private void InitControllerModule()
		{
			GameObject gameObject = new GameObject("ControlledPlayer");
			gameObject.AddComponent<CapsuleCollider>().height = 2f;
			gameObject.layer = whLayer;
			gameObject.transform.parent = serviceContainer.transform;
			gameObject.transform.localPosition = new Vector3(-0.75f, 0f, -1f);
			charControllerPlayer = gameObject.AddComponent<CharacterController>();
		}

		private void UninitRigidModule()
		{
			if ((bool)rigidPlayer)
			{
				UnityEngine.Object.Destroy(rigidPlayer.gameObject);
				rigidPlayer = null;
			}
		}

		private void UninitControllerModule()
		{
			if ((bool)charControllerPlayer)
			{
				UnityEngine.Object.Destroy(charControllerPlayer.gameObject);
				charControllerPlayer = null;
			}
		}

		private bool Detect()
		{
			bool result = false;
			if (controllerDetections > maxFalsePositives || rigidbodyDetections > maxFalsePositives || wireframeDetections > maxFalsePositives || raycastDetections > maxFalsePositives)
			{
				OnCheatingDetected();
				result = true;
			}
			return result;
		}

		private static Color32 GenerateColor()
		{
			return new Color32((byte)UnityEngine.Random.Range(0, 256), (byte)UnityEngine.Random.Range(0, 256), (byte)UnityEngine.Random.Range(0, 256), byte.MaxValue);
		}

		private static bool ColorsSimilar(Color32 c1, Color32 c2, int tolerance)
		{
			return Math.Abs(c1.r - c2.r) < tolerance && Math.Abs(c1.g - c2.g) < tolerance && Math.Abs(c1.b - c2.b) < tolerance;
		}
	}
}
