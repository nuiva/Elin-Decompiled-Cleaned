using System;

public class TileTypeStairs : TileTypeBaseBlock
{
	public override TileType.RampType Ramp
	{
		get
		{
			return TileType.RampType.Full;
		}
	}

	public override byte slopeHeight
	{
		get
		{
			return 6;
		}
	}

	public override bool CastShadowBack
	{
		get
		{
			return false;
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
