using UnityEngine;

[RequireComponent(typeof(UILabel))]
public class LabelShadow : MonoBehaviour
{
	public Vector2 Offset = new Vector2(0f, 0f);

	[Header("Optional Color Settings")]
	public Color ShadowTextColor = Color.clear;

	public Color ShadowEffectColor = Color.clear;

	private UILabel _TargetLabel;

	private UILabel _ShadowLabel;

	private bool _IsUpdateAnchored;

	private void Start()
	{
		if (!(_ShadowLabel == null))
		{
			return;
		}
		_TargetLabel = GetComponent<UILabel>();
		_TargetLabel.LabelShadow = this;
		GameObject gameObject = NGUITools.AddChild(_TargetLabel.gameObject, _TargetLabel.gameObject);
		Component[] components = gameObject.GetComponents<Component>();
		foreach (Component component in components)
		{
			if (!(component is UILabel) && !(component is Transform))
			{
				Object.Destroy(component);
			}
		}
		Transform[] componentsInChildren = gameObject.GetComponentsInChildren<Transform>();
		foreach (Transform transform in componentsInChildren)
		{
			if (transform.gameObject != gameObject)
			{
				Object.Destroy(transform.gameObject);
			}
		}
		UILabel component2 = gameObject.GetComponent<UILabel>();
		_IsUpdateAnchored = _TargetLabel.isAnchored && _TargetLabel.updateAnchors == UIRect.AnchorUpdate.OnUpdate;
		if (_IsUpdateAnchored)
		{
			component2.leftAnchor.target = _TargetLabel.transform;
			component2.leftAnchor.absolute = Mathf.RoundToInt(Offset.x);
			component2.rightAnchor.target = _TargetLabel.transform;
			component2.rightAnchor.relative = 1f;
			component2.rightAnchor.absolute = Mathf.RoundToInt(Offset.x);
			component2.bottomAnchor.target = _TargetLabel.transform;
			component2.bottomAnchor.relative = 0f;
			component2.bottomAnchor.absolute = Mathf.RoundToInt(Offset.y);
			component2.topAnchor.target = _TargetLabel.transform;
			component2.topAnchor.absolute = Mathf.RoundToInt(Offset.y);
			component2.updateAnchors = UIRect.AnchorUpdate.OnUpdate;
			component2.ResetAndUpdateAnchors();
		}
		else
		{
			component2.leftAnchor = new UIRect.AnchorPoint();
			component2.rightAnchor = new UIRect.AnchorPoint();
			component2.bottomAnchor = new UIRect.AnchorPoint();
			component2.topAnchor = new UIRect.AnchorPoint();
		}
		gameObject.name = _TargetLabel.name + "_shadow";
		_ShadowLabel = gameObject.GetComponent<UILabel>();
		ShadowTextColor = ((!(ShadowTextColor != Color.clear)) ? _TargetLabel.effectColor : ShadowTextColor);
		ShadowEffectColor = ((!(ShadowEffectColor != Color.clear)) ? _TargetLabel.effectColor : ShadowEffectColor);
		_ShadowLabel.effectStyle = _TargetLabel.effectStyle;
		_ShadowLabel.maxLineCount = _TargetLabel.maxLineCount;
		_TargetLabel.depth++;
		RefreshShadowLabel();
	}

	private void OnEnable()
	{
		RefreshShadowLabel();
	}

	public void RefreshShadowLabel()
	{
		if (_ShadowLabel != null)
		{
			_ShadowLabel.text = _TargetLabel.text;
			_ShadowLabel.color = ShadowTextColor;
			_ShadowLabel.effectColor = ShadowEffectColor;
			_ShadowLabel.transform.localPosition = new Vector3(Offset.x, Offset.y, 0f);
			_ShadowLabel.fontSize = _TargetLabel.fontSize;
			_ShadowLabel.depth = _TargetLabel.depth - 1;
			_ShadowLabel.width = _TargetLabel.width;
			_ShadowLabel.height = _TargetLabel.height;
		}
	}
}
