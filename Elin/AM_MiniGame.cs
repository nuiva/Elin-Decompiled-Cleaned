using System;

public class AM_MiniGame : ActionMode
{
	public override bool ShowActionHint
	{
		get
		{
			return false;
		}
	}

	public override bool ShowMouseoverTarget
	{
		get
		{
			return false;
		}
	}

	public override bool AllowGeneralInput
	{
		get
		{
			return false;
		}
	}

	public override bool AllowHotbar
	{
		get
		{
			return false;
		}
	}

	public override BaseTileSelector.HitType hitType
	{
		get
		{
			return BaseTileSelector.HitType.None;
		}
	}
}
