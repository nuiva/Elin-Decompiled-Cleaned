public class TileTypeVine : TileTypeObj
{
	public override int GetDesiredDir(Point p, int d)
	{
		if (p.cell.Back.HasFullBlockOrWallOrFence)
		{
			return 0;
		}
		if (p.cell.Left.HasFullBlockOrWallOrFence)
		{
			return 1;
		}
		if (p.cell.Front.HasFullBlockOrWallOrFence)
		{
			return 2;
		}
		if (p.cell.Right.HasFullBlockOrWallOrFence)
		{
			return 3;
		}
		return -1;
	}
}
