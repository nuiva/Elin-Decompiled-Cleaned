using System;

public class TraitInnkeeper : TraitMerchantFood
{
	public override bool CanServeFood
	{
		get
		{
			return true;
		}
	}
}
