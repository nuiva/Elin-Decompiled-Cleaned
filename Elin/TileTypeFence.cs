using System;

public class TileTypeFence : TileTypeWall
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

	public override bool IsFence
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

	public override bool RepeatBlock
	{
		get
		{
			return false;
		}
	}

	public override bool UseLowBlock
	{
		get
		{
			return false;
		}
	}

	public override float MountHeight
	{
		get
		{
			return 0f;
		}
	}
}
