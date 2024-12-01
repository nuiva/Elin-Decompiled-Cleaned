public class TileTypeWall : TileTypeBlock
{
	public override bool CastAmbientShadowBack => true;

	public override bool CastShadowBack => false;

	public override bool IsFullBlock => false;

	public override bool IsWallOrFence => true;

	public override bool IsWall => true;

	public override float MountHeight => 0.32f;

	public override bool IsSkipFloor => false;

	public override BaseTileSelector.BoxType BoxType => BaseTileSelector.BoxType.Fence;

	public override BlockRenderMode blockRenderMode => BlockRenderMode.WallOrFence;
}
