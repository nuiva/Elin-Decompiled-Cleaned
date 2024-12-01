public class TileTypeRoad : TileTypeObj
{
	public override bool CanStack => false;

	public override bool RemoveOnFloorChange => false;

	public override bool IsPlayFootSound => true;

	public override bool CanBuiltOnBlock => true;
}
