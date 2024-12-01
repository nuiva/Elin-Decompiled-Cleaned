public class TraitToolRangeGun : TraitToolRange
{
	public override bool NeedReload => true;

	public override Element WeaponSkill => owner.elements.GetOrCreateElement(105);

	public override bool IsAmmo(Thing t)
	{
		return t.trait is TraitAmmoBullet;
	}
}
