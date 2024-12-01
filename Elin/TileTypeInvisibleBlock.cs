public class TileTypeInvisibleBlock : TileType
{
	public override bool IsFloodBlock => true;

	public override bool IsFloodDoor => true;

	public override bool Invisible => true;

	public override bool RepeatBlock => false;

	public override BaseTileSelector.SelectType SelectType => BaseTileSelector.SelectType.Multiple;

	public override BlockRenderMode blockRenderMode => BlockRenderMode.FullBlock;
}
