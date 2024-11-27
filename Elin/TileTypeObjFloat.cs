using System;

public class TileTypeObjFloat : TileTypeObj
{
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

	public override bool CanBuiltOnBlock
	{
		get
		{
			return true;
		}
	}

	public override bool UseMountHeight
	{
		get
		{
			return true;
		}
	}

	public override bool AlwaysShowShadow
	{
		get
		{
			return true;
		}
	}
}
