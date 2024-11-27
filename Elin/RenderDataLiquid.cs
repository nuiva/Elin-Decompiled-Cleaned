using System;

public class RenderDataLiquid : RenderDataTile
{
	public override void Draw(RenderParam p)
	{
		MeshBatch meshBatch = this.pass.batches[this.pass.batchIdx];
		meshBatch.matrices[this.pass.idx].m03 = p.x + this.offset.x;
		meshBatch.matrices[this.pass.idx].m13 = p.y + this.offset.y;
		meshBatch.matrices[this.pass.idx].m23 = p.z + ((this.effectType == RenderDataLiquid.EffectType.Liquid && p.liquidLv <= 10) ? this.puddleZ : this.offset.z);
		meshBatch.tiles[this.pass.idx] = p.tile;
		meshBatch.colors[this.pass.idx] = p.color;
		meshBatch.matColors[this.pass.idx] = p.matColor;
		this.pass.idx++;
		if (this.pass.idx == this.pass.batchSize)
		{
			this.pass.NextBatch();
		}
	}

	private void OnValidate()
	{
		this._offset = this.offset;
	}

	public RenderDataLiquid.EffectType effectType;

	public float puddleZ;

	public enum EffectType
	{
		Liquid,
		Bubble
	}
}
