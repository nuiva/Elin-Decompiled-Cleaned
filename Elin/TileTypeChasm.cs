using System;

public class TileTypeChasm : TileTypeRoad
{
	public override bool IsBlockPass
	{
		get
		{
			return true;
		}
	}
}
