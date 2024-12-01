public class TileTypeLadder : TileTypeBaseBlock
{
	public override bool IsLadder => true;

	public override bool CanInstaComplete => true;

	public override bool IsOccupyCell => false;

	public override byte slopeHeight => 6;
}
