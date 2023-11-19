using UnityEngine;
using System.Collections.Generic;

public class UIInput : MonoBehaviour
{
	public enum InputType
	{
		Standard = 0,
		AutoCorrect = 1,
		Password = 2,
	}

	public enum OnReturnKey
	{
		Default = 0,
		Submit = 1,
		NewLine = 2,
	}

	public enum KeyboardType
	{
		Default = 0,
		ASCIICapable = 1,
		NumbersAndPunctuation = 2,
		URL = 3,
		NumberPad = 4,
		PhonePad = 5,
		NamePhonePad = 6,
		EmailAddress = 7,
	}

	public enum Validation
	{
		None = 0,
		Integer = 1,
		Float = 2,
		Alphanumeric = 3,
		Username = 4,
		Name = 5,
	}

	public UILabel label;
	public InputType inputType;
	public OnReturnKey onReturnKey;
	public KeyboardType keyboardType;
	public bool hideInput;
	public Validation validation;
	public int characterLimit;
	public string savedAs;
	public GameObject selectOnTab;
	public Color activeTextColor;
	public Color caretColor;
	public Color selectionColor;
	public List<EventDelegate> onSubmit;
	public List<EventDelegate> onChange;
	public List<EventDelegate> onCancel;
	[SerializeField]
	protected string mValue;
}
