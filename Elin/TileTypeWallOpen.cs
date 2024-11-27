using System;

public class TileTypeWallOpen : TileTypeWall
{
	public override bool CastAmbientShadowBack
	{
		get
		{
			return false;
		}
	}

	public override bool CastAmbientShadow
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
			return false;
		}
	}

	public override bool IsBlockSight
	{
		get
		{
			return false;
		}
	}

	public override bool IsOpenSight
	{
		get
		{
			return true;
		}
	}
}
