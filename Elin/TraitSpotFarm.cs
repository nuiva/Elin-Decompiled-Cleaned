using System;

public class TraitSpotFarm : TraitSpot
{
	public override TraitRadiusType radiusType
	{
		get
		{
			return TraitRadiusType.Farm;
		}
	}

	public override int radius
	{
		get
		{
			return 2;
		}
	}
}
