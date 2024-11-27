using System;

public class TileTypeBlockShip : TileTypeBaseBlock
{
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

	public override float MountHeight
	{
		get
		{
			return 0.1f;
		}
	}

	public override bool RenderWaterBlock
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
			return BlockRenderMode.FullBlock;
		}
	}
}
