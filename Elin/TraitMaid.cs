using System;

public class TraitMaid : TraitCitizen
{
	public override bool CanSellPlan
	{
		get
		{
			return true;
		}
	}
}
