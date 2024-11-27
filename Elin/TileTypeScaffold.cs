using System;

public class TileTypeScaffold : TileTypeBaseBlock
{
	public override BaseTileSelector.SelectType SelectType
	{
		get
		{
			return BaseTileSelector.SelectType.Multiple;
		}
	}

	public override bool IsLadder
	{
		get
		{
			return true;
		}
	}

	public override bool CanBuiltOnThing
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

	public override bool ShowPillar
	{
		get
		{
			return false;
		}
	}
}
