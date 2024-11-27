using System;

public class TraitThrown : Trait
{
	public override bool ShowAsTool
	{
		get
		{
			return this.owner.id == "boomerang";
		}
	}

	public override bool RequireFullStackCheck
	{
		get
		{
			return true;
		}
	}
}
