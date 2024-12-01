public class Zone_Harvest : Zone_Civilized
{
	public override string IdProfile => "Random/I_Harvest";

	public override bool UseFog => true;

	public override void OnGenerateMap()
	{
		name = "";
	}

	public override void OnCreateBP()
	{
		bp.ignoreRoad = true;
	}
}
