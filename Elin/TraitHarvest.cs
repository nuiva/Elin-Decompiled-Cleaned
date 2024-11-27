using System;

public class TraitHarvest : Trait
{
	public override string ReqHarvest
	{
		get
		{
			return base.GetParam(1, null) + "," + base.GetParam(2, null);
		}
	}
}
