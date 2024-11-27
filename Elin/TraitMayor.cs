using System;

public class TraitMayor : TraitCitizen
{
	public override int GuidePriotiy
	{
		get
		{
			return 100;
		}
	}

	public override string GetDramaText()
	{
		return "dramaText_town".lang((EClass._zone.development / 10).ToString() ?? "", null, null, null, null);
	}

	public override string IDRumor
	{
		get
		{
			return "mayor";
		}
	}
}
