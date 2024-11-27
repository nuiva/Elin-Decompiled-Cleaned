using System;
using UnityEngine;

public class RenderDataHalfBlock : RenderDataTile
{
	public override void Draw(RenderParam p)
	{
		MeshBatch meshBatch = this.pass.batches[this.pass.batchIdx];
		meshBatch.matrices[this.pass.idx].m03 = p.x + this.offset.x;
		meshBatch.matrices[this.pass.idx].m13 = p.y + this.offset.y;
		meshBatch.matrices[this.pass.idx].m23 = p.z + this.offset.z;
		meshBatch.tiles[this.pass.idx] = p.tile + 2000000f;
		meshBatch.colors[this.pass.idx] = p.color - 1048576f;
		meshBatch.matColors[this.pass.idx] = p.matColor;
		this.pass.idx++;
		if (this.pass.idx == this.pass.batchSize)
		{
			this.pass.NextBatch();
		}
		MeshPass passFloor = EClass.core.screen.tileMap.passFloor;
		MeshBatch meshBatch2 = passFloor.batches[passFloor.batchIdx];
		meshBatch2.matrices[passFloor.idx].m03 = p.x + this.floorFix.x;
		meshBatch2.matrices[passFloor.idx].m13 = p.y + this.floorFix.y;
		meshBatch2.matrices[passFloor.idx].m23 = p.z + this.floorFix.z;
		meshBatch2.tiles[passFloor.idx] = (float)p.tile2;
		meshBatch2.colors[passFloor.idx] = p.color - 1048576f;
		meshBatch2.matColors[passFloor.idx] = (float)p.halfBlockColor;
		passFloor.idx++;
		if (passFloor.idx == passFloor.batchSize)
		{
			passFloor.NextBatch();
		}
	}

	private void OnValidate()
	{
		this._offset = this.offset;
	}

	public Vector3 floorFix;
}
