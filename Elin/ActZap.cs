using System;
using UnityEngine;

public class ActZap : Act
{
	public override int MaxRadius
	{
		get
		{
			return 2;
		}
	}

	public override int PerformDistance
	{
		get
		{
			return 99;
		}
	}

	public override TargetType TargetType
	{
		get
		{
			return TargetType.Ground;
		}
	}

	public override bool Perform()
	{
		Act.CC.Say("zapRod", Act.CC, this.trait.owner.Name, null);
		if (this.trait.owner.c_charges > 0)
		{
			this.trait.owner.ModCharge(-1, false);
			Act.CC.PlayEffect("rod", true, 0f, default(Vector3));
			Act.CC.PlaySound("rod", 1f, true);
			Act.TC = Act.CC;
			EffectId idEffect = this.trait.IdEffect;
			int power = this.trait.Power * (100 + Act.CC.Evalue(305) * 10 + Act.CC.MAG / 2 + Act.CC.PER / 2) / 100;
			ActEffect.ProcAt(idEffect, power, this.trait.owner.blessedState, Act.CC, null, Act.TP, this.trait.IsNegative, new ActRef
			{
				refThing = this.trait.owner.Thing,
				aliasEle = this.trait.aliasEle,
				n1 = this.trait.N1,
				act = ((this.trait.source != null) ? ACT.Create(this.trait.source) : null)
			});
			if (Act.CC.IsPC && (idEffect == EffectId.Identify || idEffect == EffectId.GreaterIdentify))
			{
				this.trait.owner.Thing.Identify(Act.CC.IsPCParty, IDTSource.Identify);
			}
			Act.CC.ModExp(305, 50);
			return true;
		}
		Act.CC.Say("nothingHappens", null, null);
		Act.CC.PlaySound("rod_empty", 1f, true);
		return true;
	}

	public TraitRod trait;
}
