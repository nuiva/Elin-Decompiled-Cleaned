using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using UnityEngine;

public class GameDate : Date
{
	public void AdvanceSec(int a)
	{
		base.sec += a;
		if (base.sec >= 60)
		{
			this.AdvanceMin(base.sec / 60);
			base.sec %= 60;
		}
	}

	public void AdvanceMin(int a)
	{
		EClass.game.sessionMin += a;
		EClass.player.stats.mins += a;
		base.min += a;
		if (base.min >= 60)
		{
			while (base.min >= 60)
			{
				base.min -= 60;
				this.AdvanceHour();
			}
			EClass.screen.RefreshGrading();
		}
		else if (base.min % 3 == 2)
		{
			EClass.screen.RefreshGrading();
		}
		if (EClass._map.footmarks.Count > 0)
		{
			for (int i = EClass._map.footmarks.Count - 1; i >= 0; i--)
			{
				Footmark footmark = EClass._map.footmarks[i];
				footmark.remaining--;
				if (footmark.remaining <= 0)
				{
					CellDetail detail = footmark.pos.detail;
					if (detail != null && detail.footmark == footmark)
					{
						detail.footmark = null;
						footmark.pos.cell.TryDespawnDetail();
					}
					EClass._map.footmarks.RemoveAt(i);
				}
			}
		}
		EClass.player.countNewline++;
		if (EClass.player.countNewline > EClass.core.config.game.newlineCount)
		{
			Msg.NewLine();
			EClass.player.countNewline = 0;
		}
		EClass.screen.pcOrbit.OnChangeMin();
		foreach (ZoneEvent zoneEvent in EClass._zone.events.list)
		{
			zoneEvent.minElapsed += a;
		}
	}

	public void AdvanceHour()
	{
		new VirtualDate(0)
		{
			IsRealTime = true
		}.SimulateHour();
		int hour = base.hour;
		base.hour = hour + 1;
		if (base.hour >= 24)
		{
			base.hour = 0;
			this.AdvanceDay();
		}
		if (!this.shaken && EClass.rnd(24) == 0 && !EClass._zone.IsRegion)
		{
			Msg.Say("earthquake");
			if (!EClass.core.config.graphic.disableShake)
			{
				Shaker.ShakeCam("earthquake", 1f);
			}
			this.shaken = true;
		}
		EClass.scene.OnChangeHour();
		EClass.world.weather.OnChangeHour();
		EClass.player.OnAdvanceHour();
		EClass.game.quests.OnAdvanceHour();
		EClass._zone.OnAdvanceHour();
		if (EClass._zone.IsRegion)
		{
			EClass._zone.Region.CheckRandomSites();
		}
		foreach (Chara chara in EClass.game.cards.globalCharas.Values)
		{
			if (!chara.IsPCParty && chara.currentZone != EClass.game.activeZone && chara.trait.UseGlobalGoal)
			{
				if (chara.global.goal == null && !chara.IsPCFaction)
				{
					GlobalGoalAdv globalGoalAdv = new GlobalGoalAdv();
					globalGoalAdv.SetOwner(chara);
					globalGoalAdv.Start();
				}
				if (chara.global.goal != null)
				{
					chara.global.goal.AdvanceHour();
				}
			}
		}
		EClass.pc.RecalculateFOV();
		if (base.hour == 5)
		{
			this.ShipGoods();
			this.ShipPackages();
			this.ShipLetter();
			if (EClass.rnd(30) == 0 && EClass.game.cards.listPackage.Count <= 2)
			{
				this.ShipRandomPackages();
			}
			if (base.month == 10)
			{
				Tutorial.Reserve("season", null);
			}
			if (base.month == 11)
			{
				Tutorial.Reserve("season2", null);
				return;
			}
			Tutorial.Remove("season2");
		}
	}

	public void AdvanceDay()
	{
		int num = base.day;
		base.day = num + 1;
		base.min = 0;
		EClass.player.stats.days++;
		if (EClass.player.stats.days >= 90)
		{
			Tutorial.Reserve("death_penalty", null);
		}
		if (base.day > 30)
		{
			base.day = 1;
			this.AdvanceMonth();
		}
		EClass.world.CreateDayData();
		EClass.Sound.Play("owl");
		if (!EClass.player.prayed && EClass.pc.Evalue(1655) > 0)
		{
			ActPray.TryPray(EClass.pc, true);
		}
		Msg.Say("endDay");
		this.shaken = false;
		EClass.player.OnAdvanceDay();
		EClass.game.relations.UpdateRelations();
		EClass.pc.faction.OnAdvanceDay();
		foreach (Chara chara in EClass.game.cards.listAdv)
		{
			if (!chara.IsPCFaction && EClass.rnd(10) == 0 && (chara.isDead || chara.currentZone == null || chara.currentZone.id == "somewhere"))
			{
				chara.SetHomeZone(EClass.world.region.ListTowns().RandomItem<Zone>());
				chara.Revive(null, false);
				chara.MoveZone(chara.homeZone, ZoneTransition.EnterState.Auto);
			}
		}
		if (EClass.pc.homeZone != null && EClass.pc.homeZone.mainFaction == EClass.pc.faction)
		{
			WidgetSticky.Add(new StickyHomeReport(), true);
		}
		if (EClass.player.stats.days >= 7 && EClass.game.cards.globalCharas.Find("fiama").currentZone == EClass.game.StartZone && EClass.game.quests.GetGlobal("fiama_starter_gift") == null && !EClass.game.quests.IsCompleted("fiama_starter_gift"))
		{
			EClass.game.quests.Add("fiama_starter_gift", "fiama");
		}
		if (EClass.game.quests.IsCompleted("exploration"))
		{
			Player.Flags flags = EClass.player.flags;
			num = flags.daysAfterQuestExploration;
			flags.daysAfterQuestExploration = num + 1;
			if (EClass.player.flags.daysAfterQuestExploration >= 7 && !EClass.player.flags.magicChestSent)
			{
				EClass.player.flags.magicChestSent = true;
				Thing thing = ThingGen.Create("parchment", -1, -1);
				thing.SetStr(53, "letter_magic_chest");
				Thing p = ThingGen.CreateParcel(null, new Thing[]
				{
					ThingGen.Create("container_magic", -1, -1),
					thing
				});
				EClass.world.SendPackage(p);
			}
		}
	}

	public void AdvanceMonth()
	{
		int month = base.month;
		base.month = month + 1;
		if (base.month > 12)
		{
			base.month = 1;
			this.AdvanceYear();
		}
		EClass.player.stats.months++;
		EClass.player.nums.OnAdvanceMonth();
		if (base.month % 2 == 0)
		{
			EClass.player.holyWell++;
		}
		EClass._map.RefreshAllTiles();
		foreach (Thing thing in EClass._map.things)
		{
			if (thing.renderer.hasActor)
			{
				thing.renderer.RefreshSprite();
			}
		}
		EClass.pc.faction.OnAdvanceMonth();
	}

	public void AdvanceYear()
	{
		int year = base.year;
		base.year = year + 1;
		EClass.player.wellWished = false;
		EClass.player.nums.OnAdvanceYear();
		EClass.world.SendPackage(ThingGen.Create("gift_newyear", -1, -1));
	}

	public void ShipGoods()
	{
		Thing container_shipping = EClass.game.cards.container_shipping;
		if (container_shipping.things.Count == 0)
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		List<Thing> list = new List<Thing>();
		List<string> list2 = new List<string>();
		Zone zone = EClass.game.spatials.Find(EClass.player.uidLastShippedZone);
		if (zone == null || zone.branch == null)
		{
			zone = EClass.pc.homeZone;
		}
		ShippingResult shippingResult = new ShippingResult();
		shippingResult.rawDate = EClass.world.date.GetRaw(0);
		shippingResult.uidZone = zone.uid;
		shippingResult.total = EClass.player.stats.shipMoney;
		shippingResult.hearthLv = zone.branch.lv;
		shippingResult.hearthExp = zone.branch.exp;
		shippingResult.debt = EClass.player.debt;
		foreach (Thing thing in container_shipping.things)
		{
			if (thing.trait.CanBeShipped)
			{
				int price = thing.GetPrice(CurrencyType.Money, true, PriceType.Shipping, null);
				int num5 = price * thing.Num;
				num3 += num5;
				num += thing.Num;
				num2 += EClass.rndHalf(thing.Num * Mathf.Min(15 + price, 10000) / 100 + 1);
				list.Add(thing);
				shippingResult.items.Add(new ShippingResult.Item
				{
					text = thing.Name,
					income = num5
				});
			}
		}
		if (list.Count == 0)
		{
			return;
		}
		num2 = num2 / 2 + 1;
		shippingResult.hearthExpGained = num2;
		EClass.pc.homeBranch.log.Add(Msg.Say("shipped_collect"), null);
		foreach (string text in list2)
		{
			EClass.pc.homeBranch.log.Add(text, null);
		}
		int shippingBonus = EClass.player.stats.GetShippingBonus(EClass.player.stats.shipMoney);
		EClass.player.stats.shipNum += num;
		EClass.player.stats.shipMoney += num3;
		int shippingBonus2 = EClass.player.stats.GetShippingBonus(EClass.player.stats.shipMoney);
		if (shippingBonus2 > shippingBonus)
		{
			num4 = shippingBonus2 - shippingBonus;
		}
		foreach (Thing thing2 in list)
		{
			thing2.Destroy();
		}
		Thing thing3 = null;
		Thing thing4 = null;
		string text2 = "";
		if (num3 != 0)
		{
			thing3 = ThingGen.Create("money", -1, -1).SetNum(num3);
		}
		if (num4 != 0)
		{
			thing4 = ThingGen.Create("money2", -1, -1).SetNum(num4);
		}
		if (thing3 != null && thing4 != null)
		{
			text2 = "_and".lang(thing3.Name, thing4.Name, null, null, null);
			SE.Pay();
		}
		else if (thing3 != null || thing4 != null)
		{
			text2 = ((thing3 != null) ? thing3 : thing4).Name;
		}
		EClass.pc.homeBranch.log.Add(Msg.Say((text2 == "") ? "shipped_none" : "shipped", num.ToString() ?? "", text2, null, null), FontColor.Good);
		EClass.player.shippingResults.Add(shippingResult);
		EClass.player.showShippingResult = EClass.core.config.game.showShippingResult;
		for (int i = 0; i < EClass.player.shippingResults.Count - 10; i++)
		{
			EClass.player.shippingResults.RemoveAt(0);
		}
		zone.branch.statistics.ship += num3;
		zone.branch.ModExp(num2);
		if (thing3 != null)
		{
			EClass.pc.Pick(thing3, true, true);
		}
		if (thing4 != null)
		{
			EClass.pc.Pick(thing4, true, true);
		}
	}

	public void ShipPackages()
	{
		Thing container_deliver = EClass.game.cards.container_deliver;
		if (container_deliver.things.Count == 0)
		{
			return;
		}
		int num = 0;
		while (container_deliver.things.Count > 0)
		{
			num++;
			if (num > 100)
			{
				Debug.Log("too many tries");
				return;
			}
			int uidZone = 0;
			foreach (Thing thing in container_deliver.things)
			{
				int @int = thing.GetInt(102, null);
				if (@int != 0)
				{
					uidZone = @int;
					thing.SetInt(102, 0);
					break;
				}
			}
			int num2 = 20;
			Thing thing2 = ThingGen.CreateCardboardBox(uidZone);
			for (int i = container_deliver.things.Count - 1; i >= 0; i--)
			{
				Thing c = container_deliver.things[i];
				thing2.AddCard(c);
				num2 += 5;
				if (thing2.things.IsFull(0))
				{
					break;
				}
			}
			EClass.world.SendPackage(thing2);
			Thing bill = ThingGen.CreateBill(num2, false);
			EClass.pc.faction.TryPayBill(bill);
		}
	}

	public void ShipLetter()
	{
		int num = -1;
		int lutz = EClass.player.flags.lutz;
		if (EClass.player.stats.days >= 2 && lutz <= 0)
		{
			num = 1;
		}
		else if (EClass.player.stats.days >= 5 && lutz <= 1)
		{
			num = 2;
		}
		else if (EClass.player.stats.days >= 8 && lutz <= 2)
		{
			num = 3;
		}
		else if (EClass.player.stats.days >= 11 && lutz <= 3)
		{
			num = 4;
		}
		else if (EClass.player.stats.days >= 15 && lutz <= 4)
		{
			num = 5;
		}
		else if (EClass.player.stats.days >= 17 && lutz <= 5)
		{
			num = 6;
		}
		else if (EClass.player.stats.days >= 30 && lutz <= 6)
		{
			num = 7;
		}
		else if (EClass.player.stats.days >= 50 && lutz <= 7)
		{
			num = 8;
		}
		if (num != -1)
		{
			EClass.player.flags.lutz = num;
			Thing thing = ThingGen.Create("letter", -1, -1);
			thing.SetStr(53, "lutz_" + num.ToString());
			EClass.world.SendPackage(thing);
		}
	}

	public void ShipRandomPackages()
	{
		GameDate.<>c__DisplayClass11_0 CS$<>8__locals1;
		CS$<>8__locals1.box = ThingGen.CreateCardboardBox(-1);
		TraitContainer traitContainer = CS$<>8__locals1.box.trait as TraitContainer;
		bool flag = EClass.pc.homeBranch.policies.IsActive(2708, -1);
		if (EClass.rnd(EClass.debug.enable ? 1 : 100) == 0 && !EClass.player.flags.statueShipped)
		{
			EClass.player.flags.statueShipped = true;
			GameDate.<ShipRandomPackages>g__Add|11_0("statue_weird", 1, ref CS$<>8__locals1);
			flag = false;
		}
		else if (EClass.rnd(10) == 0)
		{
			GameDate.<ShipRandomPackages>g__Add|11_0("234", 1, ref CS$<>8__locals1);
		}
		else if (EClass.rnd(5) == 0)
		{
			GameDate.<ShipRandomPackages>g__AddThing|11_1(ThingGen.CreateFromCategory("junk", -1), 1, ref CS$<>8__locals1);
		}
		else if (EClass.rnd(10) == 0)
		{
			GameDate.<ShipRandomPackages>g__AddThing|11_1(ThingGen.CreateFromTag("garbage", -1), 1, ref CS$<>8__locals1);
		}
		else if (EClass.rnd(8) == 0)
		{
			CardRow cardRow = SpawnList.Get("chara", null, null).Select(EClass.pc.LV + 10, -1);
			traitContainer.PutChara(cardRow.id);
			flag = false;
		}
		else if (EClass.rnd(8) == 0)
		{
			GameDate.<ShipRandomPackages>g__Add|11_0("plat", 1 + EClass.rnd(4), ref CS$<>8__locals1);
			flag = false;
		}
		else if (EClass.rnd(8) == 0)
		{
			GameDate.<ShipRandomPackages>g__Add|11_0("money2", 1 + EClass.rnd(4), ref CS$<>8__locals1);
			flag = false;
		}
		else
		{
			string id = "trash2";
			if (EClass.rnd(3) == 0)
			{
				id = "trash1";
			}
			if (EClass.rnd(3) == 0)
			{
				id = ((EClass.rnd(3) == 0) ? "529" : "1170");
			}
			if (EClass.rnd(5) == 0)
			{
				id = "_poop";
			}
			if (EClass.rnd(100) == 0)
			{
				id = "goodness";
				flag = false;
			}
			GameDate.<ShipRandomPackages>g__Add|11_0(id, 1, ref CS$<>8__locals1);
		}
		if (!flag)
		{
			EClass.world.SendPackage(CS$<>8__locals1.box);
		}
	}

	[CompilerGenerated]
	internal static void <ShipRandomPackages>g__Add|11_0(string id, int num, ref GameDate.<>c__DisplayClass11_0 A_2)
	{
		GameDate.<ShipRandomPackages>g__AddThing|11_1(ThingGen.Create(id, -1, -1), num, ref A_2);
	}

	[CompilerGenerated]
	internal static void <ShipRandomPackages>g__AddThing|11_1(Thing t, int num, ref GameDate.<>c__DisplayClass11_0 A_2)
	{
		t.SetNum(num);
		A_2.box.AddCard(t);
	}

	[JsonProperty]
	public bool shaken;

	public const int minPerRound = 5;
}
