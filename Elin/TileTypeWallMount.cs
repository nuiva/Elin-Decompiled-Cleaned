using UnityEngine;

public class TileTypeWallMount : TileTypeObj
{
	public override string LangPlaceType => "place_WallMount";

	public override bool CanStack => false;

	public override bool IsSkipLowBlock => true;

	public override bool IsBlockMount => true;

	public override bool CanBuiltOnBlock => true;

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
