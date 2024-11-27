using System;

public class Zone_StartVillage : Zone_Civilized
{
	public override bool MakeEnemiesNeutral
	{
		get
		{
			return true;
		}
	}

	public override bool GrowPlant
	{
		get
		{
			return true;
		}
	}
}
