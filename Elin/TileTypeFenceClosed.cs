using System;

public class TileTypeFenceClosed : TileTypeFence
{
	public override bool IsBlockSight
	{
		get
		{
			return true;
		}
	}

	public override bool IsOpenSight
	{
		get
		{
			return false;
		}
	}

	public override bool CastShadowSelf
	{
		get
		{
			return true;
		}
	}
}
