public class TileTypeBlockShip : TileTypeBaseBlock
{
	public override bool IsBlockPass => true;

	public override bool IsBlockSight => true;

	public override float MountHeight => 0.1f;

	public override bool RenderWaterBlock => false;

	public override BlockRenderMode blockRenderMode => BlockRenderMode.FullBlock;
}
