public class TraitToolRangeCrossbow : TraitToolRangeBow
{
	public override Element WeaponSkill => owner.elements.GetOrCreateElement(109);

	public override bool IsAmmo(Thing t)
	{
		return t.trait is TraitAmmoBolt;
	}
}
