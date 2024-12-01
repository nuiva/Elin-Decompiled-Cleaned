public class Zone_Arena2 : Zone_Arena
{
	public override string IdProfile
	{
		get
		{
			if (EClass.rnd(2) != 0)
			{
				return "Random/R_Forest";
			}
			return "Random/R_Plain";
		}
	}

	public override void OnGenerateMap()
	{
		name = "";
	}

	public override void OnCreateBP()
	{
		bp.ignoreRoad = true;
	}
}
