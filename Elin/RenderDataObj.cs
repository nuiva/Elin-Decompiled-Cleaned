using System;

public class RenderDataObj : RenderDataTile
{
	public override void Draw(RenderParam p)
	{
		MeshPass meshPass = (this.hasSubPass && SubPassData.Current.enable) ? this.pass.subPass : this.pass;
		MeshBatch meshBatch = meshPass.batches[meshPass.batchIdx];
		int num = (p.tile > 0f) ? 1 : -1;
		if (this.useOffsetBack)
		{
			this._offset = ((p.dir == 2 || p.dir == 3) ? this.offsetBack : this.offset);
		}
		if (meshPass == this.pass.subPass)
		{
			meshBatch.colors[meshPass.idx] = p.color - 1572864f;
			meshBatch.matrices[meshPass.idx].SetTRS(p.NewVector3 + this._offset + SubPassData.Current.offset, SubPassData.Current.rotation, SubPassData.Current.scale);
		}
		else
		{
			meshBatch.colors[meshPass.idx] = p.color;
			meshBatch.matrices[meshPass.idx].m03 = p.x + this._offset.x * (float)num;
			meshBatch.matrices[meshPass.idx].m13 = p.y + this._offset.y;
			meshBatch.matrices[meshPass.idx].m23 = p.z + this._offset.z;
		}
		meshBatch.tiles[meshPass.idx] = p.tile + (float)(p.liquidLv * 10000 * num);
		meshBatch.matColors[meshPass.idx] = p.matColor;
		meshPass.idx++;
		if (meshPass.idx == meshPass.batchSize)
		{
			meshPass.NextBatch();
		}
		if (p.snow && this.hasSnowPass && meshPass == this.pass)
		{
			meshPass = this.pass.snowPass;
			meshBatch = meshPass.batches[meshPass.batchIdx];
			meshBatch.colors[meshPass.idx] = p.color;
			meshBatch.matrices[meshPass.idx].m03 = p.x + this._offset.x * (float)num;
			meshBatch.matrices[meshPass.idx].m13 = p.y + this._offset.y;
			meshBatch.matrices[meshPass.idx].m23 = p.z + this._offset.z + this.snowZ;
			meshBatch.tiles[meshPass.idx] = p.tile + (float)(p.liquidLv * 10000 * num);
			meshBatch.matColors[meshPass.idx] = 104025f;
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
}
