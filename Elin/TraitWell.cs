using System;

public class TraitWell : Trait
{
	public bool IsHoly
	{
		get
		{
			return this is TraitWellHoly;
		}
	}

	public override bool IsBlendBase
	{
		get
		{
			return true;
		}
	}

	public override bool HasCharges
	{
		get
		{
			return true;
		}
	}

	public override bool ShowCharges
	{
		get
		{
			return false;
		}
	}

	public int pollution
	{
		get
		{
			return this.owner.GetInt(26, null);
		}
	}

	public int Charges
	{
		get
		{
			if (!this.IsHoly)
			{
				return this.owner.c_charges;
			}
			return EClass.player.holyWell;
		}
		set
		{
			if (this.IsHoly)
			{
				EClass.player.holyWell = value;
				return;
			}
			this.owner.c_charges = value;
		}
	}

	public bool polluted
	{
		get
		{
			return this.pollution >= 10;
		}
	}

	public override bool CanBlend(Thing t)
	{
		return t.category.IsChildOf("drink");
	}

	public override void OnBlend(Thing t, Chara c)
	{
		bool polluted = this.polluted;
		if (!this.IsHoly)
		{
			this.ModCharges(1);
		}
		if ((t.id == "water" || t.id == "bucket") && t.IsBlessed)
		{
			if (this.IsHoly)
			{
				this.ModCharges(1);
			}
			if (this.polluted)
			{
				Msg.Say("unpolluted", this.owner, null, null, null);
				this.Charges = 1;
			}
			this.owner.SetInt(26, 0);
		}
		else
		{
			string name = this.owner.Name;
			this.owner.SetInt(26, this.pollution + EClass.rnd(3) + ((t.blessedState <= BlessedState.Cursed) ? 10 : 0) + (this.IsHoly ? 10 : 0));
			if (!polluted && this.polluted)
			{
				Msg.Say("polluted", name, null, null, null);
			}
		}
		t.ModNum(-1, true);
	}

	public override void OnCreate(int lv)
	{
		if (!this.IsHoly)
		{
			this.owner.c_charges = EClass.rnd(6) + 2;
		}
	}

	public override void TrySetAct(ActPlan p)
	{
		p.TrySetAct("actDrink", delegate()
		{
			if (this.Charges <= 0)
			{
				EClass.pc.Say("drinkWell_empty", EClass.pc, this.owner, null, null);
				return false;
			}
			EClass.pc.Say("drinkWell", EClass.pc, this.owner, null, null);
			EClass.pc.PlaySound("drink", 1f, true);
			EClass.pc.PlayAnime(AnimeID.Shiver, false);
			if (this.IsHoly || EClass.rnd(5) == 0)
			{
				ActEffect.Proc(EffectId.ModPotential, EClass.pc, null, (!this.polluted && (this.IsHoly || EClass.rnd(2) == 0)) ? 100 : -100, default(ActRef));
			}
			else if (EClass.rnd(5) == 0)
			{
				TraitWell.BadEffect(EClass.pc);
			}
			else if (EClass.rnd(4) == 0)
			{
				ActEffect.Proc(EffectId.Mutation, EClass.pc, null, 100, default(ActRef));
			}
			else if (EClass.rnd(EClass.debug.enable ? 2 : 10) == 0 && !this.polluted && !EClass.player.wellWished)
			{
				if (EClass.player.CountKeyItem("well_wish") > 0)
				{
					EClass.player.ModKeyItem("well_wish", -1, true);
					ActEffect.Proc(EffectId.Wish, EClass.pc, null, 10 + EClass.player.CountKeyItem("well_enhance") * 10, default(ActRef));
					EClass.player.wellWished = true;
				}
				else
				{
					Msg.SayNothingHappen();
				}
			}
			else if (this.polluted)
			{
				EClass.pc.Say("drinkWater_dirty", null, null);
				TraitWell.BadEffect(EClass.pc);
			}
			else
			{
				EClass.pc.Say("drinkWater_clear", null, null);
			}
			this.ModCharges(-1);
			return true;
		}, this.owner, null, 1, false, true, false);
	}

	public void ModCharges(int a)
	{
		this.Charges += a;
		if (this.Charges <= 0)
		{
			EClass.pc.Say("drinkWell_dry", EClass.pc, this.owner, null, null);
		}
	}

	public static void BadEffect(Chara c)
	{
		switch (EClass.rnd(7))
		{
		case 0:
			ActEffect.Proc(EffectId.Blind, c, null, 100, default(ActRef));
			return;
		case 1:
			ActEffect.Proc(EffectId.Paralyze, c, null, 100, default(ActRef));
			return;
		case 2:
			ActEffect.Proc(EffectId.Sleep, c, null, 100, default(ActRef));
			return;
		case 3:
			ActEffect.Proc(EffectId.Poison, c, null, 100, default(ActRef));
			return;
		case 4:
			ActEffect.Proc(EffectId.Faint, c, null, 100, default(ActRef));
			return;
		case 5:
			ActEffect.Proc(EffectId.Disease, c, null, 100, default(ActRef));
			return;
		default:
			ActEffect.Proc(EffectId.Confuse, c, null, 100, default(ActRef));
			return;
		}
	}

	public override void SetName(ref string s)
	{
		if (this.polluted)
		{
			s = "_polluted".lang(s, null, null, null, null);
		}
	}
}
