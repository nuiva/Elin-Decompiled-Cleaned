public class RecipeBridgePillar : Recipe
{
	public override TileType tileType => TileType.BridgePillar;

	public override bool IsBlock => false;

	public override bool RequireIngredients => false;

	public override void Build(Chara chara, Card t, Point pos, int mat, int dir, int altitude, int bridgeHeight)
	{
		if (pos.cell.bridgePillar == tileRow.id)
		{
			pos.cell.bridgePillar = byte.MaxValue;
		}
		else
		{
			pos.cell.bridgePillar = (byte)tileRow.id;
		}
	}

	public override Recipe Duplicate()
	{
		return IO.DeepCopy(this);
	}
}
