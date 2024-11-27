using System;
using UnityEngine;

public class TraitScrollStatic : TraitScroll
{
	public override SourceElement.Row source
	{
		get
		{
			if (this.owner.sourceCard.vals.Length == 0)
			{
				return null;
			}
			return EClass.sources.elements.alias.TryGetValue(this.owner.sourceCard.vals[0], null);
		}
	}

	public virtual EffectId idEffect
	{
		get
		{
			return this.owner.sourceCard.vals[1].ToEnum(true);
		}
	}

	public virtual int Power
	{
		get
		{
			return int.Parse(this.owner.sourceCard.vals[2]);
		}
	}

	public virtual string AliasEle
	{
		get
		{
			return this.owner.sourceCard.vals.TryGet(3, true);
		}
	}

	public virtual string N1
	{
		get
		{
			return this.owner.sourceCard.vals.TryGet(4, -1);
		}
	}

	public override bool CanRead(Chara c)
	{
		return !c.isBlind;
	}

	public override void OnRead(Chara c)
	{
		if ((c.isConfused || c.HasCondition<ConDim>()) && EClass.rnd(4) == 0)
		{
			c.Say("stagger", c, null, null);
			if (EClass.rnd(2) == 0)
			{
				TraitBaseSpellbook.ReadFailEffect(c);
			}
			return;
		}
		if (c.IsPC && (this.idEffect == EffectId.Identify || this.idEffect == EffectId.GreaterIdentify))
		{
			foreach (Thing thing in EClass.pc.things.List((Thing t) => t.id == this.owner.id, true))
			{
				thing.Identify(false, IDTSource.Identify);
			}
		}
		this.owner.ModNum(-1, true);
		c.PlayEffect("cast", true, 0f, default(Vector3));
		ActEffect.ProcAt(this.idEffect, this.Power, this.owner.blessedState, c, null, c.pos, false, new ActRef
		{
			aliasEle = this.AliasEle,
			n1 = this.N1,
			refThing = this.owner.Thing
		});
		c.elements.ModExp(285, 50, false);
	}
}
