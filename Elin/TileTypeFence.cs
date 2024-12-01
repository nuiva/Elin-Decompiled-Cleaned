public class TileTypeFence : TileTypeWall
{
	public override bool CastAmbientShadowBack => false;

	public override bool CastAmbientShadow => false;

	public override bool CastShadowSelf => false;

	public override bool IsFence => true;

	public override bool IsWall => false;

	public override bool IsBlockSight => false;

	public override bool IsOpenSight => true;

	public override bool RepeatBlock => false;

	public override bool UseLowBlock => false;

	public override float MountHeight => 0f;
}
