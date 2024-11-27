using System;

public class TraitToolRangeGunEnergy : TraitToolRangeGun
{
	public override bool IsAmmo(Thing t)
	{
		return t.trait is TraitAmmoEnergy;
	}
}
