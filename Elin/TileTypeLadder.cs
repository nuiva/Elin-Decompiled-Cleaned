using System;

public class TileTypeLadder : TileTypeBaseBlock
{
	public override bool IsLadder
	{
		get
		{
			return true;
		}
	}

	public override bool CanInstaComplete
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

	public override byte slopeHeight
	{
		get
		{
			return 6;
		}
	}
}
