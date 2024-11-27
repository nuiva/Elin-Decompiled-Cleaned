using System;

public class RenderDataTile : RenderData
{
	public override void Draw(RenderParam p)
	{
		if (this.useOffsetBack)
		{
			this._offset = ((p.dir % 4 >= 2) ? this.offsetBack : this.offset);
		}
		int num = (p.tile % 2f == 1f || p.tile < 0f || !this.symmetry) ? 1 : -1;
		int num2 = (p.tile < 0f) ? -1 : 1;
		MeshBatch meshBatch = this.pass.batches[this.pass.batchIdx];
		meshBatch.matrices[this.pass.idx].m03 = p.x + this._offset.x * (float)num;
		meshBatch.matrices[this.pass.idx].m13 = p.y + this._offset.y;
		meshBatch.matrices[this.pass.idx].m23 = p.z + this._offset.z;
		meshBatch.tiles[this.pass.idx] = p.tile + (float)(this.liquid ? (p.liquidLv * 10000 * num2) : 0);
		meshBatch.colors[this.pass.idx] = p.color;
		meshBatch.matColors[this.pass.idx] = p.matColor;
		this.pass.idx++;
		if (this.pass.idx == this.pass.batchSize)
		{
			this.pass.NextBatch();
		}
		if (this.multiSize)
		{
			meshBatch.matrices[this.pass.idx].m03 = p.x + this._offset.x * (float)num;
			meshBatch.matrices[this.pass.idx].m13 = p.y + this._offset.y + this.pass.pmesh.size.y;
			meshBatch.matrices[this.pass.idx].m23 = p.z + this._offset.z + RenderData.renderSetting.vFix.z;
			meshBatch.tiles[this.pass.idx] = p.tile - this.pass.pmesh.tiling.x + (float)(this.liquid ? (p.liquidLv * 10000 * num2) : 0);
			meshBatch.colors[this.pass.idx] = p.color;
			meshBatch.matColors[this.pass.idx] = p.matColor;
			this.pass.idx++;
			if (this.pass.idx == this.pass.batchSize)
			{
				this.pass.NextBatch();
			}
		}
		if (p.snow && this.hasSnowPass)
		{
			MeshPass snowPass = this.pass.snowPass;
			meshBatch = snowPass.batches[snowPass.batchIdx];
			meshBatch.colors[snowPass.idx] = p.color;
			meshBatch.matrices[snowPass.idx].m03 = p.x + this._offset.x * (float)num;
			meshBatch.matrices[snowPass.idx].m13 = p.y + this._offset.y;
			meshBatch.matrices[snowPass.idx].m23 = p.z + this._offset.z + this.snowZ;
			meshBatch.tiles[snowPass.idx] = p.tile + (float)(p.liquidLv * 10000 * num2);
			meshBatch.matColors[snowPass.idx] = 104025f;
			snowPass.idx++;
			if (snowPass.idx == snowPass.batchSize)
			{
				snowPass.NextBatch();
			}
		}
	}

	public override void DrawWithRotation(RenderParam p, float angle)
	{
		MeshPass subPass = this.pass.subPass;
		p.NewVector3 + this.offset;
	}

	private void OnValidate()
	{
		this._offset = this.offset;
	}

	public bool liquid;

	public float snowZ = -0.01f;
}
