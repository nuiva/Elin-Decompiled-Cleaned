using System;

public class TileTypeBridge : TileTypeFloor
{
	public override bool IsFloor
	{
		get
		{
			return false;
		}
	}

	public override bool IsBridge
	{
		get
		{
			return true;
		}
	}

	public override bool CanBuiltOnWater
	{
		get
		{
			return true;
		}
	}

	public override int MaxAltitude
	{
		get
		{
			return 15;
		}
	}
}
