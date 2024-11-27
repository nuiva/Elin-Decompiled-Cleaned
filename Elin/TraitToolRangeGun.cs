using System;

public class TraitToolRangeGun : TraitToolRange
{
	public override bool NeedReload
	{
		get
		{
			return true;
		}
	}

	public override Element WeaponSkill
	{
		get
		{
			return this.owner.elements.GetOrCreateElement(105);
		}
	}

	public override bool IsAmmo(Thing t)
	{
		return t.trait is TraitAmmoBullet;
	}
}
