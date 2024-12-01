using UnityEngine;

public class RenderDataHalfBlock : RenderDataTile
{
	public Vector3 floorFix;

	public override void Draw(RenderParam p)
	{
		MeshBatch meshBatch = pass.batches[pass.batchIdx];
		meshBatch.matrices[pass.idx].m03 = p.x + offset.x;
		meshBatch.matrices[pass.idx].m13 = p.y + offset.y;
		meshBatch.matrices[pass.idx].m23 = p.z + offset.z;
		meshBatch.tiles[pass.idx] = p.tile + 2000000f;
		meshBatch.colors[pass.idx] = p.color - 1048576f;
		meshBatch.matColors[pass.idx] = p.matColor;
		pass.idx++;
		if (pass.idx == pass.batchSize)
		{
			pass.NextBatch();
		}
		MeshPass passFloor = EClass.core.screen.tileMap.passFloor;
		MeshBatch meshBatch2 = passFloor.batches[passFloor.batchIdx];
		meshBatch2.matrices[passFloor.idx].m03 = p.x + floorFix.x;
		meshBatch2.matrices[passFloor.idx].m13 = p.y + floorFix.y;
		meshBatch2.matrices[passFloor.idx].m23 = p.z + floorFix.z;
		meshBatch2.tiles[passFloor.idx] = p.tile2;
		meshBatch2.colors[passFloor.idx] = p.color - 1048576f;
		meshBatch2.matColors[passFloor.idx] = p.halfBlockColor;
		passFloor.idx++;
		if (passFloor.idx == passFloor.batchSize)
		{
			passFloor.NextBatch();
		}
	}

	private void OnValidate()
	{
		_offset = offset;
	}
}
