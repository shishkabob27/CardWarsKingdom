using System;
using System.Collections.Generic;
using UnityEngine;

public class TutorialState : ILoadableData
{
	public enum TargetEnum
	{
		None,
		Floating,
		PointObject,
		TapObject,
		PointBuilding,
		TapBuilding,
		League,
		Quest,
		PointCreature,
		TapCreature,
		PointEnemyCreature,
		TapEnemyCreature,
		EnemyCreatureStr,
		CreatureAttack,
		PointCard,
		TapCard,
		CardEnergy,
		PopupCard,
		CardPlay,
		CardPlayOnEnemy,
		CreatureCardPlay,
		Status,
		EnemyTurn,
		Special,
		Conditionals,
		AdvanceIfNotTargetingCard,
		PointKeyword,
		EditDeck
	}

	public enum SpecialTargetEnum
	{
		None,
		PlayableCards,
		DroppedEgg,
		UnusedCreatureInList,
		BestCreatureInList,
		TutorialFuseCreature,
		TutorialEnhanceCreature,
		TutorialEvoCreature,
		TutorialCardCreature,
		TutorialGemcraftCreature,
		UnusedGemInList,
		BestGemInList,
		TutorialFuseGem,
		TutorialGemFuseCreature,
		ZoomedCardText,
		TapNewestCreatureInList,
		TapNewestEVOCreatureInList,
		AddNewestCreatureInList,
		AnyGachaCard,
		BestCreatureCards,
		UnusedCardInList,
		HighestLeague,
		HighestQuest,
		HelperCreature,
		TutorialNextTeam,
		TutorialPrevTeam
	}

	public enum DragDirectionEnum
	{
		None,
		Up,
		Right
	}

	public enum ForceActionEnum
	{
		None,
		Draw,
		Attack,
		Crit,
		PlayAttackCard,
		PlayCreatureCard,
		Drop,
		StartHand,
		StartHandEnemy,
		FillLeaderBar,
		StartAP,
		StartAPEnemy
	}

	public enum ConditionTypeEnum
	{
		None,
		AttackWith,
		LowHealth,
		CreatureDied,
		CreatureOverused,
		CardCombo,
		KeywordCombo,
		PlayedCombo,
		CardOnStatus,
		CreatureDrop,
		HaveDebuff,
		StackableEffects,
		CanFillLeaderBar,
		DidNothing,
		HandAlmostFull,
		HandEmpty,
		CardOnLandscape,
		LeaderBar,
		HeroCard
	}

	public class ForceAction
	{
		public ForceActionEnum Action;

		public List<string> Targets = new List<string>();

		public string CardDrawOverride;

		public float HealthPercent;
	}

	private string _ID;

	private int _Index;

	private string _Block;

	private string _Trigger;

	private Vector3 _PopupPos;

	private bool _PositionSet;

	private bool _EndState;

	private bool _ContinueThrough;

	private TargetEnum _Target;

	private List<string> _TargetIds = new List<string>();

	private SpecialTargetEnum _SpecialTarget;

	private string _Text;

	private List<ForceAction> _ForceActions = new List<ForceAction>();

	private string _CharacterTexture;

	private bool _HideArrow;

	private bool _ManualClose;

	private bool _ManualAdvance;

	private List<string> _EntryFunctionCalls = new List<string>();

	private List<string> _ExitFunctionCalls = new List<string>();

	private float _StartDelay;

	private ConditionTypeEnum _ConditionType;

	private string[] _ConditionIds = new string[3];

	private bool _Repeat;

	private int _MaxHeight;

	private string _tutorialBaord;

	public List<TutorialObject> ObjectList = new List<TutorialObject>();

	public List<TutorialDragObject> DragTargetList = new List<TutorialDragObject>();

	public bool Passed;

	private static string mCurrentBlock = string.Empty;

	public string ID
	{
		get
		{
			return _ID;
		}
	}

	public int Index
	{
		get
		{
			return _Index;
		}
	}

	public string Block
	{
		get
		{
			return _Block;
		}
	}

	public string Trigger
	{
		get
		{
			return _Trigger;
		}
	}

	public Vector3 PopupPos
	{
		get
		{
			return _PopupPos;
		}
	}

	public bool PositionSet
	{
		get
		{
			return _PositionSet;
		}
	}

	public bool EndState
	{
		get
		{
			return _EndState;
		}
	}

	public bool ContinueThrough
	{
		get
		{
			return _ContinueThrough;
		}
		set
		{
			_ContinueThrough = value;
		}
	}

	public TargetEnum Target
	{
		get
		{
			return _Target;
		}
	}

	public SpecialTargetEnum SpecialTarget
	{
		get
		{
			return _SpecialTarget;
		}
	}

	public string Text
	{
		get
		{
			return _Text;
		}
	}

	public List<ForceAction> ForceActions
	{
		get
		{
			return _ForceActions;
		}
	}

	public string CharacterTexture
	{
		get
		{
			return _CharacterTexture;
		}
	}

	public List<string> EntryFunctionCalls
	{
		get
		{
			return _EntryFunctionCalls;
		}
	}

	public List<string> ExitFunctionCalls
	{
		get
		{
			return _ExitFunctionCalls;
		}
	}

	public float StartDelay
	{
		get
		{
			return _StartDelay;
		}
	}

	public ConditionTypeEnum ConditionType
	{
		get
		{
			return _ConditionType;
		}
	}

	public string[] ConditionIds
	{
		get
		{
			return _ConditionIds;
		}
	}

	public bool Repeat
	{
		get
		{
			return _Repeat;
		}
	}

	public int MaxHeight
	{
		get
		{
			return _MaxHeight;
		}
	}

	public string VOEvent { get; private set; }

	public string TextTitle { get; private set; }

	public string tutorialBoard
	{
		get
		{
			return _tutorialBaord;
		}
	}

	public string TargetId(int index)
	{
		if (index >= _TargetIds.Count)
		{
			return null;
		}
		return _TargetIds[index];
	}

	public void Populate(Dictionary<string, object> dict)
	{
		_ID = TFUtils.LoadString(dict, "State", string.Empty);
		_Index = TutorialDataManager.Instance.GetDatabase().Count;
		_Trigger = TFUtils.LoadString(dict, "Trigger", string.Empty);
		_EndState = TFUtils.LoadBool(dict, "EndState", false);
		_ContinueThrough = TFUtils.LoadBool(dict, "ContinueThrough", false);
		_Text = TFUtils.LoadLocalizedString(dict, "Text", string.Empty);
		_CharacterTexture = TFUtils.LoadString(dict, "CharacterTexture", string.Empty);
		_HideArrow = TFUtils.LoadBool(dict, "HideArrow", false);
		_ManualClose = TFUtils.LoadBool(dict, "ManualClose", false);
		_ManualAdvance = TFUtils.LoadBool(dict, "ManualAdvance", false);
		_StartDelay = TFUtils.LoadFloat(dict, "StartDelay", 0f);
		_Repeat = TFUtils.LoadBool(dict, "Repeat", false);
		_MaxHeight = TFUtils.LoadInt(dict, "MaxHeight", -1);
		_tutorialBaord = TFUtils.LoadString(dict, "tutorialBoard", null);
		VOEvent = TFUtils.LoadString(dict, "VO", null);
		TextTitle = TFUtils.LoadLocalizedString(dict, "TextTitle", string.Empty);
		char[] separator = new char[2] { ':', ',' };
		char[] separator2 = new char[1] { ';' };
		string text = TFUtils.LoadString(dict, "EntryFunctionCall", string.Empty);
		if (text != string.Empty)
		{
			string[] array = text.Split(separator2);
			string[] array2 = array;
			foreach (string text2 in array2)
			{
				if (!(text2 == string.Empty))
				{
					_EntryFunctionCalls.Add(text2.Trim());
				}
			}
		}
		text = TFUtils.LoadString(dict, "ExitFunctionCall", string.Empty);
		if (text != string.Empty)
		{
			string[] array3 = text.Split(separator2);
			string[] array4 = array3;
			foreach (string text3 in array4)
			{
				if (!(text3 == string.Empty))
				{
					_ExitFunctionCalls.Add(text3.Trim());
				}
			}
		}
		string text4 = TFUtils.LoadString(dict, "Target", string.Empty);
		if (text4 != string.Empty)
		{
			string[] array5 = text4.Split(separator);
			for (int k = 0; k < array5.Length; k++)
			{
				array5[k] = array5[k].Trim();
			}
			if (array5.Length > 0)
			{
				_Target = (TargetEnum)(int)Enum.Parse(typeof(TargetEnum), array5[0], true);
				if (_Target == TargetEnum.Special)
				{
					_SpecialTarget = (SpecialTargetEnum)(int)Enum.Parse(typeof(SpecialTargetEnum), array5[1], true);
				}
				else
				{
					for (int l = 1; l < array5.Length; l++)
					{
						_TargetIds.Add(array5[l]);
					}
				}
			}
		}
		int num = TFUtils.LoadInt(dict, "PopupX", int.MinValue);
		int num2 = TFUtils.LoadInt(dict, "PopupY", int.MinValue);
		if (num != int.MinValue && num2 != int.MinValue)
		{
			_PositionSet = true;
			_PopupPos = new Vector3(num, num2, 0f);
		}
		else
		{
			_PositionSet = false;
		}
		string text5 = TFUtils.LoadString(dict, "ForceAction", string.Empty);
		if (text5 != string.Empty)
		{
			string[] array6 = text5.Split(separator2);
			string[] array7 = array6;
			foreach (string text6 in array7)
			{
				if (text6 == string.Empty)
				{
					continue;
				}
				string[] array8 = text6.Split(separator);
				for (int n = 0; n < array8.Length; n++)
				{
					array8[n] = array8[n].Trim();
				}
				if (array8.Length > 0)
				{
					ForceAction forceAction = new ForceAction();
					forceAction.Action = (ForceActionEnum)(int)Enum.Parse(typeof(ForceActionEnum), array8[0], true);
					for (int num3 = 1; num3 < array8.Length; num3++)
					{
						forceAction.Targets.Add(array8[num3]);
					}
					ForceAction forceAction2 = null;
					if (_ForceActions.Count > 0)
					{
						forceAction2 = _ForceActions[_ForceActions.Count - 1];
					}
					if (forceAction.Action == ForceActionEnum.Draw && forceAction2 != null && forceAction2.Action == ForceActionEnum.Attack)
					{
						forceAction2.CardDrawOverride = forceAction.Targets[0];
					}
					_ForceActions.Add(forceAction);
				}
			}
		}
		string text7 = TFUtils.LoadString(dict, "Condition", null);
		if (text7 != null)
		{
			string[] array9 = text7.Split(separator);
			for (int num4 = 0; num4 < array9.Length; num4++)
			{
				array9[num4] = array9[num4].Trim();
			}
			_ConditionType = (ConditionTypeEnum)(int)Enum.Parse(typeof(ConditionTypeEnum), array9[0], true);
			for (int num5 = 0; num5 < _ConditionIds.Length; num5++)
			{
				if (num5 + 1 < array9.Length)
				{
					_ConditionIds[num5] = array9[num5 + 1];
				}
			}
		}
		if (!(CharacterTexture != string.Empty) || Text == string.Empty)
		{
		}
		_Block = TFUtils.LoadString(dict, "Block", string.Empty);
		if (_Block != string.Empty)
		{
			mCurrentBlock = _Block;
			TutorialDataManager.Instance.AddBlock(this);
		}
		else
		{
			_Block = mCurrentBlock;
		}
	}

	public void ClearOutDestroyedTutorialObjects()
	{
		ObjectList.RemoveAll((TutorialObject match) => match == null);
		DragTargetList.RemoveAll((TutorialDragObject match) => match == null);
	}

	public bool ManualClose()
	{
		if (_ManualClose)
		{
			return true;
		}
		if (Target == TargetEnum.Special && (SpecialTarget == SpecialTargetEnum.UnusedCreatureInList || SpecialTarget == SpecialTargetEnum.BestCreatureInList || SpecialTarget == SpecialTargetEnum.TutorialFuseCreature || SpecialTarget == SpecialTargetEnum.TutorialEnhanceCreature || SpecialTarget == SpecialTargetEnum.TutorialEvoCreature || SpecialTarget == SpecialTargetEnum.TutorialCardCreature || SpecialTarget == SpecialTargetEnum.UnusedCardInList || SpecialTarget == SpecialTargetEnum.HelperCreature))
		{
			return true;
		}
		return false;
	}

	public bool ManualAdvance()
	{
		if (_ManualAdvance)
		{
			return true;
		}
		if (Target == TargetEnum.CreatureAttack || Target == TargetEnum.CardPlay || Target == TargetEnum.CardPlayOnEnemy || Target == TargetEnum.CreatureCardPlay || Target == TargetEnum.TapEnemyCreature || Target == TargetEnum.TapBuilding)
		{
			return true;
		}
		return false;
	}

	public bool HideArrow()
	{
		if (_HideArrow)
		{
			return true;
		}
		if (Target == TargetEnum.Floating || Target == TargetEnum.CreatureAttack || Target == TargetEnum.CardPlay || Target == TargetEnum.CardPlayOnEnemy || Target == TargetEnum.CreatureCardPlay)
		{
			return true;
		}
		if (Target == TargetEnum.Special && (SpecialTarget == SpecialTargetEnum.UnusedCreatureInList || SpecialTarget == SpecialTargetEnum.BestCreatureInList || SpecialTarget == SpecialTargetEnum.AddNewestCreatureInList || SpecialTarget == SpecialTargetEnum.TutorialFuseCreature || SpecialTarget == SpecialTargetEnum.TutorialEnhanceCreature || SpecialTarget == SpecialTargetEnum.TutorialEvoCreature || SpecialTarget == SpecialTargetEnum.TutorialCardCreature || SpecialTarget == SpecialTargetEnum.UnusedCardInList || SpecialTarget == SpecialTargetEnum.HelperCreature))
		{
			return true;
		}
		return false;
	}

	public bool UseFullScreenCollider()
	{
		if (Target == TargetEnum.Floating || Target == TargetEnum.PointObject || Target == TargetEnum.PointCreature || Target == TargetEnum.PointEnemyCreature || Target == TargetEnum.EnemyCreatureStr || Target == TargetEnum.PointCard || Target == TargetEnum.CardEnergy || Target == TargetEnum.PointBuilding || Target == TargetEnum.PointKeyword)
		{
			return true;
		}
		return false;
	}

	public DragDirectionEnum DragDirection()
	{
		if (Target == TargetEnum.CreatureCardPlay)
		{
			return DragDirectionEnum.Right;
		}
		if ((Target == TargetEnum.CreatureAttack || Target == TargetEnum.CardPlay) && TargetId(1) == null)
		{
			return DragDirectionEnum.Up;
		}
		return DragDirectionEnum.None;
	}

	public bool ShouldHavePositionSet()
	{
		if (Text == string.Empty)
		{
			return false;
		}
		if (Target == TargetEnum.Floating || Target == TargetEnum.CreatureAttack || Target == TargetEnum.CardPlay || Target == TargetEnum.CardPlayOnEnemy || Target == TargetEnum.CreatureCardPlay)
		{
			return true;
		}
		return false;
	}
}
