using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using UnityEngine;

public class FactionBranch : EClass
{
	public int GetNextExp(int _lv = -1)
	{
		if (_lv == -1)
		{
			_lv = this.lv;
		}
		return _lv * _lv * 100 + 100;
	}

	public int Evalue(int ele)
	{
		SourceElement.Row row = EClass.sources.elements.map[ele];
		if (row.category == "policy")
		{
			return this.policies.GetValue(ele);
		}
		if (this.HasNetwork && row.tag.Contains("network"))
		{
			return EClass.pc.faction.elements.Value(ele) + this.elements.Value(ele);
		}
		return this.elements.Value(ele);
	}

	public int Money
	{
		get
		{
			return this.resources.money.value;
		}
	}

	public int Worth
	{
		get
		{
			return this.resources.worth.value;
		}
	}

	public int NumHeirloom
	{
		get
		{
			return 4 + this.Evalue(2120);
		}
	}

	public int MaxAP
	{
		get
		{
			return 4 + this.Evalue(2115) + this.Evalue(3900) * 5 + this.Evalue(3709) * this.lv;
		}
	}

	public int MaxPopulation
	{
		get
		{
			return 5 + this.Evalue(2204);
		}
	}

	public int MaxSoil
	{
		get
		{
			return ((int)(Mathf.Sqrt((float)(EClass._map.bounds.Width * EClass._map.bounds.Height)) * 3f) + this.Evalue(2200) * 5) * (100 + this.Evalue(3700) * 25) / 100;
		}
	}

	public int ContentLV
	{
		get
		{
			return Mathf.Max(1, this.lv * 4 + EClass.scene.elomap.GetRoadDist(EClass._zone.x, EClass._zone.y) - 4);
		}
	}

	public int DangerLV
	{
		get
		{
			return Mathf.Max(1, this.ContentLV - (int)Mathf.Sqrt((float)this.Evalue(2704)) * 2 + (int)Mathf.Sqrt((float)this.Evalue(2706)) * 4);
		}
	}

	public bool HasNetwork
	{
		get
		{
			return this.lv >= 5;
		}
	}

	public bool IsTaxFree
	{
		get
		{
			return this.policies.IsActive(2514, -1);
		}
	}

	public int GetProductBonus(Chara c)
	{
		if (c.isDead || c.IsPCParty)
		{
			return 0;
		}
		return Mathf.Max(1, (90 + this.Evalue(2116) / 2) * this.efficiency / 100);
	}

	public void RefreshEfficiency()
	{
		int num = 100;
		int num2 = this.CountMembers(FactionMemberType.Default, false);
		FactionBranch.<>c__DisplayClass52_0 CS$<>8__locals1;
		CS$<>8__locals1.ration = 0;
		foreach (Chara chara in this.members)
		{
			if (chara.memberType == FactionMemberType.Default)
			{
				if (chara.IsPCParty || chara.homeBranch == null || chara.homeBranch.owner == null)
				{
					return;
				}
				foreach (Hobby h in chara.ListHobbies(true))
				{
					FactionBranch.<RefreshEfficiency>g__TryAdd|52_0(h, ref CS$<>8__locals1);
				}
				foreach (Hobby h2 in chara.ListWorks(true))
				{
					FactionBranch.<RefreshEfficiency>g__TryAdd|52_0(h2, ref CS$<>8__locals1);
				}
			}
		}
		if (num2 > this.MaxPopulation)
		{
			num -= (num2 - this.MaxPopulation) * 20 * 100 / (100 + 20 * (int)Mathf.Sqrt((float)CS$<>8__locals1.ration));
		}
		this.efficiency = num;
	}

	public string RankText
	{
		get
		{
			return (0.01f * (float)this.exp + (float)this.rank).ToString("F2");
		}
	}

	public string TextLv
	{
		get
		{
			return this.lv.ToString() + " (" + (100f * (float)this.exp / (float)this.GetNextExp(-1)).ToString("F1") + ")";
		}
	}

	public bool IsStartBranch
	{
		get
		{
			return this.owner.id == "startSite";
		}
	}

	public ElementContainerZone elements
	{
		get
		{
			return this.owner.elements;
		}
	}

	public void Abandon()
	{
	}

	public void OnCreate(Zone zone)
	{
		this.SetOwner(zone);
		this.stash = ThingGen.Create("container_salary", -1, -1);
		this.stash.RemoveThings();
		this.faith = EClass.game.religions.Eyth;
		this.dateFound = EClass.world.date.GetRaw(0);
	}

	public void SetOwner(Zone zone)
	{
		this.owner = zone;
		this.resources.SetOwner(this);
		this.researches.SetOwner(this);
		this.policies.SetOwner(this);
		this.happiness.SetOwner(this);
		this.expeditions.SetOwner(this);
		this.meetings.SetOwner(this);
		foreach (Chara chara in EClass.game.cards.globalCharas.Values)
		{
			if (chara == null)
			{
				Debug.LogError("exception: c==null");
			}
			else if (EClass.pc == null)
			{
				Debug.LogError("exception: pc==null");
			}
			else if (chara.homeZone == zone && chara.faction == EClass.pc.faction)
			{
				this.members.Add(chara);
				EClass.pc.faction.charaElements.OnAddMemeber(chara);
			}
		}
		this.RefreshEfficiency();
	}

	public void OnActivateZone()
	{
		this.incomeInn = 0;
		this.incomeTourism = 0;
		foreach (Chara chara in this.members)
		{
			if (chara.IsAliveInCurrentZone && !chara.pos.IsInBounds)
			{
				chara.MoveImmediate(EClass._map.GetCenterPos().GetNearestPoint(false, true, true, false), true, true);
			}
			chara.RefreshWorkElements(this.elements);
		}
		this.ValidateUpgradePolicies();
	}

	public void OnAfterSimulate()
	{
		if (!EClass.game.isLoading)
		{
			this.GetDailyIncome();
		}
	}

	public void OnUnloadMap()
	{
	}

	public void OnSimulateHour(VirtualDate date)
	{
		int num = this.CountMembers(FactionMemberType.Default, true);
		int starveChance = Mathf.Max(num - this.MaxPopulation, 0);
		this.members.ForeachReverse(delegate(Chara c)
		{
			if (c.IsPCParty || !c.IsAliveInCurrentZone || c.noMove)
			{
				return;
			}
			int starveChance;
			if (EClass.rnd(24 * this.members.Count) < starveChance)
			{
				starveChance = starveChance;
				starveChance--;
			}
			else
			{
				c.TickWork(date);
			}
			if (!date.IsRealTime && EClass.rnd(3) != 0 && c.memberType == FactionMemberType.Default)
			{
				c.hunger.Mod(1);
				if (c.hunger.GetPhase() >= 3)
				{
					Thing meal = this.GetMeal(c);
					if (meal != null)
					{
						FoodEffect.Proc(c, meal);
						meal.Destroy();
						return;
					}
					c.hunger.Mod(-50);
				}
			}
		});
		this.expeditions.OnSimulateHour();
		this.meetings.OnSimulateHour(date);
		this.policies.OnSimulateHour(date);
		int num2 = this.Evalue(3707);
		int num3 = this.Evalue(3705) + this.Evalue(3710) * 2;
		int num4 = 4 + num3 * 4;
		int num5 = Mathf.Max(1, (100 + this.Evalue(2205) * 5 + this.Evalue(3703) * 100 - num2 * 80) / 10);
		int num6 = 10;
		if (this.policies.IsActive(2709, -1))
		{
			num6 = 0;
		}
		if (EClass.world.date.IsNight)
		{
			num5 /= 2;
		}
		if ((date.IsRealTime || num2 > 0) && !EClass.debug.enable && EClass.rnd(num5) == 0 && !EClass.pc.IsDeadOrSleeping && EClass._map.CountHostile() < num4)
		{
			int num7 = 1 + EClass.rnd(num2 + num3 + 1);
			for (int i = 0; i < num7; i++)
			{
				EClass._zone.SpawnMob(null, SpawnSetting.HomeEnemy(this.DangerLV));
			}
		}
		if ((date.IsRealTime || num2 == 0) && num6 != 0 && EClass.rnd(num6) == 0 && EClass._map.CountWildAnimal() < num4)
		{
			EClass._zone.SpawnMob(null, SpawnSetting.HomeWild(5));
		}
		if (EClass.rnd(5) == 0 && this.policies.IsActive(2810, -1))
		{
			int num8 = 3 + this.lv + this.Evalue(2206) / 5 + this.Evalue(3702) * 2 + this.Evalue(2202) / 2;
			num8 = num8 * (100 + this.Evalue(3702) * 20 + this.Evalue(2206)) / 100;
			num8 = num8 * (100 + (int)Mathf.Sqrt((float)this.Evalue(2811)) * 3) / 100;
			if (EClass._map.CountGuest() < num8)
			{
				Chara chara;
				if (this.policies.IsActive(2822, -1) && Mathf.Sqrt((float)(this.Evalue(2822) / 2)) + 5f >= (float)EClass.rnd(100))
				{
					chara = CharaGen.CreateWealthy(this.ContentLV);
					EClass._zone.AddCard(chara, EClass._zone.GetSpawnPos(SpawnPosition.Random, 100) ?? EClass._map.GetRandomSurface(false, true, false));
				}
				else
				{
					chara = EClass._zone.SpawnMob(null, SpawnSetting.HomeGuest(this.ContentLV));
				}
				if (chara != null && (chara.id == "nun_mother" || chara.id == "prostitute") && this.policies.IsActive(2710, -1))
				{
					chara.Destroy();
					chara = null;
				}
				if (chara != null)
				{
					this.statistics.visitor++;
					chara.memberType = FactionMemberType.Guest;
					chara.SetInt(34, EClass.world.date.GetRaw(0));
					chara.c_allowance = chara.LV * 100;
					if (chara.IsWealthy)
					{
						chara.c_allowance *= 10;
					}
					if (date.IsRealTime)
					{
						Msg.Say("guestArrive", chara.Name, null, null, null);
					}
					else
					{
						chara.TryAssignBed();
					}
				}
			}
		}
		foreach (Chara chara2 in EClass._map.charas)
		{
			if (chara2.memberType == FactionMemberType.Guest && ((chara2.c_allowance <= 0 && EClass.rnd(2) == 0) || chara2.GetInt(34, null) + 10080 < EClass.world.date.GetRaw(0)))
			{
				if (!chara2.IsGlobal)
				{
					chara2.Destroy();
				}
				chara2.ClearBed(null);
				break;
			}
		}
		if (date.hour == 5)
		{
			this.DailyOutcome(date);
			this.GenerateGarbage(date);
			if (!date.IsRealTime)
			{
				this.AutoClean();
			}
			this.CalcTourismIncome();
			this.CalcInnIncome();
			if (date.IsRealTime)
			{
				this.GetDailyIncome();
			}
		}
		if (!EClass.player.simulatingZone && date.hour == 6)
		{
			this.ReceivePackages(date);
		}
		if (!date.IsRealTime && date.hour % 8 == 0)
		{
			foreach (Chara c2 in (from c in EClass._map.charas
			where c.memberType == FactionMemberType.Guest
			select c).ToList<Chara>())
			{
				for (int j = 0; j < 3; j++)
				{
					AI_Shopping.TryShop(c2, false);
				}
			}
			AI_Shopping.TryRestock(EClass.pc, false);
		}
		if (EClass.rnd(24) == 0 || EClass.debug.enable)
		{
			foreach (Thing thing in EClass._map.things)
			{
				if (EClass.rnd(20) == 0 && thing.trait is TraitFoodEggFertilized && thing.pos.FindThing<TraitBed>() != null)
				{
					Chara chara3 = TraitFoodEggFertilized.Incubate(thing, thing.pos, null);
					thing.ModNum(-1, true);
					if (date.IsRealTime)
					{
						chara3.PlaySound("egg", 1f, true);
						break;
					}
					break;
				}
			}
		}
		this.resources.SetDirty();
	}

	public void AutoClean()
	{
		foreach (Chara chara in this.members)
		{
			if (!chara.IsPCParty && chara.ExistsOnMap && chara.memberType == FactionMemberType.Default)
			{
				Hobby hobby = chara.GetWork("Clean") ?? chara.GetWork("Chore");
				if (hobby != null)
				{
					int num = (5 + EClass.rnd(5)) * hobby.GetEfficiency(chara) / 100;
					for (int i = 0; i < num; i++)
					{
						Thing thingToClean = AI_Haul.GetThingToClean(chara);
						if (thingToClean == null)
						{
							break;
						}
						EClass._zone.TryAddThingInSharedContainer(thingToClean, null, true, false, null, true);
					}
				}
			}
		}
	}

	public void OnSimulateDay(VirtualDate date)
	{
		if (this.owner == null || this.owner.mainFaction != EClass.Home)
		{
			return;
		}
		this.researches.OnSimulateDay();
		this.resources.OnSimulateDay();
		this.happiness.OnSimulateDay();
		if (!date.IsRealTime)
		{
			BranchMap branchMap = date.GetBranchMap();
			foreach (Chara chara in this.members)
			{
				if (!chara.IsPCParty && chara.memberType != FactionMemberType.Livestock && !chara.faith.IsEyth && !chara.c_isPrayed && branchMap.altarMap.Contains(chara.faith.id))
				{
					AI_Pray.Pray(chara, true);
				}
			}
		}
		foreach (Chara chara2 in this.members)
		{
			chara2.c_isPrayed = false;
			chara2.c_isTrained = false;
		}
	}

	public void OnAdvanceDay()
	{
		int num = 0;
		foreach (Chara chara in this.members)
		{
			if (!chara.IsPC && !chara.isDead && chara.memberType == FactionMemberType.Default)
			{
				if (EClass.rnd(3) == 0)
				{
					num++;
				}
				if (chara.GetWork("Pioneer") != null)
				{
					num += 3;
				}
			}
		}
		this.ModExp(num);
	}

	public void OnSimulateMonth(VirtualDate date)
	{
		this.lastStatistics = this.statistics;
		this.statistics = new FactionBranch.Statistics();
	}

	public void GenerateGarbage(VirtualDate date)
	{
		FactionBranch.<>c__DisplayClass72_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.sortChance = 40 + this.GetCivility();
		CS$<>8__locals1.unsortedCount = 0;
		foreach (Chara chara in this.members)
		{
			if (!chara.IsPCParty && chara.ExistsOnMap)
			{
				Hobby hobby = chara.GetWork("Clean") ?? chara.GetWork("Chore");
				if (hobby != null)
				{
					CS$<>8__locals1.sortChance += 20 * hobby.GetEfficiency(chara) / 100;
				}
			}
		}
		int num = this.Evalue(2702);
		int num2 = EClass.rnd(2);
		for (int i = 0; i < num2; i++)
		{
			if (num == 0 || EClass.rnd(num + (EClass.debug.enable ? 100000 : 10)) == 0)
			{
				this.<GenerateGarbage>g__Generate|72_0(null, ref CS$<>8__locals1);
			}
		}
		foreach (Chara chara2 in this.members)
		{
			if (!chara2.IsPCParty && chara2.ExistsOnMap && (num == 0 || EClass.rnd(num + (EClass.debug.enable ? 100000 : 10)) == 0))
			{
				this.<GenerateGarbage>g__Generate|72_0(chara2, ref CS$<>8__locals1);
			}
		}
	}

	public bool TryTrash(Thing t)
	{
		Thing thing;
		if (t.id == "_poop")
		{
			thing = EClass._map.props.installed.FindEmptyContainer<TraitContainerCompost>(t);
		}
		else if (!t.isFireproof)
		{
			thing = EClass._map.props.installed.FindEmptyContainer<TraitContainerBurnable>(t);
		}
		else
		{
			thing = EClass._map.props.installed.FindEmptyContainer<TraitContainerUnburnable>(t);
		}
		if (thing != null)
		{
			thing.AddCard(t);
			return true;
		}
		return EClass._zone.TryAddThingInSpot<TraitSpotGarbage>(t, false, false);
	}

	public void ReceivePackages(VirtualDate date)
	{
		if (EClass.player.simulatingZone)
		{
			return;
		}
		List<Thing> listPackage = EClass.game.cards.listPackage;
		if (EClass.player.stats.days >= 2 && !EClass.player.flags.elinGift && Steam.HasDLC(ID_DLC.BackerReward))
		{
			listPackage.Add(ThingGen.Create("gift", -1, -1));
			EClass.player.flags.elinGift = true;
		}
		if (listPackage.Count == 0)
		{
			return;
		}
		SE.Play("notice");
		int num = 0;
		foreach (Thing thing in listPackage)
		{
			if (thing.id != "bill")
			{
				num++;
			}
			this.PutInMailBox(thing, thing.id == "cardboard_box" || thing.id == "gift", true);
			WidgetPopText.Say("popDeliver".lang(thing.Name, null, null, null, null), FontColor.Default, null);
		}
		Msg.Say("deliver_arrive", num.ToString() ?? "", null, null, null);
		listPackage.Clear();
	}

	public void DailyOutcome(VirtualDate date)
	{
		FactionBranch.<>c__DisplayClass75_0 CS$<>8__locals1 = new FactionBranch.<>c__DisplayClass75_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.date = date;
		Thing thing = null;
		foreach (Chara m in this.members)
		{
			FactionBranch.<>c__DisplayClass75_1 CS$<>8__locals2;
			CS$<>8__locals2.m = m;
			if (!CS$<>8__locals2.m.IsPCParty && CS$<>8__locals2.m.ExistsOnMap)
			{
				CS$<>8__locals2.m.RefreshWorkElements(this.elements);
				if (CS$<>8__locals2.m.memberType == FactionMemberType.Livestock)
				{
					if (thing == null)
					{
						thing = (EClass._map.Stocked.Find("pasture", -1, -1, true) ?? EClass._map.Installed.Find("pasture", -1, -1, false));
					}
					if (thing != null)
					{
						if (CS$<>8__locals2.m.race.breeder >= EClass.rnd(2500 - (int)Mathf.Sqrt((float)(this.Evalue(2827) * 100))))
						{
							if (EClass.rnd(3) != 0)
							{
								Thing t = CS$<>8__locals2.m.MakeEgg(CS$<>8__locals1.date.IsRealTime, 1, CS$<>8__locals1.date.IsRealTime);
								if (!CS$<>8__locals1.date.IsRealTime)
								{
									CS$<>8__locals2.m.TryPutShared(t, null, true);
								}
							}
							else
							{
								Thing t2 = CS$<>8__locals2.m.MakeMilk(CS$<>8__locals1.date.IsRealTime, 1, CS$<>8__locals1.date.IsRealTime);
								if (!CS$<>8__locals1.date.IsRealTime)
								{
									CS$<>8__locals2.m.TryPutShared(t2, null, true);
								}
							}
						}
						if (CS$<>8__locals2.m.HaveFur())
						{
							Chara m2 = CS$<>8__locals2.m;
							int c_fur = m2.c_fur;
							m2.c_fur = c_fur + 1;
						}
						thing.ModNum(-1, true);
						if (thing.isDestroyed || thing.Num == 0)
						{
							thing = null;
						}
					}
				}
				else
				{
					foreach (Hobby h in CS$<>8__locals2.m.ListHobbies(true))
					{
						CS$<>8__locals1.<DailyOutcome>g__GetOutcome|1(h, ref CS$<>8__locals2);
					}
					foreach (Hobby h2 in CS$<>8__locals2.m.ListWorks(true))
					{
						CS$<>8__locals1.<DailyOutcome>g__GetOutcome|1(h2, ref CS$<>8__locals2);
					}
				}
			}
		}
		int soilCost = EClass._zone.GetSoilCost();
		int num = Mathf.Min(100, 70 + (this.MaxSoil - soilCost));
		CS$<>8__locals1.flower = 5;
		CS$<>8__locals1.lv = 1;
		EClass._map.bounds.ForeachCell(delegate(Cell cell)
		{
			if (cell.obj != 0 && cell.sourceObj.tag.Contains("flower"))
			{
				PlantData plantData = cell.TryGetPlant();
				int flower;
				if (plantData != null && plantData.seed != null)
				{
					CS$<>8__locals1.lv += plantData.seed.encLV + 1;
				}
				else
				{
					flower = CS$<>8__locals1.lv;
					CS$<>8__locals1.lv = flower + 1;
				}
				flower = CS$<>8__locals1.flower;
				CS$<>8__locals1.flower = flower + 1;
			}
		});
		CS$<>8__locals1.lv /= CS$<>8__locals1.flower;
		int num2 = 0;
		foreach (Thing thing2 in EClass._map.things)
		{
			if (thing2.IsInstalled && thing2.trait is TraitBeekeep && !thing2.things.IsFull(0))
			{
				CS$<>8__locals1.flower -= 3 + EClass.rnd(5 + num2 * 4);
				num2++;
				if (CS$<>8__locals1.flower < 0)
				{
					break;
				}
				if (EClass.rnd(100) <= num)
				{
					Thing thing3 = ThingGen.Create("honey", -1, -1);
					thing3.SetEncLv(CS$<>8__locals1.lv / 10);
					thing3.elements.SetBase(2, EClass.curve(CS$<>8__locals1.lv, 50, 10, 80), 0);
					thing2.AddThing(thing3, true, -1, -1);
				}
			}
		}
	}

	public void PutInMailBox(Thing t, bool outside = false, bool install = true)
	{
		if (!outside)
		{
			Thing mailBox = this.GetMailBox();
			if (mailBox != null)
			{
				mailBox.AddCard(t);
				return;
			}
		}
		Point mailBoxPos = this.GetMailBoxPos();
		EClass._zone.AddCard(t, mailBoxPos);
		if (install)
		{
			t.Install();
		}
	}

	public Thing GetMailBox()
	{
		return EClass._map.props.installed.FindEmptyContainer<TraitMailPost>();
	}

	public Point GetMailBoxPos()
	{
		Thing thing = this.GetMailBox();
		if (thing == null)
		{
			thing = EClass._map.props.installed.Find<TraitCoreZone>();
		}
		if (thing != null)
		{
			return thing.pos.GetNearestPoint(false, true, false, true).Clamp(true);
		}
		return EClass._map.GetCenterPos();
	}

	public int GetResidentTax()
	{
		if (!this.policies.IsActive(2512, 30))
		{
			return 0;
		}
		int num = 0;
		int num2 = this.policies.IsActive(2500, 30) ? this.Evalue(2500) : 0;
		int num3 = this.policies.IsActive(2501, 30) ? this.Evalue(2501) : 0;
		int num4 = 50 + (int)Mathf.Sqrt((float)this.Evalue(2512)) * 5;
		foreach (Chara chara in this.members)
		{
			if (!chara.IsPC && chara.memberType == FactionMemberType.Default)
			{
				bool isWealthy = chara.IsWealthy;
				int num5 = 0;
				foreach (Hobby hobby in chara.ListWorks(true).Concat(chara.ListHobbies(true)))
				{
					int num6 = hobby.source.tax * 100 / 100;
					if (num6 > num5)
					{
						num5 = num6;
					}
				}
				int num7 = ((isWealthy ? 50 : 10) + chara.LV * 2) * num5 / 100 * num4 / 100;
				if (isWealthy && num2 > 0)
				{
					num7 = num7 * (150 + (int)Mathf.Sqrt((float)num2) * 5) / 100;
				}
				if (num3 > 0)
				{
					num7 += (80 + (int)Mathf.Sqrt((float)num3) * 5) * chara.faith.source.tax / 100;
				}
				num7 = num7 * this.efficiency / 100;
				num += num7;
				if (num7 > 0)
				{
					this.Log("bTax", num7.ToString() ?? "", chara.Name, null, null);
				}
			}
		}
		this.statistics.tax += num;
		return num;
	}

	public void CalcInnIncome()
	{
		int num = this.CountGuests();
		if (num == 0)
		{
			return;
		}
		int num2 = 0;
		int num3 = 0;
		foreach (Thing thing in EClass._map.things)
		{
			if (thing.IsInstalled)
			{
				TraitBed traitBed = thing.trait as TraitBed;
				if (traitBed != null && traitBed.owner.c_bedType == BedType.guest)
				{
					num2++;
					num3 += traitBed.owner.LV;
				}
			}
		}
		num = Mathf.Min(num, num2);
		if (num == 0)
		{
			return;
		}
		num3 /= num2;
		num3 = num3 * (100 + 5 * (int)Mathf.Sqrt((float)this.Evalue(2812))) / 100;
		float num4 = 10f + Mathf.Sqrt((float)num) * 10f;
		if (this.policies.IsActive(2813, -1))
		{
			num4 += Mathf.Sqrt((float)this.CountWealthyGuests()) * 50f * (80f + 5f * Mathf.Sqrt((float)this.Evalue(2813))) / 100f;
		}
		if (this.policies.IsActive(2812, -1))
		{
			num4 = num4 * (float)(100 + num3) / 100f;
		}
		num4 = Mathf.Min(num4, Mathf.Sqrt((float)this.Worth) / 15f + 5f);
		this.incomeInn += EClass.rndHalf((int)num4) + EClass.rndHalf((int)num4);
	}

	public void CalcTourismIncome()
	{
		int num = this.CountGuests();
		if (num == 0)
		{
			return;
		}
		float num2 = 10f + Mathf.Sqrt((float)num) * 10f;
		if (this.policies.IsActive(2815, -1))
		{
			num2 += Mathf.Sqrt((float)this.CountWealthyGuests()) * 50f * (80f + 5f * Mathf.Sqrt((float)this.Evalue(2815))) / 100f;
		}
		num2 = Mathf.Min(num2, Mathf.Sqrt((float)this.tourism) / 5f);
		this.incomeTourism += EClass.rndHalf((int)num2) + EClass.rndHalf((int)num2);
	}

	public void GetDailyIncome()
	{
		this.<GetDailyIncome>g__GetIncome|82_0(ref this.incomeShop, ref this.statistics.shop, "getIncomeShop", false);
		this.<GetDailyIncome>g__GetIncome|82_0(ref this.incomeInn, ref this.statistics.inn, "getIncomeInn", true);
		this.<GetDailyIncome>g__GetIncome|82_0(ref this.incomeTourism, ref this.statistics.tourism, "getIncomeTourism", true);
	}

	public Thing GetMeal(Chara c)
	{
		Thing thing = EClass._zone.TryGetThingFromSharedContainer((Thing t) => c.CanEat(t, true));
		if (thing != null)
		{
			thing = thing.Split(1);
		}
		return thing;
	}

	public void OnClaimZone()
	{
		if (EClass.debug.allHomeSkill)
		{
			foreach (SourceElement.Row row in from a in EClass.sources.elements.rows
			where a.category == "tech"
			select a)
			{
				this.elements.SetBase(row.id, 1, 0);
			}
			using (IEnumerator<SourceElement.Row> enumerator = (from a in EClass.sources.elements.rows
			where a.category == "policy" && !a.tag.Contains("hidden")
			select a).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SourceElement.Row row2 = enumerator.Current;
					this.policies.AddPolicy(row2.id, true);
				}
				goto IL_1B0;
			}
		}
		this.elements.SetBase(2003, 1, 0);
		this.elements.SetBase(4002, 1, 0);
		this.elements.SetBase(2115, 1, 0);
		this.elements.SetBase(2204, 1, 0);
		this.elements.SetBase(2120, 1, 0);
		this.policies.AddPolicy(2512, true);
		this.policies.AddPolicy(2702, true);
		this.policies.AddPolicy(2703, true);
		this.policies.AddPolicy(2516, true);
		this.policies.AddPolicy(2515, true);
		this.policies.AddPolicy(2514, true);
		IL_1B0:
		if (EClass.pc.faction.CountTaxFreeLand() < 3)
		{
			this.policies.Activate(2514);
		}
		Element element = EClass._zone.ListLandFeats()[0];
		this.elements.SetBase(element.id, 1, 0);
		int id = element.id;
		if (id != 3602)
		{
			if (id == 3604)
			{
				this.elements.SetBase(2206, 10, 0);
			}
		}
		else
		{
			this.elements.SetBase(2206, 15, 0);
		}
		if (EClass.game.StartZone == this.owner || this.owner is Zone_Vernis)
		{
			this.AddMemeber(EClass.pc);
		}
		if (EClass.debug.allPolicy)
		{
			foreach (SourceElement.Row row3 in from a in EClass.sources.elements.rows
			where a.category == "policy"
			select a)
			{
				this.policies.AddPolicy(row3.id, true);
			}
		}
		this.ValidateUpgradePolicies();
	}

	public void OnUnclaimZone()
	{
	}

	public void ValidateUpgradePolicies()
	{
		if (this.lv >= 2)
		{
			if (!this.policies.HasPolicy(2705))
			{
				this.policies.AddPolicy(2705, true);
				EClass.pc.faction.SetGlobalPolicyActive(2705, EClass.pc.faction.IsGlobalPolicyActive(2705));
			}
			if (!this.policies.HasPolicy(2708))
			{
				this.policies.AddPolicy(2708, true);
				EClass.pc.faction.SetGlobalPolicyActive(2708, EClass.pc.faction.IsGlobalPolicyActive(2708));
			}
			if (!this.policies.HasPolicy(2707))
			{
				this.policies.AddPolicy(2707, true);
			}
			if (!this.policies.HasPolicy(2709))
			{
				this.policies.AddPolicy(2709, true);
			}
		}
		foreach (int id in EClass.pc.faction.globalPolicies)
		{
			if (!this.policies.HasPolicy(id))
			{
				this.policies.AddPolicy(id, false);
				EClass.pc.faction.SetGlobalPolicyActive(id, EClass.pc.faction.IsGlobalPolicyActive(id));
			}
		}
	}

	public void Upgrade()
	{
		List<Element> list = this.owner.ListLandFeats();
		if (this.owner.IsActiveZone)
		{
			TraitCoreZone traitCoreZone = EClass._map.FindThing<TraitCoreZone>();
			if (traitCoreZone != null)
			{
				SE.Play("godbless");
				traitCoreZone.owner.PlayEffect("aura_heaven", true, 0f, default(Vector3));
			}
		}
		this.lv++;
		this.exp = 0;
		FactionBranch.<>c__DisplayClass87_0 CS$<>8__locals1;
		CS$<>8__locals1.admin = 4;
		CS$<>8__locals1.food = 4;
		FactionBranch.<Upgrade>g__Set|87_0(4, 3, ref CS$<>8__locals1);
		int id = list[0].id;
		if (id != 3500)
		{
			switch (id)
			{
			case 3600:
				FactionBranch.<Upgrade>g__Set|87_0(5, 3, ref CS$<>8__locals1);
				break;
			case 3601:
				FactionBranch.<Upgrade>g__Set|87_0(4, 4, ref CS$<>8__locals1);
				break;
			case 3602:
				FactionBranch.<Upgrade>g__Set|87_0(5, 2, ref CS$<>8__locals1);
				break;
			case 3603:
				FactionBranch.<Upgrade>g__Set|87_0(6, 2, ref CS$<>8__locals1);
				break;
			case 3604:
				FactionBranch.<Upgrade>g__Set|87_0(4, 3, ref CS$<>8__locals1);
				break;
			}
		}
		else
		{
			FactionBranch.<Upgrade>g__Set|87_0(5, 3, ref CS$<>8__locals1);
		}
		this.elements.SetBase(2003, Mathf.Min((this.lv - 1) * 2 + 1, 10), 0);
		this.elements.SetBase(2115, (this.lv - 1) * CS$<>8__locals1.admin + 1, 0);
		this.elements.SetBase(2204, (this.lv - 1) * CS$<>8__locals1.food + 1, 0);
		this.elements.GetElement(2003).CheckLevelBonus(this.elements, null);
		this.ValidateUpgradePolicies();
		if (this.lv == 4)
		{
			this.elements.SetBase(list[1].id, 1, 0);
		}
		if (this.lv == 7)
		{
			this.elements.SetBase(list[2].id, 1, 0);
		}
		if (this.lv >= 5)
		{
			List<Element> list2 = (from a in this.elements.dict.Values
			where a.source.category == "landfeat" && a.HasTag("network")
			select a).ToList<Element>();
			foreach (Element element in list2)
			{
				EClass.pc.faction.elements.ModBase(element.id, element.Value);
			}
			foreach (Element element2 in list2)
			{
				this.elements.Remove(element2.id);
			}
		}
		Msg.Say("upgrade_hearth", this.lv.ToString() ?? "", this.owner.Name, null, null);
		this.LogRaw("upgrade_hearth".langGame(this.lv.ToString() ?? "", this.owner.Name, null, null), "Good");
		Tutorial.Reserve("stone", null);
	}

	public int MaxLv
	{
		get
		{
			return 7;
		}
	}

	public bool CanUpgrade()
	{
		return this.lv < this.MaxLv && (this.exp >= this.GetNextExp(-1) || EClass.debug.enable);
	}

	public int GetUpgradeCost()
	{
		return this.lv * this.lv * this.lv * 1000;
	}

	public void ModExp(int a)
	{
		if (this.policies.IsActive(2515, -1))
		{
			return;
		}
		if (this.policies.IsActive(2516, -1))
		{
			a = a * 150 / 100 + EClass.rnd(2);
		}
		this.exp += a;
		if (this.exp >= this.GetNextExp(-1) && this.CanUpgrade())
		{
			if (EClass.core.version.demo && this.lv >= 3)
			{
				this.exp = 0;
				Msg.Say("demoLimit2");
				return;
			}
			this.Upgrade();
		}
	}

	public string GetHearthHint(int a)
	{
		if (a <= 1)
		{
			return "hearth1".lang();
		}
		string text = "";
		for (int i = 1; i < a; i++)
		{
			string text2 = ("hearth" + (i + 1).ToString()).lang();
			if (!text2.IsEmpty())
			{
				text = text + text2 + Environment.NewLine;
			}
		}
		return text.TrimEnd(Environment.NewLine.ToCharArray());
	}

	public void AddFeat(int ele, int v)
	{
		this.elements.ModBase(ele, v);
		WidgetPopText.Say("rewardElement".lang(EClass.sources.elements.map[ele].GetName(), null, null, null, null), FontColor.Default, null);
	}

	public void AddMemeber(Chara c)
	{
		if (this.members.Contains(c))
		{
			return;
		}
		FactionBranch factionBranch = EClass.Home.FindBranch(c);
		if (factionBranch != null)
		{
			factionBranch.RemoveMemeber(c);
		}
		EClass.Home.RemoveReserve(c);
		c.SetGlobal();
		c.SetFaction(EClass.Home);
		c.SetHomeZone(this.owner);
		if (c.OriginalHostility <= Hostility.Ally)
		{
			c.c_originalHostility = Hostility.Ally;
		}
		c.hostility = Hostility.Ally;
		c.enemy = null;
		c.orgPos = null;
		c.memberType = FactionMemberType.Default;
		if (c.hp > c.MaxHP)
		{
			c.hp = c.MaxHP;
		}
		if (c.mana.value > c.mana.max)
		{
			c.mana.value = c.mana.max;
		}
		if (c.stamina.value > c.stamina.max)
		{
			c.stamina.value = c.stamina.max;
		}
		this.members.Add(c);
		EClass.pc.faction.charaElements.OnAddMemeber(c);
		this.RefreshEfficiency();
		c.RefreshWorkElements(this.elements);
		if (this.uidMaid == 0 && c.id == "maid")
		{
			this.uidMaid = c.uid;
		}
	}

	public void ChangeMemberType(Chara c, FactionMemberType type)
	{
		c.ClearBed(null);
		c.memberType = type;
		this.RefreshEfficiency();
		c.RefreshWorkElements(this.elements);
		this.policies.Validate();
	}

	public void BanishMember(Chara c, bool sell = false)
	{
		this.RemoveMemeber(c);
		if (!sell)
		{
			Msg.Say("banish", c, EClass._zone.Name, null, null);
		}
		if (c.IsGlobal)
		{
			c.OnBanish();
			return;
		}
		c.Destroy();
	}

	public void RemoveMemeber(Chara c)
	{
		c.isSale = false;
		if (c.currentZone != null && c.currentZone.map != null)
		{
			c.ClearBed(c.currentZone.map);
			c.currentZone.map.props.sales.Remove(c);
		}
		c.RefreshWorkElements(null);
		c.RemoveGlobal();
		this.members.Remove(c);
		EClass.pc.faction.charaElements.OnRemoveMember(c);
		c.SetFaction(EClass.game.factions.Wilds);
		this.policies.Validate();
		this.RefreshEfficiency();
	}

	public int GetHappiness(FactionMemberType type)
	{
		float num = 0f;
		if (this.members.Count == 0)
		{
			return 0;
		}
		foreach (Chara chara in this.members)
		{
			num += (float)chara.happiness;
		}
		return (int)(num / (float)this.members.Count);
	}

	public bool IsAllDead()
	{
		using (List<Chara>.Enumerator enumerator = this.members.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (!enumerator.Current.isDead)
				{
					return false;
				}
			}
		}
		return true;
	}

	public void Recruit(Chara c)
	{
		this.RemoveRecruit(c);
		this.AddMemeber(c);
		if (c.currentZone != EClass._zone && !c.isDead)
		{
			Trait random = EClass._map.Installed.traits.GetTraitSet<TraitSpotExit>().GetRandom();
			Point point = ((random != null) ? random.GetPoint() : null) ?? EClass.pc.pos;
			EClass._zone.AddCard(c, point);
		}
		this.RefreshEfficiency();
		c.RefreshWorkElements(null);
		Msg.Say("hire".langGame(c.Name, null, null, null));
	}

	public Chara GetMaid()
	{
		foreach (Chara chara in this.members)
		{
			if (chara.IsAliveInCurrentZone && chara.IsMaid)
			{
				return chara;
			}
		}
		return null;
	}

	public string GetRandomName()
	{
		if (EClass.rnd(4) == 0 || this.members.Count == 0)
		{
			return EClass.player.title;
		}
		return this.members.RandomItem<Chara>().Name;
	}

	public int CountMembers(FactionMemberType type, bool onlyAlive = false)
	{
		int num = 0;
		foreach (Chara chara in this.members)
		{
			if (chara.memberType == type && chara.trait.IsCountAsResident && (!onlyAlive || !chara.isDead))
			{
				num++;
			}
		}
		return num;
	}

	public int CountGuests()
	{
		int num = 0;
		using (List<Chara>.Enumerator enumerator = EClass._map.charas.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.memberType == FactionMemberType.Guest)
				{
					num++;
				}
			}
		}
		return num;
	}

	public int CountWealthyGuests()
	{
		int num = 0;
		foreach (Chara chara in EClass._map.charas)
		{
			if (chara.memberType == FactionMemberType.Guest && chara.IsWealthy)
			{
				num++;
			}
		}
		return num;
	}

	public void UpdateReqruits(bool clear = false)
	{
		this.resources.Refresh();
		if (clear)
		{
			this.listRecruit.ForeachReverse(delegate(HireInfo i)
			{
				if (!i.chara.IsGlobal)
				{
					i.chara.Destroy();
				}
				if (i.chara.isDestroyed)
				{
					this.listRecruit.Remove(i);
				}
			});
			this.listRecruit.Clear();
			this.lastUpdateReqruit = -1;
		}
		else
		{
			this.listRecruit.ForeachReverse(delegate(HireInfo i)
			{
				if (i.Hours < 0)
				{
					if (!i.chara.IsGlobal)
					{
						i.chara.Destroy();
					}
					this.listRecruit.Remove(i);
				}
			});
		}
		if (this.lastUpdateReqruit == EClass.world.date.day)
		{
			return;
		}
		this.lastUpdateReqruit = EClass.world.date.day;
		int num = 2 + (int)Mathf.Sqrt((float)this.Evalue(2513)) / 2;
		int num2 = EClass.rnd(3 + (int)Mathf.Sqrt((float)this.Evalue(2513)) / 2) + num - this.listRecruit.Count;
		if (num2 <= 0)
		{
			return;
		}
		new List<Chara>(EClass.game.cards.globalCharas.Values).Shuffle<Chara>();
		for (int j = 0; j < num2; j++)
		{
			Chara chara = CharaGen.CreateFromFilter("c_neutral", this.ContentLV + Mathf.Min(EClass.player.stats.days, 10), -1);
			if (chara.isBackerContent)
			{
				j--;
			}
			else
			{
				this.AddRecruit(chara);
			}
		}
	}

	public void AddRecruit(Chara c)
	{
		HireInfo hireInfo = new HireInfo
		{
			chara = c,
			isNew = true
		};
		hireInfo.deadline = EClass.world.date.GetRaw(0) + (EClass.rnd(5) + 1) * 1440 + EClass.rnd(24) * 60;
		this.listRecruit.Add(hireInfo);
	}

	public void RemoveRecruit(Chara c)
	{
		this.listRecruit.ForeachReverse(delegate(HireInfo i)
		{
			if (i.chara == c)
			{
				this.listRecruit.Remove(i);
			}
		});
	}

	public int CountNewRecruits()
	{
		int num = 0;
		using (List<HireInfo>.Enumerator enumerator = this.listRecruit.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.isNew)
				{
					num++;
				}
			}
		}
		return num;
	}

	public void ClearNewRecruits()
	{
		foreach (HireInfo hireInfo in this.listRecruit)
		{
			hireInfo.isNew = false;
		}
	}

	public bool IsRecruit(Chara c)
	{
		using (List<HireInfo>.Enumerator enumerator = this.listRecruit.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.chara == c)
				{
					return true;
				}
			}
		}
		return false;
	}

	public int CountPasture()
	{
		return EClass._map.Stocked.GetNum("pasture", true) + EClass._map.Installed.GetNum("pasture", false);
	}

	public int GetPastureCost()
	{
		return this.CountMembers(FactionMemberType.Livestock, false);
	}

	public int GetCivility()
	{
		int num = 10 + this.Evalue(2203);
		if (this.Evalue(2701) > 0)
		{
			num += 10 + (int)Mathf.Sqrt((float)this.Evalue(2701));
		}
		if (this.Evalue(2702) > 0)
		{
			num -= 10 + (int)Mathf.Sqrt((float)this.Evalue(2702));
		}
		return num;
	}

	public float GetHearthIncome()
	{
		float num = 0f;
		foreach (Element element in this.elements.dict.Values)
		{
			if (element.source.category == "culture")
			{
				num += this.GetHearthIncome(element.source.alias);
			}
		}
		return num;
	}

	public float GetHearthIncome(string id)
	{
		int num = 0;
		foreach (KeyValuePair<int, Element> keyValuePair in this.elements.dict)
		{
			if (keyValuePair.Value.source.aliasParent == id)
			{
				num++;
			}
		}
		return 0.2f * (float)num;
	}

	public int GetTechUpgradeCost(Element e)
	{
		int num = e.ValueWithoutLink;
		num += e.CostLearn;
		if (e.source.max != 0 && e.ValueWithoutLink >= e.source.max)
		{
			return 0;
		}
		return num;
	}

	public string Log(string idLang, string ref1 = null, string ref2 = null, string ref3 = null, string ref4 = null)
	{
		Msg.alwaysVisible = true;
		return this.LogRaw(Msg.GetRawText(idLang, ref1, ref2, ref3, ref4), EClass.sources.langGame.map[idLang].logColor);
	}

	public string Log(string idLang, Card c1, Card c2, string ref1 = null, string ref2 = null)
	{
		Msg.alwaysVisible = true;
		return this.LogRaw(Msg.GetRawText(idLang, c1, c2, ref1, ref2), EClass.sources.langGame.map[idLang].logColor);
	}

	public string Log(string idLang, Card c1, string ref1 = null, string ref2 = null, string ref3 = null)
	{
		Msg.alwaysVisible = true;
		return this.LogRaw(Msg.GetRawText(idLang, c1, ref1, ref2, ref3), EClass.sources.langGame.map[idLang].logColor);
	}

	public string LogRaw(string text, string col = null)
	{
		this.log.Add(text, col.IsEmpty() ? null : col);
		Msg.alwaysVisible = false;
		Msg.SetColor();
		return text;
	}

	[CompilerGenerated]
	internal static void <RefreshEfficiency>g__TryAdd|52_0(Hobby h, ref FactionBranch.<>c__DisplayClass52_0 A_1)
	{
		if (h.source.elements.IsEmpty())
		{
			return;
		}
		for (int i = 0; i < h.source.elements.Length; i += 2)
		{
			if (h.source.elements[i] == 2207)
			{
				A_1.ration += h.source.elements[i + 1];
			}
		}
	}

	[CompilerGenerated]
	private void <GenerateGarbage>g__Generate|72_0(Chara c, ref FactionBranch.<>c__DisplayClass72_0 A_2)
	{
		FactionBranch.<>c__DisplayClass72_1 CS$<>8__locals1;
		CS$<>8__locals1.c = c;
		if (CS$<>8__locals1.c != null && EClass.rnd(5) != 0)
		{
			return;
		}
		if (EClass.rnd(80) == 0)
		{
			this.<GenerateGarbage>g__Add|72_1(ThingGen.CreateFromCategory("junk", -1), ref A_2, ref CS$<>8__locals1);
			return;
		}
		if (EClass.rnd(40) == 0)
		{
			this.<GenerateGarbage>g__Add|72_1(ThingGen.CreateFromTag("garbage", -1), ref A_2, ref CS$<>8__locals1);
			return;
		}
		string id = "trash2";
		if (EClass.rnd(3) == 0)
		{
			id = "trash1";
		}
		if (EClass.rnd(10) == 0)
		{
			id = ((EClass.rnd(3) == 0) ? "529" : "1170");
		}
		if (CS$<>8__locals1.c != null && (!CS$<>8__locals1.c.IsUnique || CS$<>8__locals1.c.memberType == FactionMemberType.Livestock) && EClass.rnd((CS$<>8__locals1.c.memberType == FactionMemberType.Livestock) ? 2 : 50) == 0)
		{
			id = "_poop";
		}
		if (EClass.rnd(200000) == 0)
		{
			id = "goodness";
		}
		this.<GenerateGarbage>g__Add|72_1(ThingGen.Create(id, -1, -1), ref A_2, ref CS$<>8__locals1);
	}

	[CompilerGenerated]
	private void <GenerateGarbage>g__Add|72_1(Thing t, ref FactionBranch.<>c__DisplayClass72_0 A_2, ref FactionBranch.<>c__DisplayClass72_1 A_3)
	{
		if (A_2.sortChance < EClass.rnd(100))
		{
			EClass._zone.TryAddThing(t, (A_3.c != null) ? (A_3.c.pos.GetRandomPoint(2, true, true, false, 100) ?? A_3.c.pos).GetNearestPoint(false, true, true, false) : EClass._zone.bounds.GetRandomSurface(false, true, false), true);
			return;
		}
		if (t.id == "_poop" && EClass.rnd(150) == 0)
		{
			t.ChangeMaterial((EClass.rnd(2) == 0) ? "silver" : "gold");
		}
		if (!this.TryTrash(t))
		{
			EClass._zone.AddCard(t, EClass._map.bounds.GetRandomSurface(false, true, false));
			int unsortedCount = A_2.unsortedCount;
			A_2.unsortedCount = unsortedCount + 1;
			if (A_2.unsortedCount >= 5)
			{
				Tutorial.Reserve("garbage", null);
			}
		}
	}

	[CompilerGenerated]
	private void <GetDailyIncome>g__GetIncome|82_0(ref int n, ref int stat, string lang, bool tax)
	{
		if (tax && !this.IsTaxFree)
		{
			n = n * 10 / 100;
		}
		if (n == 0)
		{
			return;
		}
		Msg.Say(lang, Lang._currency(n, "money"), this.owner.Name, null, null);
		Thing t = ThingGen.Create("money", -1, -1).SetNum(n);
		EClass.pc.Pick(t, true, true);
		stat += n;
		n = 0;
	}

	[CompilerGenerated]
	internal static void <Upgrade>g__Set|87_0(int a, int f, ref FactionBranch.<>c__DisplayClass87_0 A_2)
	{
		A_2.admin = a;
		A_2.food = f;
	}

	[JsonProperty]
	public int lv = 1;

	[JsonProperty]
	public int rank;

	[JsonProperty]
	public int exp;

	[JsonProperty]
	public int seedPlan;

	[JsonProperty]
	public int temper;

	[JsonProperty]
	public int uidMaid;

	[JsonProperty]
	public int lastUpdateReqruit;

	[JsonProperty]
	public int incomeShop;

	[JsonProperty]
	public GStability stability = new GStability
	{
		value = 1
	};

	[JsonProperty]
	public HomeResourceManager resources = new HomeResourceManager();

	[JsonProperty]
	public ResearchManager researches = new ResearchManager();

	[JsonProperty]
	public PolicyManager policies = new PolicyManager();

	[JsonProperty]
	public HappinessManager happiness = new HappinessManager();

	[JsonProperty]
	public MeetingManager meetings = new MeetingManager();

	[JsonProperty]
	public Thing stash;

	[JsonProperty]
	public MsgLog log = new MsgLog
	{
		id = "log"
	};

	[JsonProperty]
	public Religion faith;

	[JsonProperty]
	public ExpeditionManager expeditions = new ExpeditionManager();

	[JsonProperty]
	public List<HireInfo> listRecruit = new List<HireInfo>();

	[JsonProperty]
	public int dateFound;

	[JsonProperty]
	public int tourism;

	[JsonProperty]
	public FactionBranch.Statistics statistics = new FactionBranch.Statistics();

	[JsonProperty]
	public FactionBranch.Statistics lastStatistics = new FactionBranch.Statistics();

	public Zone owner;

	public int incomeInn;

	public int incomeTourism;

	public int efficiency = 100;

	public List<Chara> members = new List<Chara>();

	public class Statistics : EClass
	{
		[JsonProperty]
		public int ship;

		[JsonProperty]
		public int inn;

		[JsonProperty]
		public int tourism;

		[JsonProperty]
		public int shop;

		[JsonProperty]
		public int numSold;

		[JsonProperty]
		public int visitor;

		[JsonProperty]
		public int tax;
	}
}
