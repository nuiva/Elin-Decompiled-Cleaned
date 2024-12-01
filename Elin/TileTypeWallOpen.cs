public class TileTypeWallOpen : TileTypeWall
{
	public override bool CastAmbientShadowBack => false;

	public override bool CastAmbientShadow => false;

	public override bool CastShadowSelf => false;

	public override bool IsBlockSight => false;

	public override bool IsOpenSight => true;
}
