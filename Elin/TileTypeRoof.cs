using System;

public class TileTypeRoof : TileTypeObj
{
	public override string LangPlaceType
	{
		get
		{
			return "place_Roof";
		}
	}

	public override bool CanStack
	{
		get
		{
			return false;
		}
	}

	public override bool IsUseBlockDir
	{
		get
		{
			return true;
		}
	}

	public override bool CanRotate(bool buildMode)
	{
		return false;
	}

	protected override HitResult HitTest(Point pos)
	{
		if (!pos.cell.HasSlope)
		{
			return HitResult.Default;
		}
		if (pos.HasObj)
		{
			return HitResult.Invalid;
		}
		return HitResult.Valid;
	}
}
