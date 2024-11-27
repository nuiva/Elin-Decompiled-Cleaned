using System;

public class TileTypeRoad : TileTypeObj
{
	public override bool CanStack
	{
		get
		{
			return false;
		}
	}

	public override bool RemoveOnFloorChange
	{
		get
		{
			return false;
		}
	}

	public override bool IsPlayFootSound
	{
		get
		{
			return true;
		}
	}

	public override bool CanBuiltOnBlock
	{
		get
		{
			return true;
		}
	}
}
