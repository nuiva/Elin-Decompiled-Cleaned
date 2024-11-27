using System;

public class TileTypeWaterfall : TileTypePillar
{
	public override float RepeatSize
	{
		get
		{
			return 0.6f;
		}
	}

	public override bool IsBlockPass
	{
		get
		{
			return false;
		}
	}

	public override bool IsBlockSight
	{
		get
		{
			return false;
		}
	}
}
