using UnityEngine;

[RequireComponent(typeof(UISprite))]
public class SpriteAnimator : MonoBehaviour
{
	[Header("-1 = infinite, 0 = don't autoplay")]
	public int Loops = -1;

	private int _Index;

	private int _LoopCounter;

	private float _LastFrameTime;

	private UISprite _Sprite;

	[SerializeField]
	private float _FPS = 10f;

	[SerializeField]
	private string[] _SpriteNames = new string[0];

	private void Awake()
	{
		_Sprite = GetComponent<UISprite>();
		_LastFrameTime = Time.time;
		if (_SpriteNames.Length == 0)
		{
			Object.Destroy(this);
		}
	}

	public void PlayOnce()
	{
		Play(1);
	}

	public void Play(int numLoops = -1)
	{
		_Index = 0;
		_LoopCounter = 0;
		Loops = numLoops;
		_LastFrameTime = Time.time;
	}

	private void Update()
	{
		if ((Loops == -1 || _LoopCounter < Loops) && Time.time >= _LastFrameTime + 1f / _FPS)
		{
			_LastFrameTime = Time.time;
			NextFrame();
		}
	}

	private void NextFrame()
	{
		_Sprite.spriteName = _SpriteNames[_Index];
		_Index++;
		_Index %= _SpriteNames.Length;
		if (_Index == 0)
		{
			_LoopCounter++;
		}
	}
}
