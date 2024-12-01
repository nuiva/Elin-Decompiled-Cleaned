public class TileTypeObjFloat : TileTypeObj
{
	public override bool CanStack => false;

	public override bool IsSkipLowBlock => true;

	public override bool CanBuiltOnBlock => true;

	public override bool UseMountHeight => true;

	public override bool AlwaysShowShadow => true;
}
