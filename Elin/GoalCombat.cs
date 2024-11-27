using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GoalCombat : Goal
{
	public override CursorInfo CursorIcon
	{
		get
		{
			return CursorSystem.IconMelee;
		}
	}

	public override bool CancelWhenDamaged
	{
		get
		{
			return false;
		}
	}

	public override bool CanManualCancel()
	{
		return true;
	}

	public Tactics tactics
	{
		get
		{
			return this.owner.tactics;
		}
	}

	public override IEnumerable<AIAct.Status> Run()
	{
		if (this.destEnemy != null)
		{
			this.owner.enemy = this.destEnemy;
			this.destEnemy = null;
		}
		int count = 0;
		int lostCount = 0;
		bool dontWander = this.owner.IsPCParty && !this.owner.IsPC && EClass.game.config.tactics.dontWander;
		for (;;)
		{
			if (EClass.debug.logCombat)
			{
				Debug.Log(string.Concat(new string[]
				{
					"■",
					this.owner.Name,
					"/",
					count.ToString(),
					"/",
					lostCount.ToString()
				}));
			}
			if (EClass._zone.isPeace && this.owner.IsPCFactionOrMinion && !this.owner.IsPC)
			{
				this.owner.enemy = null;
				this.owner.ShowEmo(Emo.happy, 0f, true);
				yield return base.Success(null);
			}
			int num = count;
			count = num + 1;
			if (dontWander && this.owner.enemy != null && !EClass.pc.isBlind && !EClass.pc.CanSeeLos(this.owner.enemy, -1, false) && (this.owner.Dist(EClass.pc) > 4 || this.owner.Dist(this.owner.enemy) > 1))
			{
				Point firstStep = this.owner.GetFirstStep(EClass.pc.pos, PathManager.MoveType.Combat);
				if (firstStep.IsValid && !firstStep.HasChara)
				{
					this.owner.enemy = null;
				}
			}
			this.tc = this.owner.enemy;
			if (this.tc != null && this.owner.IsPCFaction)
			{
				AI_Shear ai_Shear = EClass.pc.ai as AI_Shear;
				if (ai_Shear != null && ai_Shear.target == this.tc)
				{
					this.tc = (this.owner.enemy = null);
				}
			}
			if (this.tc == null || this.tc.isDead || !this.tc.ExistsOnMap || !this.tc.pos.IsInBounds || lostCount >= 5)
			{
				this.tc = (this.owner.enemy = null);
				if (this.owner.IsPC && EClass.game.config.autoCombat.abortOnKill)
				{
					yield return base.Success(null);
				}
				this.owner.FindNewEnemy();
				lostCount = 0;
				if (this.owner.enemy == null)
				{
					yield return base.Success(null);
				}
				this.tc = this.owner.enemy;
			}
			else if (!this.owner.CanSeeLos(this.tc, -1, false))
			{
				num = lostCount;
				lostCount = num + 1;
			}
			else
			{
				lostCount = 0;
			}
			if (this.owner.IsPC && this.tc.HasEditorTag(EditorTag.Invulnerable))
			{
				Msg.Say("abort_idle");
				yield return base.Success(null);
			}
			if (this.tc.IsPCFaction && this.owner.id == "melilith_boss" && EClass._map.plDay.list.Count > 1 && EClass._map.plDay.list[0].data.id != 107)
			{
				EClass._zone.SetBGM(107, true);
			}
			if (this.abilities == null)
			{
				this.abilities = new List<GoalCombat.ItemAbility>();
				this.BuildAbilityList();
			}
			if (this.owner.IsPCFaction && this.tc.IsPCFaction && EClass.rnd(5) == 0 && count > 2)
			{
				this.<Run>g__CalmDown|14_0();
				yield return base.Success(null);
			}
			if (this.owner.OriginalHostility >= Hostility.Neutral && this.tc.OriginalHostility >= Hostility.Neutral && !this.owner.IsPCParty && this.owner.c_bossType == BossType.none && (!(this.owner.trait is TraitGuard) || this.owner.IsPCFaction || !this.tc.IsPCParty || !EClass.player.IsCriminal))
			{
				if (this.owner.calmCheckTurn < 0 || (!this.owner.enemy.IsPCParty && EClass.rnd(10) == 0))
				{
					this.<Run>g__CalmDown|14_0();
					yield return base.Success(null);
				}
				this.owner.calmCheckTurn--;
			}
			if (this.owner.IsPC)
			{
				CursorSystem.ignoreCount = 1;
			}
			if (this.tc.host != null && (this.tc.hp == 0 || EClass.rnd(5) == 0))
			{
				this.tc = this.owner.SetEnemy(this.tc.host);
			}
			if (this.tc.parasite != null && !this.tc.isRestrained && this.tc.parasite.hp > 0 && EClass.rnd(5) == 0)
			{
				this.tc = this.owner.SetEnemy(this.tc.parasite);
			}
			if (this.tc.ride != null && !this.tc.isRestrained && this.tc.ride.hp > 0 && EClass.rnd(5) == 0)
			{
				this.tc = this.owner.SetEnemy(this.tc.ride);
			}
			if (this.tc.enemy != null)
			{
				this.tc.TrySetEnemy(this.owner);
			}
			if (!this.tc.IsMinion && EClass.rnd(10) == 0 && EClass.rnd(this.tc.DEX + 10) > this.owner.LV && this.tc.HasElement(1315, 1) && !this.owner.HasElement(1315, 1) && this.owner.race.IsMachine && this.owner.CanBeTempAlly(this.tc))
			{
				this.owner.Say("dominate_machine", this.tc, this.owner, null, null);
				this.owner.PlayEffect("boost", true, 0f, default(Vector3));
				this.owner.PlaySound("boost", 1f, true);
				this.owner.ShowEmo(Emo.love, 0f, true);
				this.owner.lastEmo = Emo.angry;
				this.owner.MakeMinion(this.tc.IsPCParty ? EClass.pc : this.tc, MinionType.Default);
				yield return base.Success(null);
			}
			if (EClass.rnd(5) == 0 && this.tc.HasElement(1325, 1) && this.owner.race.IsPlant && this.owner.CanBeTempAlly(this.tc))
			{
				this.owner.Say("dominate_plant", this.tc, this.owner, null, null);
				this.owner.ShowEmo(Emo.love, 0f, true);
				this.owner.lastEmo = Emo.angry;
				this.owner.MakeMinion((this.tc.IsPCParty || this.tc.IsPCFactionMinion) ? EClass.pc : this.tc, MinionType.Friend);
				yield return base.Success(null);
			}
			if (EClass.rnd(20) == 0 && this.owner.isRestrained)
			{
				this.owner.Talk("restrained", null, null, false);
			}
			if (this is GoalAutoCombat)
			{
				ActionMode.Adv.SetTurbo(EClass.game.config.autoCombat.turbo ? -1 : 0);
				EClass.pc.ModExp(135, 20);
			}
			int dist = this.owner.Dist(this.tc);
			bool move = this.owner.host == null && (this.tactics.ChanceMove > EClass.rnd(100) || (this.owner.IsPC && this.tc.HasCondition<ConFear>() && dist >= EClass.pc.GetSightRadius() - 1));
			bool haltSecondMove = false;
			if (!this.owner.IsPC && !this.owner.isBlind && this.tc.source.HasTag(CTAG.suicide) && this.owner.IsNeutralOrAbove() && !this.owner.isSummon && !this.owner.IsMinion && !this.tc.HasCondition<ConWet>())
			{
				if (dist <= 3)
				{
					if (EClass.rnd(15) == 0)
					{
						this.owner.Talk("run_suicide", null, null, false);
					}
					if (this.owner.host == null && this.owner.TryMoveFrom(this.tc.pos) != Card.MoveResult.Fail)
					{
						yield return AIAct.Status.Running;
						this.idleCount = 0;
						continue;
					}
				}
				else if (dist <= 5)
				{
					haltSecondMove = true;
					move = false;
				}
			}
			if (dontWander)
			{
				int num2 = this.owner.Dist(EClass.pc);
				if (num2 > 3)
				{
					int x = this.tc.pos.x;
					int z = this.tc.pos.z;
					if (EClass.pc.pos.Distance(this.owner.pos.x + ((x > this.owner.pos.x) ? 1 : ((x < this.owner.pos.x) ? -1 : 0)), this.owner.pos.z + ((z > this.owner.pos.z) ? 1 : ((z < this.owner.pos.z) ? -1 : 0))) >= num2)
					{
						move = false;
						haltSecondMove = true;
					}
				}
			}
			if (this.owner.IsPC && EClass.game.config.autoCombat.bDontChase)
			{
				move = false;
				haltSecondMove = true;
			}
			if (EClass.debug.logCombat)
			{
				Debug.Log(string.Concat(new string[]
				{
					this.owner.Name,
					"/",
					move.ToString(),
					"/",
					haltSecondMove.ToString(),
					"/",
					dist.ToString()
				}));
			}
			if (move)
			{
				if (this.owner.IsPC && dist <= this.owner.GetSightRadius() && this.TryUseAbility(dist, true))
				{
					yield return AIAct.Status.Running;
					this.idleCount = 0;
					continue;
				}
				if (this.TryMove(dist))
				{
					if (EClass.debug.logCombat)
					{
						Debug.Log("moved:" + this.owner.Name);
					}
					yield return AIAct.Status.Running;
					this.idleCount = 0;
					continue;
				}
			}
			if (this.owner == null)
			{
				yield return this.Cancel();
			}
			if (dist <= this.owner.GetSightRadius() && this.TryUseAbility(dist, false))
			{
				yield return AIAct.Status.Running;
				this.idleCount = 0;
			}
			else
			{
				if (EClass.debug.logCombat)
				{
					Debug.Log(string.Concat(new string[]
					{
						this.owner.Name,
						"/",
						move.ToString(),
						"/",
						haltSecondMove.ToString(),
						"/",
						this.tactics.ChanceSecondMove.ToString()
					}));
				}
				if (!move && !haltSecondMove && this.tactics.ChanceSecondMove > EClass.rnd(100) && this.TryMove(dist))
				{
					yield return AIAct.Status.Running;
					this.idleCount = 0;
				}
				else
				{
					if (this.owner == null)
					{
						yield return this.Cancel();
					}
					this.idleCount++;
					if (this.TryAbortCombat())
					{
						yield return base.Success(null);
					}
					if (this.idleCount > 2)
					{
						if (dontWander)
						{
							yield return base.Success(null);
						}
						this.idleCount = 0;
						string aiIdle = this.owner.source.aiIdle;
						if (aiIdle == "stand" || aiIdle == "root")
						{
							yield return base.Success(null);
						}
						yield return base.DoGoto(this.tc.pos, 0, false, null);
					}
					else if (this.owner.FindNearestNewEnemy())
					{
						continue;
					}
					yield return AIAct.Status.Running;
				}
			}
		}
		yield break;
	}

	public bool TryMove(int dist)
	{
		if (this.owner.host != null)
		{
			return false;
		}
		if (this.owner.isBlind)
		{
			return this.owner.MoveRandom();
		}
		int num = this.tc.HasCondition<ConFear>() ? 1 : this.tactics.DestDist;
		if (!this.owner.IsPC && (this.tactics.source.id == "archer" || this.tactics.source.id == "gunner") && !this.owner.TryEquipRanged())
		{
			num = 1;
		}
		if (!this.owner.IsPC && num > 1)
		{
			if (this.tactics.DestDist == 2)
			{
				if (EClass.rnd(5) == 0)
				{
					num = 1;
				}
			}
			else if (this.owner.turn / 3 % 5 > 2)
			{
				num--;
			}
		}
		bool flag = false;
		if (dist > num)
		{
			flag = (this.owner.TryMoveTowards(this.tc.pos) > Card.MoveResult.Fail);
			if (!flag)
			{
				this.moveFail++;
			}
		}
		else if (dist < num)
		{
			flag = (this.owner.TryMoveFrom(this.tc.pos) > Card.MoveResult.Fail);
		}
		if (flag)
		{
			this.moveFail = 0;
		}
		if (EClass.debug.logCombat)
		{
			Debug.Log(string.Concat(new string[]
			{
				"TryMove:",
				this.owner.Name,
				"/",
				flag.ToString(),
				"/",
				dist.ToString(),
				"/",
				num.ToString()
			}));
		}
		return flag;
	}

	public void AddAbility(Act a, int mod = 0, int chance = 100, bool aiPt = false)
	{
		this.abilities.Add(new GoalCombat.ItemAbility
		{
			act = a,
			priorityMod = mod,
			chance = chance,
			aiPt = aiPt
		});
	}

	public virtual bool TryUseRanged(int dist)
	{
		return this.owner.TryEquipRanged() && ACT.Ranged.Perform(this.owner, this.tc, null);
	}

	public virtual bool TryThrow(int dist)
	{
		if (dist > this.owner.GetSightRadius())
		{
			return false;
		}
		Thing thing = this.owner.TryGetThrowable();
		if (thing == null)
		{
			return false;
		}
		if (!ACT.Throw.CanPerform(this.owner, this.tc, this.tc.pos))
		{
			return false;
		}
		ActThrow.Throw(this.owner, this.tc.pos, this.tc, thing.HasElement(410, 1) ? thing : thing.Split(1), ThrowMethod.Default);
		return true;
	}

	public virtual bool TryUseAbility(int dist, bool beforeMove = false)
	{
		GoalCombat.<>c__DisplayClass19_0 CS$<>8__locals1 = new GoalCombat.<>c__DisplayClass19_0();
		CS$<>8__locals1.<>4__this = this;
		if (this.abilities.Count == 0)
		{
			string str = "no ability:";
			Chara owner = this.owner;
			Debug.Log(str + ((owner != null) ? owner.ToString() : null));
			return false;
		}
		CS$<>8__locals1.numEnemy = -1;
		CS$<>8__locals1.numFriend = -1;
		CS$<>8__locals1.numNeutral = -1;
		CS$<>8__locals1.charaBuilt = false;
		bool flag = this.owner.CanSeeLos(this.tc, dist, false);
		bool isPCFaction = this.owner.IsPCFaction;
		bool flag2 = this.owner.HasCondition<ConSilence>();
		bool isBlind = this.owner.isBlind;
		bool flag3 = this.owner.HasCondition<ConFear>();
		bool isConfused = this.owner.isConfused;
		bool flag4 = this.owner.HasCondition<ConDim>();
		foreach (GoalCombat.ItemAbility itemAbility in this.abilities)
		{
			if (EClass.rnd(100) >= itemAbility.chance)
			{
				itemAbility.priority = 0;
			}
			else
			{
				Act act = itemAbility.act;
				SourceElement.Row s = act.source;
				itemAbility.priority = 0;
				itemAbility.tg = null;
				itemAbility.pt = false;
				if (s.abilityType.Length != 0 && (!this.owner.IsPC || !flag2 || !(act is Spell)) && (!beforeMove || act.HasTag("before_move")))
				{
					string text = s.abilityType[0];
					if (this.id != 6602 || (dist > 1 && !this.tc.HasCondition<ConEntangle>()))
					{
						uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
						int num2;
						if (num <= 2140930120U)
						{
							if (num <= 979982427U)
							{
								if (num <= 115652151U)
								{
									if (num != 13211034U)
									{
										if (num != 115652151U)
										{
											goto IL_BCA;
										}
										if (!(text == "debuff"))
										{
											goto IL_BCA;
										}
										if (flag)
										{
											num2 = this.tactics.P_Debuff;
											goto IL_BCD;
										}
										continue;
									}
									else
									{
										if (!(text == "summon"))
										{
											goto IL_BCA;
										}
										if (this.owner.isRestrained || (this.tc != null && this.tc.IsRestrainedResident))
										{
											continue;
										}
										int num3 = EClass._zone.CountMinions(this.owner);
										if (num3 < this.owner.MaxSummon)
										{
											num2 = this.tactics.P_Summon - 20 * num3 / this.owner.MaxSummon;
											goto IL_BCD;
										}
										continue;
									}
								}
								else if (num != 601369519U)
								{
									if (num != 740945997U)
									{
										if (num != 979982427U)
										{
											goto IL_BCA;
										}
										if (!(text == "heal"))
										{
											goto IL_BCA;
										}
										goto IL_72E;
									}
									else
									{
										if (!(text == "any"))
										{
											goto IL_BCA;
										}
										num2 = 50;
										goto IL_BCD;
									}
								}
								else
								{
									if (!(text == "suicide"))
									{
										goto IL_BCA;
									}
									if (this.owner.IsPC || this.owner.HasCondition<ConWet>())
									{
										continue;
									}
									if (this.owner.HasTag(CTAG.kamikaze))
									{
										num2 = ((dist <= 1) ? 1000 : 0);
										goto IL_BCD;
									}
									num2 = 100 - 125 * this.owner.hp / this.owner.MaxHP;
									if (EClass.rnd(200) > num2)
									{
										continue;
									}
									goto IL_BCD;
								}
							}
							else if (num <= 1167440125U)
							{
								if (num != 1000963043U)
								{
									if (num != 1167440125U)
									{
										goto IL_BCA;
									}
									if (!(text == "attack"))
									{
										goto IL_BCA;
									}
								}
								else if (!(text == "attackMelee"))
								{
									goto IL_BCA;
								}
							}
							else if (num != 1244307660U)
							{
								if (num != 2043306875U)
								{
									if (num != 2140930120U)
									{
										goto IL_BCA;
									}
									if (!(text == "summonAlly"))
									{
										goto IL_BCA;
									}
									if (!this.owner.isRestrained && (this.tc == null || !this.tc.IsRestrainedResident))
									{
										if (this.owner.IsPC)
										{
											if (EClass.player.lastEmptyAlly <= 0)
											{
												continue;
											}
										}
										else if (EClass._zone.CountMinions(this.owner) > 0)
										{
											continue;
										}
										num2 = this.tactics.P_Summon;
										goto IL_BCD;
									}
									continue;
								}
								else
								{
									if (!(text == "melee"))
									{
										goto IL_BCA;
									}
									if (dist > this.owner.body.GetMeleeDistance())
									{
										continue;
									}
									num2 = (flag3 ? (this.owner.IsPC ? 0 : (this.tactics.P_Melee / 2)) : this.tactics.P_Melee);
									if (isConfused)
									{
										num2 -= (this.owner.IsPC ? 30 : 10);
									}
									if (isBlind)
									{
										num2 -= (this.owner.IsPC ? 50 : 10);
									}
									if (this.tc.HasElement(1221, 1))
									{
										num2 -= 40;
									}
									if (this.tc.HasElement(1223, 1))
									{
										num2 -= 40;
										goto IL_BCD;
									}
									goto IL_BCD;
								}
							}
							else
							{
								if (!(text == "buff"))
								{
									goto IL_BCA;
								}
								num2 = CS$<>8__locals1.<TryUseAbility>g__ForeachChara|4(itemAbility, delegate(Chara c)
								{
									if (!c.HasCondition(s.proc[1]))
									{
										return CS$<>8__locals1.<>4__this.tactics.P_Buff;
									}
									return 0;
								}, true);
								if (itemAbility.aiPt || (this.owner.IsPC && this.tactics.CastPartyBuff))
								{
									itemAbility.pt = true;
									goto IL_BCD;
								}
								goto IL_BCD;
							}
						}
						else if (num <= 2762313447U)
						{
							if (num <= 2360683452U)
							{
								if (num != 2301512864U)
								{
									if (num != 2360683452U)
									{
										goto IL_BCA;
									}
									if (!(text == "attackArea"))
									{
										goto IL_BCA;
									}
									if (this.owner.isRestrained || (this.tc != null && this.tc.IsRestrainedResident))
									{
										continue;
									}
									bool flag5 = itemAbility.act is ActBolt;
									if (!flag || (this.owner.IsPCParty && (EClass._zone.IsTown || EClass._zone.IsPCFaction)))
									{
										continue;
									}
									CS$<>8__locals1.<TryUseAbility>g__GetNumEnemy|0(flag5 ? 6 : 5);
									if (CS$<>8__locals1.numEnemy != 0 && (!this.owner.IsPCFactionOrMinion || CS$<>8__locals1.<TryUseAbility>g__GetNumNeutral|1(flag5 ? 6 : 5) <= 0))
									{
										num2 = this.tactics.P_Spell - 20 + CS$<>8__locals1.numEnemy * 10 + CS$<>8__locals1.<TryUseAbility>g__GetAttackMod|5(act);
										goto IL_BCD;
									}
									continue;
								}
								else
								{
									if (!(text == "wait"))
									{
										goto IL_BCA;
									}
									if (!this.owner.IsPCParty)
									{
										num2 = 50;
										goto IL_BCD;
									}
									continue;
								}
							}
							else if (num != 2627848062U)
							{
								if (num != 2671260646U)
								{
									if (num != 2762313447U)
									{
										goto IL_BCA;
									}
									if (!(text == "buffStats"))
									{
										goto IL_BCA;
									}
									num2 = CS$<>8__locals1.<TryUseAbility>g__ForeachChara|4(itemAbility, delegate(Chara c)
									{
										Element buffStats = c.GetBuffStats(s.proc[1]);
										if (buffStats != null && buffStats.Value >= 0)
										{
											return 0;
										}
										return CS$<>8__locals1.<>4__this.tactics.P_Buff;
									}, true);
									if (itemAbility.aiPt || (this.owner.IsPC && this.tactics.CastPartyBuff))
									{
										itemAbility.pt = true;
										goto IL_BCD;
									}
									goto IL_BCD;
								}
								else
								{
									if (!(text == "item"))
									{
										goto IL_BCA;
									}
									num2 = (itemAbility.act as ActItem).BuildAct(this.owner);
									goto IL_BCD;
								}
							}
							else
							{
								if (!(text == "ground"))
								{
									goto IL_BCA;
								}
								if (!flag || this.owner.isRestrained || (this.tc != null && this.tc.IsRestrainedResident))
								{
									continue;
								}
								num2 = 50;
								if (isPCFaction)
								{
									num2 -= 10;
									goto IL_BCD;
								}
								goto IL_BCD;
							}
						}
						else if (num <= 3546849056U)
						{
							if (num != 3164374115U)
							{
								if (num != 3289626814U)
								{
									if (num != 3546849056U)
									{
										goto IL_BCA;
									}
									if (!(text == "dot"))
									{
										goto IL_BCA;
									}
								}
								else
								{
									if (!(text == "teleport"))
									{
										goto IL_BCA;
									}
									num2 = 40;
									goto IL_BCD;
								}
							}
							else
							{
								if (!(text == "taunt"))
								{
									goto IL_BCA;
								}
								bool flag6 = this.owner.HasCondition<StanceTaunt>();
								bool flag7 = this.tactics.source.taunt != -1 && 100 * this.owner.hp / this.owner.MaxHP >= this.tactics.source.taunt;
								num2 = ((flag6 && !flag7) ? 100 : ((!flag6 && flag7) ? 100 : 0));
								goto IL_BCD;
							}
						}
						else if (num != 4208725202U)
						{
							if (num != 4274235348U)
							{
								if (num != 4289936826U)
								{
									goto IL_BCA;
								}
								if (!(text == "debuffStats"))
								{
									goto IL_BCA;
								}
								if (flag)
								{
									num2 = CS$<>8__locals1.<TryUseAbility>g__ForeachChara|4(itemAbility, delegate(Chara c)
									{
										Element buffStats = c.GetBuffStats(s.proc[1]);
										if (buffStats != null && buffStats.Value <= 0)
										{
											return 0;
										}
										return CS$<>8__locals1.<>4__this.tactics.P_Debuff;
									}, false);
									goto IL_BCD;
								}
								continue;
							}
							else
							{
								if (!(text == "hot"))
								{
									goto IL_BCA;
								}
								goto IL_72E;
							}
						}
						else
						{
							if (!(text == "range"))
							{
								goto IL_BCA;
							}
							if (!flag || EClass.rnd(100) > this.tactics.RangedChance)
							{
								continue;
							}
							num2 = (flag3 ? (this.owner.IsPC ? 0 : (this.tactics.P_Range / 2)) : this.tactics.P_Range);
							if (isConfused)
							{
								num2 -= (this.owner.IsPC ? 30 : 10);
							}
							if (isBlind)
							{
								num2 -= (this.owner.IsPC ? 50 : 10);
							}
							if (this.owner.ranged != null && this.owner.ranged.trait is TraitToolRangeCane && this.owner.mana.value <= 0)
							{
								continue;
							}
							goto IL_BCD;
						}
						if (!flag)
						{
							continue;
						}
						bool flag8 = text == "dot";
						if (flag8 && (this.owner.isRestrained || (this.tc != null && this.tc.IsRestrainedResident)))
						{
							continue;
						}
						num2 = ((text == "attackMelee") ? this.tactics.P_Melee : this.tactics.P_Spell) + CS$<>8__locals1.<TryUseAbility>g__GetAttackMod|5(act);
						if (num2 > 0 && flag8)
						{
							num2 += 10;
						}
						IL_BCD:
						if (s.target == "Neighbor")
						{
							if (dist > 1)
							{
								continue;
							}
							num2 += 10;
						}
						if (s.proc.Length == 0 || !(s.proc[0] == "Debuff") || !this.tc.HasCondition(s.proc[1]))
						{
							if (s.abilityType.Length > 1)
							{
								num2 += (this.owner.IsPC ? s.abilityType[2] : s.abilityType[1]).ToInt();
							}
							if (act is Spell)
							{
								if (this.owner.IsPC)
								{
									if (act.vPotential <= 0)
									{
										continue;
									}
									if (flag2 || isConfused || flag4)
									{
										num2 -= 50;
									}
								}
								else
								{
									if (flag2)
									{
										num2 -= 30;
									}
									if (isConfused || flag4)
									{
										num2 -= 10;
									}
								}
							}
							if (num2 > 0)
							{
								num2 += itemAbility.priorityMod + EClass.rnd(this.tactics.RandomFacotr + itemAbility.priorityMod);
							}
							itemAbility.priority = num2;
							continue;
						}
						continue;
						IL_72E:
						bool isHOT = text == "hot";
						num2 = CS$<>8__locals1.<TryUseAbility>g__ForeachChara|4(itemAbility, (Chara c) => base.<TryUseAbility>g__HealFactor|7(c), true);
						if (itemAbility.aiPt || (this.owner.IsPC && this.tactics.CastPartyBuff))
						{
							itemAbility.pt = true;
							goto IL_BCD;
						}
						goto IL_BCD;
						IL_BCA:
						num2 = 0;
						goto IL_BCD;
					}
				}
			}
		}
		this.abilities.Sort((GoalCombat.ItemAbility a, GoalCombat.ItemAbility b) => b.priority - a.priority);
		foreach (GoalCombat.ItemAbility itemAbility2 in this.abilities)
		{
			if (itemAbility2.priority > 0)
			{
				if (EClass.debug.logCombat && this.owner.IsPC)
				{
					Debug.Log(itemAbility2.act.Name + "/" + itemAbility2.priority.ToString());
				}
				if (itemAbility2.act.source.alias == "ActRanged")
				{
					if (this.TryThrow(dist))
					{
						return true;
					}
					if (this.TryUseRanged(dist))
					{
						return true;
					}
				}
				else
				{
					Act.Cost cost = itemAbility2.act.GetCost(this.owner);
					if (!this.owner.IsPCParty || !itemAbility2.pt || itemAbility2.act.TargetType.ForceParty || cost.cost * EClass.pc.party.members.Count <= this.owner.mana.value)
					{
						if (isPCFaction && cost.cost > 0)
						{
							Act.CostType type = cost.type;
							if (type != Act.CostType.MP)
							{
								if (type == Act.CostType.SP)
								{
									if (cost.cost > this.owner.stamina.value)
									{
										continue;
									}
								}
							}
							else if (cost.cost > this.owner.mana.value)
							{
								continue;
							}
						}
						if ((cost.cost <= 0 || EClass.rnd(100) <= this.tactics.AbilityChance) && itemAbility2.act.CanPerform(this.owner, itemAbility2.tg ?? this.tc, null) && this.owner.UseAbility(itemAbility2.act, itemAbility2.tg ?? this.tc, null, (itemAbility2.act.HaveLongPressAction && itemAbility2.pt) || itemAbility2.aiPt))
						{
							if (EClass.debug.logCombat)
							{
								string[] array = new string[9];
								array[0] = this.owner.Name;
								array[1] = "/";
								array[2] = itemAbility2.act.id.ToString();
								array[3] = "/";
								array[4] = itemAbility2.act.CanPerform(this.owner, itemAbility2.tg ?? this.tc, null).ToString();
								array[5] = "/";
								int num4 = 6;
								Chara tg = itemAbility2.tg;
								array[num4] = ((tg != null) ? tg.ToString() : null);
								array[7] = "/";
								int num5 = 8;
								Chara chara = this.tc;
								array[num5] = ((chara != null) ? chara.ToString() : null);
								Debug.Log(string.Concat(array));
							}
							return true;
						}
					}
				}
			}
		}
		if (EClass.debug.logCombat)
		{
			Debug.Log(this.owner.Name + "/" + this.abilities.Count.ToString());
			foreach (GoalCombat.ItemAbility itemAbility3 in this.abilities)
			{
				Debug.Log(itemAbility3.act.Name);
			}
		}
		return false;
	}

	public virtual void BuildAbilityList()
	{
		foreach (ActList.Item item in this.owner.ability.list.items)
		{
			this.AddAbility(item.act, 0, item.chance, item.pt);
		}
		this.AddAbility(ACT.Ranged, 0, 100, false);
		this.AddAbility(ACT.Melee, 0, 100, false);
		this.AddAbility(ACT.Item, 0, 100, false);
	}

	public virtual bool TryAbortCombat()
	{
		return false;
	}

	[CompilerGenerated]
	private void <Run>g__CalmDown|14_0()
	{
		this.owner.enemy = null;
		if (this.owner.ride != null)
		{
			this.owner.ride.enemy = null;
		}
		if (this.owner.parasite != null)
		{
			this.owner.parasite.enemy = null;
		}
		this.owner.hostility = this.owner.OriginalHostility;
		if (this.tc.enemy == this.owner)
		{
			this.tc.enemy = null;
			if (this.tc.ride != null)
			{
				this.tc.ride.enemy = null;
			}
			if (this.tc.parasite != null)
			{
				this.tc.parasite.enemy = null;
			}
			this.tc.hostility = this.tc.OriginalHostility;
		}
		this.owner.Say("calmDown", this.owner, null, null);
	}

	public Chara destEnemy;

	public Chara tc;

	public int idleCount;

	public int moveFail;

	public List<GoalCombat.ItemAbility> abilities;

	public List<Chara> charas = new List<Chara>();

	public class ItemAbility
	{
		public Act act;

		public int priority;

		public int priorityMod;

		public int chance;

		public Chara tg;

		public bool pt;

		public bool aiPt;
	}
}
