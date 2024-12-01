using UnityEngine;

public class RenderDataObjAdd : RenderDataObj
{
	public Vector3 addPos;

	public Vector2Int tilePos = new Vector2Int(0, -1);

	public override bool ForceAltHeldPosition => true;

	public override void Draw(RenderParam p)
	{
		MeshPass meshPass = ((hasSubPass && SubPassData.Current.enable) ? pass.subPass : pass);
		MeshBatch meshBatch = meshPass.batches[meshPass.batchIdx];
		int num = ((p.tile > 0f) ? 1 : (-1));
		float num2 = p.tile + ((float)tilePos.x + (float)tilePos.y * meshPass.pmesh.tiling.x) * (float)num;
		if (meshPass == pass.subPass)
		{
			meshBatch.colors[meshPass.idx] = p.color - 1572864f;
			meshBatch.matrices[meshPass.idx].SetTRS(p.NewVector3 + offset + SubPassData.Current.offset, SubPassData.Current.rotation, SubPassData.Current.scale);
		}
		else
		{
			meshBatch.colors[meshPass.idx] = p.color;
			meshBatch.matrices[meshPass.idx].m03 = p.x + offset.x * (float)num;
			meshBatch.matrices[meshPass.idx].m13 = p.y + offset.y;
			meshBatch.matrices[meshPass.idx].m23 = p.z + offset.z;
		}
		meshBatch.tiles[meshPass.idx] = p.tile + (float)(p.liquidLv * 10000 * num);
		meshBatch.matColors[meshPass.idx] = p.matColor;
		meshPass.idx++;
		if (meshPass.idx == meshPass.batchSize)
		{
			meshPass.NextBatch();
			meshBatch = meshPass.batches[meshPass.batchIdx];
		}
		meshBatch.tiles[meshPass.idx] = num2;
		meshBatch.matColors[meshPass.idx] = p.matColor;
		if (meshPass == pass.subPass)
		{
			meshBatch.colors[meshPass.idx] = p.color - 1572864f;
			meshBatch.matrices[meshPass.idx].SetTRS(p.NewVector3 + offset + new Vector3(addPos.x * (float)num * SubPassData.Current.scale.x, addPos.y * SubPassData.Current.scale.y, addPos.z) + SubPassData.Current.offset, SubPassData.Current.rotation, SubPassData.Current.scale);
		}
		else
		{
			meshBatch.colors[meshPass.idx] = p.color;
			meshBatch.matrices[meshPass.idx].m03 = p.x + offset.x * (float)num + addPos.x * (float)num;
			meshBatch.matrices[meshPass.idx].m13 = p.y + offset.y + addPos.y;
			meshBatch.matrices[meshPass.idx].m23 = p.z + offset.z + addPos.z;
		}
		meshPass.idx++;
		if (meshPass.idx == meshPass.batchSize)
		{
			meshPass.NextBatch();
		}
		if (hasSnowPass && p.snow && meshPass == pass)
		{
			meshPass = pass.snowPass;
			meshBatch = meshPass.batches[meshPass.batchIdx];
			meshBatch.colors[meshPass.idx] = p.color;
			meshBatch.matrices[meshPass.idx].m03 = p.x + offset.x * (float)num;
			meshBatch.matrices[meshPass.idx].m13 = p.y + offset.y;
			meshBatch.matrices[meshPass.idx].m23 = p.z + offset.z - 0.01f;
			meshBatch.tiles[meshPass.idx] = p.tile + (float)(p.liquidLv * 10000 * num);
			meshBatch.matColors[meshPass.idx] = 104025f;
			meshPass.idx++;
			if (meshPass.idx == meshPass.batchSize)
			{
				meshPass.NextBatch();
				meshBatch = meshPass.batches[meshPass.batchIdx];
			}
			meshBatch.tiles[meshPass.idx] = num2;
			meshBatch.matColors[meshPass.idx] = 104025f;
			meshBatch.colors[meshPass.idx] = p.color;
			meshBatch.matrices[meshPass.idx].m03 = p.x + offset.x * (float)num + addPos.x * (float)num;
			meshBatch.matrices[meshPass.idx].m13 = p.y + offset.y + addPos.y;
			meshBatch.matrices[meshPass.idx].m23 = p.z + offset.z - 0.01f + addPos.z;
			meshPass.idx++;
			if (meshPass.idx == meshPass.batchSize)
			{
				meshPass.NextBatch();
			}
		}
	}

	private void OnValidate()
	{
		_offset = offset;
	}
}
