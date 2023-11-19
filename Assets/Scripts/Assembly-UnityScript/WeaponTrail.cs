// WeaponTrail
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

	public void DestroyMesh()
	{
		if ((bool)mesh)
		{
			UnityEngine.Object.DestroyImmediate(mesh);
			mesh = null;
		}
	}

	public void LateUpdate()
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

	public void UpdateTrailMesh()
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
				vertices[num3 * 2 + 0] = trailNodeInfo.pt;
				vertices[num3 * 2 + 1] = trailNodeInfo.endpt;
			}
			else
			{
				Vector3 vector = trailNodeInfo.endpt - trailNodeInfo.pt;
				Vector3 vector2 = vector * 0.5f;
				Vector3 vector3 = trailNodeInfo.pt + vector2;
				vertices[num3 * 2 + 0] = vector3 - vector2 * num4;
				vertices[num3 * 2 + 1] = vector3 + vector2 * num4;
			}
			triangles[num3 * 6 + 0] = num3 * 2 + 0;
			triangles[num3 * 6 + 1] = num3 * 2 + 1;
			triangles[num3 * 6 + 2] = num3 * 2 + 2;
			triangles[num3 * 6 + 3] = num3 * 2 + 1;
			triangles[num3 * 6 + 4] = num3 * 2 + 3;
			triangles[num3 * 6 + 5] = num3 * 2 + 2;
			uv[num3 * 2 + 0] = new Vector2(0f, num2);
			uv[num3 * 2 + 1] = new Vector2(1f, num2);
			normals[num3 * 2 + 0] = new Vector3(0f, 0f, -1f);
			normals[num3 * 2 + 1] = new Vector3(0f, 0f, -1f);
			color = new Color(color2.r + (color1.r - color2.r) * num4, color2.g + (color1.g - color2.g) * num4, color2.b + (color1.b - color2.b) * num4, color2.a + (color1.a - color2.a) * num4);
			vertexcolors[num3 * 2 + 0] = new Color(color.r, color.g, color.b, color.a);
			vertexcolors[num3 * 2 + 1] = new Color(color.r, color.g, color.b, color.a);
			num2 += num;
		}
		object obj2 = trailNodes[0];
		if (!(obj2 is TrailNodeInfo))
		{
			obj2 = RuntimeServices.Coerce(obj2, typeof(TrailNodeInfo));
		}
		trailNodeInfo = (TrailNodeInfo)obj2;
		vertices[num3 * 2 + 0] = trailNodeInfo.pt;
		vertices[num3 * 2 + 1] = trailNodeInfo.endpt;
		uv[num3 * 2 + 0] = new Vector2(0f, num2);
		uv[num3 * 2 + 1] = new Vector2(1f, num2);
		normals[num3 * 2 + 0] = new Vector3(0f, 0f, -1f);
		normals[num3 * 2 + 1] = new Vector3(0f, 0f, -1f);
		vertexcolors[num3 * 2 + 0] = new Color(color.r, color.g, color.b, 0f);
		vertexcolors[num3 * 2 + 1] = new Color(color.r, color.g, color.b, 0f);
		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.normals = normals;
		mesh.triangles = triangles;
		mesh.colors = vertexcolors;
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
	}

	public void Reset()
	{
		ClearTrailNodes();
	}

	public void ClearTrailNodes()
	{
		trailNodes.Clear();
	}

	public void OnDrawGizmosSelected()
	{
		Vector3 position = transform.position;
		Vector3 to = transform.TransformPoint(endpointOffset);
		Gizmos.DrawLine(position, to);
	}

	public Vector3 GetBasePosition()
	{
		return transform.position;
	}

	public Vector3 GetTipPosition()
	{
		return transform.TransformPoint(endpointOffset);
	}

	public void Enable(bool e)
	{
		trail_enabled = e;
		if (!e)
		{
			trailNodes.Clear();
		}
	}

	public void Main()
	{
	}
}
