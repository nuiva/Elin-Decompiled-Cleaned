using System;

public class TraitCurrency : Trait
{
	public override bool CanBeShipped
	{
		get
		{
			return false;
		}
	}
}
