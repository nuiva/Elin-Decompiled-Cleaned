using System;

public class TileTypeSky : TileType
{
	public override bool Invisible
	{
		get
		{
			return true;
		}
	}

	public override bool IsSkipFloor
	{
		get
		{
			return true;
		}
	}

	public override bool RepeatBlock
	{
		get
		{
			return false;
		}
	}

	public override bool IsBlockPass
	{
		get
		{
			return true;
		}
	}

	public override bool AllowBlood
	{
		get
		{
			return false;
		}
	}

	public override BaseTileSelector.SelectType SelectType
	{
		get
		{
			return BaseTileSelector.SelectType.Multiple;
		}
	}

	public override BlockRenderMode blockRenderMode
	{
		get
		{
			return BlockRenderMode.FullBlock;
		}
	}
}
