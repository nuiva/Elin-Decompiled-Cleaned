using System.Collections.Generic;
using UnityEngine;

public class ProceduralMesh : ScriptableObject
{
	public Vector2 tiling = Vector2.one;

	public float UVPadding = 0.02f;

	public Vector3 pos = new Vector3(0f, 0f, -1f);

	public Vector3 offset;

	public Vector3 size = Vector3.one;

	public bool top;

	public bool calculateNormal = true;

	private Mesh mesh;

	private List<int> triangles = new List<int>();

	private List<Vector3> vertices = new List<Vector3>();

	private List<Vector2> uv = new List<Vector2>();

	private int triOffset;

	public Mesh GetMesh()
	{
		if (!mesh)
		{
			Create();
		}
		return mesh;
	}

	public void Create()
	{
		mesh = new Mesh();
		triOffset = 0;
		triangles.Clear();
		vertices.Clear();
		uv.Clear();
		if (top)
		{
			vertices.AddRange(new Vector3[4]
			{
				new Vector3(0f, size.y, size.z) + pos,
				new Vector3(size.x, size.y, size.z) + pos,
				new Vector3(size.x, size.y, 0f) + pos,
				new Vector3(0f, size.y, 0f) + pos
			});
			uv.AddRange(GetUVs(0));
			AddTriangles();
		}
		else
		{
			vertices.AddRange(new Vector3[4]
			{
				new Vector3(0f, size.y, 0f) + pos,
				new Vector3(size.x, size.y, 0f) + pos,
				new Vector3(size.x, offset.y, 0f) + pos,
				new Vector3(0f, offset.y, 0f) + pos
			});
			uv.AddRange(GetUVs(0));
			AddTriangles();
		}
		mesh.SetVertices(vertices);
		mesh.SetUVs(0, uv);
		mesh.SetTriangles(triangles, 0);
		if (calculateNormal)
		{
			mesh.RecalculateNormals();
		}
	}

	public void AddTriangles()
	{
		triangles.AddRange(new int[6]
		{
			triOffset,
			1 + triOffset,
			2 + triOffset,
			2 + triOffset,
			3 + triOffset,
			triOffset
		});
		triOffset += 4;
	}

	private Vector2[] GetUVs(int id)
	{
		Vector2[] array = new Vector2[4];
		Vector2 vector = new Vector2(1f / tiling.x, 1f / tiling.y);
		int num = id % (int)tiling.x;
		int num2 = id / (int)tiling.x;
		float num3 = UVPadding / tiling.x;
		array[0] = new Vector2((float)num / tiling.x + num3, 1f - (float)num2 / tiling.y - num3);
		array[1] = new Vector2((float)num / tiling.x + vector.x - num3, 1f - (float)num2 / tiling.y - num3);
		array[2] = new Vector2((float)num / tiling.x + vector.x - num3, 1f - ((float)num2 / tiling.y + vector.y) + num3);
		array[3] = new Vector2((float)num / tiling.x + num3, 1f - ((float)num2 / tiling.y + vector.y) + num3);
		return array;
	}
}
