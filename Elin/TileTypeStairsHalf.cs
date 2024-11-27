using System;

public class TileTypeStairsHalf : TileTypeStairs
{
	public override TileType.RampType Ramp
	{
		get
		{
			return TileType.RampType.Half;
		}
	}
}
