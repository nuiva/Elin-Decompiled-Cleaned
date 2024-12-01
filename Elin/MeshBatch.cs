using UnityEngine;

public class MeshBatch
{
	public Matrix4x4[] matrices;

	public float[] tiles;

	public float[] colors;

	public float[] matColors;

	public float[] extras;

	public MaterialPropertyBlock mpb;

	public Material mat;

	public int size;

	public MeshBatch(MeshPass pass)
	{
		Debug.Log("#pass New Batch  " + pass.name + "/" + pass.batches.Count + "/" + pass.batchSize);
		mpb = new MaterialPropertyBlock();
		size = pass.batchSize;
		matrices = new Matrix4x4[size];
		tiles = new float[size];
		if (pass.setColor)
		{
			colors = new float[size];
		}
		if (pass.setMatColor)
		{
			matColors = new float[size];
		}
		if (pass.setExtra)
		{
			extras = new float[size];
		}
		for (int i = 0; i < size; i++)
		{
			matrices[i].SetTRS(Vector3.zero, Quaternion.Euler(0f, 0f, 0f), Vector3.one);
		}
	}
}
