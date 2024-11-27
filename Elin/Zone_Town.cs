using System;

public class Zone_Town : Zone_Civilized
{
	public override bool IsTown
	{
		get
		{
			return true;
		}
	}

	public override bool IsExplorable
	{
		get
		{
			return false;
		}
	}

	public override bool CanDigUnderground
	{
		get
		{
			return false;
		}
	}

	public override bool CanSpawnAdv
	{
		get
		{
			return base.lv == 0;
		}
	}

	public override bool AllowCriminal
	{
		get
		{
			return false;
		}
	}
}
