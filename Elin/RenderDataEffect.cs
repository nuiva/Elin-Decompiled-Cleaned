using UnityEngine;

public class RenderDataEffect : RenderData
{
	public float speed;

	public new void Draw(RenderParam p, int tile = 0)
	{
		float num = (Time.realtimeSinceStartup * speed + p.x + p.y) % 5f;
		MeshBatch meshBatch = pass.batches[pass.batchIdx];
		meshBatch.matrices[pass.idx].m03 = p.x + offset.x;
		meshBatch.matrices[pass.idx].m13 = p.y + offset.y;
		meshBatch.matrices[pass.idx].m23 = p.z + offset.z;
		meshBatch.tiles[pass.idx] = num;
		meshBatch.colors[pass.idx] = p.color;
		pass.idx++;
		if (pass.idx == pass.batchSize)
		{
			pass.NextBatch();
		}
	}
}
