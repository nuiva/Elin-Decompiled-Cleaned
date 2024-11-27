using System;

public class TileTypeBridgePillar : TileTypeObj
{
	public override BaseTileSelector.SelectType SelectType
	{
		get
		{
			return BaseTileSelector.SelectType.Multiple;
		}
	}

	public override bool CanBuiltOnBlock
	{
		get
		{
			return true;
		}
	}

	protected override HitResult HitTest(Point pos)
	{
		if (pos.cell._bridge == 0)
		{
			return HitResult.Invalid;
		}
		return base.HitTest(pos);
	}
}
