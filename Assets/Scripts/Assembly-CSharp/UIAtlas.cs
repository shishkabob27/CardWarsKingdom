using UnityEngine;
using System;
using System.Collections.Generic;

public class UIAtlas : MonoBehaviour
{
	[Serializable]
	private class Sprite
	{
		public string name;
		public Rect outer;
		public Rect inner;
		public bool rotated;
		public float paddingLeft;
		public float paddingRight;
		public float paddingTop;
		public float paddingBottom;
	}

	private enum Coordinates
	{
		Pixels = 0,
		TexCoords = 1,
	}

	[SerializeField]
	private Material material;
	[SerializeField]
	private List<UISpriteData> mSprites;
	[SerializeField]
	private float mPixelSize;
	[SerializeField]
	private UIAtlas mReplacement;
	[SerializeField]
	private Coordinates mCoordinates;
	[SerializeField]
	private List<UIAtlas.Sprite> sprites;
}
