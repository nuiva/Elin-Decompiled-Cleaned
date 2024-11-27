using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class AI_Idle : AIAct
{
	public override bool IsIdle
	{
		get
		{
			return !base.IsChildRunning;
		}
	}

	public override bool InformCancel
	{
		get
		{
			return false;
		}
	}

	public override int MaxRestart
	{
		get
		{
			return this.maxRepeat;
		}
	}

	public override void OnStart()
	{
		this.owner.SetTempHand(-1, -1);
		this.owner.ShowEmo(Emo.none, 0f, true);
	}

	public unsafe override IEnumerable<AIAct.Status> Run()
	{
		for (;;)
		{
			if (this.owner.held != null)
			{
				this.owner.PickHeld(false);
			}
			if (this.owner.nextUse != null)
			{
				Thing nextUse = this.owner.nextUse;
				this.owner.nextUse = null;
				if (nextUse.parent == this.owner && !nextUse.isDestroyed)
				{
					this.owner.TryUse(nextUse);
				}
				yield return base.KeepRunning();
			}
			if (EClass.rnd(this.owner.IsPCParty ? 10 : 100) == 0 && this.owner.hunger.GetPhase() >= 3)
			{
				Thing thing = this.owner.things.Find((Thing a) => this.owner.CanEat(a, this.owner.IsPCFaction), false);
				if (thing == null && this.owner.IsPCFaction)
				{
					thing = this.owner.things.Find((Thing a) => this.owner.CanEat(a, false), false);
				}
				if (thing == null && this.owner.IsPCFaction && EClass._zone.IsPCFaction)
				{
					thing = EClass._zone.branch.GetMeal(this.owner);
					if (thing != null)
					{
						this.owner.Pick(thing, true, true);
					}
				}
				if (thing == null && !this.owner.IsPCParty)
				{
					if (!this.owner.IsPCFaction && EClass.rnd(8) != 0)
					{
						this.owner.hunger.Mod(-30);
					}
					else
					{
						thing = ThingGen.CreateFromCategory("food", EClass.rnd(EClass.rnd(60) + 1) + 10);
						thing.isNPCProperty = true;
						if ((thing.ChildrenAndSelfWeight < 5000 || !this.owner.IsPCParty) && thing.trait.CanEat(this.owner))
						{
							thing = this.owner.AddThing(thing, true, -1, -1);
						}
					}
				}
				if (thing != null)
				{
					if (EClass._zone.IsRegion)
					{
						this.owner.InstantEat(thing, false);
						yield return base.Restart();
					}
					else
					{
						yield return base.Do(new AI_Eat
						{
							target = thing
						}, null);
					}
				}
			}
			if (EClass.rnd(3) == 0 && this.owner.mana.value > 0)
			{
				Act act = null;
				Act actRevive = null;
				foreach (ActList.Item item in this.owner.ability.list.items)
				{
					Act act2 = item.act;
					if (act2.id == 8430)
					{
						actRevive = act2;
					}
					string[] abilityType = act2.source.abilityType;
					if (!abilityType.IsEmpty() && (abilityType[0] == "heal" || abilityType[0] == "hot"))
					{
						act = item.act;
					}
				}
				if (act != null)
				{
					List<Chara> list;
					if (!this.owner.IsPCParty)
					{
						(list = new List<Chara>()).Add(this.owner);
					}
					else
					{
						list = EClass.pc.party.members;
					}
					List<Chara> list2 = list;
					foreach (Chara chara in list2)
					{
						if ((float)chara.hp <= (float)chara.MaxHP * 0.75f && this.owner.CanSeeLos(chara, -1, false) && (!(act.source.abilityType[0] == "hot") || !chara.HasCondition<ConHOT>()))
						{
							this.owner.UseAbility(act, chara, null, false);
							yield return base.KeepRunning();
							break;
						}
					}
					List<Chara>.Enumerator enumerator2 = default(List<Chara>.Enumerator);
				}
				if (actRevive != null)
				{
					if ((from a in EClass.game.cards.globalCharas
					where a.Value.isDead && a.Value.faction == EClass.pc.faction && !a.Value.isSummon && a.Value.GetInt(103, null) != 0
					select a).ToList<KeyValuePair<int, Chara>>().Count > 0 && this.owner.UseAbility(actRevive.source.alias, this.owner, null, false))
					{
						yield return base.KeepRunning();
					}
				}
				actRevive = null;
			}
			if (this.owner.IsPCFaction && EClass._zone.IsPCFaction)
			{
				this.owner.sharedCheckTurn--;
				if (this.owner.sharedCheckTurn < 0 && EClass.rnd(EClass.debug.enable ? 2 : 20) == 0)
				{
					this.owner.TryTakeSharedItems(true);
					this.owner.TryPutSharedItems(true);
					this.owner.sharedCheckTurn += (EClass.debug.enable ? 20 : 200);
				}
			}
			if ((EClass._zone is Zone_Civilized || EClass._zone.IsPCFaction) && (this.owner.IsPCParty ? 10 : (this.owner.IsPCFaction ? 2 : 0)) > EClass.rnd(100))
			{
				Thing thing2 = this.owner.things.Find("polish_powder", -1, -1);
				if (thing2 != null && EClass._map.props.installed.Find<TraitGrindstone>() != null)
				{
					foreach (Thing thing3 in this.owner.things)
					{
						if (thing3.IsEquipment && thing3.encLV < 0)
						{
							int num = 0;
							while (num < 5 && thing3.encLV < 0)
							{
								this.owner.Say("polish", this.owner, thing3, null, null);
								thing3.ModEncLv(1);
								thing2.ModNum(-1, true);
								if (thing2.isDestroyed)
								{
									break;
								}
								num++;
							}
							if (thing2.isDestroyed)
							{
								break;
							}
						}
					}
				}
			}
			if (this.owner.IsPCParty)
			{
				if (this.owner.IsRestrainedResident && this.owner.stamina.value > this.owner.stamina.max / 2)
				{
					this.owner.SetAI(new AI_Torture
					{
						shackle = this.owner.pos.FindThing<TraitShackle>()
					});
					yield return base.Restart();
				}
				if (EClass.rnd(100) == 0 && !EClass._zone.IsRegion && this.owner.HasElement(1227, 1))
				{
					List<Chara> list3 = new List<Chara>();
					foreach (Chara chara2 in EClass.pc.party.members)
					{
						if (chara2.Evalue(1227) > 0)
						{
							list3.Add(chara2);
						}
					}
					if (list3.Count > 2)
					{
						list3.Remove(this.owner);
						this.owner.SetEnemy(list3.RandomItem<Chara>());
						yield return base.Success(null);
					}
				}
				if (EClass.rnd(150) == 0 && this.owner.host != null && this.owner.host.parasite == this.owner && this.owner.GetInt(108, null) == 1)
				{
					this.owner.host.PlaySound("whip", 1f, true);
					this.owner.host.Say("use_whip3", this.owner, this.owner.host, null, null);
					this.owner.Talk("insult", null, null, false);
					this.owner.host.PlayAnime(AnimeID.Shiver, false);
					this.owner.host.DamageHP(5 + EClass.rndHalf(this.owner.host.MaxHP / 5), 919, 100, AttackSource.Condition, null, true);
					this.owner.host.OnInsulted();
					yield return base.KeepRunning();
				}
				if (EClass.rnd(EClass.debug.enable ? 2 : 20) == 0 && this.owner.CanSee(EClass.pc) && !(EClass.pc.ai is AI_Eat))
				{
					this.owner.TryTakeSharedItems(EClass.pc.things.List((Thing t) => t.IsSharedContainer, false), true, true);
				}
				if (this.owner.isSynced && EClass.rnd((this.owner.host == null) ? 200 : 150) == 0 && this.owner.GetInt(106, null) == 0)
				{
					if (EClass.rnd(2) == 0 && this.owner.GetInt(108, null) == 1)
					{
						this.owner.Talk("insult", null, null, false);
					}
					else
					{
						this.owner.TalkTopic("calm");
					}
				}
				if (EClass.rnd(100) == 0 && EClass._zone.IsTown)
				{
					this.owner.ClearInventory(ClearInventoryType.SellAtTown);
				}
				if ((EClass.rnd(20) == 0 || EClass.debug.enable) && this.owner.GetCurrency("money") >= 500)
				{
					bool flag = EClass._zone.IsTown;
					if (EClass._zone.IsPCFaction)
					{
						foreach (Chara chara3 in EClass._zone.branch.members)
						{
							if (chara3.ExistsOnMap && chara3.trait is TraitTrainer)
							{
								flag = true;
							}
						}
					}
					if (flag)
					{
						bool flag2 = false;
						foreach (Element element in this.owner.elements.dict.Values)
						{
							if (!(element.source.category != "skill") && element.vTempPotential < 900)
							{
								flag2 = true;
								break;
							}
						}
						if (flag2)
						{
							int num2 = this.owner.GetCurrency("money");
							if (num2 >= 20000)
							{
								num2 = 20000;
							}
							this.owner.PlaySound("pay", 1f, true);
							int num3 = num2 / 200;
							foreach (Element element2 in this.owner.elements.dict.Values)
							{
								if (!(element2.source.category != "skill"))
								{
									int num4 = num3 * 100 / (100 + (100 + element2.vTempPotential / 2 + element2.ValueWithoutLink) * (100 + element2.vTempPotential / 2 + element2.ValueWithoutLink) / 100);
									num4 += 1 + EClass.rnd(3);
									this.owner.elements.ModTempPotential(element2.id, Mathf.Max(1, num4), 9999);
								}
							}
							Msg.Say("party_train", this.owner, Lang._currency(num2, false, 14), null, null);
							this.owner.PlaySound("ding_potential", 1f, true);
							this.owner.ModCurrency(-num2, "money");
						}
					}
				}
			}
			if (this.owner.c_uidMaster != 0)
			{
				Chara chara4 = this.owner.master;
				if (chara4 == null || !chara4.IsAliveInCurrentZone)
				{
					chara4 = this.owner.FindMaster();
				}
				if (chara4 != null && chara4.IsAliveInCurrentZone)
				{
					if (this.owner.enemy == null)
					{
						this.owner.SetEnemy(chara4.enemy);
					}
					int num5 = this.owner.Dist(chara4.pos);
					if (this.owner.source.aiIdle != "root" && num5 > EClass.game.config.tactics.AllyDistance(this.owner) && EClass._zone.PetFollow && this.owner.c_minionType == MinionType.Default)
					{
						if (this.owner.HasAccess(chara4.pos))
						{
							this.owner.TryMoveTowards(chara4.pos);
						}
						yield return base.KeepRunning();
						continue;
					}
				}
			}
			Party party = this.owner.party;
			if (party == null || party.leader == this.owner || !party.leader.IsAliveInCurrentZone || this.owner.host != null || !EClass._zone.PetFollow)
			{
				break;
			}
			if (this.owner.source.aiIdle == "root")
			{
				yield return base.KeepRunning();
			}
			else if (this.owner.Dist(party.leader.pos) <= EClass.game.config.tactics.AllyDistance(this.owner))
			{
				yield return base.KeepRunning();
			}
			else
			{
				if (this.owner.HasAccess(party.leader.pos) && this.owner.TryMoveTowards(party.leader.pos) == Card.MoveResult.Fail && this.owner.Dist(party.leader) > 4)
				{
					this.moveFailCount++;
					bool flag3 = (EClass._zone is Zone_Civilized || EClass._zone.IsPCFaction) && (EClass.pc.enemy == null || !EClass.pc.enemy.IsAliveInCurrentZone);
					if (this.moveFailCount >= (flag3 ? 100 : 10))
					{
						this.owner.Teleport(party.leader.pos.GetNearestPoint(false, false, true, true), false, true);
						this.moveFailCount = 0;
					}
				}
				else
				{
					this.moveFailCount = 0;
				}
				yield return base.KeepRunning();
			}
		}
		if (EClass._zone.IsNefia && EClass._zone.Boss == this.owner && EClass.rnd(20) == 0)
		{
			this.owner.SetEnemy(EClass.pc);
		}
		if (EClass._zone.IsRegion && EClass.rnd(10) != 0)
		{
			yield return base.Restart();
		}
		if (((this.owner.homeBranch != null && this.owner.homeBranch == EClass.Branch && EClass.rnd(100) == 0) || (this.owner.IsGuest() && EClass.rnd(50) == 0)) && this.owner.FindBed() == null)
		{
			this.owner.TryAssignBed();
		}
		if (EClass._zone.IsPCFaction)
		{
			Room room = this.owner.pos.cell.room;
			if (room != null)
			{
				Point point = null;
				if (this.owner.memberType == FactionMemberType.Guest && room.data.accessType != BaseArea.AccessType.Public)
				{
					point = this.<Run>g__FindMovePoint|10_9(BaseArea.AccessType.Public);
				}
				else if (this.owner.memberType == FactionMemberType.Default && room.data.accessType == BaseArea.AccessType.Private)
				{
					point = (this.<Run>g__FindMovePoint|10_9(BaseArea.AccessType.Resident) ?? this.<Run>g__FindMovePoint|10_9(BaseArea.AccessType.Public));
				}
				if (point != null)
				{
					yield return base.DoGoto(point, 0, false, null);
				}
			}
		}
		if (this.owner.isSynced && !this.owner.IsPCParty)
		{
			if (this.owner.IsPCFaction && this.owner.GetInt(32, null) + 4320 < EClass.world.date.GetRaw(0))
			{
				if (this.owner.GetInt(32, null) != 0 && Zone.okaerinko < 10)
				{
					this.owner.Talk("welcomeBack", null, null, false);
					Zone.okaerinko++;
				}
				this.owner.SetInt(32, EClass.world.date.GetRaw(0));
			}
			else if (EClass.player.stats.turns > this.owner.turnLastSeen + 50 && Los.IsVisible(EClass.pc, this.owner) && this.owner.CanSee(EClass.pc))
			{
				if (EClass.rnd(5) == 0 && this.owner.hostility >= Hostility.Neutral && EClass.pc.IsPCC && EClass.pc.pccData.state == PCCState.Undie)
				{
					this.owner.Talk("pervert3", null, null, false);
				}
				else if (EClass.rnd(15) == 0 && EClass._zone.IsTown && this.owner.hostility >= Hostility.Neutral && !this.owner.IsPCFaction && !EClass.pc.HasCondition<ConIncognito>())
				{
					bool flag4 = EClass._zone is Zone_Derphy;
					string text = (EClass.player.karma <= 10) ? (flag4 ? "rumor_good" : "rumor_bad") : ((EClass.player.karma >= 90) ? (flag4 ? "rumor_bad" : "rumor_good") : "");
					if (!text.IsEmpty())
					{
						this.owner.Talk(text, null, null, false);
					}
					if ((flag4 ? (EClass.player.karma >= 90) : (EClass.player.karma <= 10)) && EClass.rnd(10) == 0)
					{
						Thing t3 = ThingGen.Create("stone", -1, -1);
						AI_PlayMusic.ignoreDamage = true;
						ActThrow.Throw(this.owner, EClass.pc.pos, t3, ThrowMethod.Punish, 0f);
						AI_PlayMusic.ignoreDamage = false;
					}
				}
				else
				{
					this.owner.TalkTopic("fov");
				}
				this.owner.turnLastSeen = EClass.player.stats.turns;
			}
		}
		if (EClass.rnd(25) == 0 && this.owner.IsInMutterDistance(10))
		{
			if (this.owner.isRestrained)
			{
				this.owner.PlayAnime(AnimeID.Shiver, false);
			}
			TCText tc = this.owner.HostRenderer.GetTC<TCText>();
			if (tc == null || tc.pop.items.Count == 0)
			{
				if (this.owner.noMove)
				{
					foreach (Thing thing4 in this.owner.pos.Things)
					{
						if (thing4.IsInstalled && thing4.trait is TraitGeneratorWheel)
						{
							this.owner.Talk("labor", null, null, false);
							this.owner.PlayAnime(AnimeID.Shiver, false);
							yield return base.Restart();
						}
					}
					List<Thing>.Enumerator enumerator6 = default(List<Thing>.Enumerator);
				}
				if (this.owner.isDrunk && (this.owner.race.id == "cat" || this.owner.id == "sailor"))
				{
					this.owner.Talk("drunk_cat", null, null, false);
				}
				else if (this.owner.isRestrained)
				{
					this.owner.Talk("restrained", null, null, false);
				}
				else if (this.owner.GetInt(106, null) == 0 && !this.owner.IsPCParty)
				{
					if (this.owner.HasElement(1232, 1) && EClass.rnd(4) == 0)
					{
						this.owner.Talk("baby", null, null, false);
					}
					else if (EClass.rnd((this.owner.host == null) ? 2 : 10) == 0 && this.owner.isSynced && this.owner.TalkTopic("calm").IsEmpty())
					{
						this.owner.Talk(this.owner.pos.IsHotSpring ? "hotspring" : "idle", null, null, false);
					}
				}
			}
		}
		if (EClass.rnd(8) == 0 && this.owner.race.id == "chicken")
		{
			this.owner.PlaySound("Animal/Chicken/chicken", 1f, true);
		}
		if (EClass.rnd(80) == 0 && this.owner.race.id == "cat")
		{
			this.owner.PlaySound("Animal/Cat/cat", 1f, true);
		}
		if (this.owner.trait.IdAmbience != null && this.owner.IsInMutterDistance(15))
		{
			float mtp = 1f;
			Room room2 = this.owner.Cell.room;
			Room room3 = EClass.pc.Cell.room;
			if (room2 != room3 && room3 != null)
			{
				if (((room2 != null) ? room2.lot : null) == ((room3 != null) ? room3.lot : null))
				{
					mtp = 0.7f;
				}
				else
				{
					mtp = 0.4f;
				}
			}
			EClass.Sound.PlayAmbience(this.owner.trait.IdAmbience, *this.owner.pos.Position(), mtp);
		}
		if (EClass.rnd(2000) == 0 && this.owner.IsHuman && (this.owner.host == null || this.owner.host.ride != this.owner))
		{
			Thing thing5 = this.owner.things.Find((Thing a) => !a.IsNegativeGift && a.trait.CanDrink(this.owner), false);
			if (thing5 != null && thing5.trait is TraitPotion && this.owner.IsPCParty)
			{
				thing5 = null;
			}
			if (thing5 == null && (this.owner.homeBranch == null || !this.owner.homeBranch.policies.IsActive(2503, -1)))
			{
				thing5 = ThingGen.Create("crimAle", -1, -1);
				this.owner.Drink(thing5);
			}
			if (thing5 != null && !thing5.isDestroyed)
			{
				this.owner.TryUse(thing5);
				yield return base.Restart();
			}
		}
		if (EClass.rnd(this.owner.IsPCParty ? 1000 : 200) == 0 && this.owner.isDrunk && (this.owner.isSynced || EClass.rnd(5) == 0))
		{
			this.DoSomethingToCharaInRadius(3, null, delegate(Chara c)
			{
				this.owner.Say("drunk_mess", this.owner, c, null, null);
				this.owner.Talk("drunk_mess", null, null, false);
				bool flag5 = EClass.rnd(5) == 0 && !c.IsPC;
				if (c.IsPCParty && this.owner.hostility >= Hostility.Friend)
				{
					flag5 = false;
				}
				if (flag5)
				{
					this.owner.Say("drunk_counter", c, this.owner, null, null);
					c.Talk("drunk_counter", null, null, false);
					c.DoHostileAction(this.owner, false);
				}
			});
		}
		if (EClass.rnd(100) == 0 && this.owner.trait.CanFish)
		{
			Point fishingPoint = AI_Fish.GetFishingPoint(this.owner.pos);
			if (fishingPoint.IsValid)
			{
				yield return base.Do(new AI_Fish
				{
					pos = fishingPoint
				}, null);
			}
		}
		string idAct = this.owner.source.actIdle.RandomItem<string>();
		if (EClass.rnd(EClass.world.date.IsNight ? 1500 : 15000) == 0 && !this.owner.IsPCFaction && !this.owner.noMove)
		{
			this.owner.AddCondition<ConSleep>(1000 + EClass.rnd(1000), true);
		}
		if (EClass.rnd((this.owner.host != null && this.owner.GetInt(106, null) != 0) ? 1000 : 40) == 0 && this.owner.IsHuman)
		{
			this.DoSomethingToNearChara((Chara c) => (!c.IsPCParty || EClass.rnd(5) == 0) && c.IsMofuable && !this.owner.IsHostile(c) && !c.IsInCombat, delegate(Chara c)
			{
				this.owner.Cuddle(c, false);
			});
		}
		if (EClass.rnd(100) == 0 && this.owner.trait is TraitBitch)
		{
			Chara chara5 = this.DoSomethingToNearChara((Chara c) => c.IsIdle && !c.IsPCParty && !(c.trait is TraitBitch), null);
			if (chara5 != null)
			{
				yield return base.Do(new AI_Fuck
				{
					target = chara5,
					bitch = true
				}, null);
			}
		}
		if (EClass.rnd(50) == 0 && this.owner.trait is TraitBard)
		{
			yield return base.Do(new AI_PlayMusic(), null);
		}
		if (EClass.rnd(4) == 0 && this.TryPerformIdleUse())
		{
			yield return base.Restart();
		}
		if (EClass.rnd(20) == 0 && this.owner.trait.IdleAct())
		{
			yield return base.Restart();
		}
		if (idAct == "janitor" && EClass.rnd(5) == 0)
		{
			this.DoSomethingToCharaInRadius(4, null, delegate(Chara c)
			{
				if (c.HasElement(1211, 1) && !(EClass._zone is Zone_Casino))
				{
					this.owner.Talk("snail", null, null, false);
					Thing t4 = ThingGen.Create("1142", -1, -1);
					ActThrow.Throw(this.owner, c.pos, t4, ThrowMethod.Default, 0f);
				}
			});
			yield return base.Restart();
		}
		if (this.owner.IsRestrainedResident && this.owner.stamina.value > this.owner.stamina.max / 2)
		{
			this.owner.SetAI(new AI_Torture
			{
				shackle = this.owner.pos.FindThing<TraitShackle>()
			});
			yield return base.Restart();
		}
		if (!this.owner.IsPCFactionOrMinion && EClass.rnd(this.owner.isSynced ? 50 : 2000) == 0 && this.owner.hostility == Hostility.Neutral && EClass.pc.party.HasElement(1563) && !this.owner.race.tag.Contains("animal") && EClass._zone.IsTown && !EClass._zone.IsPCFaction)
		{
			EClass.pc.DoHostileAction(this.owner, false);
		}
		if (EClass.rnd(200) == 0)
		{
			Point cleanPoint = AI_Clean.GetCleanPoint(this.owner, 4, 10);
			if (cleanPoint != null)
			{
				yield return base.Do(new AI_Clean
				{
					pos = cleanPoint
				}, null);
			}
		}
		if (EClass.rnd(35) == 0 && this.owner.id == "child" && this.owner.pos.cell.IsSnowTile)
		{
			foreach (Chara chara6 in EClass._map.charas)
			{
				if (EClass.rnd(3) != 0 && chara6 != this.owner && chara6.pos.cell.IsSnowTile && chara6.Dist(this.owner) <= 6 && Los.IsVisible(chara6, this.owner))
				{
					Thing t2 = ThingGen.Create("snow", -1, -1);
					ActThrow.Throw(this.owner, chara6.pos, t2, ThrowMethod.Default, 0f);
					break;
				}
			}
		}
		if (EClass.rnd(EClass.debug.enable ? 3 : 30) == 0)
		{
			Thing thing6 = this.owner.things.Find<TraitBall>();
			if (thing6 == null)
			{
				this.owner.pos.ForeachNeighbor(delegate(Point p)
				{
					TraitBall traitBall = p.FindThing<TraitBall>();
					Card card = (traitBall != null) ? traitBall.owner : null;
					if (card != null)
					{
						this.owner.Pick(card.Thing, true, true);
					}
				}, true);
			}
			else
			{
				foreach (Chara chara7 in EClass._map.charas)
				{
					if (EClass.rnd(3) != 0 && chara7 != this.owner && chara7.Dist(this.owner) <= 6 && chara7.Dist(this.owner) >= 3 && Los.IsVisible(chara7, this.owner))
					{
						ActThrow.Throw(this.owner, chara7.pos, thing6, ThrowMethod.Default, 0f);
						break;
					}
				}
			}
		}
		if (EClass.rnd(20) == 0 && AI_Shopping.TryShop(this.owner, true))
		{
			yield return base.Restart();
		}
		if (EClass.rnd(20) == 0 && this.owner.IsPCFaction && AI_Shopping.TryRestock(this.owner, true))
		{
			yield return base.Restart();
		}
		this.owner.idleActTimer--;
		string a2;
		if (this.owner.idleActTimer <= 0 && !this.owner.source.actIdle.IsEmpty())
		{
			this.owner.idleActTimer = 10 + EClass.rnd(50);
			a2 = idAct;
			if (!(a2 == "torture_snail"))
			{
				if (!(a2 == "buffMage"))
				{
					if (!(a2 == "buffThief"))
					{
						if (!(a2 == "buffGuildWatch"))
						{
							if (!(a2 == "buffHealer"))
							{
								if (!(a2 == "readBook"))
								{
									if (LangGame.Has("idle_" + idAct))
									{
										this.IdleActText(idAct);
									}
								}
								else if (EClass.rnd(2) != 0 && (!this.owner.IsPCParty || EClass.rnd(20) == 0))
								{
									List<Thing> list4 = this.owner.things.List((Thing a) => a.parent == this.owner && (a.category.id == "spellbook" || a.category.id == "ancientbook" || a.category.id == "skillbook"), false);
									Thing thing7 = null;
									if (list4.Count > 0)
									{
										thing7 = list4.RandomItem<Thing>();
										if (!thing7.trait.CanRead(this.owner))
										{
											thing7 = null;
										}
									}
									if (thing7 == null)
									{
										thing7 = ThingGen.CreateFromCategory((EClass.rnd(5) != 0) ? "spellbook" : "ancientbook", -1);
										thing7.isNPCProperty = true;
									}
									if (!(thing7.id == "1084") || !this.owner.IsPCFaction)
									{
										if (!this.owner.HasElement(285, 1))
										{
											this.owner.elements.ModBase(285, 1);
										}
										yield return base.Do(new AI_Read
										{
											target = thing7
										}, null);
									}
								}
							}
							else
							{
								this.TryCast(EffectId.Heal, 100);
							}
						}
						else
						{
							this.TryCast<ConGravity>(EffectId.Gravity, 300 + EClass.rnd(300));
						}
					}
					else
					{
						this.TryCast<ConNightVision>(EffectId.CatsEye, 100 + EClass.rnd(100));
					}
				}
				else if (EClass.rnd(2) == 0)
				{
					this.TryCast<ConHolyVeil>(EffectId.HolyVeil, 300 + EClass.rnd(300));
				}
				else
				{
					this.TryCast<ConLevitate>(EffectId.Levitate, 300 + EClass.rnd(300));
				}
			}
			else
			{
				this.DoSomethingToNearChara((Chara c) => c.race.id == "snail", delegate(Chara c)
				{
					this.owner.Say("use_whip3", this.owner, c, null, null);
					this.owner.PlaySound("whip", 1f, true);
					this.owner.Talk("insult", null, null, false);
					c.PlayAnime(AnimeID.Shiver, false);
					c.OnInsulted();
				});
			}
			yield return base.Restart();
		}
		if (this.owner.host != null)
		{
			yield return base.Restart();
		}
		if (this.owner.HasEditorTag(EditorTag.AINoMove) || this.owner.trait.IdleBehaviour == AI_Idle.Behaviour.NoMove || this.owner.noMove)
		{
			if (this.owner.orgPos != null && !this.owner.pos.Equals(this.owner.orgPos) && !this.owner.orgPos.IsBlocked && !this.owner.orgPos.HasChara)
			{
				yield return base.DoGoto(this.owner.orgPos, 0, false, null);
			}
			yield return base.Restart();
		}
		if (this.owner.HasEditorTag(EditorTag.AIFollow) && this.owner.pos.Distance(EClass.pc.GetDestination()) > 1)
		{
			yield return base.DoGoto(EClass.pc, null);
		}
		if (EClass.rnd(100) == 0 && !this.owner.IsPCFaction)
		{
			if (this.owner.id == "ashland" || this.owner.id == "fiama")
			{
				Trait trait = EClass._map.Installed.traits.restSpots.RandomItem<int, Trait>();
				if (trait != null)
				{
					yield return base.DoGotoSpot(trait.owner, null);
				}
				else
				{
					Room room4 = this.owner.FindRoom();
					if (room4 != null)
					{
						yield return base.DoGoto(room4.GetRandomPoint(true, true).GetNearestPoint(false, true, true, false), 0, false, null);
					}
				}
			}
			else if (this.owner.orgPos != null && !this.owner.pos.Equals(this.owner.orgPos) && !this.owner.orgPos.IsBlocked && !this.owner.orgPos.HasChara && this.owner.orgPos.IsInBounds)
			{
				yield return base.DoGoto(this.owner.orgPos, 0, false, delegate()
				{
					if (!EClass._zone.IsPCFaction)
					{
						this.owner.Teleport(this.owner.orgPos, false, true);
					}
					return AIAct.Status.Success;
				});
			}
		}
		if (EClass.rnd(100) == 0 && this.owner.id == "bee")
		{
			List<Thing> list5 = EClass._map.ListThing<TraitBeekeep>();
			Thing thing8 = (list5 != null) ? list5.RandomItem<Thing>() : null;
			if (thing8 != null)
			{
				yield return base.DoGoto(thing8.pos, 0, false, null);
			}
		}
		a2 = this.owner.source.aiIdle;
		if (!(a2 == "stand") && !(a2 == "root"))
		{
			if (EClass.rnd(15) == 0)
			{
				this.owner.MoveRandom();
			}
			if (this.owner == null)
			{
				yield return this.Cancel();
			}
		}
		if (EClass._zone.IsPCFaction && this.owner.IsPCFaction && !this.owner.IsPCParty && (this.owner.GetWork("Clean") != null || this.owner.GetWork("Chore") != null) && !(EClass.pc.ai is AI_UseCrafter))
		{
			AI_Haul ai_Haul = AI_Haul.TryGetAI(this.owner);
			if (ai_Haul != null)
			{
				yield return base.Do(ai_Haul, null);
			}
		}
		yield return base.Restart();
		yield break;
		yield break;
	}

	public void IdleActText(string id)
	{
		string text = "idle_" + id;
		this.owner.PlaySound(text, 1f, true);
		if (Lang.Game.map.ContainsKey(text))
		{
			this.owner.Say(text, this.owner, null, null);
		}
	}

	public void TryCast<T>(EffectId id, int power = 100) where T : Condition
	{
		if (!this.owner.HasCondition<T>())
		{
			this.TryCast(id, power);
		}
	}

	public void TryCast(EffectId id, int power = 100)
	{
		this.owner.Say("idle_cast", this.owner, null, null);
		ActEffect.Proc(id, power, BlessedState.Normal, this.owner, null, default(ActRef));
	}

	public BaseArea GetRandomAssignedRoom()
	{
		AI_Idle._listRoom.Clear();
		foreach (BaseArea baseArea in EClass._map.rooms.listRoom.Concat(EClass._map.rooms.listArea))
		{
			if (baseArea.type != null && baseArea.type.uidCharas.Contains(this.owner.uid))
			{
				AI_Idle._listRoom.Add(baseArea);
			}
		}
		return AI_Idle._listRoom.RandomItem<BaseArea>();
	}

	public Chara DoSomethingToNearChara(Func<Chara, bool> funcPickChara, Action<Chara> action = null)
	{
		List<Chara> list = this.owner.pos.ListCharasInNeighbor((Chara c) => c != this.owner && this.owner.CanSee(c) && (funcPickChara == null || funcPickChara(c)));
		if (list.Count > 0)
		{
			Chara chara = list.RandomItem<Chara>();
			if (action != null)
			{
				action(chara);
			}
			return chara;
		}
		return null;
	}

	public Chara DoSomethingToCharaInRadius(int radius, Func<Chara, bool> funcPickChara, Action<Chara> action = null)
	{
		List<Chara> list = this.owner.pos.ListCharasInRadius(this.owner, radius, (Chara c) => c != this.owner && this.owner.CanSee(c) && (funcPickChara == null || funcPickChara(c)));
		if (list.Count > 0)
		{
			Chara chara = list.RandomItem<Chara>();
			if (action != null)
			{
				action(chara);
			}
			return chara;
		}
		return null;
	}

	public bool TryPerformIdleUse()
	{
		for (int i = 0; i < 10; i++)
		{
			Point randomPoint = this.owner.pos.GetRandomPoint(7, true, true, true, 100);
			if (randomPoint != null && randomPoint.detail != null)
			{
				foreach (Thing thing in randomPoint.detail.things)
				{
					if (thing.IsInstalled)
					{
						int num = this.owner.Dist(thing);
						if (EClass.rnd((this.owner.memberType == FactionMemberType.Guest) ? 5 : 50) == 0 && thing.HasTag(CTAG.tourism) && num <= 2)
						{
							this.owner.LookAt(thing);
							this.owner.Talk("nice_statue", null, null, false);
							return true;
						}
						if (EClass.rnd(thing.trait.IdleUseChance) == 0 && thing.trait.IdleUse(this.owner, num))
						{
							this.owner.LookAt(thing);
							return true;
						}
					}
				}
			}
		}
		return false;
	}

	[CompilerGenerated]
	private Point <Run>g__FindMovePoint|10_9(BaseArea.AccessType type)
	{
		for (int i = 0; i < 20; i++)
		{
			Point randomPoint = this.owner.pos.GetRandomPoint(5 + i, false, true, false, 100);
			if (randomPoint != null && randomPoint.IsInBounds && (randomPoint.cell.room == null || randomPoint.cell.room.data.accessType == type))
			{
				return randomPoint;
			}
		}
		return null;
	}

	public int maxRepeat = 10;

	public int moveFailCount;

	private static List<BaseArea> _listRoom = new List<BaseArea>();

	public enum Behaviour
	{
		Default,
		NoMove
	}
}
