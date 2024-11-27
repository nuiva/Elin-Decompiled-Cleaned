using System;

public class TileTypeBaseBlock : TileType
{
	public override string LangPlaceType
	{
		get
		{
			return "place_Block";
		}
	}

	public override bool CanBuiltOnWater
	{
		get
		{
			return true;
		}
	}
}
