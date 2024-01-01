using System;
using System.Collections.Generic;
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

	private Mesh mesh;

	private Vector3[] vertices;

	private int[] triangles;

	private Vector2[] uv;

	private Vector3[] normals;

	private Color[] vertexcolors;

	public int lastMaxTrailNodes;

    private List<TrailNodeInfo> trailNodes = new List<TrailNodeInfo>();

	public WeaponTrail()
	{
		time = 1f;
		minVertexDistance = 0.01f;
		maxTrailNodes = 100;
		trail_enabled = true;
		lastMaxTrailNodes = -1;
	}

	public void DestroyMesh()
	{
		if ((bool)mesh)
		{
			UnityEngine.Object.DestroyImmediate(mesh);
			mesh = null;
		}
	}

    private void LateUpdate()
    {
        if (!trail_enabled || material == null)
            return;

        TrailNodeInfo trailNodeInfo = null;
        float num = 0f;

        while (trailNodes.Count > 0)
        {
            trailNodeInfo = trailNodes[0];
            num = Time.time - trailNodeInfo.beginTime;

            if (!(num <= time))
            {
                trailNodes.RemoveAt(0);
                continue;
            }
            break;
        }

        Vector3 position = transform.position;
        Vector3 vector = transform.TransformPoint(endpointOffset);
        bool flag = true;

        if (trailNodes.Count > 0)
        {
            flag = false;
            trailNodeInfo = trailNodes[trailNodes.Count - 1];

            if ((trailNodeInfo.pt - position).sqrMagnitude >= minVertexDistance * minVertexDistance || !((trailNodeInfo.endpt - vector).sqrMagnitude < minVertexDistance * minVertexDistance))
            {
                flag = true;
            }
        }

        if (flag)
        {
            trailNodeInfo = new TrailNodeInfo
            {
                pt = position,
                endpt = vector,
                beginTime = Time.time
            };
            trailNodes.Add(trailNodeInfo);
        }

        while (trailNodes.Count > maxTrailNodes)
        {
            trailNodes.RemoveAt(0);
        }

        if (trailNodes.Count >= 2)
        {
            UpdateTrailMesh();
            Graphics.DrawMesh(mesh, Matrix4x4.identity, material, trail_layer);
        }
    }

    private void UpdateTrailMesh()
    {
        if (mesh != null)
        {
            mesh.Clear();
        }

        int length = trailNodes.Count;

        if (length < 2)
        {
            return;
        }

        if (mesh == null)
        {
            mesh = new Mesh();
        }

        vertices = new Vector3[length * 2];
        triangles = new int[(length - 1) * 6];
        uv = new Vector2[length * 2];
        normals = new Vector3[length * 2];
        vertexcolors = new Color[length * 2];

        float interval = 1f / (float)(length - 1);
        float uvOffset = 0f;

        for (int i = 0; i < length - 1; i++)
        {
            float lerpValue = 1f - (float)i * interval;
            TrailNodeInfo currentNodeInfo = trailNodes[length - i - 1];

            Vector3 midPoint = Vector3.Lerp(currentNodeInfo.pt, currentNodeInfo.endpt, 0.5f);
            Vector3 scaledVector = (currentNodeInfo.endpt - currentNodeInfo.pt) * 0.5f;

            vertices[i * 2] = midPoint - scaledVector * lerpValue;
            vertices[i * 2 + 1] = midPoint + scaledVector * lerpValue;

            triangles[i * 6] = i * 2;
            triangles[i * 6 + 1] = i * 2 + 1;
            triangles[i * 6 + 2] = i * 2 + 2;
            triangles[i * 6 + 3] = i * 2 + 1;
            triangles[i * 6 + 4] = i * 2 + 3;
            triangles[i * 6 + 5] = i * 2 + 2;

            uv[i * 2] = new Vector2(0f, uvOffset);
            uv[i * 2 + 1] = new Vector2(1f, uvOffset);

            normals[i * 2] = new Vector3(0f, 0f, -1f);
            normals[i * 2 + 1] = new Vector3(0f, 0f, -1f);

            Color interpolatedColor = Color.Lerp(color2, color1, lerpValue);
            vertexcolors[i * 2] = interpolatedColor;
            vertexcolors[i * 2 + 1] = interpolatedColor;

            uvOffset += interval;
        }

        TrailNodeInfo firstNodeInfo = trailNodes[0];
        vertices[(length - 1) * 2] = firstNodeInfo.pt;
        vertices[(length - 1) * 2 + 1] = firstNodeInfo.endpt;

        uv[(length - 1) * 2] = new Vector2(0f, uvOffset);
        uv[(length - 1) * 2 + 1] = new Vector2(1f, uvOffset);

        normals[(length - 1) * 2] = new Vector3(0f, 0f, -1f);
        normals[(length - 1) * 2 + 1] = new Vector3(0f, 0f, -1f);

        Color lastNodeColor = new Color(color2.r, color2.g, color2.b, 0f);
        vertexcolors[(length - 1) * 2] = lastNodeColor;
        vertexcolors[(length - 1) * 2 + 1] = lastNodeColor;

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
