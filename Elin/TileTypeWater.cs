public class TileTypeWater : TileTypeWaterShallow
{
	public override float FloorHeight => -0.16f;

	public override int FloorAltitude => -6;

	public override int LiquidLV => 6;

	public override bool IsDeepWater => true;
}
