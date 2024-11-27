using System;

public class TraitToolRangeBow : TraitToolRange
{
	public override Element WeaponSkill
	{
		get
		{
			return this.owner.elements.GetOrCreateElement(104);
		}
	}

	public override bool IsAmmo(Thing t)
	{
		return t.trait is TraitAmmoArrow;
	}
}
