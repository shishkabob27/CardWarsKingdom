using UnityEngine;

public class BattleTombstoneAnimState : MonoBehaviour
{
	public Animator anim;

	private int p1TombIdx = Animator.StringToHash("Base Layer.P1Tombstone");

	private int p2TombIdx = Animator.StringToHash("Base Layer.P2Tombstone");

	private void Start()
	{
		anim = GetComponent<Animator>();
	}

	private void Update()
	{
		if (!(anim.runtimeAnimatorController == null))
		{
			int nameHash = anim.GetCurrentAnimatorStateInfo(0).nameHash;
			if (nameHash == p1TombIdx)
			{
				anim.SetBool("P1Tombstone", false);
			}
			if (nameHash == p2TombIdx)
			{
				anim.SetBool("P2Tombstone", false);
			}
		}
	}
}
