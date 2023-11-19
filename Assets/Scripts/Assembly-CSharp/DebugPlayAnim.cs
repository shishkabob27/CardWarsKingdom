using UnityEngine;

public class DebugPlayAnim : MonoBehaviour
{
	public CharAnimType animType;

	public PlayerType player;

	public UILabel buttonLabel;

	private void Start()
	{
	}

	private void OnClick()
	{
		Singleton<CharacterAnimController>.Instance.PlayHeroAnim(player, animType);
	}

	private void Update()
	{
	}
}
