public class TileTypeHalfBlock : TileTypeBaseBlock
{
	public override RampType Ramp => RampType.Half;

	public override BlockRenderMode blockRenderMode => BlockRenderMode.HalfBlock;

	public override bool IsOccupyCell => false;
}
