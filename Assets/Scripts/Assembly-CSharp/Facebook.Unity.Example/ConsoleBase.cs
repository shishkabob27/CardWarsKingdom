using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Facebook.Unity.Example
{
	internal class ConsoleBase : MonoBehaviour
	{
		private const int DpiScalingFactor = 160;

		protected static int ButtonHeight = ((!Constants.IsMobile) ? 24 : 60);

		protected static int MainWindowWidth = ((!Constants.IsMobile) ? 700 : (Screen.width - 30));

		protected static int MainWindowFullWidth = ((!Constants.IsMobile) ? 760 : Screen.width);

		protected static int MarginFix = ((!Constants.IsMobile) ? 48 : 0);

		private static Stack<string> menuStack = new Stack<string>();

		private string status = "Ready";

		private string lastResponse = string.Empty;

		private Vector2 scrollPosition = Vector2.zero;

		private float? scaleFactor;

		private GUIStyle textStyle;

		private GUIStyle buttonStyle;

		private GUIStyle textInputStyle;

		private GUIStyle labelStyle;

		protected static Stack<string> MenuStack
		{
			get
			{
				return menuStack;
			}
			set
			{
				menuStack = value;
			}
		}

		protected string Status
		{
			get
			{
				return status;
			}
			set
			{
				status = value;
			}
		}

		protected Texture2D LastResponseTexture { get; set; }

		protected string LastResponse
		{
			get
			{
				return lastResponse;
			}
			set
			{
				lastResponse = value;
			}
		}

		protected Vector2 ScrollPosition
		{
			get
			{
				return scrollPosition;
			}
			set
			{
				scrollPosition = value;
			}
		}

		protected float ScaleFactor
		{
			get
			{
				if (!scaleFactor.HasValue)
				{
					scaleFactor = Screen.dpi / 160f;
				}
				return scaleFactor.Value;
			}
		}

		protected int FontSize
		{
			get
			{
				return (int)Math.Round(ScaleFactor * 16f);
			}
		}

		protected GUIStyle TextStyle
		{
			get
			{
				if (textStyle == null)
				{
					textStyle = new GUIStyle(GUI.skin.textArea);
					textStyle.alignment = TextAnchor.UpperLeft;
					textStyle.wordWrap = true;
					textStyle.padding = new RectOffset(10, 10, 10, 10);
					textStyle.stretchHeight = true;
					textStyle.stretchWidth = false;
					textStyle.fontSize = FontSize;
				}
				return textStyle;
			}
		}

		protected GUIStyle ButtonStyle
		{
			get
			{
				if (buttonStyle == null)
				{
					buttonStyle = new GUIStyle(GUI.skin.button);
					buttonStyle.fontSize = FontSize;
				}
				return buttonStyle;
			}
		}

		protected GUIStyle TextInputStyle
		{
			get
			{
				if (textInputStyle == null)
				{
					textInputStyle = new GUIStyle(GUI.skin.textField);
					textInputStyle.fontSize = FontSize;
				}
				return textInputStyle;
			}
		}

		protected GUIStyle LabelStyle
		{
			get
			{
				if (labelStyle == null)
				{
					labelStyle = new GUIStyle(GUI.skin.label);
					labelStyle.fontSize = FontSize;
				}
				return labelStyle;
			}
		}

		protected virtual void Awake()
		{
			Application.targetFrameRate = 60;
		}

		protected bool Button(string label)
		{
			return GUILayout.Button(label, ButtonStyle, GUILayout.MinHeight((float)ButtonHeight * ScaleFactor), GUILayout.MaxWidth(MainWindowWidth));
		}

		protected void LabelAndTextField(string label, ref string text)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label(label, LabelStyle, GUILayout.MaxWidth(200f * ScaleFactor));
			text = GUILayout.TextField(text, TextInputStyle, GUILayout.MaxWidth(MainWindowWidth - 150));
			GUILayout.EndHorizontal();
		}

		protected bool IsHorizontalLayout()
		{
			return Screen.orientation == ScreenOrientation.LandscapeLeft;
		}

		protected void SwitchMenu(Type menuClass)
		{
			menuStack.Push(GetType().Name);
			Application.LoadLevel(menuClass.Name);
		}

		protected void GoBack()
		{
			if (menuStack.Any())
			{
				Application.LoadLevel(menuStack.Pop());
			}
		}
	}
}
