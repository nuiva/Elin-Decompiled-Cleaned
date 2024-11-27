using System;

public class TraitBill : Trait
{
	public override bool CanBeShipped
	{
		get
		{
			return false;
		}
	}

	public override bool CanBeDestroyed
	{
		get
		{
			return false;
		}
	}
}
