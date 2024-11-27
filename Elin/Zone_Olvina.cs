using System;

public class Zone_Olvina : Zone_Town
{
	public override string IDPlaylistOverwrite
	{
		get
		{
			if (!this.IsFestival)
			{
				return null;
			}
			return "Festival_Olvina";
		}
	}

	public override bool IsFestival
	{
		get
		{
			return base.lv == 0 && EClass.world.date.month == 3;
		}
	}

	public override void OnActivate()
	{
		EClass.scene.LoadPrefab("MoonOlvina");
	}
}
