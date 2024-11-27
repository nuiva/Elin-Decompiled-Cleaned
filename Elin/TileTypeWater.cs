using System;

public class TileTypeWater : TileTypeWaterShallow
{
	public override float FloorHeight
	{
		get
		{
			return -0.16f;
		}
	}

	public override int FloorAltitude
	{
		get
		{
			return -6;
		}
	}

	public override int LiquidLV
	{
		get
		{
			return 6;
		}
	}

	public override bool IsDeepWater
	{
		get
		{
			return true;
		}
	}
}
