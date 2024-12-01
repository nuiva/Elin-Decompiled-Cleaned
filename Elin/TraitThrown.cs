public class TraitThrown : Trait
{
	public override bool ShowAsTool => owner.id == "boomerang";

	public override bool RequireFullStackCheck => true;
}
