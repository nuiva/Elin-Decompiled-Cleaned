using System;
using UnityEngine;

public class TileTypeWallMount : TileTypeObj
{
	public override string LangPlaceType
	{
		get
		{
			return "place_WallMount";
		}
	}

	public override bool CanStack
	{
		get
		{
			return false;
		}
	}

	public override bool IsSkipLowBlock
	{
		get
		{
			return true;
		}
	}

	public override bool IsBlockMount
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

	protected override HitResult HitTest(Point pos)
	{
		if (pos.sourceBlock.tileType.MountHeight == 0f)
		{
			return HitResult.Default;
		}
		if (pos.HasObj)
		{
			return HitResult.Invalid;
		}
		return HitResult.Valid;
	}

	public override void GetMountHeight(ref Vector3 v, Point p, int d, Card target = null)
	{
		v.y += p.sourceBlock.tileType.MountHeight;
	}
}
