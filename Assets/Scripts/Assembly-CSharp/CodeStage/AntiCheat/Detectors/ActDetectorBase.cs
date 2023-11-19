using UnityEngine;
using UnityEngine.Events;

namespace CodeStage.AntiCheat.Detectors
{
	public class ActDetectorBase : MonoBehaviour
	{
		public bool autoStart;
		public bool keepAlive;
		public bool autoDispose;
		[SerializeField]
		protected UnityEvent detectionEvent;
		[SerializeField]
		protected bool detectionEventHasListener;
	}
}
