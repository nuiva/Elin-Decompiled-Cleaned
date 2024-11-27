using System;

public class TraitDrink : Trait
{
	public virtual EffectId IdEffect
	{
		get
		{
			return (base.GetParam(1, null) ?? "Drink").ToEnum(true);
		}
	}

	public virtual int Power
	{
		get
		{
			if (base.GetParam(2, null) == null)
			{
				return 100;
			}
			return base.GetParam(2, null).ToInt();
		}
	}

	public virtual int IdEle
	{
		get
		{
			if (!this.AliasEle.IsEmpty())
			{
				return EClass.sources.elements.alias[this.AliasEle].id;
			}
			return 0;
		}
	}

	public virtual string AliasEle
	{
		get
		{
			return this.N1;
		}
	}

	public virtual string N1
	{
		get
		{
			return base.GetParam(3, null);
		}
	}

	public virtual bool IsNeg
	{
		get
		{
			return this.owner.HasTag(CTAG.neg);
		}
	}

	public override bool IsThrowMainAction
	{
		get
		{
			return true;
		}
	}

	public override ThrowType ThrowType
	{
		get
		{
			return ThrowType.Potion;
		}
	}

	public override bool CanDrink(Chara c)
	{
		return true;
	}

	public override bool IsBlendBase
	{
		get
		{
			return this.IsBlend();
		}
	}

	public bool IsBlend()
	{
		EffectId idEffect = this.IdEffect;
		if (idEffect <= EffectId.Poison)
		{
			if (idEffect != EffectId.DrinkWater && idEffect != EffectId.Poison)
			{
				return false;
			}
		}
		else if (idEffect != EffectId.Love && idEffect != EffectId.Acidproof)
		{
			return false;
		}
		return true;
	}

	public override bool CanBlend(Thing t)
	{
		EffectId idEffect = this.IdEffect;
		if (idEffect <= EffectId.Poison)
		{
			if (idEffect == EffectId.DrinkWater)
			{
				return t.category.ignoreBless != 1 && t.id != "water" && t.id != "bucket";
			}
			if (idEffect != EffectId.Poison)
			{
				return false;
			}
		}
		else if (idEffect != EffectId.Love)
		{
			if (idEffect != EffectId.Acidproof)
			{
				return false;
			}
			return !t.isAcidproof && t.IsEquipmentOrRanged;
		}
		return t.IsFood;
	}

	public override void OnBlend(Thing t, Chara c)
	{
		EffectId idEffect = this.IdEffect;
		if (idEffect <= EffectId.Poison)
		{
			if (idEffect != EffectId.DrinkWater)
			{
				if (idEffect == EffectId.Poison)
				{
					TraitDrink.BlendPoison(c, t);
				}
			}
			else
			{
				BlessedState blessedState = this.owner.blessedState;
				t = t.Split(1);
				if (blessedState >= BlessedState.Blessed)
				{
					Msg.Say("waterBless", t, null, null, null);
					t.SetBlessedState(BlessedState.Blessed);
					c.Pick(t, true, true);
				}
				else if (blessedState <= BlessedState.Cursed)
				{
					Msg.Say("waterCurse", t, null, null, null);
					t.SetBlessedState(BlessedState.Cursed);
					c.Pick(t, true, true);
				}
				else
				{
					Msg.SayNothingHappen();
				}
			}
		}
		else if (idEffect != EffectId.Love)
		{
			if (idEffect == EffectId.Acidproof)
			{
				c.Say("coated", t, c, null, null);
				if (t.IsEquipment && this.owner.blessedState <= BlessedState.Cursed)
				{
					c.Say("blend_curse", t, null, null);
					t.ModEncLv(-1);
				}
				else
				{
					t.isAcidproof = true;
				}
			}
		}
		else
		{
			TraitDrink.BlendLove(c, t, false);
		}
		this.owner.ModNum(-1, true);
	}

	public static void BlendLove(Chara c, Thing t, bool dream = false)
	{
		Msg.Say("blend_love", t, null, null, null);
		Thing thing = t.Split(1);
		thing.elements.ModBase(dream ? 704 : 703, t.IsBlessed ? 15 : (t.IsCursed ? 5 : 10));
		c.Pick(thing, true, true);
	}

	public static void BlendPoison(Chara c, Thing t)
	{
		Msg.Say("blend_poison", t, null, null, null);
		Thing thing = t.Split(1);
		thing.elements.ModBase(702, t.IsBlessed ? 5 : (t.IsCursed ? 15 : 10));
		c.Pick(thing, true, true);
	}

	public override void OnDrink(Chara c)
	{
		ActEffect.Proc(this.IdEffect, this.Power, this.owner.blessedState, c, null, new ActRef
		{
			n1 = this.N1,
			isPerfume = (this is TraitPerfume)
		});
	}

	public override void OnThrowGround(Chara c, Point p)
	{
		if (this.IdEffect != EffectId.Drink)
		{
			bool isHostileAct = c.IsPCParty && this.IsNeg;
			EClass._map.SetLiquid(p.x, p.z, new CellEffect
			{
				id = 4,
				idEffect = this.IdEffect,
				power = this.Power,
				amount = 5,
				isHostileAct = isHostileAct,
				color = BaseTileMap.GetColorInt(this.owner.GetRandomColor(), this.owner.sourceRenderCard.colorMod),
				idEle = this.IdEle,
				n1 = this.N1,
				isBlessed = (this.owner.blessedState >= BlessedState.Blessed),
				isCursed = (this.owner.blessedState <= BlessedState.Cursed)
			});
			return;
		}
		EClass._map.SetLiquid(p.x, p.z, 1, 3);
	}

	public override Action GetHealAction(Chara c)
	{
		if (this.IdEffect == EffectId.Heal)
		{
			return delegate()
			{
				c.Drink(this.owner);
			};
		}
		return null;
	}
}
