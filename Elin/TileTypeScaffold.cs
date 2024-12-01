public class TileTypeScaffold : TileTypeBaseBlock
{
	public override BaseTileSelector.SelectType SelectType => BaseTileSelector.SelectType.Multiple;

	public override bool IsLadder => true;

	public override bool CanBuiltOnThing => true;

	public override bool CanInstaComplete => true;

	public override bool IsOccupyCell => false;

	public override bool ShowPillar => false;
}
