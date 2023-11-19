using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaboratorySequence : MonoBehaviour
{
	public delegate void SequencePop();

	public delegate void ElectricStart();

	public delegate void SequenceComplete();

	public UITweenController[] ModuleResetTweens;

	public UITweenController[] ModuleIntroTweens;

	public UITweenController[] ModuleTurnOnTweens;

	public UITweenController[] ModuleFlashTweens;

	public UITweenController[] ModuleOutroTweens;

	public UITweenController[] GuagesResetTweens;

	public UITweenController[] GuagesIntroTweens;

	public UITweenController[] GuagesOutroTweens;

	public UITweenController[] StarIntroTweens;

	public UITweenController[] StarTurnOnTweens;

	public UITweenController[] StarMoveDownTweens;

	public UITweenController[] StarOutroTweens;

	public UITweenController GlowIntroTween;

	public UITweenController GlowFlashTween;

	public UITweenController GlowOutroTween;

	public UITweenController TitleTween;

	public GameObject MainObject;

	public GameObject Lightning3D;

	public GameObject centerPoint;

	public GameObject[] ModuleRoots;

	public GameObject[] LightningRoots;

	public LineRenderer[] LightningBeams;

	public UITexture[] TileTextures;

	public GameObject StarHolderRoot;

	public GameObject[] StarHolderRoots;

	public GameObject[] StarTransFromPoint;

	public GameObject[] StarTransToPoint;

	public bool isAwakening;

	public int numStars;

	private int[] attachOrder;

	public int testNums = 1;

	public bool testSequence;

	public static event SequencePop onSequencePop;

	public static event ElectricStart onElectricStart;

	public static event SequenceComplete onSequenceComplete;

	private void Start()
	{
		ResetSequence();
	}

	private void Update()
	{
		if (testSequence)
		{
			testSequence = false;
			List<UITexture> list = new List<UITexture>();
			UITexture item = new UITexture();
			for (int i = 0; i < testNums; i++)
			{
				list.Add(item);
			}
			StartModuleSequence(list);
		}
	}

	public void ResetTweens()
	{
		for (int i = 0; i < ModuleResetTweens.Length; i++)
		{
			ModuleResetTweens[i].Play();
		}
		for (int j = 0; j < GuagesResetTweens.Length; j++)
		{
			GuagesResetTweens[j].Play();
		}
	}

	public void StartModuleSequence(List<UITexture> fodderTiles)
	{
		MainObject.SetActive(true);
		ResetTweens();
		Singleton<SLOTMusic>.Instance.LowerMusicVolume(true);
		Singleton<SLOTAudioManager>.Instance.PlaySound("SFX_EnhanceSeq_Slides");
		if (fodderTiles.Count == 0)
		{
			attachOrder = new int[5] { 0, 1, 2, 3, 4 };
		}
		if (fodderTiles.Count == 1)
		{
			attachOrder = new int[1] { 2 };
		}
		if (fodderTiles.Count == 2)
		{
			attachOrder = new int[2] { 1, 3 };
		}
		if (fodderTiles.Count == 3)
		{
			attachOrder = new int[3] { 1, 2, 3 };
		}
		if (fodderTiles.Count == 4)
		{
			attachOrder = new int[4] { 0, 1, 3, 4 };
		}
		if (fodderTiles.Count == 5)
		{
			attachOrder = new int[5] { 0, 1, 2, 3, 4 };
		}
		for (int i = 0; i < fodderTiles.Count; i++)
		{
			int num = attachOrder[i];
			TileTextures[num].mainTexture = fodderTiles[i].mainTexture;
			TileTextures[num].transform.localScale = Vector3.zero;
		}
		for (int j = 0; j < ModuleRoots.Length; j++)
		{
			bool active = false;
			for (int k = 0; k < attachOrder.Length; k++)
			{
				if (attachOrder[k] == j)
				{
					active = true;
				}
			}
			ModuleRoots[j].SetActive(active);
		}
		if (StarHolderRoot != null)
		{
			StarHolderRoot.SetActive(true);
			for (int l = 0; l < StarHolderRoots.Length; l++)
			{
				StarHolderRoots[l].SetActive(false);
			}
			int num2 = -50 * (numStars - 1);
			int num3 = 50 * (numStars - 1);
			for (int m = 0; m < numStars; m++)
			{
				int num4 = m;
				if (m == numStars - 1)
				{
					num4 = StarHolderRoots.Length - 1;
				}
				int num5 = numStars - 1;
				if (num5 < 1)
				{
					num5 = 1;
				}
				float t = (float)m * 1f / ((float)num5 * 1f);
				StarHolderRoots[num4].transform.localPosition = new Vector3(Mathf.Lerp(num2, num3, t), 0f, 0f);
				StarTransFromPoint[num4].transform.localPosition = StarHolderRoots[num4].transform.localPosition;
				StarTransFromPoint[num4].transform.localPosition += new Vector3(StarTransFromPoint[num4].transform.localPosition.x * 1.1f, -500f, 0f);
				StarTransToPoint[num4].transform.localPosition = StarHolderRoots[num4].transform.localPosition;
			}
		}
		Singleton<SLOTAudioManager>.Instance.PlaySound("SFX_EnhanceSeq_TurnOn");
		StartCoroutine(ModuleSequenceCo());
	}

	public IEnumerator ModuleSequenceCo()
	{
		UICamera.LockInput();
		yield return new WaitForSeconds(0.5f);
		for (int i5 = 0; i5 < attachOrder.Length; i5++)
		{
			int ii = attachOrder[i5];
			Singleton<SLOTAudioManager>.Instance.PlaySound("SFX_EnhanceSeq_Popup");
			ModuleIntroTweens[ii].Play();
			yield return new WaitForSeconds(0.15f);
		}
		if (StarHolderRoot != null)
		{
			for (int i4 = 0; i4 < numStars; i4++)
			{
				int s4 = i4;
				if (i4 == numStars - 1)
				{
					s4 = StarHolderRoots.Length - 1;
				}
				StarIntroTweens[s4].Play();
				yield return new WaitForSeconds(0.1f);
			}
		}
		for (int i3 = 0; i3 < GuagesIntroTweens.Length; i3++)
		{
			GuagesIntroTweens[i3].Play();
		}
		Singleton<SLOTAudioManager>.Instance.PlaySound("SFX_EnhanceSeq_Charge");
		yield return new WaitForSeconds(0.2f);
		GlowIntroTween.Play();
		yield return new WaitForSeconds(0.2f);
		if (LaboratorySequence.onElectricStart != null)
		{
			LaboratorySequence.onElectricStart();
		}
		LaboratorySequence.onElectricStart = null;
		float moduleResponsibility = 0f;
		float responsibilityMet = 0f;
		int starsTurnedOn = 0;
		if (StarHolderRoot != null)
		{
			moduleResponsibility = (float)numStars * 1f / ((float)attachOrder.Length * 1f);
		}
		Lightning3D.SetActive(true);
		for (int i2 = 0; i2 < LightningBeams.Length; i2++)
		{
			LightningBeams[i2].gameObject.SetActive(false);
		}
		int lastStarIndex = 0;
		StartCoroutine(ScrollLightningMats());
		for (int n = 0; n < attachOrder.Length; n++)
		{
			int ii2 = attachOrder[n];
			Singleton<SLOTAudioManager>.Instance.PlaySound("SFX_EnhanceSeq_TVOn");
			ModuleTurnOnTweens[ii2].Play();
			if (StarHolderRoot != null)
			{
				for (int r = 0; (float)r < Mathf.Ceil(moduleResponsibility); r++)
				{
					if (responsibilityMet + moduleResponsibility >= (float)starsTurnedOn && starsTurnedOn < numStars)
					{
						int s = starsTurnedOn;
						if (s == numStars - 1)
						{
							s = StarTurnOnTweens.Length - 1;
						}
						lastStarIndex = s;
						StarTurnOnTweens[s].Play();
						starsTurnedOn++;
						if (LightningBeams[ii2] != null)
						{
							Vector3 startPos4 = LightningRoots[ii2].transform.position;
							Vector3 endPos4 = StarHolderRoots[s].transform.position;
							LightningBeams[ii2].SetPosition(0, startPos4);
							LightningBeams[ii2].SetPosition(1, endPos4);
							LightningBeams[ii2].gameObject.SetActive(true);
							ScaleTextureBasedOnDistance(LightningBeams[ii2], startPos4, endPos4, 1f);
						}
					}
					else if (LightningBeams[ii2] != null)
					{
						Vector3 startPos3 = LightningRoots[ii2].transform.position;
						Vector3 endPos3 = StarHolderRoots[lastStarIndex].transform.position;
						LightningBeams[ii2].SetPosition(0, startPos3);
						LightningBeams[ii2].SetPosition(1, endPos3);
						LightningBeams[ii2].gameObject.SetActive(true);
						ScaleTextureBasedOnDistance(LightningBeams[ii2], startPos3, endPos3, 1f);
					}
				}
				responsibilityMet += moduleResponsibility;
			}
			else if (isAwakening)
			{
				if (LightningBeams[ii2] != null && ii2 < LightningRoots.Length - 1)
				{
					Vector3 startPos2 = LightningRoots[ii2].transform.position;
					Vector3 endPos2 = LightningRoots[ii2 + 1].transform.position;
					Vector3 mid1Pos2 = Vector3.Lerp(startPos2, endPos2, 0.333f);
					Vector3 mid2Pos2 = Vector3.Lerp(startPos2, endPos2, 0.666f);
					mid1Pos2 = Vector3.Lerp(mid1Pos2, centerPoint.transform.position, 0.1f);
					mid2Pos2 = Vector3.Lerp(mid2Pos2, centerPoint.transform.position, 0.1f);
					LightningBeams[ii2].SetPosition(0, startPos2);
					LightningBeams[ii2].SetPosition(1, mid1Pos2);
					LightningBeams[ii2].SetPosition(2, mid2Pos2);
					LightningBeams[ii2].SetPosition(3, endPos2);
					LightningBeams[ii2].gameObject.SetActive(true);
					ScaleTextureBasedOnDistance(LightningBeams[ii2], startPos2, endPos2, 3f);
				}
			}
			else if (LightningBeams[ii2] != null)
			{
				Vector3 startPos = LightningRoots[ii2].transform.position;
				Vector3 endPos = centerPoint.transform.position;
				LightningBeams[ii2].SetPosition(0, startPos);
				LightningBeams[ii2].SetPosition(1, endPos);
				LightningBeams[ii2].gameObject.SetActive(true);
				ScaleTextureBasedOnDistance(LightningBeams[ii2], startPos, endPos, 1f);
			}
			yield return new WaitForSeconds(0.3f);
		}
		yield return new WaitForSeconds(0.2f);
		for (int m = 0; m < attachOrder.Length; m++)
		{
			int ii3 = attachOrder[m];
			Singleton<SLOTAudioManager>.Instance.PlaySound("SFX_EnhanceSeq_Siren");
			if (ModuleFlashTweens[ii3] != null)
			{
				ModuleFlashTweens[ii3].Play();
			}
			yield return new WaitForSeconds(0.02f);
		}
		yield return new WaitForSeconds(0.1f);
		for (int l = 0; l < attachOrder.Length; l++)
		{
			int ii4 = attachOrder[l];
			Singleton<SLOTAudioManager>.Instance.PlaySound("SFX_EnhanceSeq_TVOff");
			ModuleOutroTweens[ii4].Play();
			LightningBeams[ii4].gameObject.SetActive(false);
			yield return new WaitForSeconds(0.1f);
		}
		if (StarHolderRoot != null)
		{
			for (int k = 0; k < numStars; k++)
			{
				int s3 = k;
				if (k == numStars - 1)
				{
					s3 = StarHolderRoots.Length - 1;
				}
				StarTransFromPoint[s3].transform.localPosition = StarHolderRoots[s3].transform.localPosition;
				StarTransToPoint[s3].transform.localPosition = StarHolderRoots[s3].transform.localPosition;
				StarTransToPoint[s3].transform.localPosition = new Vector3(StarTransToPoint[s3].transform.localPosition.x * 1.2f, -100f, 0f);
				StarMoveDownTweens[s3].Play();
				yield return new WaitForSeconds(0.15f);
			}
		}
		for (int j = 0; j < GuagesOutroTweens.Length; j++)
		{
			GuagesOutroTweens[j].Play();
		}
		Singleton<SLOTMusic>.Instance.LowerMusicVolume(false);
		if (LaboratorySequence.onSequencePop != null)
		{
			LaboratorySequence.onSequencePop();
		}
		LaboratorySequence.onSequencePop = null;
		Singleton<SLOTAudioManager>.Instance.PlaySound("SFX_EnhanceSeq_Finish");
		yield return new WaitForSeconds(0.75f);
		GlowFlashTween.Play();
		TitleTween.Play();
		yield return new WaitForSeconds(1f);
		GlowOutroTween.Play();
		yield return new WaitForSeconds(2f);
		if (StarHolderRoot != null)
		{
			for (int i = 0; i < numStars; i++)
			{
				int s2 = i;
				if (i == numStars - 1)
				{
					s2 = StarHolderRoots.Length - 1;
				}
				StarTransFromPoint[s2].transform.localPosition = StarHolderRoots[s2].transform.localPosition;
				StarTransToPoint[s2].transform.localPosition = new Vector3(StarHolderRoots[s2].transform.localPosition.x * 2f, -600f, 0f);
				StarOutroTweens[s2].Play();
				yield return new WaitForSeconds(0.05f);
			}
		}
		yield return new WaitForSeconds(0.5f);
		ResetSequence();
		if (LaboratorySequence.onSequenceComplete != null)
		{
			LaboratorySequence.onSequenceComplete();
		}
		LaboratorySequence.onSequenceComplete = null;
	}

	public void ResetSequence()
	{
		if (MainObject.activeInHierarchy)
		{
			ResetTweens();
		}
		MainObject.SetActive(false);
		Lightning3D.SetActive(false);
	}

	private void ScaleTextureBasedOnDistance(LineRenderer thisLine, Vector3 startPos, Vector3 endPos, float numSegments = 1f)
	{
		float num = Mathf.Sqrt(Mathf.Pow(endPos.x - startPos.x, 2f) + Mathf.Pow(endPos.y - startPos.y, 2f)) * numSegments;
		thisLine.material.SetTextureScale("_MainTex", new Vector2(num * 0.5f, 1f));
	}

	private IEnumerator ScrollLightningMats()
	{
		float scrollSpeed = -0.75f;
		float amtScrolled2 = 0f;
		while (Lightning3D.gameObject.activeInHierarchy)
		{
			for (int i = 0; i < LightningBeams.Length; i++)
			{
				amtScrolled2 = Time.time * scrollSpeed + 0.2f * (float)i;
				LightningBeams[i].material.SetTextureOffset("_MainTex", new Vector2(amtScrolled2, 0f));
			}
			yield return 0;
		}
	}
}
