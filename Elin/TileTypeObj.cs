public class TileTypeObj : TileType
{
	public override bool CanBuiltOnThing => true;

	public override bool CanBuiltOnWater => true;

	public override bool FreeStyle => true;

	public override bool CanStack => true;
}
