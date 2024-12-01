public class TileTypeBridgePillar : TileTypeObj
{
	public override BaseTileSelector.SelectType SelectType => BaseTileSelector.SelectType.Multiple;

	public override bool CanBuiltOnBlock => true;

	protected override HitResult HitTest(Point pos)
	{
		if (pos.cell._bridge == 0)
		{
			return HitResult.Invalid;
		}
		return base.HitTest(pos);
	}
}
