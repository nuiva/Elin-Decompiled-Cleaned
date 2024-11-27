using System;

public class TileTypeObj : TileType
{
	public override bool CanBuiltOnThing
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

	public override bool FreeStyle
	{
		get
		{
			return true;
		}
	}

	public override bool CanStack
	{
		get
		{
			return true;
		}
	}
}
