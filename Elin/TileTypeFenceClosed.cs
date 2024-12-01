public class TileTypeFenceClosed : TileTypeFence
{
	public override bool IsBlockSight => true;

	public override bool IsOpenSight => false;

	public override bool CastShadowSelf => true;
}
