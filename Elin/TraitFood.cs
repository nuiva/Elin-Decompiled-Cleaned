public class TraitFood : Trait
{
	public override bool RequireFullStackCheck => true;

	public override bool CanEat(Chara c)
	{
		return true;
	}

	public override bool CanStackTo(Thing to)
	{
		if (owner.c_altName != to.c_altName)
		{
			return false;
		}
		if (owner.c_idRefCard != to.c_idRefCard)
		{
			return false;
		}
		return base.CanStackTo(to);
	}
}
