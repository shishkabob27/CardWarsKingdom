using UnityEngine;

public class GachaEggAnimState : MonoBehaviour
{
	public Animator anim;

	private int idleIdx = Animator.StringToHash("Base Layer.LootIdle");

	private int dropIdx = Animator.StringToHash("Base Layer.LootDrop");

	private int shake1Idx = Animator.StringToHash("Base Layer.Shake1");

	private int shake2Idx = Animator.StringToHash("Base Layer.Shake2");

	private int shake3Idx = Animator.StringToHash("Base Layer.Shake3");

	private int shake4Idx = Animator.StringToHash("Base Layer.Shake4");

	private int burstIdx = Animator.StringToHash("Base Layer.Burst");

	private void Start()
	{
		anim = GetComponent<Animator>();
	}

	private void Update()
	{
		if (!(anim.runtimeAnimatorController == null))
		{
			int nameHash = anim.GetCurrentAnimatorStateInfo(0).nameHash;
			if (nameHash == dropIdx)
			{
				anim.SetBool("LootDrop", false);
			}
			if (nameHash == shake1Idx)
			{
				anim.SetBool("Shake1", false);
			}
			if (nameHash == shake2Idx)
			{
				anim.SetBool("Shake2", false);
			}
			if (nameHash == shake3Idx)
			{
				anim.SetBool("Shake3", false);
			}
			if (nameHash == shake4Idx)
			{
				anim.SetBool("Shake4", false);
			}
			if (nameHash == burstIdx)
			{
				anim.SetBool("Burst", false);
			}
		}
	}
}
