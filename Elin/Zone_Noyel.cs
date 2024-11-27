using System;

public class Zone_Noyel : Zone_Town
{
	public override string IDPlaylistOverwrite
	{
		get
		{
			if (!this.IsFestival)
			{
				return null;
			}
			return "Festival_Noyel";
		}
	}

	public override bool IsFestival
	{
		get
		{
			return base.lv == 0 && EClass.world.date.month == 12;
		}
	}
}
