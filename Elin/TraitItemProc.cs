using System;
using UnityEngine;

public class TraitItemProc : TraitItem
{
	public EffectId IdEffect
	{
		get
		{
			return base.GetParam(1, null).ToEnum(true);
		}
	}

	public string n1
	{
		get
		{
			return base.GetParam(2, null);
		}
	}

	public override int CraftNum
	{
		get
		{
			return this.GetCraftNum();
		}
	}

	public override bool CanChangeHeight
	{
		get
		{
			return false;
		}
	}

	public int GetCraftNum()
	{
		if (this.owner.id == "bandage")
		{
			return 2 + EClass.rnd(2);
		}
		return 0;
	}

	public override bool OnUse(Chara c)
	{
		int num = this.owner.Power;
		if (this.IdEffect == EffectId.Buff && this.n1 == "ConBandage")
		{
			num += this.owner.Evalue(750) * 5;
			num = num * (100 + (int)Mathf.Sqrt((float)c.Evalue(300)) * 5) / 100;
		}
		ActEffect.Proc(this.IdEffect, base.GetParamInt(3, num), this.owner.blessedState, c, null, new ActRef
		{
			n1 = this.n1
		});
		if (c.ExistsOnMap)
		{
			FoodEffect.ProcTrait(c, this.owner);
		}
		if (this.IdEffect == EffectId.RemedyJure)
		{
			c.Say("destory_feather", this.owner, null, null);
		}
		this.owner.ModNum(-1, true);
		return true;
	}

	public override Action GetHealAction(Chara c)
	{
		if (this.IdEffect == EffectId.Buff && this.n1 == "ConBandage" && !c.HasCondition<ConBandage>())
		{
			return delegate()
			{
				this.OnUse(c);
			};
		}
		return null;
	}
}
