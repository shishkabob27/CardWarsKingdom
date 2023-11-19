using UnityEngine;

public class BattleLootDropAnimState : MonoBehaviour
{
	public Animator anim;

	private int idleIdx = Animator.StringToHash("Base Layer.LootIdle");

	private int dropIdx = Animator.StringToHash("Base Layer.LootDrop");

	private int flyIdx = Animator.StringToHash("Base Layer.LootFly");

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
			if (nameHash == flyIdx)
			{
				anim.SetBool("LootFly", false);
			}
		}
	}
}
