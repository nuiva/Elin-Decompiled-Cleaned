using System;
using UnityEngine;

public class MeshBatch
{
	public MeshBatch(MeshPass pass)
	{
		Debug.Log(string.Concat(new string[]
		{
			"#pass New Batch  ",
			pass.name,
			"/",
			pass.batches.Count.ToString(),
			"/",
			pass.batchSize.ToString()
		}));
		this.mpb = new MaterialPropertyBlock();
		this.size = pass.batchSize;
		this.matrices = new Matrix4x4[this.size];
		this.tiles = new float[this.size];
		if (pass.setColor)
		{
			this.colors = new float[this.size];
		}
		if (pass.setMatColor)
		{
			this.matColors = new float[this.size];
		}
		if (pass.setExtra)
		{
			this.extras = new float[this.size];
		}
		for (int i = 0; i < this.size; i++)
		{
			this.matrices[i].SetTRS(Vector3.zero, Quaternion.Euler(0f, 0f, 0f), Vector3.one);
		}
	}

	public Matrix4x4[] matrices;

	public float[] tiles;

	public float[] colors;

	public float[] matColors;

	public float[] extras;

	public MaterialPropertyBlock mpb;

	public Material mat;

	public int size;
}
