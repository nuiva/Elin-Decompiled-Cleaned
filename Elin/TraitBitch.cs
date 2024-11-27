using System;

public class TraitBitch : TraitCitizen
{
	public override string IDRumor
	{
		get
		{
			return "bitch";
		}
	}

	public override bool CanWhore
	{
		get
		{
			return true;
		}
	}

	public override bool CanGuide
	{
		get
		{
			return EClass._zone.id == "derphy";
		}
	}
}
