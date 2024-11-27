using System;
using UnityEngine;

public class RenderDataEffect : RenderData
{
	public new void Draw(RenderParam p, int tile = 0)
	{
		float num = (Time.realtimeSinceStartup * this.speed + p.x + p.y) % 5f;
		MeshBatch meshBatch = this.pass.batches[this.pass.batchIdx];
		meshBatch.matrices[this.pass.idx].m03 = p.x + this.offset.x;
		meshBatch.matrices[this.pass.idx].m13 = p.y + this.offset.y;
		meshBatch.matrices[this.pass.idx].m23 = p.z + this.offset.z;
		meshBatch.tiles[this.pass.idx] = num;
		meshBatch.colors[this.pass.idx] = p.color;
		this.pass.idx++;
		if (this.pass.idx == this.pass.batchSize)
		{
			this.pass.NextBatch();
		}
	}

	public float speed;
}
