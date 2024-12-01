public class TileTypeWaterShallow : TileTypeFloor
{
	public override bool AllowBlood => false;

	public override bool AllowLitter => false;

	public override bool IsWater => true;

	public override int FloorAltitude => -4;

	public override int LiquidLV => 3;

	public override float FloorHeight => -0.06f;

	public override bool ShowPillar => false;
}
