using System;
using UnityEngine;

[Serializable]
public class WeaponTrail : MonoBehaviour
{
	public Vector3 endpointOffset;
	public float time;
	public Color color1;
	public Color color2;
	public Material material;
	public float endWidthScale;
	public float minVertexDistance;
	public int maxTrailNodes;
	public bool trail_enabled;
	public int trail_layer;
	public int lastMaxTrailNodes;
}
