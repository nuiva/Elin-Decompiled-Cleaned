using System;

public class TileTypeObjWater : TileTypeObj
{
	public override bool CanSpawnOnWater
	{
		get
		{
			return true;
		}
	}
}
