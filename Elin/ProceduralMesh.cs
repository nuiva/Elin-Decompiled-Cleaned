using System;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralMesh : ScriptableObject
{
	public Mesh GetMesh()
	{
		if (!this.mesh)
		{
			this.Create();
		}
		return this.mesh;
	}

	public void Create()
	{
		this.mesh = new Mesh();
		this.triOffset = 0;
		this.triangles.Clear();
		this.vertices.Clear();
		this.uv.Clear();
		if (this.top)
		{
			this.vertices.AddRange(new Vector3[]
			{
				new Vector3(0f, this.size.y, this.size.z) + this.pos,
				new Vector3(this.size.x, this.size.y, this.size.z) + this.pos,
				new Vector3(this.size.x, this.size.y, 0f) + this.pos,
				new Vector3(0f, this.size.y, 0f) + this.pos
			});
			this.uv.AddRange(this.GetUVs(0));
			this.AddTriangles();
		}
		else
		{
			this.vertices.AddRange(new Vector3[]
			{
				new Vector3(0f, this.size.y, 0f) + this.pos,
				new Vector3(this.size.x, this.size.y, 0f) + this.pos,
				new Vector3(this.size.x, this.offset.y, 0f) + this.pos,
				new Vector3(0f, this.offset.y, 0f) + this.pos
			});
			this.uv.AddRange(this.GetUVs(0));
			this.AddTriangles();
		}
		this.mesh.SetVertices(this.vertices);
		this.mesh.SetUVs(0, this.uv);
		this.mesh.SetTriangles(this.triangles, 0);
		if (this.calculateNormal)
		{
			this.mesh.RecalculateNormals();
		}
	}

	public void AddTriangles()
	{
		this.triangles.AddRange(new int[]
		{
			this.triOffset,
			1 + this.triOffset,
			2 + this.triOffset,
			2 + this.triOffset,
			3 + this.triOffset,
			this.triOffset
		});
		this.triOffset += 4;
	}

	private Vector2[] GetUVs(int id)
	{
		Vector2[] array = new Vector2[4];
		Vector2 vector = new Vector2(1f / this.tiling.x, 1f / this.tiling.y);
		int num = id % (int)this.tiling.x;
		int num2 = id / (int)this.tiling.x;
		float num3 = this.UVPadding / this.tiling.x;
		array[0] = new Vector2((float)num / this.tiling.x + num3, 1f - (float)num2 / this.tiling.y - num3);
		array[1] = new Vector2((float)num / this.tiling.x + vector.x - num3, 1f - (float)num2 / this.tiling.y - num3);
		array[2] = new Vector2((float)num / this.tiling.x + vector.x - num3, 1f - ((float)num2 / this.tiling.y + vector.y) + num3);
		array[3] = new Vector2((float)num / this.tiling.x + num3, 1f - ((float)num2 / this.tiling.y + vector.y) + num3);
		return array;
	}

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
}
