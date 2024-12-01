public class TileTypeStairs : TileTypeBaseBlock
{
	public override RampType Ramp => RampType.Full;

	public override byte slopeHeight => 6;

	public override bool CastShadowBack => false;

	public override bool IsOccupyCell => false;
}
