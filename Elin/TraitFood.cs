using System;

public class TraitFood : Trait
{
	public override bool RequireFullStackCheck
	{
		get
		{
			return true;
		}
	}

	public override bool CanEat(Chara c)
	{
		return true;
	}

	public override bool CanStackTo(Thing to)
	{
		return !(this.owner.c_altName != to.c_altName) && !(this.owner.c_idRefCard != to.c_idRefCard) && base.CanStackTo(to);
	}
}
