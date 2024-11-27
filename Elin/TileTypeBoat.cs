using System;

public class TileTypeBoat : TileTypeObj
{
	public override bool CanBuiltOnBridge
	{
		get
		{
			return false;
		}
	}

	public override bool CanBuiltOnFloor
	{
		get
		{
			return false;
		}
	}
}
