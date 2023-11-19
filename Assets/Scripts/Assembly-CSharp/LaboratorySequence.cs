using UnityEngine;

public class LaboratorySequence : MonoBehaviour
{
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
	public int testNums;
	public bool testSequence;
}
