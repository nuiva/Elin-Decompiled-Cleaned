using System;

public class TileTypeHalfBlock : TileTypeBaseBlock
{
	public override TileType.RampType Ramp
	{
		get
		{
			return TileType.RampType.Half;
		}
	}

	public override BlockRenderMode blockRenderMode
	{
		get
		{
			return BlockRenderMode.HalfBlock;
		}
	}

	public override bool IsOccupyCell
	{
		get
		{
			return false;
		}
	}
}
