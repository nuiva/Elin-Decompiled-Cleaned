public class Zone_Olvina : Zone_Town
{
	public override string IDPlaylistOverwrite
	{
		get
		{
			if (!IsFestival)
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
			if (base.lv == 0)
			{
				return EClass.world.date.month == 3;
			}
			return false;
		}
	}

	public override void OnActivate()
	{
		EClass.scene.LoadPrefab("MoonOlvina");
	}
}
