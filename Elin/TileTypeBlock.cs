public class TileTypeBlock : TileTypeBaseBlock
{
	public override bool CanBuiltOnThing => EClass.core.config.test.allowBlockOnItem;

	public override bool CastShadowSelf => true;

	public override bool CastShadowBack => true;

	public override bool CastAmbientShadow => true;

	public override bool IsBlockPass => true;

	public override bool IsBlockSight => true;

	public override bool IsFullBlock => true;

	public override bool IsWallOrFullBlock => true;

	public override bool IsFloodBlock => true;

	public override bool IsPlayFootSound => true;

	public override bool CanBuiltOnArea => false;

	public override bool RepeatBlock => true;

	public override bool UseLowBlock => true;

	public override float MountHeight => 0.1f;

	public override bool IsSkipFloor => true;

	public override BaseTileSelector.SelectType SelectType => BaseTileSelector.SelectType.Multiple;

	public override BlockRenderMode blockRenderMode => BlockRenderMode.FullBlock;
}
