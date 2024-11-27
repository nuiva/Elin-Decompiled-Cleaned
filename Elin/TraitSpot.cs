using System;

public class TraitSpot : Trait
{
	public override bool CanBeMasked
	{
		get
		{
			return true;
		}
	}

	public override int radius
	{
		get
		{
			return 4;
		}
	}
}
