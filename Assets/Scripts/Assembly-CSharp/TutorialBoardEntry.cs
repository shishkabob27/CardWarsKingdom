using System.Collections.Generic;

public class TutorialBoardEntry
{
	private string _CreatureID;

	private bool _isOut;

	private float _HPFactor;

	private PlayerType _whichPlayer;

	public string CreatureID
	{
		get
		{
			return _CreatureID;
		}
	}

	public bool isOut
	{
		get
		{
			return _isOut;
		}
	}

	public float HPFactor
	{
		get
		{
			return _HPFactor;
		}
	}

	public PlayerType whichPlayer
	{
		get
		{
			return _whichPlayer;
		}
	}

	public TutorialBoardEntry(Dictionary<string, object> dict)
	{
		_CreatureID = TFUtils.LoadString(dict, "CreatureID", string.Empty);
		_HPFactor = TFUtils.LoadFloat(dict, "HPFactor", 1f);
		_isOut = TFUtils.LoadBool(dict, "isOut", false);
		int num = TFUtils.LoadInt(dict, "Player", 0);
		_whichPlayer = ((num != 0) ? PlayerType.Opponent : PlayerType.User);
	}
}
