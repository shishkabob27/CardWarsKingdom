using System.Collections.Generic;

public class LeaderVFXEntry
{
	private string _AnimStateName;

	private string _VFXName;

	private string _AttachNode;

	private float _StartOffsetTime;

	public string AnimStateName
	{
		get
		{
			return _AnimStateName;
		}
	}

	public string VFXName
	{
		get
		{
			return _VFXName;
		}
	}

	public string AttachNode
	{
		get
		{
			return _AttachNode;
		}
	}

	public float StartOffsetTime
	{
		get
		{
			return _StartOffsetTime;
		}
	}

	public LeaderVFXEntry(Dictionary<string, object> dict)
	{
		_VFXName = TFUtils.LoadString(dict, "VFXName", string.Empty);
		_AnimStateName = TFUtils.LoadString(dict, "AnimStateName", string.Empty);
		_AttachNode = TFUtils.LoadString(dict, "AttachNode", string.Empty);
		_StartOffsetTime = TFUtils.LoadFloat(dict, "StartOffsetFrame", 0f) / 30f;
	}
}
