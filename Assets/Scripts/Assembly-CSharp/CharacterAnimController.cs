using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimController : Singleton<CharacterAnimController>
{
	public List<Animator> playerCharacters = new List<Animator>();

	public List<BattleCharacterAnimState> playerAnimState = new List<BattleCharacterAnimState>();

	public LeaderData[] playerData = new LeaderData[2];

	public List<GameObject> holdHandBones = new List<GameObject>();

	public List<GameObject> playHandBones = new List<GameObject>();

	public float cardReleaseTimePlayCard;

	public float cardReleaseTimeRareCard;

	public string playerID;

	public string opponentID;

	private CharAnimType[] HitReactionTriggers = new CharAnimType[3];

	public GameObject HoldCardsPrefab;

	public GameObject PlayCardPrefab;

	private GameObject[] mP1HoldCards = new GameObject[10];

	private GameObject[] mP2HoldCards = new GameObject[10];

	public Material P2CardMaterial;

	private int[] mNumHandCards = new int[2];

	public float VOProbability = 0.5f;

	public int GetNumberOfHands(int player)
	{
		return mNumHandCards[player];
	}

	public void AddPlayer(Animator anim)
	{
		playerCharacters.Add(anim);
		playerAnimState.Add(anim.GetComponent<BattleCharacterAnimState>());
	}

	public void ResetCharacterAnimState()
	{
		foreach (Animator playerCharacter in playerCharacters)
		{
			BattleCharacterAnimState componentInChildren = playerCharacter.GetComponentInChildren<BattleCharacterAnimState>();
			componentInChildren.Reset();
		}
	}

	public void StopCharacterFidgets(bool stop = true)
	{
		foreach (Animator playerCharacter in playerCharacters)
		{
			BattleCharacterAnimState componentInChildren = playerCharacter.GetComponentInChildren<BattleCharacterAnimState>();
			componentInChildren.StopFidget(stop);
		}
	}

	private void OnEnable()
	{
		SetupCharacters();
		HitReactionTriggers[0] = CharAnimType.HR1;
		HitReactionTriggers[1] = CharAnimType.HR2;
	}

	private void Start()
	{
	}

	public void SetupCharacters()
	{
		if (playerCharacters.Count == 0)
		{
			return;
		}
		for (int i = 0; i < playerCharacters.Count; i++)
		{
			string text = null;
			if ((PlayerType)i == PlayerType.User)
			{
				if (Singleton<PlayerInfoScript>.Instance.SaveData.SelectedCardBack != CardBackDataManager.DefaultData)
				{
					text = Singleton<PlayerInfoScript>.Instance.SaveData.SelectedCardBack.Texture3D;
				}
			}
			else if (Singleton<PlayerInfoScript>.Instance.StateData.MultiplayerMode && Singleton<PlayerInfoScript>.Instance.PvPData.OpponentCardBack != CardBackDataManager.DefaultData)
			{
				text = Singleton<PlayerInfoScript>.Instance.PvPData.OpponentCardBack.Texture3D;
			}
			Texture2D cardTextureSwap = null;
			if (text != null)
			{
				cardTextureSwap = Singleton<SLOTResourceManager>.Instance.LoadResource(text) as Texture2D;
			}
			GameObject gameObject = FindInChildren(playerCharacters[i].gameObject, "Puppet_CardPlay");
			playHandBones.Add(gameObject);
			SpawnCardObjects(i, PlayCardPrefab, gameObject, cardTextureSwap);
			GameObject gameObject2 = FindInChildren(playerCharacters[i].gameObject, "Puppet_CardsHand");
			holdHandBones.Add(gameObject2);
			SpawnCardObjects(i, HoldCardsPrefab, gameObject2, cardTextureSwap);
			GameObject gameObject3 = FindInChildren(playerCharacters[i].gameObject, "M_MOUTH");
			if (gameObject3 != null)
			{
				gameObject3.GetComponent<Renderer>().material.mainTextureScale = new Vector2(0.25f, 0.25f);
			}
		}
		Init3DHoldCards(PlayerType.User);
		Init3DHoldCards(PlayerType.Opponent);
	}

	private void SpawnCardObjects(PlayerType player, GameObject prefab, GameObject parentBone, Texture2D cardTextureSwap)
	{
		int layer = LayerMask.NameToLayer((player != PlayerType.User) ? "PIP2" : "PIP");
		GameObject gameObject = parentBone.transform.InstantiateAsChild(prefab);
		gameObject.transform.localRotation = ((!(prefab == HoldCardsPrefab)) ? Quaternion.identity : Quaternion.Euler(new Vector3(270f, 0f, 0f)));
		gameObject.ChangeLayer(layer);
		if (prefab == HoldCardsPrefab)
		{
			GameObject[] array = ((player != PlayerType.User) ? mP2HoldCards : mP1HoldCards);
			for (int i = 0; i < array.Length; i++)
			{
				string childName = "Card_" + i;
				GameObject gameObject2 = FindInChildren(gameObject, childName);
				if (!(gameObject2 != null))
				{
					continue;
				}
				array[i] = gameObject2.gameObject;
				if (cardTextureSwap != null)
				{
					MeshRenderer[] components = gameObject2.GetComponents<MeshRenderer>();
					MeshRenderer[] array2 = components;
					foreach (MeshRenderer meshRenderer in array2)
					{
						meshRenderer.material.mainTexture = cardTextureSwap;
					}
				}
			}
		}
		else if (cardTextureSwap != null)
		{
			GameObject gameObject3 = FindInChildren(gameObject, "Card_Play");
			MeshRenderer[] components2 = gameObject3.GetComponents<MeshRenderer>();
			MeshRenderer[] array3 = components2;
			foreach (MeshRenderer meshRenderer2 in array3)
			{
				meshRenderer2.material.mainTexture = cardTextureSwap;
			}
		}
	}

	public void HideHandCards(bool hide)
	{
		foreach (GameObject holdHandBone in holdHandBones)
		{
			holdHandBone.SetActive(!hide);
		}
	}

	private GameObject FindInChildren(GameObject parentObj, string childName)
	{
		Transform[] componentsInChildren = parentObj.GetComponentsInChildren<Transform>();
		Transform[] array = componentsInChildren;
		foreach (Transform transform in array)
		{
			if (transform.name == childName)
			{
				return transform.gameObject;
			}
		}
		return null;
	}

	public void ForceIdleForBoth()
	{
		ForceIdle(PlayerType.User);
		ForceIdle(PlayerType.Opponent);
	}

	public void ForceIdle(PlayerType player)
	{
		PlayHeroAnim(player, CharAnimType.Idle);
	}

	public void PlayHeroAnim(PlayerType player, CharAnimType animType)
	{
		playerCharacters[player.IntValue].SetTrigger(animType.ToString());
		LeaderItem leader = Singleton<DWGame>.Instance.GetLeader(player);
		BattleCharacterAnimState component = playerCharacters[player.IntValue].GetComponent<BattleCharacterAnimState>();
		component.PlayHeroVFX(animType);
		PlayFaceAnimation(player, animType.ToString());
	}

	private IEnumerator DelayReaction(PlayerType player, CharAnimType animType)
	{
		yield return new WaitForSeconds(0.5f);
		PlayHeroAnim(player, animType);
	}

	private void PlayFaceAnimation(PlayerType player, string faceAnimBaseName)
	{
		BattleCharacterAnimState component = playerCharacters[player.IntValue].GetComponent<BattleCharacterAnimState>();
		component.PlayFaceAnimation(faceAnimBaseName);
	}

	public void PlayRandomHitReaction(PlayerType player, int n)
	{
		PlayHeroAnim(player, HitReactionTriggers[n]);
	}

	private void Update()
	{
	}

	public Vector3 GetOpponentHandPosition()
	{
		return playHandBones[PlayerType.Opponent].transform.position;
	}

	public bool IsOpponentPlayingBigCard()
	{
		return playerAnimState[PlayerType.Opponent].PlayingBigCard();
	}

	public void Init3DHoldCards(PlayerType player)
	{
		GameObject[] array = ((player != PlayerType.User) ? mP2HoldCards : mP1HoldCards);
		mNumHandCards[player.IntValue] = Singleton<DWGame>.Instance.GetHand(player).Count;
		Update3DHoldCards(player);
	}

	public void Reset3DHoldCards(PlayerType player)
	{
		mNumHandCards[player.IntValue] = 0;
		Update3DHoldCards(player);
	}

	public void Add3DHoldCard(PlayerType player)
	{
		mNumHandCards[player.IntValue]++;
		Update3DHoldCards(player);
	}

	public void Remove3DHoldCard(PlayerType player)
	{
		mNumHandCards[player.IntValue]--;
		Update3DHoldCards(player);
	}

	private void Update3DHoldCards(PlayerType player)
	{
		GameObject[] array = ((player != PlayerType.User) ? mP2HoldCards : mP1HoldCards);
		for (int i = 0; i < 10; i++)
		{
			array[i].SetActive(i < mNumHandCards[player.IntValue]);
		}
	}

	public void TriggerHeroThinking2(PlayerType player)
	{
		if (Random.Range(0, 2) == 0)
		{
			PlayHeroAnim(player, CharAnimType.Fidget_Think);
		}
		else
		{
			PlayHeroAnim(player, CharAnimType.Fidget_Think2);
		}
	}
}
