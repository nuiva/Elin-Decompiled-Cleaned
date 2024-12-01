public class TileTypeWallHangSign : TileTypeObj
{
	public override int GetDesiredDir(Point p, int d)
	{
		return -1;
	}
}
