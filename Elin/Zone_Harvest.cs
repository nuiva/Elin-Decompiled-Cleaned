using System;

public class Zone_Harvest : Zone_Civilized
{
	public override string IdProfile
	{
		get
		{
			return "Random/I_Harvest";
		}
	}

	public override bool UseFog
	{
		get
		{
			return true;
		}
	}

	public override void OnGenerateMap()
	{
		this.name = "";
	}

	public override void OnCreateBP()
	{
		this.bp.ignoreRoad = true;
	}
}
