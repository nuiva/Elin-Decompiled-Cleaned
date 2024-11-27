using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TraitCrafter : Trait
{
	public override bool ShowFuelWindow
	{
		get
		{
			return false;
		}
	}

	public virtual Emo Icon
	{
		get
		{
			return Emo.none;
		}
	}

	public virtual int numIng
	{
		get
		{
			return 1;
		}
	}

	public virtual string IdSource
	{
		get
		{
			return "";
		}
	}

	public virtual TraitCrafter.AnimeType animeType
	{
		get
		{
			return TraitCrafter.AnimeType.Default;
		}
	}

	public virtual AnimeID IdAnimeProgress
	{
		get
		{
			return AnimeID.HitObj;
		}
	}

	public virtual string idSoundProgress
	{
		get
		{
			return "";
		}
	}

	public virtual string idSoundComplete
	{
		get
		{
			return null;
		}
	}

	public virtual bool StopSoundProgress
	{
		get
		{
			return false;
		}
	}

	public override bool IsNightOnlyLight
	{
		get
		{
			return false;
		}
	}

	public virtual bool CanUseFromInventory
	{
		get
		{
			return true;
		}
	}

	public override bool HoldAsDefaultInteraction
	{
		get
		{
			return true;
		}
	}

	public virtual string idSoundBG
	{
		get
		{
			return null;
		}
	}

	public virtual string CrafterTitle
	{
		get
		{
			return "";
		}
	}

	public virtual bool AutoTurnOff
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsConsumeIng
	{
		get
		{
			return true;
		}
	}

	public virtual bool CloseOnComplete
	{
		get
		{
			return false;
		}
	}

	public virtual int CostSP
	{
		get
		{
			return 1;
		}
	}

	public virtual string IDReqEle(RecipeSource r)
	{
		return base.GetParam(1, null) ?? "handicraft";
	}

	public virtual bool IsCraftIngredient(Card c, int idx)
	{
		foreach (SourceRecipe.Row r in EClass.sources.recipes.rows)
		{
			if (idx == 1)
			{
				Card card = LayerDragGrid.Instance.buttons[0].Card;
				if (!this.IsIngredient(0, r, card) || (card == c && card.Num < 2))
				{
					continue;
				}
			}
			if (this.IsIngredient(idx, r, c))
			{
				return true;
			}
		}
		return false;
	}

	public bool IsIngredient(int idx, SourceRecipe.Row r, Card c)
	{
		if (r.factory != this.IdSource || c == null)
		{
			return false;
		}
		if (c.c_isImportant && this.ShouldConsumeIng(r, idx))
		{
			return false;
		}
		string[] array = (idx == 0) ? r.ing1 : r.ing2;
		if (r.type.ToEnum(true) == TraitCrafter.MixType.Grind && idx == 1)
		{
			if (r.tag.Contains("rust") && c.encLV >= 0)
			{
				return false;
			}
			if (r.tag.Contains("mod_eject"))
			{
				if (c.sockets == null)
				{
					return false;
				}
				bool flag = false;
				using (List<int>.Enumerator enumerator = c.sockets.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current != 0)
						{
							flag = true;
						}
					}
				}
				if (!flag)
				{
					return false;
				}
			}
		}
		foreach (string text in array)
		{
			if (r.tag.Contains("noCarbone") && c.material.alias == "carbone")
			{
				return false;
			}
			if (text.StartsWith('#'))
			{
				string text2 = text.Replace("#", "");
				if (c.category.IsChildOf(text2) && this.IsIngredient(text2, c))
				{
					return true;
				}
			}
			else
			{
				string[] array3 = text.Split('@', StringSplitOptions.None);
				if (array3.Length > 1)
				{
					if (c.id != array3[0] && c.sourceCard._origin != array3[0])
					{
						return false;
					}
					SourceChara.Row row = c.refCard as SourceChara.Row;
					if (row != null && row.race_row.tag.Contains(array3[1]))
					{
						return true;
					}
					if (c.material.tag.Contains(array3[1]))
					{
						return true;
					}
				}
				else
				{
					if (text == "any" && !c.IsUnique && !c.IsImportant && !c.trait.CanOnlyCarry)
					{
						return true;
					}
					if (c.id == text || c.sourceCard._origin == text)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public virtual bool IsIngredient(string cat, Card c)
	{
		return true;
	}

	public int GetSortVal(SourceRecipe.Row r)
	{
		int num = r.id;
		string[] ing = r.ing1;
		for (int i = 0; i < ing.Length; i++)
		{
			if (ing[i].Contains('@'))
			{
				num -= 10000;
			}
		}
		return num;
	}

	public virtual int GetDuration(AI_UseCrafter ai, int costSp)
	{
		int a = 1;
		int num = this.GetSource(ai).time * 100;
		int num2 = 80;
		Card pc = EClass.pc;
		Recipe recipe = ai.recipe;
		return Mathf.Max(a, num / (num2 + pc.Evalue(this.IDReqEle((recipe != null) ? recipe.source : null)) * 5));
	}

	public virtual int GetCostSp(AI_UseCrafter ai)
	{
		return this.GetSource(ai).sp;
	}

	public SourceRecipe.Row GetSource(AI_UseCrafter ai)
	{
		List<SourceRecipe.Row> list = new List<SourceRecipe.Row>();
		foreach (SourceRecipe.Row row in EClass.sources.recipes.rows)
		{
			if (row.factory == this.IdSource)
			{
				list.Add(row);
			}
		}
		for (int i = 0; i < this.numIng; i++)
		{
			foreach (SourceRecipe.Row row2 in EClass.sources.recipes.rows)
			{
				if (i >= ai.ings.Count || !this.IsIngredient(i, row2, ai.ings[i]))
				{
					list.Remove(row2);
				}
			}
		}
		if (list.Count == 0)
		{
			return null;
		}
		list.Sort((SourceRecipe.Row a, SourceRecipe.Row b) => this.GetSortVal(a) - this.GetSortVal(b));
		return list[0];
	}

	public virtual bool ShouldConsumeIng(SourceRecipe.Row item, int index)
	{
		if (this.IsFactory)
		{
			return true;
		}
		if (item == null)
		{
			return false;
		}
		int id = item.id;
		return id - 47 > 1 || index == 0;
	}

	public virtual Thing Craft(AI_UseCrafter ai)
	{
		Thing thing = ai.ings[0];
		Thing thing2 = (this.numIng > 1) ? ai.ings[1] : null;
		SourceRecipe.Row source = this.GetSource(ai);
		if (source == null)
		{
			return null;
		}
		if (!EClass.player.knownCraft.Contains(source.id))
		{
			SE.Play("idea");
			Msg.Say("newKnownCraft");
			EClass.player.knownCraft.Add(source.id);
			if (LayerDragGrid.Instance)
			{
				LayerDragGrid.Instance.info.Refresh();
			}
		}
		string text = source.thing;
		TraitCrafter.MixType mixType = source.type.ToEnum(true);
		int num = source.num.Calc(0, 0, 0);
		TraitCrafter.<>c__DisplayClass47_0 CS$<>8__locals1;
		CS$<>8__locals1.t = null;
		switch (mixType)
		{
		case TraitCrafter.MixType.Food:
			CS$<>8__locals1.t = CraftUtil.MixIngredients(text, ai.ings, CraftUtil.MixType.General, 0, EClass.pc);
			break;
		case TraitCrafter.MixType.Resource:
		{
			string[] array = text.Split('%', StringSplitOptions.None);
			CS$<>8__locals1.t = CraftUtil.MixIngredients(ThingGen.Create(array[0], (array.Length > 1) ? EClass.sources.materials.alias[array[1]].id : thing.material.id, -1), ai.ings, CraftUtil.MixType.General, 999, EClass.pc).Thing;
			break;
		}
		case TraitCrafter.MixType.Dye:
			CS$<>8__locals1.t = ThingGen.Create(text, thing2.material.id, -1);
			break;
		case TraitCrafter.MixType.Butcher:
			text = SpawnListThing.Get("butcher", (SourceThing.Row a) => a.Category.id == "bodyparts").Select(-1, -1).id;
			CS$<>8__locals1.t = ThingGen.Create(text, -1, -1);
			break;
		case TraitCrafter.MixType.Grind:
			if (source.tag.Contains("rust"))
			{
				EClass.pc.Say("polish", EClass.pc, ai.ings[1], null, null);
				ai.ings[1].ModEncLv(1);
				ai.ings[0].ModNum(-1, true);
			}
			if (source.tag.Contains("mod_eject"))
			{
				ai.ings[1].EjectSockets();
				ai.ings[0].ModNum(-1, true);
			}
			break;
		case TraitCrafter.MixType.Sculpture:
		{
			CS$<>8__locals1.t = ThingGen.Create(text, -1, -1);
			List<CardRow> list = EClass.player.codex.ListKills();
			list.Add(EClass.sources.cards.map["putty"]);
			list.Add(EClass.sources.cards.map["snail"]);
			CardRow cardRow = list.RandomItemWeighted((CardRow a) => (float)Mathf.Max(50 - a.LV, 1));
			CS$<>8__locals1.t.c_idRefCard = cardRow.id;
			CS$<>8__locals1.t.ChangeMaterial(thing.material);
			CS$<>8__locals1.t.SetEncLv(Mathf.Min(EClass.rnd(EClass.rnd(Mathf.Max(5 + EClass.pc.Evalue(258) - cardRow.LV, 1))), 12));
			CS$<>8__locals1.t = CraftUtil.MixIngredients(CS$<>8__locals1.t, ai.ings, CraftUtil.MixType.General, 999, EClass.pc).Thing;
			break;
		}
		case TraitCrafter.MixType.Talisman:
		{
			int num2 = EClass.pc.Evalue(1418);
			Thing thing3 = ai.ings[1];
			SourceElement.Row source2 = (thing3.trait as TraitSpellbook).source;
			int num3 = thing3.c_charges * source2.charge * (100 + num2 * 50) / 500 + 1;
			int num4 = 100;
			Thing thing4 = ThingGen.Create("talisman", -1, -1).SetNum(num3);
			thing4.refVal = source2.id;
			thing4.encLV = num4 * (100 + num2 * 10) / 100;
			thing.ammoData = thing4;
			thing.c_ammo = num3;
			EClass.pc.Say("talisman", thing, thing4, null, null);
			thing3.Destroy();
			break;
		}
		case TraitCrafter.MixType.Scratch:
		{
			TraitCrafter.<>c__DisplayClass47_1 CS$<>8__locals2;
			CS$<>8__locals2.claimed = false;
			TraitCrafter.<Craft>g__Prize|47_2(20, "medal", "save", false, ref CS$<>8__locals1, ref CS$<>8__locals2);
			TraitCrafter.<Craft>g__Prize|47_2(10, "plat", "save", false, ref CS$<>8__locals1, ref CS$<>8__locals2);
			TraitCrafter.<Craft>g__Prize|47_2(10, "furniture", "nice", true, ref CS$<>8__locals1, ref CS$<>8__locals2);
			TraitCrafter.<Craft>g__Prize|47_2(4, "plamo_box", "nice", false, ref CS$<>8__locals1, ref CS$<>8__locals2);
			TraitCrafter.<Craft>g__Prize|47_2(4, "food", "", false, ref CS$<>8__locals1, ref CS$<>8__locals2);
			TraitCrafter.<Craft>g__Prize|47_2(1, "casino_coin", "", false, ref CS$<>8__locals1, ref CS$<>8__locals2);
			break;
		}
		case TraitCrafter.MixType.Incubator:
			TraitFoodEggFertilized.Incubate(ai.ings[0], this.owner.pos, this.owner);
			break;
		default:
			CS$<>8__locals1.t = ThingGen.Create(text, -1, -1);
			if (text == "gene")
			{
				if (ai.ings[0].c_DNA != null)
				{
					CS$<>8__locals1.t.MakeRefFrom(ai.ings[0].c_idRefCard);
					CS$<>8__locals1.t.c_DNA = ai.ings[0].c_DNA;
					CS$<>8__locals1.t.c_DNA.GenerateWithGene(DNA.Type.Inferior, CS$<>8__locals1.t, null);
				}
			}
			else
			{
				CS$<>8__locals1.t = CraftUtil.MixIngredients(CS$<>8__locals1.t, ai.ings, CraftUtil.MixType.General, 999, EClass.pc).Thing;
			}
			break;
		}
		if (CS$<>8__locals1.t != null)
		{
			if (CS$<>8__locals1.t.HasElement(1229, 1))
			{
				num = 1;
			}
			if (CS$<>8__locals1.t.HasElement(704, 1))
			{
				num = 1;
			}
			if (CS$<>8__locals1.t.HasElement(703, 1))
			{
				num = 1;
			}
			CS$<>8__locals1.t.SetNum(num);
		}
		return CS$<>8__locals1.t;
	}

	public override void TrySetAct(ActPlan p)
	{
		if (this.IsFactory)
		{
			Thing _t = this.owner.Thing;
			p.TrySetAct("craft", delegate()
			{
				if (EClass.player.recipes.ListSources(_t, null).Count > 0)
				{
					EClass.ui.AddLayer<LayerCraft>().SetFactory(_t);
				}
				else
				{
					Msg.Say("noRecipes");
				}
				return false;
			}, _t, CursorSystem.Craft, 1, false, true, false);
			return;
		}
		if (this is TraitIncubator && !EClass._zone.IsPCFaction)
		{
			return;
		}
		p.TrySetAct(this.CrafterTitle, delegate()
		{
			LayerDragGrid.CreateCraft(this);
			return false;
		}, this.owner, null, 1, false, true, false);
	}

	public override bool CanUse(Chara c)
	{
		return this.CanUseFromInventory;
	}

	public override bool OnUse(Chara c)
	{
		if (this.IsFactory)
		{
			Thing thing = this.owner.Thing;
			if (EClass.player.recipes.ListSources(thing, null).Count > 0)
			{
				EClass.ui.AddLayer<LayerCraft>().SetFactory(thing);
			}
			else
			{
				Msg.Say("noRecipes");
			}
			return false;
		}
		LayerDragGrid.CreateCraft(this);
		return false;
	}

	[CompilerGenerated]
	internal static void <Craft>g__Prize|47_2(int chance, string s, string col, bool cat, ref TraitCrafter.<>c__DisplayClass47_0 A_4, ref TraitCrafter.<>c__DisplayClass47_1 A_5)
	{
		if (A_5.claimed || EClass.rnd(chance) != 0)
		{
			return;
		}
		A_4.t = (cat ? ThingGen.CreateFromCategory(s, EClass.pc.LV) : ThingGen.Create(s, -1, EClass.pc.LV));
		A_5.claimed = true;
		if (col != "")
		{
			Msg.SetColor(col);
		}
	}

	public enum MixType
	{
		None,
		Food,
		Resource,
		Dye,
		Butcher,
		Grind,
		Sculpture,
		Talisman,
		Scratch,
		Incubator
	}

	public enum AnimeType
	{
		Default,
		Microwave,
		Pot
	}
}
