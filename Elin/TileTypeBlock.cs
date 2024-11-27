using System;

public class TileTypeBlock : TileTypeBaseBlock
{
	public override bool CanBuiltOnThing
	{
		get
		{
			return EClass.core.config.test.allowBlockOnItem;
		}
	}

	public override bool CastShadowSelf
	{
		get
		{
			return true;
		}
	}

	public override bool CastShadowBack
	{
		get
		{
			return true;
		}
	}

	public override bool CastAmbientShadow
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

	public override bool IsBlockSight
	{
		get
		{
			return true;
		}
	}

	public override bool IsFullBlock
	{
		get
		{
			return true;
		}
	}

	public override bool IsWallOrFullBlock
	{
		get
		{
			return true;
		}
	}

	public override bool IsFloodBlock
	{
		get
		{
			return true;
		}
	}

	public override bool IsPlayFootSound
	{
		get
		{
			return true;
		}
	}

	public override bool CanBuiltOnArea
	{
		get
		{
			return false;
		}
	}

	public override bool RepeatBlock
	{
		get
		{
			return true;
		}
	}

	public override bool UseLowBlock
	{
		get
		{
			return true;
		}
	}

	public override float MountHeight
	{
		get
		{
			return 0.1f;
		}
	}

	public override bool IsSkipFloor
	{
		get
		{
			return true;
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
