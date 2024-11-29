using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TraitBaseSpellbook : TraitScroll
{
	public virtual TraitBaseSpellbook.Type BookType
	{
		get
		{
			return TraitBaseSpellbook.Type.Spell;
		}
	}

	public virtual int Difficulty
	{
		get
		{
			return 10 + this.owner.LV;
		}
	}

	public override bool CanRead(Chara c)
	{
		return !c.isBlind;
	}

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

	public override float MTPValue
	{
		get
		{
			return 10f;
		}
	}

	public override int GetActDuration(Chara c)
	{
		return Mathf.Max(this.Difficulty, 100) * Mathf.Max(100 - (int)Mathf.Sqrt((float)c.Evalue(285)) * 7, 10) / 400;
	}

	public override void OnCreate(int lv)
	{
		this.owner.c_charges = 1 + EClass.rnd(4) + EClass.rnd(EClass.rnd(4) + 1);
		switch (this.BookType)
		{
		case TraitBaseSpellbook.Type.Ancient:
		{
			int max = Lang.GetList("ancientbook").Length;
			this.owner.refVal = EClass.rnd(Mathf.Clamp(EClass.rnd(lv / 5 + 5), 1, max));
			return;
		}
		case TraitBaseSpellbook.Type.Spell:
			break;
		case TraitBaseSpellbook.Type.RandomSpell:
			this.owner.refVal = TraitBaseSpellbook.selecter.Select(lv);
			return;
		case TraitBaseSpellbook.Type.Ero:
		{
			IEnumerable<SourceChara.Row> ie = from a in EClass.sources.charas.rows
			where !a.name.IsEmpty() && a.name.Length > 2 && a.name[0] != '<' && a.race != "god"
			select a;
			this.owner.c_idRefName = ie.RandomItem<SourceChara.Row>().id;
			return;
		}
		case TraitBaseSpellbook.Type.Dojin:
			this.owner.c_idRefName = EClass.game.religions.dictAll.RandomItem<string, Religion>().id;
			break;
		default:
			return;
		}
	}

	public override bool TryProgress(AIProgress p)
	{
		TraitBaseSpellbook.<>c__DisplayClass15_0 CS$<>8__locals1;
		CS$<>8__locals1.c = p.owner;
		if (this.BookType == TraitBaseSpellbook.Type.Ancient && this.owner.isOn)
		{
			if (CS$<>8__locals1.c.IsPC)
			{
				CS$<>8__locals1.c.Say("alreadyDecoded", null, null);
			}
			return false;
		}
		if (CS$<>8__locals1.c.isBlind)
		{
			CS$<>8__locals1.c.Say("blinded", CS$<>8__locals1.c, null, null);
			return false;
		}
		CS$<>8__locals1.diff = Mathf.Max(1, this.Difficulty * ((this.owner.blessedState >= BlessedState.Blessed) ? 75 : ((this.owner.blessedState <= BlessedState.Cursed) ? 300 : 100)) / 100);
		CS$<>8__locals1.check = Mathf.Max(1, CS$<>8__locals1.c.Evalue(this.eleParent) * (100 + CS$<>8__locals1.c.Evalue(285) * 10) / 100);
		if (TraitBaseSpellbook.<TryProgress>g__ReadCheck|15_0(ref CS$<>8__locals1) || EClass.debug.godMode || this.owner.HasEditorTag(EditorTag.NoReadFail))
		{
			return true;
		}
		TraitBaseSpellbook.ReadFailEffect(CS$<>8__locals1.c);
		this.ModCharge(CS$<>8__locals1.c, -1);
		return false;
	}

	public static void ReadFailEffect(Chara c)
	{
		if (EClass.rnd(2) == 0)
		{
			c.Say("spell_fail_mana", c, null, null);
			c.mana.Mod(-c.mana.max / (c.IsPC ? 2 : 5));
			return;
		}
		if (EClass.rnd(3) == 0)
		{
			c.Say("spell_fail_confuse", c, null, null);
			c.AddCondition<ConConfuse>(100, false);
			return;
		}
		if (EClass.rnd(3) == 0)
		{
			c.Say("spell_fail_monster", c, null, null);
			c.PlaySound("spell_funnel", 1f, true);
			for (int i = 0; i < 1 + EClass._zone.DangerLv / 15 + EClass.rnd(3 + EClass._zone.DangerLv / 15); i++)
			{
				Chara chara = CharaGen.CreateFromFilter("c_readFail", EClass._zone.DangerLv, -1);
				EClass._zone.AddCard(chara, c.pos.GetNearestPoint(false, false, true, false));
				chara.pos.PlayEffect("teleport");
			}
			return;
		}
		c.Say("spell_fail_teleport", c, null, null);
		if (c.IsPCFaction && !c.IsPC)
		{
			c.SayNothingHappans();
			return;
		}
		ActEffect.Proc(EffectId.Teleport, c, null, 100, default(ActRef));
	}

	public override void OnRead(Chara c)
	{
		bool flag = this.BookType == TraitBaseSpellbook.Type.Spell || this.BookType == TraitBaseSpellbook.Type.RandomSpell;
		int a = -1;
		string name = this.owner.Name;
		if (c.IsPCParty)
		{
			Thing thing = this.owner.Thing;
			if (thing != null)
			{
				thing.Identify(true, IDTSource.SuperiorIdentify);
			}
			this.owner.isOn = true;
		}
		switch (this.BookType)
		{
		case TraitBaseSpellbook.Type.Ancient:
			c.Say("book_decode", c, name, null);
			if (!c.IsPC)
			{
				this.ModCharge(c, -this.owner.c_charges);
			}
			if (c.IsPC)
			{
				Guild.Mage.AddContribution(5 + this.owner.refVal * 2);
			}
			break;
		case TraitBaseSpellbook.Type.Spell:
		case TraitBaseSpellbook.Type.RandomSpell:
			if (c.IsPC)
			{
				c.GainAbility(this.source.id, 100);
			}
			this.ModCharge(c, -1);
			break;
		case TraitBaseSpellbook.Type.Ero:
		case TraitBaseSpellbook.Type.Dojin:
		{
			c.PlaySound("wow", 1f, true);
			c.Say("book_decode", c, this.owner, null, null);
			if (!c.IsPC)
			{
				c.Talk("wow", null, null, false);
			}
			TraitBaseSpellbook.Type bookType = this.BookType;
			if (bookType != TraitBaseSpellbook.Type.Ero)
			{
				if (bookType == TraitBaseSpellbook.Type.Dojin)
				{
					Religion religion = EClass.game.religions.dictAll.TryGetValue(this.owner.c_idRefName, null) ?? EClass.game.religions.Eyth;
					if (c.IsPC)
					{
						EClass.player.ModKarma(-1);
					}
					c.AddCondition<ConInsane>(500, false);
					c.AddCondition<ConFaint>(100, false);
					if (!c.IsPC && c.faith != religion)
					{
						if (!c.source.faith.IsEmpty())
						{
							c.Say("faith_stands", c, null, null);
						}
						else
						{
							religion.JoinFaith(c);
							a = -this.owner.c_charges;
						}
					}
				}
			}
			else
			{
				if (c.IsPC)
				{
					EClass.pc.SAN.Mod(-(EClass.rnd(5) + 1));
				}
				if (!this.owner.c_idRefName.IsEmpty())
				{
					CardRow cardRow = EClass.sources.cards.map[this.owner.c_idRefName];
					c.Say("learn_weakspot", c, cardRow.GetName(), null);
					if (c.IsPC)
					{
						EClass.player.codex.AddWeakspot(cardRow.id);
					}
				}
				ActEffect.Proc(EffectId.Sleep, c, null, 100, default(ActRef));
			}
			this.ModCharge(c, a);
			break;
		}
		}
		c.ModExp(285, 180);
		if (flag)
		{
			c.ModExp(307, 200);
		}
		if (c.IsPC)
		{
			LayerAbility.Redraw();
		}
	}

	public void ModCharge(Chara c, int a = -1)
	{
		this.owner.ModCharge(a, false);
		LayerInventory.SetDirtyAll(false);
		if (this.owner.c_charges <= 0)
		{
			c.Say("spellbookCrumble", this.owner, null, null);
			this.owner.ModNum(-1, true);
		}
	}

	public override void SetName(ref string s)
	{
		if (!this.owner.IsIdentified)
		{
			return;
		}
		TraitBaseSpellbook.Type bookType = this.BookType;
		if (bookType != TraitBaseSpellbook.Type.Ancient)
		{
			if (bookType - TraitBaseSpellbook.Type.Ero > 1)
			{
				return;
			}
			string c_idRefName = this.owner.c_idRefName;
			if (c_idRefName.IsEmpty())
			{
				return;
			}
			string @ref = (this.BookType == TraitBaseSpellbook.Type.Dojin) ? EClass.game.religions.dictAll[c_idRefName].Name : EClass.sources.charas.map[c_idRefName].GetName();
			s = "_'s".lang(@ref, s, null, null, null);
		}
		else
		{
			s = "_titled".lang(Lang.GetList("ancientbook")[this.owner.refVal], s, null, null, null);
			if (this.owner.isOn)
			{
				s = "_deciphered ".lang(s, null, null, null, null);
				return;
			}
		}
	}

	[CompilerGenerated]
	internal static bool <TryProgress>g__ReadCheck|15_0(ref TraitBaseSpellbook.<>c__DisplayClass15_0 A_0)
	{
		return (EClass.rnd(4) == 0 || (!A_0.c.isConfused && !A_0.c.HasCondition<ConDim>())) && (A_0.check > A_0.diff * 3 || EClass.rnd(A_0.check * 30) >= A_0.diff);
	}

	public static ElementSelecter selecter = new ElementSelecter
	{
		type = "B",
		useDomain = true
	};

	public enum Type
	{
		Ancient,
		Spell,
		RandomSpell,
		Ero,
		Dojin
	}
}
