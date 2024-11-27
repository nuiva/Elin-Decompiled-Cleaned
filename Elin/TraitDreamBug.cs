using System;

public class TraitDreamBug : Trait
{
	public override bool CanStack
	{
		get
		{
			return false;
		}
	}

	public override bool IsBlendBase
	{
		get
		{
			return true;
		}
	}

	public override void OnCreate(int lv)
	{
		this.owner.c_charges = 3 + EClass.rnd(5);
	}

	public override bool CanBlend(Thing t)
	{
		return t.IsFood;
	}

	public override void OnBlend(Thing t, Chara c)
	{
		TraitDrink.BlendLove(EClass.pc, t, true);
		this.owner.ModNum(-1, true);
	}
}
