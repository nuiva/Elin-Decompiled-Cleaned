using System;

public class TileTypeWindow : TileTypeWallHang
{
	public override bool NoBackSide
	{
		get
		{
			return true;
		}
	}
}
