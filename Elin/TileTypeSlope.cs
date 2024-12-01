public class TileTypeSlope : TileTypeBaseBlock
{
	public override RampType Ramp => RampType.Full;

	public override byte slopeHeight => 6;

	public override bool FreeStyle => false;

	public override bool CanBuiltOnBlock => true;

	public override bool IsOccupyCell => false;

	public override bool CanRotate(bool buildMode)
	{
		return buildMode;
	}
}
