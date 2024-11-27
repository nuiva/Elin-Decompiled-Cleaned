using System;

public class TileTypePillar : TileTypeBaseBlock
{
	public override bool RepeatBlock
	{
		get
		{
			return true;
		}
	}

	public override bool UseLowWallTiles
	{
		get
		{
			return false;
		}
	}

	public override bool ForceRpeatBlock
	{
		get
		{
			return true;
		}
	}

	public override int MaxAltitude
	{
		get
		{
			return 7;
		}
	}

	public override bool AltitudeAsDir
	{
		get
		{
			return true;
		}
	}

	public override bool IsBlockPass
	{
		get
		{
			return true;
		}
	}

	public override bool IsOccupyCell
	{
		get
		{
			return false;
		}
	}

	public override BlockRenderMode blockRenderMode
	{
		get
		{
			return BlockRenderMode.Pillar;
		}
	}
}
