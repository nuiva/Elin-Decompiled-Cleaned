using System;

public class TileTypeWaterShallow : TileTypeFloor
{
	public override bool AllowBlood
	{
		get
		{
			return false;
		}
	}

	public override bool AllowLitter
	{
		get
		{
			return false;
		}
	}

	public override bool IsWater
	{
		get
		{
			return true;
		}
	}

	public override int FloorAltitude
	{
		get
		{
			return -4;
		}
	}

	public override int LiquidLV
	{
		get
		{
			return 3;
		}
	}

	public override float FloorHeight
	{
		get
		{
			return -0.06f;
		}
	}

	public override bool ShowPillar
	{
		get
		{
			return false;
		}
	}
}
