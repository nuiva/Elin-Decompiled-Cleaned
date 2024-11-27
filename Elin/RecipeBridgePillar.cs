using System;

public class RecipeBridgePillar : Recipe
{
	public override TileType tileType
	{
		get
		{
			return TileType.BridgePillar;
		}
	}

	public override bool IsBlock
	{
		get
		{
			return false;
		}
	}

	public override bool RequireIngredients
	{
		get
		{
			return false;
		}
	}

	public override void Build(Chara chara, Card t, Point pos, int mat, int dir, int altitude, int bridgeHeight)
	{
		if ((int)pos.cell.bridgePillar == this.tileRow.id)
		{
			pos.cell.bridgePillar = byte.MaxValue;
			return;
		}
		pos.cell.bridgePillar = (byte)this.tileRow.id;
	}

	public override Recipe Duplicate()
	{
		return IO.DeepCopy<RecipeBridgePillar>(this);
	}
}
