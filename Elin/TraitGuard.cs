using System;

public class TraitGuard : TraitCitizen
{
	public override string IDRumor
	{
		get
		{
			return "guard";
		}
	}

	public override bool CanGuide
	{
		get
		{
			return true;
		}
	}
}
