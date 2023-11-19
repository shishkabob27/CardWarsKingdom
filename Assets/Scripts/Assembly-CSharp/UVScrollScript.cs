using UnityEngine;

public class UVScrollScript : MonoBehaviour
{
	public int materialIndex;

	public Vector2 uvAnimationRate = new Vector2(0f, 1f);

	public string textureName = "_MainTex";

	private Vector2 uvOffset = Vector2.zero;

	private void LateUpdate()
	{
		uvOffset += uvAnimationRate * Time.deltaTime;
		if (Mathf.Abs(uvOffset.x) >= 10f || Mathf.Abs(uvOffset.y) >= 10f)
		{
			uvOffset = Vector2.zero;
		}
		if (GetComponent<Renderer>().enabled)
		{
			GetComponent<Renderer>().materials[materialIndex].SetTextureOffset(textureName, uvOffset);
		}
	}
}
