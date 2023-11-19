using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimController : Singleton<CharacterAnimController>
{
	public List<Animator> playerCharacters;
	public List<BattleCharacterAnimState> playerAnimState;
	public List<GameObject> holdHandBones;
	public List<GameObject> playHandBones;
	public float cardReleaseTimePlayCard;
	public float cardReleaseTimeRareCard;
	public string playerID;
	public string opponentID;
	public GameObject HoldCardsPrefab;
	public GameObject PlayCardPrefab;
	public Material P2CardMaterial;
	public float VOProbability;
}
