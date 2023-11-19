using System;
using System.Collections.Generic;
using UnityEngine;

public class GameEventFXData : ILoadableData
{
	public enum ParticleAttractorPoint
	{
		None,
		Creature,
		MyPortrait
	}

	private string _ID;

	private string _ActionFXPrefab;

	private GameEventFXSpawnPoint _ActionFXSpawnPoint;

	private string _CreatureStatusFXPrefab;

	private bool _ValueDisplay;

	private Color _DisplayTextColor;

	private GameEvent _DisableEvent;

	private string _Anim;

	private string _RezInOut;

	private bool _IgnoreDup;

	private string _SwapModel;

	private bool _FreezeModel;

	private float _ActionFXWaitTime;

	private bool _CanGroup;

	private float _ActionFXGroupedWaitTime;

	public string ID
	{
		get
		{
			return _ID;
		}
	}

	public string ActionFXPrefab
	{
		get
		{
			return _ActionFXPrefab;
		}
	}

	public GameEventFXSpawnPoint ActionFXSpawnPoint
	{
		get
		{
			return _ActionFXSpawnPoint;
		}
	}

	public string CreatureStatusFXPrefab
	{
		get
		{
			return _CreatureStatusFXPrefab;
		}
	}

	public bool ValueDisplay
	{
		get
		{
			return _ValueDisplay;
		}
	}

	public Color DisplayTextColor
	{
		get
		{
			return _DisplayTextColor;
		}
	}

	public GameEvent DisableEvent
	{
		get
		{
			return _DisableEvent;
		}
	}

	public string Anim
	{
		get
		{
			return _Anim;
		}
	}

	public string RezInOut
	{
		get
		{
			return _RezInOut;
		}
	}

	public bool IgnoreDup
	{
		get
		{
			return _IgnoreDup;
		}
	}

	public string SwapModelPrefab
	{
		get
		{
			return _SwapModel;
		}
	}

	public bool FreezeModel
	{
		get
		{
			return _FreezeModel;
		}
	}

	public float ActionFXWaitTime
	{
		get
		{
			return _ActionFXWaitTime;
		}
	}

	public bool CanGroup
	{
		get
		{
			return _CanGroup;
		}
	}

	public float ActionFXGroupedWaitTime
	{
		get
		{
			return _ActionFXGroupedWaitTime;
		}
	}

	public bool DetachStatusFXFromCreature { get; private set; }

	public ParticleAttractorPoint AttractPoint { get; private set; }

	public KeyWordData Keyword { get; set; }

	public void Populate(Dictionary<string, object> dict)
	{
		_ID = TFUtils.LoadString(dict, "ID", string.Empty);
		_ActionFXPrefab = TFUtils.LoadString(dict, "ActionFXPrefab", string.Empty);
		_CreatureStatusFXPrefab = TFUtils.LoadString(dict, "CreatureStatusFXPrefab", string.Empty);
		_ValueDisplay = TFUtils.LoadBool(dict, "ValueDisplay", false);
		_ActionFXWaitTime = TFUtils.LoadFloat(dict, "ActionFXWaitTime", 0f);
		_CanGroup = TFUtils.LoadBool(dict, "CanGroup", false);
		_ActionFXGroupedWaitTime = TFUtils.LoadFloat(dict, "GroupedWaitTime", 0f);
		DetachStatusFXFromCreature = TFUtils.LoadBool(dict, "DetachStatusFXFromCreature", false);
		string text = TFUtils.LoadString(dict, "StatusDisableEvent", string.Empty);
		GameEvent disableEvent = GameEvent.NONE;
		if (text != string.Empty)
		{
			try
			{
				disableEvent = (GameEvent)(int)Enum.Parse(typeof(GameEvent), text);
			}
			catch
			{
				disableEvent = GameEvent.NONE;
			}
		}
		_DisableEvent = disableEvent;
		text = TFUtils.LoadString(dict, "DisplayTextColor", string.Empty);
		Color displayTextColor = Color.white;
		if (text != string.Empty)
		{
			switch (text)
			{
			case "Green":
				displayTextColor = Color.green;
				break;
			case "Red":
				displayTextColor = Color.red;
				break;
			case "White":
				displayTextColor = Color.white;
				break;
			case "Blue":
				displayTextColor = Color.blue;
				break;
			case "Cyan":
				displayTextColor = Color.cyan;
				break;
			case "Grey":
				displayTextColor = Color.gray;
				break;
			case "Magenta":
				displayTextColor = Color.magenta;
				break;
			case "Yellow":
				displayTextColor = Color.yellow;
				break;
			}
		}
		_DisplayTextColor = displayTextColor;
		_Anim = TFUtils.LoadString(dict, "Anim", string.Empty);
		_RezInOut = TFUtils.LoadString(dict, "RezInOut", string.Empty);
		text = TFUtils.LoadString(dict, "ActionFXSpawnPoint", string.Empty);
		if (text != string.Empty)
		{
			_ActionFXSpawnPoint = (GameEventFXSpawnPoint)(int)Enum.Parse(typeof(GameEventFXSpawnPoint), text);
		}
		else
		{
			_ActionFXSpawnPoint = GameEventFXSpawnPoint.None;
		}
		string text2 = TFUtils.LoadString(dict, "ParticleAttractorPoint", null);
		if (text2 != null)
		{
			AttractPoint = (ParticleAttractorPoint)(int)Enum.Parse(typeof(ParticleAttractorPoint), text2);
		}
		_IgnoreDup = TFUtils.LoadBool(dict, "IgnoreDuplicates", false);
		_SwapModel = TFUtils.LoadString(dict, "SwapModel", string.Empty);
		_FreezeModel = TFUtils.LoadBool(dict, "FreezeModel", false);
	}
}
