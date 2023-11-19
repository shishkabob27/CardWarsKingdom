using UnityEngine;

[RequireComponent(typeof(UITexture))]
public class UITextureScroller : MonoBehaviour
{
	public Vector2 InitialDelay;

	public Vector2 LoopDelay;

	public float ScrollSpeed;

	private bool Vertical;

	private UITexture mTexture;

	private float mDelayTimer;

	private void Awake()
	{
		mTexture = GetComponent<UITexture>();
	}

	private void OnEnable()
	{
		mDelayTimer = Random.Range(InitialDelay.x, InitialDelay.y);
		Rect uvRect = mTexture.uvRect;
		uvRect.x = 1f;
		uvRect.y = 0f;
		mTexture.uvRect = uvRect;
	}

	private void Update()
	{
		if (mDelayTimer > 0f)
		{
			mDelayTimer -= Time.deltaTime;
			return;
		}
		Rect uvRect = mTexture.uvRect;
		if (Vertical)
		{
			uvRect.y -= ScrollSpeed * Time.deltaTime;
			if (Mathf.Abs(uvRect.y) >= 1f)
			{
				uvRect.y = 0f;
				mDelayTimer = Random.Range(LoopDelay.x, LoopDelay.y);
			}
		}
		else
		{
			uvRect.x -= ScrollSpeed * Time.deltaTime;
			if (Mathf.Abs(uvRect.x) >= 1f)
			{
				uvRect.x = 1f;
				mDelayTimer = Random.Range(LoopDelay.x, LoopDelay.y);
			}
		}
		mTexture.uvRect = uvRect;
	}
}
