using System;

public class TraitContainerPurse : TraitContainerCurrency
{
	public override bool UseDummyTile
	{
		get
		{
			return false;
		}
	}
}
