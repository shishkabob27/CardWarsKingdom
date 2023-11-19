using System.Collections.Generic;
using UnityEngine;

namespace DarkTonic.MasterAudio
{
	[AddComponentMenu("Dark Tonic/Master Audio/Button Clicker")]
	public class ButtonClicker : MonoBehaviour
	{
		public const float SmallSizeMultiplier = 0.9f;

		public const float LargeSizeMultiplier = 1.1f;

		public bool resizeOnClick = true;

		public bool resizeClickAllSiblings;

		public bool resizeOnHover;

		public bool resizeHoverAllSiblings;

		public string mouseDownSound = string.Empty;

		public string mouseUpSound = string.Empty;

		public string mouseClickSound = string.Empty;

		public string mouseOverSound = string.Empty;

		public string mouseOutSound = string.Empty;

		private Vector3 _originalSize;

		private Vector3 _smallerSize;

		private Vector3 _largerSize;

		private Transform _trans;

		private readonly Dictionary<Transform, Vector3> _siblingClickScaleByTransform = new Dictionary<Transform, Vector3>();

		private readonly Dictionary<Transform, Vector3> _siblingHoverScaleByTransform = new Dictionary<Transform, Vector3>();

		private void Awake()
		{
			_trans = base.transform;
			_originalSize = _trans.localScale;
			_smallerSize = _originalSize * 0.9f;
			_largerSize = _originalSize * 1.1f;
			Transform parent = _trans.parent;
			if (resizeOnClick && resizeClickAllSiblings && parent != null)
			{
				for (int i = 0; i < parent.transform.childCount; i++)
				{
					Transform child = parent.transform.GetChild(i);
					_siblingClickScaleByTransform.Add(child, child.localScale);
				}
			}
			if (resizeOnHover && resizeHoverAllSiblings && !(parent == null))
			{
				for (int j = 0; j < parent.transform.childCount; j++)
				{
					Transform child2 = parent.transform.GetChild(j);
					_siblingHoverScaleByTransform.Add(child2, child2.localScale);
				}
			}
		}

		private void OnPress(bool isDown)
		{
			if (isDown)
			{
				if (!base.enabled)
				{
					return;
				}
				MasterAudio.PlaySoundAndForget(mouseDownSound);
				if (resizeOnClick)
				{
					_trans.localScale = _smallerSize;
					Dictionary<Transform, Vector3>.Enumerator enumerator = _siblingClickScaleByTransform.GetEnumerator();
					while (enumerator.MoveNext())
					{
						enumerator.Current.Key.localScale = enumerator.Current.Value * 0.9f;
					}
				}
				return;
			}
			if (base.enabled)
			{
				MasterAudio.PlaySoundAndForget(mouseUpSound);
			}
			if (resizeOnClick)
			{
				_trans.localScale = _originalSize;
				Dictionary<Transform, Vector3>.Enumerator enumerator2 = _siblingClickScaleByTransform.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					enumerator2.Current.Key.localScale = enumerator2.Current.Value;
				}
			}
		}

		private void OnClick()
		{
			if (base.enabled)
			{
				MasterAudio.PlaySoundAndForget(mouseClickSound);
			}
		}

		private void OnHover(bool isOver)
		{
			if (isOver)
			{
				if (!base.enabled)
				{
					return;
				}
				MasterAudio.PlaySoundAndForget(mouseOverSound);
				if (resizeOnHover)
				{
					_trans.localScale = _largerSize;
					Dictionary<Transform, Vector3>.Enumerator enumerator = _siblingHoverScaleByTransform.GetEnumerator();
					while (enumerator.MoveNext())
					{
						enumerator.Current.Key.localScale = enumerator.Current.Value * 1.1f;
					}
				}
				return;
			}
			if (base.enabled)
			{
				MasterAudio.PlaySoundAndForget(mouseOutSound);
			}
			if (resizeOnHover)
			{
				_trans.localScale = _originalSize;
				Dictionary<Transform, Vector3>.Enumerator enumerator2 = _siblingHoverScaleByTransform.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					enumerator2.Current.Key.localScale = enumerator2.Current.Value;
				}
			}
		}
	}
}
