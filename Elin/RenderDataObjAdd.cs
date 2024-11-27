using System;
using UnityEngine;

public class RenderDataObjAdd : RenderDataObj
{
	public override bool ForceAltHeldPosition
	{
		get
		{
			return true;
		}
	}

	public override void Draw(RenderParam p)
	{
		MeshPass meshPass = (this.hasSubPass && SubPassData.Current.enable) ? this.pass.subPass : this.pass;
		MeshBatch meshBatch = meshPass.batches[meshPass.batchIdx];
		int num = (p.tile > 0f) ? 1 : -1;
		float num2 = p.tile + ((float)this.tilePos.x + (float)this.tilePos.y * meshPass.pmesh.tiling.x) * (float)num;
		if (meshPass == this.pass.subPass)
		{
			meshBatch.colors[meshPass.idx] = p.color - 1572864f;
			meshBatch.matrices[meshPass.idx].SetTRS(p.NewVector3 + this.offset + SubPassData.Current.offset, SubPassData.Current.rotation, SubPassData.Current.scale);
		}
		else
		{
			meshBatch.colors[meshPass.idx] = p.color;
			meshBatch.matrices[meshPass.idx].m03 = p.x + this.offset.x * (float)num;
			meshBatch.matrices[meshPass.idx].m13 = p.y + this.offset.y;
			meshBatch.matrices[meshPass.idx].m23 = p.z + this.offset.z;
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
		if (meshPass == this.pass.subPass)
		{
			meshBatch.colors[meshPass.idx] = p.color - 1572864f;
			meshBatch.matrices[meshPass.idx].SetTRS(p.NewVector3 + this.offset + new Vector3(this.addPos.x * (float)num * SubPassData.Current.scale.x, this.addPos.y * SubPassData.Current.scale.y, this.addPos.z) + SubPassData.Current.offset, SubPassData.Current.rotation, SubPassData.Current.scale);
		}
		else
		{
			meshBatch.colors[meshPass.idx] = p.color;
			meshBatch.matrices[meshPass.idx].m03 = p.x + this.offset.x * (float)num + this.addPos.x * (float)num;
			meshBatch.matrices[meshPass.idx].m13 = p.y + this.offset.y + this.addPos.y;
			meshBatch.matrices[meshPass.idx].m23 = p.z + this.offset.z + this.addPos.z;
		}
		meshPass.idx++;
		if (meshPass.idx == meshPass.batchSize)
		{
			meshPass.NextBatch();
		}
		if (this.hasSnowPass && p.snow && meshPass == this.pass)
		{
			meshPass = this.pass.snowPass;
			meshBatch = meshPass.batches[meshPass.batchIdx];
			meshBatch.colors[meshPass.idx] = p.color;
			meshBatch.matrices[meshPass.idx].m03 = p.x + this.offset.x * (float)num;
			meshBatch.matrices[meshPass.idx].m13 = p.y + this.offset.y;
			meshBatch.matrices[meshPass.idx].m23 = p.z + this.offset.z - 0.01f;
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
			meshBatch.matrices[meshPass.idx].m03 = p.x + this.offset.x * (float)num + this.addPos.x * (float)num;
			meshBatch.matrices[meshPass.idx].m13 = p.y + this.offset.y + this.addPos.y;
			meshBatch.matrices[meshPass.idx].m23 = p.z + this.offset.z - 0.01f + this.addPos.z;
			meshPass.idx++;
			if (meshPass.idx == meshPass.batchSize)
			{
				meshPass.NextBatch();
			}
		}
	}

	private void OnValidate()
	{
		this._offset = this.offset;
	}

	public Vector3 addPos;

	public Vector2Int tilePos = new Vector2Int(0, -1);
}
