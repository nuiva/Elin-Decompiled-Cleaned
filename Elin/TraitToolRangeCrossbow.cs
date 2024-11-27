using System;

public class TraitToolRangeCrossbow : TraitToolRangeBow
{
	public override Element WeaponSkill
	{
		get
		{
			return this.owner.elements.GetOrCreateElement(109);
		}
	}

	public override bool IsAmmo(Thing t)
	{
		return t.trait is TraitAmmoBolt;
	}
}
