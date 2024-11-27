using System;

public class TraitBanker : TraitCitizen
{
	public override int GuidePriotiy
	{
		get
		{
			return 23;
		}
	}

	public override string IDRumor
	{
		get
		{
			return "banker";
		}
	}
}
