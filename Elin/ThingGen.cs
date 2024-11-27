using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ThingGen : CardGen
{
	public static Thing _Create(string id, int idMat = -1, int lv = -1)
	{
		Thing thing = new Thing();
		CardRow s = EClass.sources.cards.map.TryGetValue(id, null);
		if (s == null)
		{
			Debug.LogError("exception: Item not found:" + id);
			id = "869";
			s = EClass.sources.cards.map.TryGetValue(id, null);
		}
		if (s.isOrigin)
		{
			CardRow cardRow = SpawnListThing.Get("origin_" + id, (SourceThing.Row a) => a.origin == s).Select(lv, -1);
			id = (((cardRow != null) ? cardRow.id : null) ?? (from a in EClass.sources.cards.rows
			where a.origin == s
			select a).First<CardRow>().id);
		}
		if (lv < 1)
		{
			lv = 1;
		}
		thing.Create(id, idMat, lv);
		if (thing.trait is TraitAmmo)
		{
			thing.SetNum(EClass.rnd(20) + 10);
		}
		return thing;
	}

	public static Thing TestCreate()
	{
		return ThingGen._Create(SpawnList.Get("thing", null, null).Select(-1, -1).id, -1, -1);
	}

	public static Thing CreateCurrency(int a, string id = "money")
	{
		return ThingGen.Create(id, -1, -1).SetNum(a);
	}

	public static Thing CreateParcel(string idLang = null, params Thing[] things)
	{
		Thing thing = ThingGen.Create("parcel", -1, -1);
		foreach (Thing c in things)
		{
			thing.AddCard(c);
		}
		thing.c_idRefName = idLang.lang();
		return thing;
	}

	public static Thing Create(string id, int idMat = -1, int lv = -1)
	{
		return ThingGen._Create(id, idMat, lv);
	}

	public static Thing Create(string id, string idMat)
	{
		return ThingGen.Create(id, idMat.IsEmpty() ? -1 : EClass.sources.materials.alias[idMat].id, -1);
	}

	public static Thing CreateFromFilter(string id, int lv = -1)
	{
		return ThingGen._Create(SpawnList.Get(id, null, null).Select(lv, -1).id, -1, lv);
	}

	public static Thing CreateRawMaterial(SourceMaterial.Row m)
	{
		Thing thing = ThingGen.Create(m.thing, -1, -1);
		thing.ChangeMaterial(m.id);
		return thing;
	}

	public static Thing CreateFromCategory(string idCat, int lv = -1)
	{
		return ThingGen._Create(SpawnListThing.Get("cat_" + idCat, (SourceThing.Row s) => EClass.sources.categories.map[s.category].IsChildOf(idCat)).Select(lv, -1).id, -1, lv);
	}

	public static Thing CreateFromTag(string idTag, int lv = -1)
	{
		return ThingGen._Create(SpawnListThing.Get("tag_" + idTag, (SourceThing.Row s) => s.tag.Contains(idTag)).Select(lv, -1).id, -1, lv);
	}

	public static Thing CreateBill(int pay, bool tax)
	{
		Thing thing = ThingGen.Create(tax ? "bill_tax" : "bill", -1, -1);
		thing.c_bill = pay;
		if (tax)
		{
			EClass.player.stats.taxBills += thing.c_bill;
			EClass.player.taxBills++;
		}
		else
		{
			EClass.player.unpaidBill += thing.c_bill;
		}
		return thing;
	}

	public static Thing CreateBlock(int id, int idMat)
	{
		Thing thing = ThingGen.Create(EClass.sources.blocks.rows[id].idThing, idMat, -1);
		thing.refVal = id;
		return thing;
	}

	public static Thing CreateFloor(int id, int idMat, bool platform = false)
	{
		SourceFloor.Row row = EClass.sources.floors.rows[id];
		Thing thing = ThingGen.Create(platform ? "platform" : "floor", idMat, -1);
		thing.refVal = id;
		return thing;
	}

	public static Thing CreateObj(int id, int idMat)
	{
		SourceObj.Row row = EClass.sources.objs.rows[id];
		Thing thing = ThingGen.Create("obj", idMat, -1);
		thing.refVal = id;
		return thing;
	}

	public static Thing CreateMap(string idSource = null, int lv = -1)
	{
		if (idSource.IsEmpty())
		{
			idSource = EClass.game.world.region.GetRandomSiteSource().id;
		}
		if (lv == -1)
		{
			lv = 1 + EClass.rnd(EClass.rnd(50) + 1);
		}
		return ThingGen.Create("map", -1, -1);
	}

	public static Thing CreatePlan(int ele)
	{
		Thing thing = ThingGen.Create("book_plan", -1, -1);
		thing.refVal = ele;
		return thing;
	}

	public static Thing CreateRecipe(string id)
	{
		Thing thing = ThingGen.Create("rp_random", -1, -1);
		thing.SetStr(53, id);
		return thing;
	}

	public static Thing CreateSpellbook(string alias, int num = 1)
	{
		return ThingGen.CreateSpellbook(EClass.sources.elements.alias[alias].id, num);
	}

	public static Thing CreateSpellbook(int ele, int num = 1)
	{
		Thing thing = ThingGen.Create("spellbook", -1, -1).SetNum(num);
		thing.refVal = ele;
		return thing;
	}

	public static Thing CreateScroll(int ele, int num = 1)
	{
		Thing thing = ThingGen.Create("scroll_random", -1, -1).SetNum(num);
		thing.refVal = ele;
		return thing;
	}

	public static Thing CreatePotion(int ele, int num = 1)
	{
		Thing thing = ThingGen.Create("potion", -1, -1).SetNum(num);
		thing.refVal = ele;
		return thing;
	}

	public static Thing CreatePerfume(int ele, int num = 1)
	{
		Thing thing = ThingGen.Create("perfume", -1, -1).SetNum(num);
		thing.refVal = ele;
		return thing;
	}

	public static Thing CreateCardboardBox(int uidZone = -1)
	{
		Thing thing = ThingGen.Create("cardboard_box", new string[]
		{
			"pine",
			"wood_birch",
			"wood_acacia",
			"oak",
			"cedar"
		}.RandomItem<string>());
		thing.things.DestroyAll(null);
		Spatial spatial = (uidZone == -1) ? null : EClass.game.spatials.map.TryGetValue(uidZone, null);
		if (spatial != null)
		{
			thing.c_idRefName = "sender_header".lang("sender_post".lang(spatial.Name, null, null, null, null), null, null, null, null);
		}
		return thing;
	}

	public static Thing CreateTreasure(string id, int lv, TreasureType type = TreasureType.Map)
	{
		Thing thing = ThingGen.Create(id, -1, lv);
		ThingGen.CreateTreasureContent(thing, lv, type, true);
		return thing;
	}

	public static void CreateTreasureContent(Thing t, int lv, TreasureType type, bool clearContent)
	{
		ThingGen.<>c__DisplayClass24_0 CS$<>8__locals1;
		CS$<>8__locals1.miracleChance = lv;
		int num = EClass.curve(lv, 20, 15, 75);
		bool flag = true;
		bool flag2 = true;
		CS$<>8__locals1.guaranteedMythical = ((type == TreasureType.BossQuest) ? 1 : 0);
		ThingGen.<CreateTreasureContent>g__ChangeSeed|24_0();
		t.AddEditorTag(EditorTag.PreciousContainer);
		if (clearContent)
		{
			t.things.DestroyAll(null);
		}
		switch (type)
		{
		case TreasureType.BossNefia:
		case TreasureType.BossQuest:
			t.Add("money", EClass.rndHalf(500 + num * 50), 1);
			t.Add("plat", EClass.rndHalf(Mathf.Min(3 + num / 10, 15)), 1);
			t.Add("rp_random", 1, lv);
			if (EClass.rnd(5) == 0)
			{
				t.Add("medal", EClass.rnd(3), 1);
			}
			else if (EClass.rnd(3) == 0)
			{
				t.Add("map_treasure", 1, EClass.rndHalf(lv + 10));
			}
			else
			{
				t.Add("book_skill", 1, lv);
			}
			t.c_lockLv /= 2;
			break;
		case TreasureType.Map:
			CS$<>8__locals1.miracleChance += 50;
			t.Add("money", EClass.rndHalf(1000 + num * 100), 1);
			t.Add("money2", EClass.rndHalf(Mathf.Min(3 + num / 10, 10)), 1);
			t.Add("medal", EClass.rnd(3), 1);
			break;
		case TreasureType.RandomChest:
			flag2 = false;
			CS$<>8__locals1.miracleChance /= 2;
			if (CS$<>8__locals1.miracleChance > 50)
			{
				CS$<>8__locals1.miracleChance = 50;
			}
			if (EClass.rnd(2) == 0)
			{
				t.Add("money", EClass.rndHalf(10 + num * 25), 1);
			}
			else if (EClass.rnd(2) == 0)
			{
				t.Add("money2", 1 + EClass.rnd(Mathf.Min(2 + num / 25, 5)), 1);
			}
			else
			{
				t.Add("plat", 1 + EClass.rnd(Mathf.Min(2 + num / 25, 5)), 1);
			}
			if (EClass._zone is Zone_Civilized)
			{
				flag = false;
			}
			else
			{
				if (EClass.rnd(10) == 0)
				{
					t.Add("map_treasure", 1, EClass.rndHalf(lv + 10));
				}
				if (EClass.rnd(3) == 0)
				{
					t.AddCard(ThingGen.Create("rp_random", -1, lv));
				}
				if (t.c_lockLv > 0 && EClass.rnd(3) == 0)
				{
					t.Add("medal", 1, 1);
				}
			}
			break;
		}
		if (type == TreasureType.RandomChest && EClass.rnd(2) == 0)
		{
			t.AddCard(ThingGen.CreateFromCategory("junk", lv));
		}
		else if (EClass.rnd(3) == 0)
		{
			t.Add("rp_random", 1, lv);
		}
		else if (type != TreasureType.Map && EClass.rnd(2) == 0)
		{
			Thing thing = ThingGen.CreateFromCategory("furniture", lv);
			if (!thing.IsContainer && (t.c_lockLv == 0 || thing.SelfWeight < 10000))
			{
				t.AddCard(thing);
			}
		}
		else
		{
			t.AddCard(ThingGen.CreateFromCategory("junk", lv));
		}
		if (EClass.rnd(3) == 0)
		{
			t.Add("book_ancient", 1, lv);
		}
		if (t.c_lockLv > 0)
		{
			CS$<>8__locals1.miracleChance += 20 + (int)Mathf.Sqrt((float)(t.c_lockLv * 5));
		}
		if (flag)
		{
			ThingGen.<CreateTreasureContent>g__SetRarity|24_1(100, ref CS$<>8__locals1);
			t.AddCard(ThingGen.CreateFromFilter("eq", lv));
			if (flag2)
			{
				ThingGen.<CreateTreasureContent>g__SetRarity|24_1(20, ref CS$<>8__locals1);
				t.AddCard(ThingGen.CreateFromFilter("eq", lv));
			}
		}
		Rand.SetSeed(-1);
	}

	public static void TryLickChest(Thing chest)
	{
		foreach (Chara chara in EClass._map.charas)
		{
			if (chara.HasElement(1412, 1) && chara.Dist(chest) < 5)
			{
				chara.Say("lick", chara, chest, null, null);
				chest.PlaySound("offering", 1f, true);
				chest.PlayEffect("mutation", true, 0f, default(Vector3));
				using (List<Thing>.Enumerator enumerator2 = chest.things.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						Thing thing = enumerator2.Current;
						thing.TryLickEnchant(chara, false, null, null);
					}
					break;
				}
			}
		}
	}

	[CompilerGenerated]
	internal static void <CreateTreasureContent>g__ChangeSeed|24_0()
	{
		EClass.player.seedChest++;
		Rand.SetSeed(EClass.game.seed + EClass.player.seedChest);
	}

	[CompilerGenerated]
	internal static void <CreateTreasureContent>g__SetRarity|24_1(int mtp, ref ThingGen.<>c__DisplayClass24_0 A_1)
	{
		ThingGen.<CreateTreasureContent>g__ChangeSeed|24_0();
		Rarity rarity = (A_1.miracleChance * mtp / 100 >= EClass.rnd(100)) ? ((EClass.rnd(20) == 0) ? Rarity.Mythical : Rarity.Legendary) : Rarity.Superior;
		if (A_1.guaranteedMythical > 0)
		{
			int guaranteedMythical = A_1.guaranteedMythical;
			A_1.guaranteedMythical = guaranteedMythical - 1;
			rarity = Rarity.Mythical;
		}
		CardBlueprint.Set(new CardBlueprint
		{
			rarity = rarity
		});
	}
}
