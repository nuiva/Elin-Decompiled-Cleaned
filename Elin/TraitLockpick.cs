using System;

public class TraitLockpick : Trait
{
	public override bool CanStack
	{
		get
		{
			return false;
		}
	}

	public override bool HasCharges
	{
		get
		{
			return true;
		}
	}

	public override void OnCreate(int lv)
	{
		this.owner.c_charges = EClass.rnd(20) + 1;
	}
}
