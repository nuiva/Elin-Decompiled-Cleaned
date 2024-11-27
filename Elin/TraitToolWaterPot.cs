using System;

public class TraitToolWaterPot : TraitTool
{
	public int MaxCharge
	{
		get
		{
			return 20 + this.owner.material.hardness * 2;
		}
	}

	public override void OnCreate(int lv)
	{
		this.owner.Dye("void");
	}

	public override bool HasCharges
	{
		get
		{
			return true;
		}
	}

	public override void SetName(ref string s)
	{
		if (this.owner.c_dyeMat != 0)
		{
			s = "_of".lang(this.owner.DyeMat.GetName(), s, null, null, null);
		}
	}
}
