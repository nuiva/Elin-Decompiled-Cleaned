public class TileTypeSky : TileType
{
	public override bool Invisible => true;

	public override bool IsSkipFloor => true;

	public override bool RepeatBlock => false;

	public override bool IsBlockPass => true;

	public override bool AllowBlood => false;

	public override BaseTileSelector.SelectType SelectType => BaseTileSelector.SelectType.Multiple;

	public override BlockRenderMode blockRenderMode => BlockRenderMode.FullBlock;
}
