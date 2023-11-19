using System;
using Boo.Lang.Runtime;
using UnityEngine;
using UnityScript.Lang;

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

	private Mesh mesh;

	private Vector3[] vertices;

	private int[] triangles;

	private Vector2[] uv;

	private Vector3[] normals;

	private Color[] vertexcolors;

	public int lastMaxTrailNodes;

	private UnityScript.Lang.Array trailNodes;

	public WeaponTrail()
	{
		time = 1f;
		minVertexDistance = 0.01f;
		maxTrailNodes = 100;
		trail_enabled = true;
		lastMaxTrailNodes = -1;
		trailNodes = new UnityScript.Lang.Array();
	}

	public override void DestroyMesh()
	{
		if ((bool)mesh)
		{
			UnityEngine.Object.DestroyImmediate(mesh);
			mesh = null;
		}
	}

	public override void LateUpdate()
	{
		if (!trail_enabled || material == null)
		{
			return;
		}
		TrailNodeInfo trailNodeInfo = null;
		float num = default(float);
		while (trailNodes.length > 0)
		{
			object obj = trailNodes[0];
			if (!(obj is TrailNodeInfo))
			{
				obj = RuntimeServices.Coerce(obj, typeof(TrailNodeInfo));
			}
			trailNodeInfo = (TrailNodeInfo)obj;
			num = Time.time - trailNodeInfo.beginTime;
			if (!(num <= time))
			{
				trailNodes.Shift();
				continue;
			}
			break;
		}
		Vector3 position = transform.position;
		Vector3 vector = transform.TransformPoint(endpointOffset);
		bool flag = true;
		if (trailNodes.length > 0)
		{
			flag = false;
			object obj2 = trailNodes[trailNodes.length - 1];
			if (!(obj2 is TrailNodeInfo))
			{
				obj2 = RuntimeServices.Coerce(obj2, typeof(TrailNodeInfo));
			}
			trailNodeInfo = (TrailNodeInfo)obj2;
			if ((trailNodeInfo.pt - position).sqrMagnitude >= minVertexDistance * minVertexDistance || !((trailNodeInfo.endpt - vector).sqrMagnitude < minVertexDistance * minVertexDistance))
			{
				flag = true;
			}
		}
		if (flag)
		{
			trailNodeInfo = new TrailNodeInfo();
			trailNodeInfo.pt = position;
			trailNodeInfo.endpt = vector;
			trailNodeInfo.beginTime = Time.time;
			trailNodes.Add(trailNodeInfo);
		}
		while (trailNodes.length > maxTrailNodes)
		{
			trailNodes.Shift();
		}
		if (trailNodes.length >= 2)
		{
			UpdateTrailMesh();
			Graphics.DrawMesh(mesh, Matrix4x4.identity, material, trail_layer);
		}
	}

	public override void UpdateTrailMesh()
	{
		if ((bool)mesh)
		{
			mesh.Clear();
		}
		int length = trailNodes.length;
		if (length < 2)
		{
			return;
		}
		if (!mesh)
		{
			mesh = new Mesh();
		}
		vertices = new Vector3[length * 2];
		triangles = new int[(length - 1) * 6];
		uv = new Vector2[length * 2];
		normals = new Vector3[length * 2];
		vertexcolors = new Color[length * 2];
		float num = 1f / (float)(length - 1);
		float num2 = 0f;
		TrailNodeInfo trailNodeInfo = null;
		int num3 = default(int);
		Color color = default(Color);
		for (num3 = 0; num3 < length - 1; num3++)
		{
			float num4 = 1f - (float)num3 * 1f / (float)(length - 1);
			object obj = trailNodes[length - num3 - 1];
			if (!(obj is TrailNodeInfo))
			{
				obj = RuntimeServices.Coerce(obj, typeof(TrailNodeInfo));
			}
			trailNodeInfo = (TrailNodeInfo)obj;
			if (endWidthScale == 1f || num4 == 1f)
			{
				ref Vector3 reference = ref vertices[num3 * 2 + 0];
				reference = trailNodeInfo.pt;
				ref Vector3 reference2 = ref vertices[num3 * 2 + 1];
				reference2 = trailNodeInfo.endpt;
			}
			else
			{
				Vector3 vector = trailNodeInfo.endpt - trailNodeInfo.pt;
				Vector3 vector2 = vector * 0.5f;
				Vector3 vector3 = trailNodeInfo.pt + vector2;
				ref Vector3 reference3 = ref vertices[num3 * 2 + 0];
				reference3 = vector3 - vector2 * num4;
				ref Vector3 reference4 = ref vertices[num3 * 2 + 1];
				reference4 = vector3 + vector2 * num4;
			}
			triangles[num3 * 6 + 0] = num3 * 2 + 0;
			triangles[num3 * 6 + 1] = num3 * 2 + 1;
			triangles[num3 * 6 + 2] = num3 * 2 + 2;
			triangles[num3 * 6 + 3] = num3 * 2 + 1;
			triangles[num3 * 6 + 4] = num3 * 2 + 3;
			triangles[num3 * 6 + 5] = num3 * 2 + 2;
			ref Vector2 reference5 = ref uv[num3 * 2 + 0];
			reference5 = new Vector2(0f, num2);
			ref Vector2 reference6 = ref uv[num3 * 2 + 1];
			reference6 = new Vector2(1f, num2);
			ref Vector3 reference7 = ref normals[num3 * 2 + 0];
			reference7 = new Vector3(0f, 0f, -1f);
			ref Vector3 reference8 = ref normals[num3 * 2 + 1];
			reference8 = new Vector3(0f, 0f, -1f);
			color = new Color(color2.r + (color1.r - color2.r) * num4, color2.g + (color1.g - color2.g) * num4, color2.b + (color1.b - color2.b) * num4, color2.a + (color1.a - color2.a) * num4);
			ref Color reference9 = ref vertexcolors[num3 * 2 + 0];
			reference9 = new Color(color.r, color.g, color.b, color.a);
			ref Color reference10 = ref vertexcolors[num3 * 2 + 1];
			reference10 = new Color(color.r, color.g, color.b, color.a);
			num2 += num;
		}
		object obj2 = trailNodes[0];
		if (!(obj2 is TrailNodeInfo))
		{
			obj2 = RuntimeServices.Coerce(obj2, typeof(TrailNodeInfo));
		}
		trailNodeInfo = (TrailNodeInfo)obj2;
		ref Vector3 reference11 = ref vertices[num3 * 2 + 0];
		reference11 = trailNodeInfo.pt;
		ref Vector3 reference12 = ref vertices[num3 * 2 + 1];
		reference12 = trailNodeInfo.endpt;
		ref Vector2 reference13 = ref uv[num3 * 2 + 0];
		reference13 = new Vector2(0f, num2);
		ref Vector2 reference14 = ref uv[num3 * 2 + 1];
		reference14 = new Vector2(1f, num2);
		ref Vector3 reference15 = ref normals[num3 * 2 + 0];
		reference15 = new Vector3(0f, 0f, -1f);
		ref Vector3 reference16 = ref normals[num3 * 2 + 1];
		reference16 = new Vector3(0f, 0f, -1f);
		ref Color reference17 = ref vertexcolors[num3 * 2 + 0];
		reference17 = new Color(color.r, color.g, color.b, 0f);
		ref Color reference18 = ref vertexcolors[num3 * 2 + 1];
		reference18 = new Color(color.r, color.g, color.b, 0f);
		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.normals = normals;
		mesh.triangles = triangles;
		mesh.colors = vertexcolors;
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
	}

	public override void Reset()
	{
		ClearTrailNodes();
	}

	public override void ClearTrailNodes()
	{
		trailNodes.Clear();
	}

	public override void OnDrawGizmosSelected()
	{
		Vector3 position = transform.position;
		Vector3 to = transform.TransformPoint(endpointOffset);
		Gizmos.DrawLine(position, to);
	}

	public override Vector3 GetBasePosition()
	{
		return transform.position;
	}

	public override Vector3 GetTipPosition()
	{
		return transform.TransformPoint(endpointOffset);
	}

	public override void Enable(bool e)
	{
		trail_enabled = e;
		if (!e)
		{
			trailNodes.Clear();
		}
	}

	public override void Main()
	{
	}
}
