using System;

public class TileTypeWall : TileTypeBlock
{
	public override bool CastAmbientShadowBack
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
			return false;
		}
	}

	public override bool IsFullBlock
	{
		get
		{
			return false;
		}
	}

	public override bool IsWallOrFence
	{
		get
		{
			return true;
		}
	}

	public override bool IsWall
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
			return 0.32f;
		}
	}

	public override bool IsSkipFloor
	{
		get
		{
			return false;
		}
	}

	public override BaseTileSelector.BoxType BoxType
	{
		get
		{
			return BaseTileSelector.BoxType.Fence;
		}
	}

	public override BlockRenderMode blockRenderMode
	{
		get
		{
			return BlockRenderMode.WallOrFence;
		}
	}
}
