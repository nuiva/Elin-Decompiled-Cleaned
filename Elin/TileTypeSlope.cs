using System;

public class TileTypeSlope : TileTypeBaseBlock
{
	public override TileType.RampType Ramp
	{
		get
		{
			return TileType.RampType.Full;
		}
	}

	public override bool CanRotate(bool buildMode)
	{
		return buildMode;
	}

	public override byte slopeHeight
	{
		get
		{
			return 6;
		}
	}

	public override bool FreeStyle
	{
		get
		{
			return false;
		}
	}

	public override bool CanBuiltOnBlock
	{
		get
		{
			return true;
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
