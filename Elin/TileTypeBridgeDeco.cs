using System;

public class TileTypeBridgeDeco : TileTypeFloor
{
	public override bool IsBridge
	{
		get
		{
			return true;
		}
	}

	public override BaseTileSelector.SelectType SelectType
	{
		get
		{
			return BaseTileSelector.SelectType.Single;
		}
	}

	public override int MinAltitude
	{
		get
		{
			return 1;
		}
	}

	public override bool CastShadowSelf
	{
		get
		{
			return false;
		}
	}
}
