using System;

public class TileTypeObjWaterTop : TileTypeObj
{
	public override bool CanSpawnOnWater
	{
		get
		{
			return true;
		}
	}

	public override bool IsWaterTop
	{
		get
		{
			return true;
		}
	}
}
