using System;

public class TraitAnimalTamer : TraitCitizen
{
	public override int GuidePriotiy
	{
		get
		{
			return 15;
		}
	}

	public override SlaverType SlaverType
	{
		get
		{
			return SlaverType.Animal;
		}
	}
}
