public class RenderDataLiquid : RenderDataTile
{
	public enum EffectType
	{
		Liquid,
		Bubble
	}

	public EffectType effectType;

	public float puddleZ;

	public override void Draw(RenderParam p)
	{
		MeshBatch meshBatch = pass.batches[pass.batchIdx];
		meshBatch.matrices[pass.idx].m03 = p.x + offset.x;
		meshBatch.matrices[pass.idx].m13 = p.y + offset.y;
		meshBatch.matrices[pass.idx].m23 = p.z + ((effectType == EffectType.Liquid && p.liquidLv <= 10) ? puddleZ : offset.z);
		meshBatch.tiles[pass.idx] = p.tile;
		meshBatch.colors[pass.idx] = p.color;
		meshBatch.matColors[pass.idx] = p.matColor;
		pass.idx++;
		if (pass.idx == pass.batchSize)
		{
			pass.NextBatch();
		}
	}

	private void OnValidate()
	{
		_offset = offset;
	}
}
