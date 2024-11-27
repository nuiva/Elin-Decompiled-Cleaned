using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActEffect : EClass
{
	public static void TryDelay(Action a)
	{
		if (ActEffect.RapidCount == 0)
		{
			a();
			return;
		}
		TweenUtil.Delay((float)ActEffect.RapidCount * ActEffect.RapidDelay, delegate
		{
			a();
		});
	}

	public static bool DamageEle(Card CC, EffectId id, int power, Element e, List<Point> points, ActRef actref, string lang = null)
	{
		if (points.Count == 0)
		{
			CC.SayNothingHappans();
			return false;
		}
		if (!EClass.setting.elements.ContainsKey(e.source.alias))
		{
			Debug.Log(e.source.alias);
			e = Element.Create(0, 1);
		}
		ElementRef elementRef = EClass.setting.elements[e.source.alias];
		Act act = actref.act;
		int num = (act != null) ? act.ElementPowerMod : 50;
		int num2 = 0;
		Point point = CC.pos.Copy();
		List<Card> list = new List<Card>();
		bool flag = false;
		if (id == EffectId.Explosive && actref.refThing != null)
		{
			power = power * actref.refThing.material.hardness / 10;
		}
		string text = id.ToString();
		string text2 = EClass.sources.calc.map.ContainsKey(text) ? text : (text.ToLower() + "_");
		using (List<Point>.Enumerator enumerator = points.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Point p = enumerator.Current;
				bool flag2 = true;
				if (id <= EffectId.BallBubble)
				{
					if (id == EffectId.Explosive)
					{
						text2 = "ball_";
						flag = false;
						goto IL_1DB;
					}
					if (id == EffectId.BallBubble)
					{
						text2 = "ball_";
						goto IL_1DB;
					}
				}
				else
				{
					if (id == EffectId.Meteor)
					{
						text2 = "SpMeteor";
						goto IL_1DB;
					}
					if (id == EffectId.Earthquake)
					{
						text2 = "SpEarthquake";
						flag2 = false;
						flag = true;
						goto IL_1DB;
					}
					if (id == EffectId.Suicide)
					{
						goto IL_1DB;
					}
				}
				if (CC.isChara && p.Equals(CC.pos) && points.Count >= 2)
				{
					continue;
				}
				IL_1DB:
				Effect effect = null;
				Effect effect2 = flag2 ? Effect.Get("trail1") : null;
				Point from = p;
				if (id != EffectId.Arrow)
				{
					if (id != EffectId.Earthquake)
					{
						effect = Effect.Get("Element/ball_" + ((e.id == 0) ? "Void" : e.source.alias.Remove(0, 3)));
						if (effect == null)
						{
							effect = Effect.Get("Element/ball_Fire");
						}
						float startDelay = ((id == EffectId.Meteor) ? 0.1f : 0.04f) * (float)CC.pos.Distance(p);
						effect.SetStartDelay(startDelay);
						effect2.SetStartDelay(startDelay);
					}
					else
					{
						if (EClass.rnd(4) == 0 && p.IsSync)
						{
							effect = Effect.Get("smoke_earthquake");
						}
						float num3 = 0.06f * (float)CC.pos.Distance(p);
						Point pos = p.Copy();
						TweenUtil.Tween(num3, null, delegate()
						{
							pos.Animate(AnimeID.Quake, true);
						});
						if (effect != null)
						{
							effect.SetStartDelay(num3);
						}
					}
				}
				else
				{
					effect = Effect.Get("spell_arrow");
					effect.sr.color = elementRef.colorSprite;
					TrailRenderer componentInChildren = effect.GetComponentInChildren<TrailRenderer>();
					componentInChildren.startColor = (componentInChildren.endColor = elementRef.colorSprite);
					from = CC.pos;
				}
				if (effect2 != null)
				{
					effect2.SetParticleColor(elementRef.colorTrail, true, "_TintColor").Play(from, 0f, null, null);
				}
				if (effect != null)
				{
					if (id == EffectId.Arrow)
					{
						ActEffect.TryDelay(delegate
						{
							effect.Play(CC.pos, 0f, p, null);
						});
					}
					else
					{
						ActEffect.TryDelay(delegate
						{
							effect.Play(p, 0f, null, null).Flip(p.x > CC.pos.x, false);
						});
					}
				}
				bool flag3 = false;
				if (CC.IsPCFactionOrMinion && (CC.HasElement(1651, 1) || EClass.pc.Evalue(1651) >= 2))
				{
					bool flag4 = false;
					foreach (Card card in p.ListCards(false))
					{
						if (card.isChara)
						{
							if (card.IsPCFactionOrMinion)
							{
								flag4 = true;
							}
						}
						else if (e.id != 910 || !card.IsFood || !card.category.IsChildOf("foodstuff"))
						{
							flag4 = true;
						}
					}
					flag3 = flag4;
				}
				if (!flag3)
				{
					if (e.id == 910)
					{
						EClass._map.TryShatter(p, 910, power);
					}
					if (e.id == 911)
					{
						EClass._map.TryShatter(p, 911, power);
					}
				}
				foreach (Card card2 in p.ListCards(false).ToList<Card>())
				{
					Card c = card2;
					if ((c.isChara || c.trait.CanBeAttacked) && (!c.IsMultisize || card2 != CC) && (!c.isChara || (c.Chara.host != CC && c.Chara.parasite != CC && c.Chara.ride != CC)))
					{
						if (id - EffectId.Arrow <= 1 && c.isChara && CC.isChara)
						{
							c.Chara.RequestProtection(CC.Chara, delegate(Chara a)
							{
								c = a;
							});
						}
						bool isChara = CC.isChara;
						int num4;
						if (id == EffectId.Suicide)
						{
							num4 = CC.MaxHP * 2;
							num4 = num4 * 100 / (50 + point.Distance(p) * 75);
							if (c.HasTag(CTAG.suicide) && !c.HasCondition<ConWet>())
							{
								list.Add(c);
							}
						}
						else
						{
							Dice dice = Dice.Create(text2, power, CC, actref.act);
							if (dice == null)
							{
								Debug.Log(text2);
							}
							num4 = dice.Roll();
							if (id == EffectId.Earthquake)
							{
								if (c.HasCondition<ConGravity>())
								{
									num4 = dice.RollMax() * 2;
								}
								else if (c.isChara && c.Chara.IsLevitating)
								{
									num4 /= 2;
								}
							}
							if (id == EffectId.Ball || id == EffectId.BallBubble || id == EffectId.Explosive)
							{
								num4 = num4 * 100 / (90 + point.Distance(p) * 10);
							}
						}
						if ((!actref.noFriendlyFire || CC.Chara.IsHostile(c as Chara)) && (!flag || c != CC))
						{
							if (isChara && points.Count > 1 && c != null && c.isChara && CC.isChara && CC.Chara.IsFriendOrAbove(c.Chara))
							{
								int num5 = CC.Evalue(302);
								if (!CC.IsPC && CC.IsPCFactionOrMinion)
								{
									num5 += EClass.pc.Evalue(302);
								}
								if (num5 > 0)
								{
									if (num5 * 10 > EClass.rnd(num4 + 1))
									{
										if (c == c.pos.FirstChara)
										{
											CC.ModExp(302, CC.IsPC ? 10 : 50);
											continue;
										}
										continue;
									}
									else
									{
										num4 = EClass.rnd(num4 * 100 / (100 + num5 * 10 + 1));
										if (c == c.pos.FirstChara)
										{
											CC.ModExp(302, CC.IsPC ? 20 : 100);
										}
										if (num4 == 0)
										{
											continue;
										}
									}
								}
								if (CC.HasElement(1214, 1) || (!CC.IsPC && (CC.IsPCFaction || CC.IsPCFactionMinion) && EClass.pc.HasElement(1214, 1) && EClass.rnd(5) != 0))
								{
									continue;
								}
							}
							if (!lang.IsEmpty())
							{
								if (lang == "spell_hand")
								{
									string[] list2 = Lang.GetList("attack" + (CC.isChara ? CC.Chara.race.meleeStyle.IsEmpty("Touch") : "Touch"));
									string @ref = "_elehand".lang(e.source.GetAltname(2), list2[4], null, null, null);
									CC.Say(c.IsPCParty ? "cast_hand_ally" : "cast_hand", CC, c, @ref, c.IsPCParty ? list2[1] : list2[2]);
								}
								else
								{
									CC.Say(lang + "_hit", CC, c, e.Name.ToLower(), null);
								}
							}
							Chara chara = CC.isChara ? CC.Chara : ((actref.refThing != null) ? EClass._map.FindChara(actref.refThing.c_uidRefCard) : null);
							c.DamageHP(num4, e.id, power * num / 100, AttackSource.None, chara ?? CC, true);
							if (id == EffectId.Explosive && CC.trait is TraitCookerMicrowave)
							{
								chara = EClass.pc;
							}
							if (chara != null && chara.IsAliveInCurrentZone)
							{
								chara.DoHostileAction(c, false);
							}
							num2++;
						}
					}
				}
				if ((id == EffectId.Explosive || id == EffectId.Suicide) && ((id != EffectId.Suicide && id != EffectId.Meteor) || !EClass._zone.IsPCFaction))
				{
					int num6 = (id == EffectId.Meteor) ? (50 + power / 20) : ((id == EffectId.Suicide) ? (CC.LV / 3 + 40) : ((actref.refThing != null) ? actref.refThing.material.hardness : (30 + power / 20)));
					bool flag5 = EClass._zone.HasLaw && !EClass._zone.IsPCFaction && (CC.IsPC || (id == EffectId.Explosive && actref.refThing == null)) && !(EClass._zone is Zone_Vernis);
					if (p.HasObj && p.cell.matObj.hardness <= num6)
					{
						EClass._map.MineObj(p, null, null);
						if (flag5)
						{
							EClass.player.ModKarma(-1);
						}
					}
					if (!p.HasObj && p.HasBlock && p.matBlock.hardness <= num6)
					{
						EClass._map.MineBlock(p, false, null);
						if (flag5)
						{
							EClass.player.ModKarma(-1);
						}
					}
				}
				if (e.id == 910)
				{
					int num7 = 0;
					if (id == EffectId.Meteor)
					{
						num7 = 2;
					}
					if (EClass._zone.IsPCFaction && EClass._zone.branch.lv >= 3)
					{
						num7 = 0;
					}
					if (num7 > EClass.rnd(10))
					{
						p.ModFire(4 + EClass.rnd(10));
					}
				}
				if (e.id == 911)
				{
					p.ModFire(-20);
				}
			}
		}
		if (ActEffect.RapidCount == 0)
		{
			foreach (Card card3 in list)
			{
				if (card3.ExistsOnMap)
				{
					ActEffect.RapidCount += 2;
					ActEffect.ProcAt(id, power, BlessedState.Normal, card3, null, card3.pos, true, actref);
				}
			}
		}
		return num2 > 0;
	}

	public static void ProcAt(EffectId id, int power, BlessedState state, Card cc, Card tc, Point tp, bool isNeg, ActRef actRef = default(ActRef))
	{
		Chara CC = cc.Chara;
		bool flag = state <= BlessedState.Cursed;
		bool flag2 = isNeg || flag;
		Element element = Element.Create(actRef.aliasEle.IsEmpty("eleFire"), power / 10);
		if (EClass.debug.enable && EInput.isShiftDown)
		{
			ActEffect.angle += 5;
			if (ActEffect.angle > 100)
			{
				ActEffect.angle = 30;
			}
			Debug.Log(ActEffect.angle);
		}
		if (ActEffect.RapidCount > 0)
		{
			power = power * 100 / (100 + ActEffect.RapidCount * 50);
		}
		if (id <= EffectId.Earthquake)
		{
			switch (id)
			{
			case EffectId.Breathe:
			{
				List<Point> list = EClass._map.ListPointsInArc(CC.pos, tp, 7, 35f);
				if (list.Count == 0)
				{
					list.Add(CC.pos.Copy());
				}
				CC.Say("spell_breathe", CC, element.Name.ToLower(), null);
				EClass.Wait(0.8f, CC);
				ActEffect.TryDelay(delegate
				{
					CC.PlaySound("spell_breathe", 1f, true);
				});
				if (CC.IsInMutterDistance(10) && !EClass.core.config.graphic.disableShake)
				{
					Shaker.ShakeCam("breathe", 1f);
				}
				ActEffect.DamageEle(CC, id, power, element, list, actRef, "spell_breathe");
				return;
			}
			case EffectId.Ball:
			case EffectId.Explosive:
			case EffectId.BallBubble:
				goto IL_9C1;
			case EffectId.Bolt:
			{
				List<Point> list2 = EClass._map.ListPointsInLine(CC.pos, tp, 10);
				if (list2.Count == 0)
				{
					list2.Add(CC.pos.Copy());
				}
				CC.Say("spell_bolt", CC, element.Name.ToLower(), null);
				EClass.Wait(0.8f, CC);
				ActEffect.TryDelay(delegate
				{
					CC.PlaySound("spell_bolt", 1f, true);
				});
				if (CC.IsInMutterDistance(10) && !EClass.core.config.graphic.disableShake)
				{
					Shaker.ShakeCam("bolt", 1f);
				}
				ActEffect.DamageEle(CC, id, power, element, list2, actRef, "spell_bolt");
				return;
			}
			case EffectId.Arrow:
			{
				List<Point> list3 = new List<Point>();
				list3.Add(tp.Copy());
				CC.Say("spell_arrow", CC, element.Name.ToLower(), null);
				EClass.Wait(0.5f, CC);
				ActEffect.TryDelay(delegate
				{
					CC.PlaySound("spell_arrow", 1f, true);
				});
				ActEffect.DamageEle(CC, id, power, element, list3, actRef, "spell_arrow");
				return;
			}
			case EffectId.Hand:
				break;
			case EffectId.Funnel:
			{
				if (EClass._zone.CountMinions(CC) > CC.MaxSummon || CC.c_uidMaster != 0)
				{
					CC.Say("summon_ally_fail", CC, null, null);
					return;
				}
				CC.Say("spell_funnel", CC, element.Name.ToLower(), null);
				CC.PlaySound("spell_funnel", 1f, true);
				Chara chara = CharaGen.Create("bit", -1);
				chara.SetMainElement(element.source.alias, element.Value, true);
				chara.SetSummon(20 + power / 20 + EClass.rnd(10));
				chara.SetLv(power / 15);
				EClass._zone.AddCard(chara, tp.GetNearestPoint(false, false, true, false));
				chara.PlayEffect("teleport", true, 0f, default(Vector3));
				chara.MakeMinion(CC, MinionType.Default);
				return;
			}
			case EffectId.TransGender:
			case EffectId.ModPotential:
			case EffectId.Buff:
			case EffectId.Debuff:
			case EffectId.Weaken:
			case EffectId.NeckHunt:
				goto IL_DF7;
			case EffectId.Summon:
			{
				CC.Say("summon_ally", CC, null, null);
				if (EClass._zone.CountMinions(CC) > CC.MaxSummon || CC.c_uidMaster != 0)
				{
					CC.Say("summon_ally_fail", CC, null, null);
					return;
				}
				string id2 = actRef.n1;
				int num = 1;
				int num2 = -1;
				int radius = 3;
				bool flag3 = false;
				int num3 = -1;
				string n = actRef.n1;
				if (!(n == "shadow"))
				{
					if (!(n == "monster") && !(n == "fire") && !(n == "animal"))
					{
						if (!(n == "special_force"))
						{
							if (n == "tentacle")
							{
								num2 = 20 + EClass.rnd(10);
								radius = 1;
							}
						}
						else
						{
							id2 = "army_palmia";
							num = 4 + EClass.rnd(2);
							num3 = EClass._zone.DangerLv;
						}
					}
					else
					{
						num = 1 + EClass.rnd(2);
					}
				}
				else
				{
					num = Mathf.Clamp(power / 100, 1, 5) + ((power < 100) ? 0 : EClass.rnd(2));
				}
				int num4 = 0;
				while (num4 < num && EClass._zone.CountMinions(CC) <= CC.MaxSummon)
				{
					Point randomPoint = tp.GetRandomPoint(radius, true, true, false, 100);
					Point point = (randomPoint != null) ? randomPoint.GetNearestPoint(false, false, true, false) : null;
					if (point != null && point.IsValid)
					{
						if (num3 != -1)
						{
							CardBlueprint.Set(new CardBlueprint
							{
								lv = num3
							});
						}
						n = actRef.n1;
						Chara chara2;
						if (!(n == "pawn"))
						{
							if (!(n == "monster"))
							{
								if (!(n == "animal"))
								{
									if (!(n == "fire"))
									{
										chara2 = CharaGen.Create(id2, power / 10);
									}
									else
									{
										chara2 = CharaGen.CreateFromElement("Fire", power / 10, "chara");
									}
								}
								else
								{
									chara2 = CharaGen.CreateFromFilter("c_animal", power / 15, -1);
								}
							}
							else
							{
								chara2 = CharaGen.CreateFromFilter("c_dungeon", power / 10, -1);
							}
						}
						else
						{
							chara2 = CharaGen.CreateFromFilter("c_pawn", power / 10, -1);
						}
						if (chara2 != null)
						{
							int num5;
							if (actRef.n1 == "shadow")
							{
								num5 = power / 10 + 1;
							}
							else
							{
								num5 = chara2.LV * (100 + power / 10) / 100 + power / 30;
							}
							if (chara2.LV < num5)
							{
								chara2.SetLv(num5);
							}
							n = actRef.n1;
							if (!(n == "shadow"))
							{
								if (n == "special_force")
								{
									chara2.homeZone = EClass._zone;
								}
							}
							else
							{
								chara2.hp = chara2.MaxHP / 2;
							}
							EClass._zone.AddCard(chara2, point);
							if (!(actRef.n1 == "monster") || actRef.refThing == null)
							{
								chara2.MakeMinion(CC, MinionType.Default);
							}
							if (num2 != -1)
							{
								chara2.SetSummon(num2);
							}
							flag3 = true;
						}
					}
					num4++;
				}
				if (!flag3)
				{
					CC.Say("summon_ally_fail", CC, null, null);
				}
				return;
			}
			case EffectId.Bubble:
			case EffectId.Web:
			case EffectId.MistOfDarkness:
			case EffectId.Puddle:
			{
				if (LangGame.Has("ab" + id.ToString()))
				{
					CC.Say("ab" + id.ToString(), CC, null, null);
				}
				tp.PlaySound("vomit", true, 1f, true);
				int num6 = 2 + EClass.rnd(3);
				int id3 = (id == EffectId.Puddle) ? 4 : ((id == EffectId.Bubble) ? 5 : ((id == EffectId.MistOfDarkness) ? 6 : 7));
				EffectId idEffect = (id == EffectId.Bubble) ? EffectId.BallBubble : EffectId.PuddleEffect;
				Color color = EClass.Colors.elementColors.TryGetValue(element.source.alias, default(Color));
				if (id == EffectId.Bubble && CC.id == "cancer")
				{
					idEffect = EffectId.Nothing;
					num6 = 1 + EClass.rnd(3);
				}
				for (int i = 0; i < num6; i++)
				{
					Point randomPoint2 = tp.GetRandomPoint(2, true, true, false, 100);
					if (randomPoint2 != null && !randomPoint2.HasBlock && (id != EffectId.Puddle || !randomPoint2.cell.IsTopWaterAndNoSnow))
					{
						int num7 = 4 + EClass.rnd(5);
						if (id == EffectId.Web)
						{
							num7 *= 3;
						}
						EClass._map.SetEffect(randomPoint2.x, randomPoint2.z, new CellEffect
						{
							id = id3,
							amount = num7,
							idEffect = idEffect,
							idEle = element.id,
							power = power,
							isHostileAct = CC.IsPCParty,
							color = BaseTileMap.GetColorInt(ref color, 100)
						});
					}
				}
				return;
			}
			default:
			{
				if (id == EffectId.Meteor)
				{
					EffectMeteor.Create(cc.pos, 6, 10, delegate(int a, Point b)
					{
					});
					List<Point> list4 = EClass._map.ListPointsInCircle(CC.pos, 10f, true, true);
					if (list4.Count == 0)
					{
						list4.Add(CC.pos.Copy());
					}
					CC.Say("spell_ball", CC, element.Name.ToLower(), null);
					ActEffect.TryDelay(delegate
					{
						CC.PlaySound("spell_ball", 1f, true);
					});
					if (CC.IsInMutterDistance(10))
					{
						Shaker.ShakeCam("ball", 1f);
					}
					EClass.Wait(1f, CC);
					ActEffect.DamageEle(CC, id, power, element, list4, actRef, "spell_ball");
					return;
				}
				if (id != EffectId.Earthquake)
				{
					goto IL_DF7;
				}
				List<Point> list5 = EClass._map.ListPointsInCircle(CC.pos, 12f, false, true);
				if (list5.Count == 0)
				{
					list5.Add(CC.pos.Copy());
				}
				CC.Say("spell_earthquake", CC, element.Name.ToLower(), null);
				ActEffect.TryDelay(delegate
				{
					CC.PlaySound("spell_earthquake", 1f, true);
				});
				if (CC.IsInMutterDistance(10))
				{
					Shaker.ShakeCam("ball", 1f);
				}
				EClass.Wait(1f, CC);
				ActEffect.DamageEle(CC, id, power, element, list5, actRef, "spell_earthquake");
				goto IL_DF7;
			}
			}
		}
		else
		{
			if (id == EffectId.Suicide)
			{
				goto IL_9C1;
			}
			if (id != EffectId.DrainBlood)
			{
				if (id != EffectId.Scream)
				{
					goto IL_DF7;
				}
				CC.PlaySound("scream", 1f, true);
				CC.PlayEffect("scream", true, 0f, default(Vector3));
				foreach (Point point2 in EClass._map.ListPointsInCircle(cc.pos, 6f, false, false))
				{
					foreach (Chara chara3 in point2.Charas)
					{
						if (chara3.ResistLv(957) <= 0)
						{
							chara3.AddCondition<ConParalyze>(power, false);
						}
					}
				}
				return;
			}
		}
		List<Point> list6 = new List<Point>();
		list6.Add(tp.Copy());
		EClass.Wait(0.3f, CC);
		ActEffect.TryDelay(delegate
		{
			CC.PlaySound("spell_hand", 1f, true);
		});
		if (!ActEffect.DamageEle(CC, id, power, element, list6, actRef, (id == EffectId.DrainBlood) ? "" : "spell_hand"))
		{
			CC.Say("spell_hand_miss", CC, element.Name.ToLower(), null);
		}
		return;
		IL_9C1:
		float radius2 = (id == EffectId.Suicide) ? 3.5f : ((float)((id == EffectId.BallBubble) ? 2 : 5));
		if (id == EffectId.Explosive && actRef.refThing != null)
		{
			radius2 = 2f;
		}
		if (id == EffectId.Suicide)
		{
			if (CC.MainElement != Element.Void)
			{
				element = CC.MainElement;
			}
			if (CC.HasTag(CTAG.kamikaze))
			{
				radius2 = 1.5f;
			}
		}
		bool flag4 = id == EffectId.Explosive || id == EffectId.Suicide;
		List<Point> list7 = EClass._map.ListPointsInCircle(cc.pos, radius2, !flag4, !flag4);
		if (list7.Count == 0)
		{
			list7.Add(cc.pos.Copy());
		}
		cc.Say((id == EffectId.Suicide) ? "abSuicide" : "spell_ball", cc, element.Name.ToLower(), null);
		EClass.Wait(0.8f, cc);
		ActEffect.TryDelay(delegate
		{
			cc.PlaySound("spell_ball", 1f, true);
		});
		if (cc.IsInMutterDistance(10) && !EClass.core.config.graphic.disableShake)
		{
			Shaker.ShakeCam("ball", 1f);
		}
		ActEffect.DamageEle(actRef.origin ?? cc, id, power, element, list7, actRef, (id == EffectId.Suicide) ? "suicide" : "spell_ball");
		if (id == EffectId.Suicide && CC.IsAliveInCurrentZone)
		{
			CC.Die(null, null, AttackSource.None);
		}
		return;
		IL_DF7:
		List<Card> list8 = tp.ListCards(false).ToList<Card>();
		list8.Reverse();
		if (list8.Contains(CC))
		{
			list8.Remove(CC);
			list8.Insert(0, CC);
		}
		bool flag5 = true;
		foreach (Card card in list8)
		{
			if (tc == null || card == tc)
			{
				ActEffect.Proc(id, power, state, CC, card, actRef);
				if (flag2 && card.isChara && card != CC)
				{
					CC.DoHostileAction(card, false);
				}
				if (actRef.refThing == null || !(actRef.refThing.trait is TraitRod))
				{
					return;
				}
				if (id - EffectId.Identify <= 3)
				{
					return;
				}
				flag5 = false;
			}
		}
		if (flag5)
		{
			CC.SayNothingHappans();
		}
	}

	public static void Proc(EffectId id, Card cc, Card tc = null, int power = 100, ActRef actRef = default(ActRef))
	{
		ActEffect.Proc(id, power, BlessedState.Normal, cc, tc, actRef);
	}

	public static void Proc(EffectId id, int power, BlessedState state, Card cc, Card tc = null, ActRef actRef = default(ActRef))
	{
		ActEffect.<>c__DisplayClass7_0 CS$<>8__locals1 = new ActEffect.<>c__DisplayClass7_0();
		CS$<>8__locals1.cc = cc;
		CS$<>8__locals1.tc = tc;
		CS$<>8__locals1.power = power;
		CS$<>8__locals1.actRef = actRef;
		CS$<>8__locals1.id = id;
		if (CS$<>8__locals1.tc == null)
		{
			CS$<>8__locals1.tc = CS$<>8__locals1.cc;
		}
		CS$<>8__locals1.TC = CS$<>8__locals1.tc.Chara;
		CS$<>8__locals1.CC = CS$<>8__locals1.cc.Chara;
		CS$<>8__locals1.blessed = (state >= BlessedState.Blessed);
		bool flag = state <= BlessedState.Cursed;
		CS$<>8__locals1.orgPower = CS$<>8__locals1.power;
		if (CS$<>8__locals1.blessed || flag)
		{
			CS$<>8__locals1.power *= 2;
		}
		EffectId id2 = CS$<>8__locals1.id;
		if (id2 <= EffectId.ModPotential)
		{
			if (id2 <= EffectId.Uncurse)
			{
				switch (id2)
				{
				case EffectId.Identify:
				case EffectId.GreaterIdentify:
				{
					bool flag2 = CS$<>8__locals1.id == EffectId.GreaterIdentify;
					if (flag)
					{
						CS$<>8__locals1.<Proc>g__Redirect|0(EffectId.ForgetItems, flag2 ? BlessedState.Cursed : BlessedState.Normal, default(ActRef));
						goto IL_12A0;
					}
					if (!CS$<>8__locals1.tc.isThing)
					{
						int count = CS$<>8__locals1.blessed ? (flag2 ? (2 + EClass.rnd(2)) : (3 + EClass.rnd(3))) : 1;
						LayerDragGrid.CreateIdentify(CS$<>8__locals1.CC, flag2, state, 0, count);
						return;
					}
					CS$<>8__locals1.cc.PlaySound("identify", 1f, true);
					CS$<>8__locals1.cc.PlayEffect("identify", true, 0f, default(Vector3));
					CS$<>8__locals1.tc.Thing.Identify(CS$<>8__locals1.cc.IsPCParty, flag2 ? IDTSource.SuperiorIdentify : IDTSource.Identify);
					goto IL_12A0;
				}
				case EffectId.Teleport:
				case EffectId.TeleportShort:
					if (!CS$<>8__locals1.tc.HasHost && !flag)
					{
						if (CS$<>8__locals1.id == EffectId.TeleportShort)
						{
							CS$<>8__locals1.tc.Teleport(ActEffect.GetTeleportPos(CS$<>8__locals1.tc.pos, 6), false, false);
						}
						else
						{
							CS$<>8__locals1.tc.Teleport(ActEffect.GetTeleportPos(CS$<>8__locals1.tc.pos, EClass._map.bounds.Width), false, false);
						}
					}
					if (flag)
					{
						CS$<>8__locals1.<Proc>g__Redirect|0(EffectId.Gravity, BlessedState.Normal, default(ActRef));
					}
					if (CS$<>8__locals1.blessed)
					{
						CS$<>8__locals1.<Proc>g__Redirect|0(EffectId.Levitate, BlessedState.Normal, default(ActRef));
						goto IL_12A0;
					}
					goto IL_12A0;
				case EffectId.CreateWall:
					goto IL_12A0;
				case EffectId.Return:
				case EffectId.Evac:
					if (!CS$<>8__locals1.cc.IsPC)
					{
						CS$<>8__locals1.<Proc>g__Redirect|0(EffectId.Teleport, state, default(ActRef));
						return;
					}
					CS$<>8__locals1.cc.PlaySound("return_cast", 1f, true);
					if (EClass.player.returnInfo == null)
					{
						if (CS$<>8__locals1.id == EffectId.Evac)
						{
							EClass.player.returnInfo = new Player.ReturnInfo
							{
								turns = EClass.rnd(10) + 10,
								isEvac = true
							};
						}
						else
						{
							if (EClass.game.spatials.ListReturnLocations().Count == 0)
							{
								Msg.Say("returnNowhere");
								goto IL_12A0;
							}
							EClass.player.returnInfo = new Player.ReturnInfo
							{
								turns = EClass.rnd(10) + 10,
								askDest = true
							};
						}
						Msg.Say("returnBegin");
						goto IL_12A0;
					}
					EClass.player.returnInfo = null;
					Msg.Say("returnAbort");
					goto IL_12A0;
				case EffectId.ChangeMaterialLesser:
				case EffectId.ChangeMaterial:
				case EffectId.ChangeMaterialGreater:
				{
					SourceMaterial.Row row = EClass.sources.materials.alias.TryGetValue(CS$<>8__locals1.actRef.n1, null);
					if (!CS$<>8__locals1.tc.isThing)
					{
						LayerDragGrid.CreateChangeMaterial(CS$<>8__locals1.CC, CS$<>8__locals1.actRef.refThing, row, CS$<>8__locals1.id, state, 0, 1);
						return;
					}
					if (CS$<>8__locals1.tc.Num > 1)
					{
						CS$<>8__locals1.tc = CS$<>8__locals1.tc.Split(1);
					}
					string name = CS$<>8__locals1.tc.Name;
					if (row == null)
					{
						bool flag3 = CS$<>8__locals1.id == EffectId.ChangeMaterialGreater;
						bool flag4 = CS$<>8__locals1.id == EffectId.ChangeMaterialLesser;
						string text4 = CS$<>8__locals1.tc.Thing.source.tierGroup;
						Dictionary<string, SourceMaterial.TierList> tierMap = SourceMaterial.tierMap;
						int num = 1;
						if (flag)
						{
							num -= 2;
						}
						if (CS$<>8__locals1.blessed)
						{
							num++;
						}
						if (flag3)
						{
							num++;
						}
						if (flag4)
						{
							num -= 2;
						}
						num = Mathf.Clamp(num + EClass.rnd(2), 0, 4);
						if (EClass.rnd(10) == 0)
						{
							text4 = ((text4 == "metal") ? "leather" : "metal");
						}
						SourceMaterial.TierList tierList = text4.IsEmpty() ? tierMap.RandomItem<string, SourceMaterial.TierList>() : tierMap[text4];
						for (int i = 0; i < 1000; i++)
						{
							row = tierList.tiers[num].Select();
							if (row != CS$<>8__locals1.tc.material)
							{
								break;
							}
						}
					}
					CS$<>8__locals1.cc.PlaySound("offering", 1f, true);
					CS$<>8__locals1.cc.PlayEffect("buff", true, 0f, default(Vector3));
					CS$<>8__locals1.tc.ChangeMaterial(row);
					if (CS$<>8__locals1.tc.trait is TraitGene && CS$<>8__locals1.tc.c_DNA != null)
					{
						DNA.Type type = DNA.GetType(CS$<>8__locals1.tc.material.alias);
						CS$<>8__locals1.tc.c_DNA.Generate(type, null);
					}
					CS$<>8__locals1.cc.Say("materialChanged", name, row.GetName());
					if (CS$<>8__locals1.CC != null)
					{
						if (CS$<>8__locals1.tc.parent == null)
						{
							CS$<>8__locals1.CC.Pick(CS$<>8__locals1.tc.Thing, false, true);
						}
						CS$<>8__locals1.CC.body.UnqeuipIfTooHeavy(CS$<>8__locals1.tc.Thing);
						goto IL_12A0;
					}
					goto IL_12A0;
				}
				default:
				{
					if (id2 != EffectId.Uncurse)
					{
						goto IL_12A0;
					}
					if (!CS$<>8__locals1.tc.isThing)
					{
						LayerDragGrid.CreateUncurse(CS$<>8__locals1.CC, state, 0, 1);
						return;
					}
					Thing thing = CS$<>8__locals1.tc.Thing;
					if (thing.blessedState == BlessedState.Cursed)
					{
						thing.SetBlessedState(BlessedState.Normal);
					}
					else if (thing.blessedState == BlessedState.Doomed)
					{
						thing.SetBlessedState(BlessedState.Normal);
					}
					Card rootCard = thing.GetRootCard();
					if (rootCard != null)
					{
						rootCard.TryStack(thing);
					}
					LayerInventory.SetDirty(thing);
					goto IL_12A0;
				}
				}
			}
			else
			{
				switch (id2)
				{
				case EffectId.EnchantWeapon:
				case EffectId.EnchantArmor:
					break;
				case EffectId.Lighten:
				{
					if (!CS$<>8__locals1.tc.isThing)
					{
						LayerDragGrid.CreateLighten(CS$<>8__locals1.CC, state, 0, 1);
						return;
					}
					if (CS$<>8__locals1.tc.Num > 1)
					{
						CS$<>8__locals1.tc = CS$<>8__locals1.tc.Split(1);
					}
					CS$<>8__locals1.cc.PlaySound("offering", 1f, true);
					CS$<>8__locals1.cc.PlayEffect("buff", true, 0f, default(Vector3));
					int num2 = CS$<>8__locals1.tc.isWeightChanged ? CS$<>8__locals1.tc.c_weight : CS$<>8__locals1.tc.Thing.source.weight;
					CS$<>8__locals1.tc.isWeightChanged = true;
					Element orCreateElement = CS$<>8__locals1.tc.elements.GetOrCreateElement(64);
					Element orCreateElement2 = CS$<>8__locals1.tc.elements.GetOrCreateElement(65);
					Element orCreateElement3 = CS$<>8__locals1.tc.elements.GetOrCreateElement(67);
					Element orCreateElement4 = CS$<>8__locals1.tc.elements.GetOrCreateElement(66);
					bool flag5 = CS$<>8__locals1.tc.IsEquipmentOrRanged || CS$<>8__locals1.tc.IsThrownWeapon || CS$<>8__locals1.tc.IsAmmo;
					if (flag)
					{
						num2 = (int)(0.01f * (float)num2 * (float)CS$<>8__locals1.power * 0.75f + 500f);
						if (num2 < 1)
						{
							num2 = 1;
						}
						if (flag5)
						{
							if (CS$<>8__locals1.tc.IsWeapon || CS$<>8__locals1.tc.IsThrownWeapon)
							{
								CS$<>8__locals1.tc.elements.ModBase(67, Mathf.Clamp(orCreateElement3.vBase * CS$<>8__locals1.power / 1000, 1, 5));
								CS$<>8__locals1.tc.elements.ModBase(66, -Mathf.Clamp(orCreateElement4.vBase * CS$<>8__locals1.power / 1000, 1, 5));
							}
							else
							{
								CS$<>8__locals1.tc.elements.ModBase(65, Mathf.Clamp(orCreateElement2.vBase * CS$<>8__locals1.power / 1000, 1, 5));
								CS$<>8__locals1.tc.elements.ModBase(64, -Mathf.Clamp(orCreateElement.vBase * CS$<>8__locals1.power / 1000, 1, 5));
							}
						}
						CS$<>8__locals1.cc.Say("lighten_curse", CS$<>8__locals1.cc, CS$<>8__locals1.tc, null, null);
					}
					else
					{
						num2 = num2 * (100 - CS$<>8__locals1.power / 10) / 100;
						if (CS$<>8__locals1.blessed)
						{
							CS$<>8__locals1.power /= 4;
						}
						if (flag5)
						{
							if (CS$<>8__locals1.tc.IsWeapon || CS$<>8__locals1.tc.IsThrownWeapon)
							{
								CS$<>8__locals1.tc.elements.ModBase(67, -Mathf.Clamp(orCreateElement3.vBase * CS$<>8__locals1.power / 1000, 1, 5));
								CS$<>8__locals1.tc.elements.ModBase(66, Mathf.Clamp(orCreateElement4.vBase * CS$<>8__locals1.power / 1000, 1, 5));
							}
							else
							{
								CS$<>8__locals1.tc.elements.ModBase(65, -Mathf.Clamp(orCreateElement2.vBase * CS$<>8__locals1.power / 1000, 1, 5));
								CS$<>8__locals1.tc.elements.ModBase(64, Mathf.Clamp(orCreateElement.vBase * CS$<>8__locals1.power / 1000, 1, 5));
							}
						}
						CS$<>8__locals1.cc.Say("lighten", CS$<>8__locals1.cc, CS$<>8__locals1.tc, null, null);
					}
					CS$<>8__locals1.tc.c_weight = num2;
					CS$<>8__locals1.tc.SetDirtyWeight();
					if (CS$<>8__locals1.tc.parent == null)
					{
						CS$<>8__locals1.CC.Pick(CS$<>8__locals1.tc.Thing, false, true);
					}
					CS$<>8__locals1.CC.body.UnqeuipIfTooHeavy(CS$<>8__locals1.tc.Thing);
					goto IL_12A0;
				}
				case EffectId.Naming:
				case EffectId.Faith:
					goto IL_12A0;
				case EffectId.ForgetItems:
				{
					CS$<>8__locals1.TC.PlaySound("curse3", 1f, true);
					CS$<>8__locals1.TC.PlayEffect("curse", true, 0f, default(Vector3));
					CS$<>8__locals1.TC.Say("forgetItems", CS$<>8__locals1.TC, null, null);
					int num3 = CS$<>8__locals1.power / 50 + 1 + EClass.rnd(3);
					List<Thing> source = CS$<>8__locals1.TC.things.List((Thing t) => t.c_IDTState == 0, false);
					for (int j = 0; j < num3; j++)
					{
						source.RandomItem<Thing>().c_IDTState = 5;
					}
					goto IL_12A0;
				}
				default:
				{
					if (id2 != EffectId.ModPotential)
					{
						goto IL_12A0;
					}
					Element element = CS$<>8__locals1.cc.elements.ListElements((Element e) => e.HasTag("primary"), null).RandomItem<Element>();
					CS$<>8__locals1.cc.elements.ModTempPotential(element.id, CS$<>8__locals1.power / 10, 0);
					goto IL_12A0;
				}
				}
			}
		}
		else
		{
			if (id2 <= EffectId.Duplicate)
			{
				if (id2 != EffectId.AbsorbMana)
				{
					switch (id2)
					{
					case EffectId.Exterminate:
					{
						CS$<>8__locals1.CC.PlaySound("clean_floor", 1f, true);
						Msg.Say("exterminate");
						List<Chara> list7 = (from c in EClass._map.charas
						where c.isCopy && !c.IsPCFaction
						select c).ToList<Chara>();
						if (list7.Count == 0)
						{
							Msg.SayNothingHappen();
							return;
						}
						using (List<Chara>.Enumerator enumerator = list7.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								Chara chara = enumerator.Current;
								chara.Say("split_fail", chara, null, null);
								chara.PlayEffect("vanish", true, 0f, default(Vector3));
								chara.Die(null, null, AttackSource.None);
							}
							goto IL_12A0;
						}
						break;
					}
					case EffectId.Earthquake:
					case EffectId.Boost:
					case EffectId.RemoveHex:
					case EffectId.RemoveHexAll:
						goto IL_12A0;
					case EffectId.MagicMap:
						if (!CS$<>8__locals1.CC.IsPC)
						{
							CS$<>8__locals1.CC.SayNothingHappans();
							goto IL_12A0;
						}
						if (flag)
						{
							CS$<>8__locals1.CC.Say("abMagicMap_curse", CS$<>8__locals1.CC, null, null);
							CS$<>8__locals1.CC.PlaySound("curse3", 1f, true);
							CS$<>8__locals1.CC.PlayEffect("curse", true, 0f, default(Vector3));
							CS$<>8__locals1.CC.AddCondition<ConConfuse>(200, true);
							goto IL_12A0;
						}
						CS$<>8__locals1.CC.Say("abMagicMap", CS$<>8__locals1.CC, null, null);
						CS$<>8__locals1.CC.PlayEffect("identify", true, 0f, default(Vector3));
						CS$<>8__locals1.CC.PlaySound("identify", 1f, true);
						if (CS$<>8__locals1.blessed)
						{
							EClass._map.RevealAll(true);
							goto IL_12A0;
						}
						EClass._map.Reveal(CS$<>8__locals1.CC.pos, CS$<>8__locals1.power);
						goto IL_12A0;
					case EffectId.Escape:
						if (CS$<>8__locals1.CC.IsPCFaction || (EClass._zone.Boss == CS$<>8__locals1.CC && EClass.rnd(30) != 0))
						{
							return;
						}
						CS$<>8__locals1.CC.Say("escape", CS$<>8__locals1.CC, null, null);
						CS$<>8__locals1.CC.PlaySound("escape", 1f, true);
						if (EClass._zone.Boss == CS$<>8__locals1.CC)
						{
							CS$<>8__locals1.CC.TryDropBossLoot();
						}
						CS$<>8__locals1.CC.Destroy();
						goto IL_12A0;
					case EffectId.EnchantWeaponGreat:
					case EffectId.EnchantArmorGreat:
						goto IL_72A;
					case EffectId.Duplicate:
					{
						Point randomPoint = CS$<>8__locals1.CC.pos.GetRandomPoint(2, false, false, false, 200);
						if (randomPoint == null || randomPoint.Equals(CS$<>8__locals1.CC.pos) || !randomPoint.IsValid || EClass._zone.IsRegion || CS$<>8__locals1.CC.HasCondition<ConPoison>() || CS$<>8__locals1.CC.HasCondition<ConConfuse>() || CS$<>8__locals1.CC.HasCondition<ConDim>() || CS$<>8__locals1.CC.HasCondition<ConParalyze>() || CS$<>8__locals1.CC.HasCondition<ConSleep>() || CS$<>8__locals1.CC.HasCondition<ConBurning>() || CS$<>8__locals1.CC.HasCondition<ConFreeze>() || CS$<>8__locals1.CC.HasCondition<ConMiasma>())
						{
							CS$<>8__locals1.CC.Say("split_fail", CS$<>8__locals1.CC, null, null);
							return;
						}
						Chara t3 = CS$<>8__locals1.CC.Duplicate();
						EClass._zone.AddCard(t3, randomPoint);
						CS$<>8__locals1.CC.Say("split", CS$<>8__locals1.CC, null, null);
						goto IL_12A0;
					}
					default:
						goto IL_12A0;
					}
				}
				else
				{
					EClass.game.religions.Element.Talk("ability", null, null);
					Dice dice = Dice.Create("ActManaAbsorb", CS$<>8__locals1.power, CS$<>8__locals1.CC, CS$<>8__locals1.actRef.act);
					CS$<>8__locals1.TC.mana.Mod(dice.Roll());
					CS$<>8__locals1.TC.PlaySound("heal", 1f, true);
					CS$<>8__locals1.TC.PlayEffect("heal", true, 0f, default(Vector3));
					if (CS$<>8__locals1.TC.IsPC)
					{
						CS$<>8__locals1.CC.Say("absorbMana", CS$<>8__locals1.CC, null, null);
						goto IL_12A0;
					}
					goto IL_12A0;
				}
			}
			else if (id2 != EffectId.Reconstruction)
			{
				if (id2 != EffectId.DropMine)
				{
					goto IL_12A0;
				}
			}
			else
			{
				if (!CS$<>8__locals1.tc.isThing)
				{
					LayerDragGrid.CreateReconstruction(CS$<>8__locals1.CC, state, 0, 1);
					return;
				}
				if (CS$<>8__locals1.tc.Num > 1)
				{
					CS$<>8__locals1.tc = CS$<>8__locals1.tc.Split(1);
				}
				CS$<>8__locals1.cc.PlaySound("mutation", 1f, true);
				CS$<>8__locals1.cc.PlayEffect("identify", true, 0f, default(Vector3));
				CS$<>8__locals1.cc.Say("reconstruct", CS$<>8__locals1.cc, CS$<>8__locals1.tc, null, null);
				EClass.game.cards.uidNext += EClass.rnd(30);
				Thing thing2 = ThingGen.Create(CS$<>8__locals1.tc.id, -1, CS$<>8__locals1.tc.LV * CS$<>8__locals1.power / 100);
				thing2.SetBlessedState(state);
				CS$<>8__locals1.tc.Destroy();
				CS$<>8__locals1.CC.Pick(thing2, false, true);
				if (!CS$<>8__locals1.CC.IsPC)
				{
					CS$<>8__locals1.CC.TryEquip(thing2, false);
					goto IL_12A0;
				}
				goto IL_12A0;
			}
			if (CS$<>8__locals1.CC.pos.Installed != null || EClass._zone.IsPCFaction)
			{
				return;
			}
			Thing thing3 = ThingGen.Create("mine", -1, -1);
			thing3.c_idRefCard = "dog_mine";
			Zone.ignoreSpawnAnime = true;
			EClass._zone.AddCard(thing3, CS$<>8__locals1.CC.pos).Install();
			goto IL_12A0;
		}
		IL_72A:
		bool armor = CS$<>8__locals1.id == EffectId.EnchantArmor || CS$<>8__locals1.id == EffectId.EnchantArmorGreat;
		bool flag6 = CS$<>8__locals1.id == EffectId.EnchantWeaponGreat || CS$<>8__locals1.id == EffectId.EnchantArmorGreat;
		if (!CS$<>8__locals1.tc.isThing)
		{
			LayerDragGrid.CreateEnchant(CS$<>8__locals1.CC, armor, flag6, state, 1);
			return;
		}
		CS$<>8__locals1.cc.PlaySound("identify", 1f, true);
		CS$<>8__locals1.cc.PlayEffect("identify", true, 0f, default(Vector3));
		if (flag)
		{
			CS$<>8__locals1.cc.Say("enc_curse", CS$<>8__locals1.tc, null, null);
			CS$<>8__locals1.tc.ModEncLv(-1);
		}
		else
		{
			int num4 = (flag6 ? 4 : 2) + (CS$<>8__locals1.blessed ? 1 : 0);
			if (CS$<>8__locals1.tc.encLV >= num4)
			{
				CS$<>8__locals1.cc.Say("enc_resist", CS$<>8__locals1.tc, null, null);
			}
			else
			{
				CS$<>8__locals1.cc.Say("enc", CS$<>8__locals1.tc, null, null);
				CS$<>8__locals1.tc.ModEncLv(1);
			}
		}
		IL_12A0:
		if (CS$<>8__locals1.TC == null)
		{
			return;
		}
		id2 = CS$<>8__locals1.id;
		switch (id2)
		{
		case EffectId.Booze:
			CS$<>8__locals1.TC.AddCondition<ConDrunk>(CS$<>8__locals1.power, false);
			if (CS$<>8__locals1.TC.HasElement(1215, 1))
			{
				CS$<>8__locals1.TC.Say("drunk_dwarf", CS$<>8__locals1.TC, null, null);
				CS$<>8__locals1.TC.AddCondition(Condition.Create<ConBuffStats>(CS$<>8__locals1.power + EClass.rnd(CS$<>8__locals1.power), delegate(ConBuffStats con)
				{
					con.SetRefVal(Element.List_MainAttributes.RandomItem<int>(), (int)CS$<>8__locals1.id);
				}), false);
				return;
			}
			break;
		case EffectId.Drink:
		case EffectId.DrinkRamune:
		case EffectId.DrinkMilk:
			if (CS$<>8__locals1.id == EffectId.DrinkRamune)
			{
				CS$<>8__locals1.TC.Say("drinkRamune", CS$<>8__locals1.TC, null, null);
			}
			if (CS$<>8__locals1.TC.IsPC)
			{
				CS$<>8__locals1.TC.Say("drinkGood", CS$<>8__locals1.TC, null, null);
			}
			if (CS$<>8__locals1.id == EffectId.DrinkMilk)
			{
				if (CS$<>8__locals1.TC.IsPC)
				{
					CS$<>8__locals1.TC.Say("drinkMilk", CS$<>8__locals1.TC, null, null);
				}
				if (CS$<>8__locals1.blessed)
				{
					CS$<>8__locals1.TC.ModHeight(EClass.rnd(5) + 3);
					return;
				}
				if (flag)
				{
					CS$<>8__locals1.TC.ModHeight((EClass.rnd(5) + 3) * -1);
					return;
				}
			}
			break;
		case EffectId.DrinkWater:
			if (flag)
			{
				if (CS$<>8__locals1.TC.IsPC)
				{
					CS$<>8__locals1.TC.Say("drinkWater_dirty", CS$<>8__locals1.TC, null, null);
				}
				TraitWell.BadEffect(CS$<>8__locals1.TC);
				return;
			}
			if (CS$<>8__locals1.TC.IsPC)
			{
				CS$<>8__locals1.TC.Say("drinkWater_clear", CS$<>8__locals1.TC, null, null);
				return;
			}
			break;
		case EffectId.DrinkWaterDirty:
			if (CS$<>8__locals1.TC.IsPC)
			{
				CS$<>8__locals1.TC.Say("drinkWater_dirty", CS$<>8__locals1.TC, null, null);
			}
			if (CS$<>8__locals1.TC.IsPCFaction)
			{
				CS$<>8__locals1.TC.Vomit();
				return;
			}
			break;
		case EffectId.SaltWater:
			if (CS$<>8__locals1.TC.HasElement(1211, 1))
			{
				CS$<>8__locals1.TC.Say("drinkSaltWater_snail", CS$<>8__locals1.TC, null, null);
				int dmg = (CS$<>8__locals1.TC.hp > 10) ? (CS$<>8__locals1.TC.hp - EClass.rnd(10)) : 10000;
				CS$<>8__locals1.TC.DamageHP(dmg, AttackSource.None, CS$<>8__locals1.CC);
				return;
			}
			if (CS$<>8__locals1.TC.IsPC)
			{
				CS$<>8__locals1.TC.Say("drinkSaltWater", CS$<>8__locals1.TC, null, null);
				return;
			}
			break;
		default:
		{
			switch (id2)
			{
			case EffectId.Heal:
				goto IL_2879;
			case EffectId.RestoreBody:
			case EffectId.RestoreMind:
			{
				bool flag7 = CS$<>8__locals1.id == EffectId.RestoreBody;
				if (flag)
				{
					CS$<>8__locals1.<Proc>g__Redirect|0(flag7 ? EffectId.DamageBodyGreat : EffectId.DamageMindGreat, BlessedState.Normal, default(ActRef));
					return;
				}
				CS$<>8__locals1.TC.Say(flag7 ? "restoreBody" : "restoreMind", CS$<>8__locals1.TC, null, null);
				CS$<>8__locals1.TC.PlaySound("heal", 1f, true);
				CS$<>8__locals1.TC.PlayEffect("heal", true, 0f, default(Vector3));
				CS$<>8__locals1.TC.CureHost(flag7 ? CureType.CureBody : CureType.CureMind, CS$<>8__locals1.power, state);
				if (CS$<>8__locals1.blessed)
				{
					CS$<>8__locals1.<Proc>g__Redirect|0(flag7 ? EffectId.EnhanceBodyGreat : EffectId.EnhanceMindGreat, BlessedState.Normal, default(ActRef));
					return;
				}
				return;
			}
			case EffectId.DamageBody:
			case EffectId.DamageMind:
			case EffectId.DamageBodyGreat:
			case EffectId.DamageMindGreat:
				break;
			case EffectId.EnhanceBody:
			case EffectId.EnhanceMind:
			case EffectId.EnhanceBodyGreat:
			case EffectId.EnhanceMindGreat:
			{
				bool flag8 = CS$<>8__locals1.id == EffectId.EnhanceBody || CS$<>8__locals1.id == EffectId.EnhanceBodyGreat;
				bool mind = CS$<>8__locals1.id == EffectId.EnhanceMind || CS$<>8__locals1.id == EffectId.EnhanceMindGreat;
				int num5 = (CS$<>8__locals1.id == EffectId.EnhanceBody || CS$<>8__locals1.id == EffectId.EnhanceMind) ? 1 : (4 + EClass.rnd(4));
				CS$<>8__locals1.TC.Say(flag8 ? "enhanceBody" : "enhanceMind", CS$<>8__locals1.TC, null, null);
				CS$<>8__locals1.TC.PlayEffect("buff", true, 0f, default(Vector3));
				CS$<>8__locals1.TC.PlaySound("buff", 1f, true);
				for (int k = 0; k < num5; k++)
				{
					CS$<>8__locals1.TC.EnhanceTempElements(CS$<>8__locals1.power, flag8, mind);
				}
				return;
			}
			case EffectId.Revive:
			{
				List<KeyValuePair<int, Chara>> list2 = (from a in EClass.game.cards.globalCharas
				where a.Value.isDead && a.Value.faction == EClass.pc.faction && !a.Value.isSummon && a.Value.GetInt(103, null) != 0
				select a).ToList<KeyValuePair<int, Chara>>();
				if (CS$<>8__locals1.TC.IsPCFaction || CS$<>8__locals1.TC.IsPCFactionMinion)
				{
					if (CS$<>8__locals1.TC.IsPC && list2.Count == 0)
					{
						list2 = (from a in EClass.game.cards.globalCharas
						where a.Value.isDead && a.Value.faction == EClass.pc.faction && !a.Value.isSummon
						select a).ToList<KeyValuePair<int, Chara>>();
					}
					if (list2.Count > 0)
					{
						list2.RandomItem<KeyValuePair<int, Chara>>().Value.Chara.GetRevived();
						return;
					}
				}
				CS$<>8__locals1.TC.SayNothingHappans();
				return;
			}
			default:
				switch (id2)
				{
				case EffectId.Sleep:
					if (CS$<>8__locals1.blessed)
					{
						CS$<>8__locals1.power /= 4;
					}
					CS$<>8__locals1.TC.AddCondition<ConSleep>(CS$<>8__locals1.power, false);
					if (flag)
					{
						CS$<>8__locals1.<Proc>g__Redirect|0(EffectId.Disease, BlessedState.Normal, default(ActRef));
						return;
					}
					return;
				case EffectId.Paralyze:
					if (CS$<>8__locals1.blessed)
					{
						CS$<>8__locals1.power /= 4;
					}
					CS$<>8__locals1.TC.AddCondition<ConParalyze>(CS$<>8__locals1.power, false);
					if (flag)
					{
						CS$<>8__locals1.<Proc>g__Redirect|0(EffectId.Blind, BlessedState.Normal, default(ActRef));
						return;
					}
					return;
				case EffectId.Blind:
					if (CS$<>8__locals1.blessed)
					{
						CS$<>8__locals1.power /= 4;
					}
					CS$<>8__locals1.TC.AddCondition<ConBlind>(CS$<>8__locals1.power, false);
					if (flag)
					{
						CS$<>8__locals1.<Proc>g__Redirect|0(EffectId.Confuse, BlessedState.Normal, default(ActRef));
						return;
					}
					return;
				case EffectId.Confuse:
					if (CS$<>8__locals1.blessed)
					{
						CS$<>8__locals1.power /= 4;
					}
					CS$<>8__locals1.TC.AddCondition<ConConfuse>(CS$<>8__locals1.power, false);
					if (flag)
					{
						CS$<>8__locals1.<Proc>g__Redirect|0(EffectId.Fear, BlessedState.Normal, default(ActRef));
						return;
					}
					return;
				case EffectId.Poison:
					if (CS$<>8__locals1.blessed)
					{
						CS$<>8__locals1.power /= 4;
					}
					CS$<>8__locals1.TC.AddCondition<ConPoison>(CS$<>8__locals1.power, false);
					if (flag)
					{
						CS$<>8__locals1.<Proc>g__Redirect|0(EffectId.Paralyze, BlessedState.Normal, default(ActRef));
						return;
					}
					return;
				case EffectId.Faint:
					if (CS$<>8__locals1.blessed)
					{
						CS$<>8__locals1.power /= 4;
					}
					CS$<>8__locals1.TC.AddCondition<ConFaint>(CS$<>8__locals1.power, false);
					if (flag)
					{
						CS$<>8__locals1.<Proc>g__Redirect|0(EffectId.Disease, BlessedState.Normal, default(ActRef));
						return;
					}
					return;
				case EffectId.Fear:
					if (CS$<>8__locals1.blessed)
					{
						CS$<>8__locals1.power /= 4;
					}
					CS$<>8__locals1.TC.AddCondition<ConFear>(CS$<>8__locals1.power, false);
					if (flag)
					{
						CS$<>8__locals1.<Proc>g__Redirect|0(EffectId.Confuse, BlessedState.Normal, default(ActRef));
						return;
					}
					return;
				case EffectId.Disease:
					if (CS$<>8__locals1.blessed)
					{
						CS$<>8__locals1.power /= 4;
					}
					CS$<>8__locals1.TC.AddCondition<ConDisease>(CS$<>8__locals1.power, false);
					if (flag)
					{
						CS$<>8__locals1.<Proc>g__Redirect|0(EffectId.Poison, BlessedState.Normal, default(ActRef));
						return;
					}
					return;
				case EffectId.Acid:
				{
					if (CS$<>8__locals1.blessed)
					{
						CS$<>8__locals1.power /= 4;
					}
					List<Thing> list3 = CS$<>8__locals1.TC.things.List((Thing t) => t.Num <= 1 && t.IsEquipmentOrRanged && !t.IsToolbelt && !t.IsLightsource && t.isEquipped, false);
					if (list3.Count == 0)
					{
						return;
					}
					Thing thing4 = list3.RandomItem<Thing>();
					CS$<>8__locals1.TC.Say("acid_hit", CS$<>8__locals1.TC, null, null);
					if (thing4.isAcidproof)
					{
						CS$<>8__locals1.TC.Say("acid_nullify", thing4, null, null);
					}
					else if (thing4.encLV > -5)
					{
						CS$<>8__locals1.TC.Say("acid_rust", CS$<>8__locals1.TC, thing4, null, null);
						thing4.ModEncLv(-1);
						LayerInventory.SetDirty(thing4);
					}
					if (CS$<>8__locals1.TC.IsPCParty)
					{
						Tutorial.Reserve("rust", null);
						return;
					}
					return;
				}
				case EffectId.HealComplete:
					Dice.Create("SpHealLight", CS$<>8__locals1.power, CS$<>8__locals1.CC, CS$<>8__locals1.actRef.act);
					CS$<>8__locals1.TC.HealHPHost(9999, HealSource.Magic);
					CS$<>8__locals1.TC.CureHost(CureType.HealComplete, CS$<>8__locals1.power, state);
					CS$<>8__locals1.TC.Say("heal_heavy", CS$<>8__locals1.TC, null, null);
					return;
				case EffectId.BuffStats:
				case EffectId.DebuffStats:
				case EffectId.LulwyTrick:
					Debug.Log(string.Concat(new string[]
					{
						CS$<>8__locals1.power.ToString(),
						"/",
						CS$<>8__locals1.id.ToString(),
						"/",
						CS$<>8__locals1.actRef.n1
					}));
					if (CS$<>8__locals1.id == EffectId.LulwyTrick)
					{
						EClass.game.religions.Wind.Talk("ability", null, null);
					}
					if (CS$<>8__locals1.power < 0 || CS$<>8__locals1.id == EffectId.DebuffStats)
					{
						CS$<>8__locals1.power = Mathf.Abs(CS$<>8__locals1.power);
						if (CS$<>8__locals1.blessed)
						{
							CS$<>8__locals1.power /= 4;
						}
						flag = true;
					}
					CS$<>8__locals1.TC.AddCondition(Condition.Create<ConBuffStats>(CS$<>8__locals1.power, delegate(ConBuffStats con)
					{
						con.SetRefVal(Element.GetId(CS$<>8__locals1.actRef.n1), (int)CS$<>8__locals1.id);
					}), false);
					return;
				case EffectId.Wish:
					if (CS$<>8__locals1.TC.IsPC)
					{
						Msg.Say("wishHappen");
						Dialog.InputName("dialogWish", "q", delegate(bool cancel, string text)
						{
							if (!cancel)
							{
								Msg.Say("wish", CS$<>8__locals1.TC, text, null, null);
								ActEffect.Wish(text, EClass.pc.NameTitled, CS$<>8__locals1.power);
							}
						}, Dialog.InputType.Default);
						return;
					}
					return;
				case EffectId.Ally:
				{
					Msg.Say("gainAlly");
					Chara chara2 = CharaGen.CreateFromFilter("chara", EClass.pc.LV, -1);
					EClass._zone.AddCard(chara2, CS$<>8__locals1.cc.pos.GetNearestPoint(false, false, true, false));
					chara2.MakeAlly(false);
					chara2.PlaySound("identify", 1f, true);
					chara2.PlayEffect("teleport", true, 0f, default(Vector3));
					return;
				}
				case EffectId.CatsEye:
					if (flag)
					{
						CS$<>8__locals1.<Proc>g__Redirect|0(EffectId.Blind, BlessedState.Normal, default(ActRef));
						return;
					}
					CS$<>8__locals1.TC.AddCondition<ConNightVision>(CS$<>8__locals1.power, false);
					return;
				case EffectId.LevelDown:
					Msg.Say("nothingHappens");
					return;
				case EffectId.Love:
					if (flag)
					{
						if (CS$<>8__locals1.CC == CS$<>8__locals1.TC)
						{
							CS$<>8__locals1.TC.Say("love_curse_self", CS$<>8__locals1.TC, null, null);
						}
						else
						{
							CS$<>8__locals1.TC.Say("love_curse", CS$<>8__locals1.CC, CS$<>8__locals1.TC, null, null);
							CS$<>8__locals1.TC.ModAffinity(CS$<>8__locals1.CC, -CS$<>8__locals1.power / 4, false);
						}
						CS$<>8__locals1.TC.ShowEmo(Emo.angry, 0f, true);
						return;
					}
					ActEffect.LoveMiracle(CS$<>8__locals1.TC, CS$<>8__locals1.CC, CS$<>8__locals1.power);
					return;
				case EffectId.Uncurse:
				case EffectId.EnchantWeapon:
				case EffectId.EnchantArmor:
				case EffectId.Lighten:
				case EffectId.Naming:
				case EffectId.ForgetItems:
				case EffectId.Breathe:
				case EffectId.Ball:
				case EffectId.Explosive:
				case EffectId.BallBubble:
				case EffectId.Bolt:
				case EffectId.Arrow:
				case EffectId.Hand:
				case EffectId.Funnel:
				case EffectId.Summon:
				case EffectId.ModPotential:
				case EffectId.Bubble:
				case EffectId.Web:
				case EffectId.MistOfDarkness:
				case EffectId.Puddle:
				case EffectId.AbsorbMana:
				case EffectId.Meteor:
				case EffectId.Exterminate:
				case EffectId.Earthquake:
				case EffectId.MagicMap:
				case EffectId.Escape:
				case EffectId.Boost:
				case EffectId.EnchantWeaponGreat:
				case EffectId.EnchantArmorGreat:
				case EffectId.Duplicate:
				case EffectId.Suicide:
				case EffectId.Reconstruction:
				case EffectId.DrainBlood:
				case EffectId.Scream:
				case EffectId.DropMine:
					return;
				case EffectId.UncurseEQ:
				case EffectId.UncurseEQGreater:
				{
					CS$<>8__locals1.TC.Say("uncurseEQ" + (CS$<>8__locals1.blessed ? "_bless" : (flag ? "_curse" : "")), CS$<>8__locals1.TC, null, null);
					CS$<>8__locals1.TC.PlaySound("uncurse", 1f, true);
					CS$<>8__locals1.TC.PlayEffect("uncurse", true, 0f, default(Vector3));
					if (flag)
					{
						CS$<>8__locals1.<Proc>g__Redirect|0(EffectId.CurseEQ, BlessedState.Normal, default(ActRef));
						return;
					}
					int success = 0;
					int fail = 0;
					List<Thing> list = new List<Thing>();
					CS$<>8__locals1.TC.things.Foreach(delegate(Thing t)
					{
						int num11 = 0;
						if (!t.isEquipped && !t.IsRangedWeapon && !CS$<>8__locals1.blessed)
						{
							return;
						}
						if (t.blessedState >= BlessedState.Normal)
						{
							return;
						}
						if (t.blessedState == BlessedState.Cursed)
						{
							num11 = EClass.rnd(200);
						}
						if (t.blessedState == BlessedState.Doomed)
						{
							num11 = EClass.rnd(1000);
						}
						if (CS$<>8__locals1.blessed)
						{
							num11 /= 2;
						}
						if (CS$<>8__locals1.id == EffectId.UncurseEQGreater)
						{
							num11 /= 10;
						}
						int num12;
						if (CS$<>8__locals1.power >= num11)
						{
							CS$<>8__locals1.TC.Say("uncurseEQ_success", t, null, null);
							t.SetBlessedState(BlessedState.Normal);
							if (t.isEquipped && t.HasElement(656, 1))
							{
								CS$<>8__locals1.TC.body.Unequip(t, true);
							}
							LayerInventory.SetDirty(t);
							num12 = success;
							success = num12 + 1;
							list.Add(t);
							return;
						}
						num12 = fail;
						fail = num12 + 1;
					}, true);
					foreach (Thing thing5 in list)
					{
						Card rootCard2 = thing5.GetRootCard();
						if (rootCard2 != null)
						{
							rootCard2.TryStack(thing5);
						}
					}
					if (success == 0 && fail == 0)
					{
						CS$<>8__locals1.TC.SayNothingHappans();
						return;
					}
					if (fail > 0)
					{
						CS$<>8__locals1.TC.Say("uncurseEQ_fail", null, null);
						return;
					}
					return;
				}
				case EffectId.CurseEQ:
				{
					if (CS$<>8__locals1.CC != null && CS$<>8__locals1.CC != CS$<>8__locals1.TC)
					{
						CS$<>8__locals1.TC.Say("curse", CS$<>8__locals1.CC, CS$<>8__locals1.TC, null, null);
					}
					CS$<>8__locals1.TC.PlaySound("curse3", 1f, true);
					CS$<>8__locals1.TC.PlayEffect("curse", true, 0f, default(Vector3));
					if (EClass.rnd(150 + CS$<>8__locals1.TC.LUC * 5 + CS$<>8__locals1.TC.Evalue(972) * 20) >= CS$<>8__locals1.power + (flag ? 200 : 0) || CS$<>8__locals1.TC.TryNullifyCurse())
					{
						return;
					}
					List<Thing> list4 = CS$<>8__locals1.TC.things.List((Thing t) => t.isEquipped && t.blessedState != BlessedState.Doomed && !t.IsToolbelt && (t.blessedState < BlessedState.Blessed || EClass.rnd(10) == 0), false);
					if (list4.Count == 0)
					{
						CS$<>8__locals1.CC.SayNothingHappans();
						return;
					}
					Thing thing6 = list4.RandomItem<Thing>();
					CS$<>8__locals1.TC.Say("curse_hit", CS$<>8__locals1.TC, thing6, null, null);
					thing6.SetBlessedState((thing6.blessedState == BlessedState.Cursed) ? BlessedState.Doomed : BlessedState.Cursed);
					LayerInventory.SetDirty(thing6);
					return;
				}
				case EffectId.HolyVeil:
					if (flag)
					{
						CS$<>8__locals1.<Proc>g__Redirect|0(EffectId.Fear, BlessedState.Normal, default(ActRef));
						return;
					}
					CS$<>8__locals1.TC.AddCondition<ConHolyVeil>(CS$<>8__locals1.power, false);
					return;
				case EffectId.Faith:
				{
					Religion faith = CS$<>8__locals1.tc.Chara.faith;
					if (faith.IsEyth)
					{
						CS$<>8__locals1.tc.SayNothingHappans();
						return;
					}
					CS$<>8__locals1.tc.PlayEffect("aura_heaven", true, 0f, default(Vector3));
					CS$<>8__locals1.tc.PlaySound("aura_heaven", 1f, true);
					CS$<>8__locals1.tc.Say("faith", CS$<>8__locals1.tc, faith.Name, null);
					if (flag)
					{
						CS$<>8__locals1.tc.Say("faith_curse", CS$<>8__locals1.tc, faith.Name, null);
						return;
					}
					if (CS$<>8__locals1.blessed)
					{
						CS$<>8__locals1.tc.Say("faith_bless", CS$<>8__locals1.tc, faith.Name, null);
					}
					CS$<>8__locals1.tc.ModExp(306, CS$<>8__locals1.power * 10);
					CS$<>8__locals1.tc.ModExp(85, CS$<>8__locals1.power * 10);
					if (CS$<>8__locals1.tc.elements.Base(85) >= CS$<>8__locals1.tc.elements.Value(306))
					{
						CS$<>8__locals1.tc.elements.SetBase(85, CS$<>8__locals1.tc.elements.Value(306), 0);
						return;
					}
					return;
				}
				case EffectId.Hero:
					if (flag)
					{
						CS$<>8__locals1.<Proc>g__Redirect|0(EffectId.Fear, BlessedState.Normal, default(ActRef));
						return;
					}
					CS$<>8__locals1.TC.AddCondition<ConHero>(CS$<>8__locals1.power, false);
					return;
				case EffectId.Acidproof:
					if (CS$<>8__locals1.blessed)
					{
						CS$<>8__locals1.power /= 4;
					}
					if (CS$<>8__locals1.TC.IsPC)
					{
						CS$<>8__locals1.TC.Say("pc_pain", null, null);
					}
					CS$<>8__locals1.TC.Say("drink_acid", CS$<>8__locals1.TC, null, null);
					CS$<>8__locals1.TC.DamageHP(CS$<>8__locals1.power / 5, 923, CS$<>8__locals1.power, AttackSource.None, null, true);
					return;
				case EffectId.Levitate:
					if (flag)
					{
						CS$<>8__locals1.<Proc>g__Redirect|0(EffectId.Gravity, BlessedState.Normal, default(ActRef));
						return;
					}
					CS$<>8__locals1.TC.AddCondition<ConLevitate>(CS$<>8__locals1.power, false);
					return;
				case EffectId.Gravity:
					if (CS$<>8__locals1.blessed)
					{
						CS$<>8__locals1.power /= 4;
					}
					CS$<>8__locals1.TC.AddCondition<ConGravity>(CS$<>8__locals1.power, false);
					if (flag)
					{
						CS$<>8__locals1.<Proc>g__Redirect|0(EffectId.BuffStats, BlessedState.Cursed, new ActRef
						{
							aliasEle = "STR"
						});
						return;
					}
					return;
				case EffectId.Mutation:
					CS$<>8__locals1.TC.MutateRandom(1, 100, false, state);
					if (EClass.core.config.game.waitOnDebuff)
					{
						EClass.Wait(0.3f, CS$<>8__locals1.TC);
						return;
					}
					return;
				case EffectId.CureMutation:
					CS$<>8__locals1.TC.MutateRandom(-1, 100, false, state);
					return;
				case EffectId.TransGender:
				{
					CS$<>8__locals1.tc.PlaySound("mutation", 1f, true);
					CS$<>8__locals1.tc.PlayEffect("mutation", true, 0f, default(Vector3));
					int gender = CS$<>8__locals1.tc.bio.gender;
					int gender2 = (gender == 2) ? 1 : ((gender == 1) ? 2 : ((EClass.rnd(2) == 0) ? 2 : 1));
					if (gender != 0 && EClass.rnd(10) == 0)
					{
						gender2 = 0;
					}
					CS$<>8__locals1.tc.bio.SetGender(gender2);
					CS$<>8__locals1.tc.Say("transGender", CS$<>8__locals1.tc, Gender.Name(CS$<>8__locals1.tc.bio.gender), null);
					CS$<>8__locals1.tc.Talk("tail", null, null, false);
					if (CS$<>8__locals1.blessed && CS$<>8__locals1.tc.bio.age > 1)
					{
						CS$<>8__locals1.tc.Say("ageDown", CS$<>8__locals1.tc, null, null);
						Biography bio = CS$<>8__locals1.tc.bio;
						int num6 = bio.age;
						bio.age = num6 - 1;
						return;
					}
					if (flag)
					{
						CS$<>8__locals1.tc.Say("ageUp", CS$<>8__locals1.tc, null, null);
						Biography bio2 = CS$<>8__locals1.tc.bio;
						int num6 = bio2.age;
						bio2.age = num6 + 1;
						return;
					}
					return;
				}
				case EffectId.Buff:
				{
					string text2 = CS$<>8__locals1.actRef.n1;
					string text3 = "";
					if (flag)
					{
						text3 = EClass.sources.stats.alias[text2].curse;
						if (!text3.IsEmpty())
						{
							text2 = text3;
						}
					}
					Condition condition = Condition.Create(text2, CS$<>8__locals1.power, delegate(Condition con)
					{
						if (!CS$<>8__locals1.actRef.aliasEle.IsEmpty())
						{
							con.SetElement(EClass.sources.elements.alias[CS$<>8__locals1.actRef.aliasEle].id);
						}
					});
					condition.isPerfume = (CS$<>8__locals1.TC.IsPC && CS$<>8__locals1.actRef.isPerfume);
					Condition condition2 = CS$<>8__locals1.TC.AddCondition(condition, false);
					if (condition2 != null && condition2.isPerfume)
					{
						condition2.value = 3;
						Msg.Say("perfume", CS$<>8__locals1.TC, null, null, null);
					}
					if (!text3.IsEmpty())
					{
						CS$<>8__locals1.CC.DoHostileAction(CS$<>8__locals1.TC, false);
						return;
					}
					return;
				}
				case EffectId.Debuff:
				{
					CS$<>8__locals1.CC.DoHostileAction(CS$<>8__locals1.TC, false);
					bool isPowerful = CS$<>8__locals1.TC.IsPowerful;
					string n = CS$<>8__locals1.actRef.n1;
					int power2 = CS$<>8__locals1.power;
					int num7 = CS$<>8__locals1.TC.WIL * (isPowerful ? 20 : 5);
					ConHolyVeil condition3 = CS$<>8__locals1.TC.GetCondition<ConHolyVeil>();
					if (condition3 != null)
					{
						num7 += condition3.power * 5;
					}
					if (EClass.rnd(power2) < num7 / EClass.sources.stats.alias[n].hexPower && EClass.rnd(10) != 0)
					{
						CS$<>8__locals1.TC.Say("debuff_resist", CS$<>8__locals1.TC, null, null);
						CS$<>8__locals1.CC.DoHostileAction(CS$<>8__locals1.TC, false);
						return;
					}
					CS$<>8__locals1.TC.AddCondition(Condition.Create(n, CS$<>8__locals1.power, delegate(Condition con)
					{
						con.givenByPcParty = CS$<>8__locals1.CC.IsPCParty;
						if (!CS$<>8__locals1.actRef.aliasEle.IsEmpty())
						{
							con.SetElement(EClass.sources.elements.alias[CS$<>8__locals1.actRef.aliasEle].id);
						}
					}), false);
					if (n == "ConBane" && CS$<>8__locals1.CC.HasElement(1416, 1))
					{
						CS$<>8__locals1.TC.AddCondition<ConExcommunication>(CS$<>8__locals1.power, false);
					}
					CS$<>8__locals1.CC.DoHostileAction(CS$<>8__locals1.TC, false);
					if (EClass.core.config.game.waitOnDebuff && !CS$<>8__locals1.CC.IsPC)
					{
						EClass.Wait(0.3f, CS$<>8__locals1.TC);
						return;
					}
					return;
				}
				case EffectId.Weaken:
					break;
				case EffectId.NeckHunt:
					CS$<>8__locals1.CC.TryNeckHunt(CS$<>8__locals1.TC, CS$<>8__locals1.power, false);
					return;
				case EffectId.PuddleEffect:
					CS$<>8__locals1.TC.DamageHP(CS$<>8__locals1.power / 5, CS$<>8__locals1.actRef.idEle, CS$<>8__locals1.power, AttackSource.None, null, true);
					return;
				case EffectId.RemedyJure:
					CS$<>8__locals1.TC.HealHP(1000000, HealSource.Magic);
					CS$<>8__locals1.TC.CureHost(CureType.Jure, CS$<>8__locals1.power, state);
					CS$<>8__locals1.TC.Say("heal_jure", CS$<>8__locals1.TC, null, null);
					return;
				case EffectId.JureHeal:
					goto IL_2879;
				case EffectId.KizuamiTrick:
				{
					EClass.game.religions.Trickery.Talk("ability", null, null);
					bool hex = CS$<>8__locals1.CC.IsHostile(CS$<>8__locals1.TC);
					List<SourceStat.Row> list5 = (from con in EClass.sources.stats.rows
					where con.tag.Contains("random") && con.@group == (hex ? "Debuff" : "Buff")
					select con).ToList<SourceStat.Row>();
					int power3 = CS$<>8__locals1.power;
					for (int l = 0; l < 4 + EClass.rnd(2); l++)
					{
						SourceStat.Row row2 = list5.RandomItem<SourceStat.Row>();
						list5.Remove(row2);
						ActEffect.Proc(hex ? EffectId.Debuff : EffectId.Buff, CS$<>8__locals1.CC, CS$<>8__locals1.TC, power3, new ActRef
						{
							n1 = row2.alias
						});
					}
					if (EClass.core.config.game.waitOnDebuff && !CS$<>8__locals1.CC.IsPC)
					{
						EClass.Wait(0.3f, CS$<>8__locals1.TC);
						return;
					}
					return;
				}
				case EffectId.CureCorruption:
					CS$<>8__locals1.TC.PlaySound("heal", 1f, true);
					CS$<>8__locals1.TC.PlayEffect("heal", true, 0f, default(Vector3));
					if (flag)
					{
						CS$<>8__locals1.TC.Say("cureCorruption_curse", CS$<>8__locals1.TC, null, null);
						CS$<>8__locals1.TC.mana.Mod(9999);
						CS$<>8__locals1.TC.ModCorruption(CS$<>8__locals1.power);
						return;
					}
					CS$<>8__locals1.TC.Say("cureCorruption", CS$<>8__locals1.TC, null, null);
					CS$<>8__locals1.TC.ModCorruption(-CS$<>8__locals1.power * 2);
					return;
				case EffectId.Headpat:
					CS$<>8__locals1.CC.Cuddle(CS$<>8__locals1.TC, true);
					return;
				case EffectId.RemoveHex:
				case EffectId.RemoveHexAll:
					if (flag)
					{
						CS$<>8__locals1.<Proc>g__Redirect|0(EffectId.CurseEQ, BlessedState.Normal, default(ActRef));
						return;
					}
					foreach (Condition condition4 in CS$<>8__locals1.TC.conditions.Copy<Condition>())
					{
						if (condition4.Type == ConditionType.Debuff && !condition4.IsKilled && EClass.rnd(CS$<>8__locals1.power * 2) > EClass.rnd(condition4.power))
						{
							CS$<>8__locals1.CC.Say("removeHex", CS$<>8__locals1.TC, condition4.Name.ToLower(), null);
							condition4.Kill(false);
							if (CS$<>8__locals1.id == EffectId.RemoveHex)
							{
								break;
							}
						}
					}
					CS$<>8__locals1.TC.AddCondition<ConHolyVeil>(CS$<>8__locals1.power / 2, false);
					return;
				case EffectId.ShutterHex:
				{
					if (!CS$<>8__locals1.CC.IsHostile(CS$<>8__locals1.TC))
					{
						return;
					}
					int hex = 0;
					using (List<Condition>.Enumerator enumerator4 = CS$<>8__locals1.TC.conditions.GetEnumerator())
					{
						while (enumerator4.MoveNext())
						{
							if (enumerator4.Current.Type == ConditionType.Debuff)
							{
								int num6 = hex;
								hex = num6 + 1;
							}
						}
					}
					if (hex == 0)
					{
						CS$<>8__locals1.CC.SayNothingHappans();
						return;
					}
					CS$<>8__locals1.TC.pos.PlayEffect("holyveil");
					CS$<>8__locals1.TC.pos.PlaySound("holyveil", true, 1f, true);
					CS$<>8__locals1.TC.pos.PlaySound("atk_eleSound", true, 1f, true);
					CS$<>8__locals1.TC.conditions.ForeachReverse(delegate(Condition c)
					{
						if (c.Type == ConditionType.Debuff)
						{
							c.Kill(false);
						}
					});
					CS$<>8__locals1.TC.Say("abShutterHex", CS$<>8__locals1.TC, null, null);
					CS$<>8__locals1.TC.pos.ForeachNeighbor(delegate(Point p)
					{
						foreach (Chara chara3 in p.ListCharas())
						{
							if (!chara3.IsHostile(CS$<>8__locals1.CC))
							{
								break;
							}
							int dmg2 = Dice.Create("SpShutterHex", CS$<>8__locals1.power * hex, CS$<>8__locals1.CC, CS$<>8__locals1.actRef.act).Roll();
							chara3.DamageHP(dmg2, 919, CS$<>8__locals1.power, AttackSource.None, CS$<>8__locals1.CC, true);
						}
					}, true);
					return;
				}
				case EffectId.Draw:
				{
					Point randomPoint2 = CS$<>8__locals1.CC.pos.GetRandomPoint(1, true, true, false, 100);
					Point point = (randomPoint2 != null) ? randomPoint2.GetNearestPoint(false, false, true, false) : null;
					if (point == null || !CS$<>8__locals1.CC.CanSeeLos(point, -1))
					{
						return;
					}
					CS$<>8__locals1.CC.Say("abDraw", CS$<>8__locals1.CC, CS$<>8__locals1.TC, null, null);
					if (CS$<>8__locals1.TC.HasCondition<ConGravity>())
					{
						CS$<>8__locals1.CC.SayNothingHappans();
						return;
					}
					CS$<>8__locals1.TC.MoveImmediate(point, !EClass.core.config.camera.smoothFollow, true);
					if (CS$<>8__locals1.CC.id == "tentacle")
					{
						CS$<>8__locals1.TC.AddCondition<ConEntangle>(100, false);
						return;
					}
					return;
				}
				case EffectId.Steal:
				{
					if (EClass._zone.instance is ZoneInstanceBout)
					{
						return;
					}
					if (CS$<>8__locals1.TC.Evalue(426) > 0)
					{
						CS$<>8__locals1.TC.Say((CS$<>8__locals1.actRef.n1 == "money") ? "abStealNegateMoney" : "abStealNegate", CS$<>8__locals1.TC, null, null);
						return;
					}
					Thing thing7 = null;
					bool flag9 = CS$<>8__locals1.actRef.n1 == "food";
					if (CS$<>8__locals1.actRef.n1 == "money")
					{
						int num8 = CS$<>8__locals1.TC.GetCurrency("money");
						if (num8 > 0)
						{
							num8 = Mathf.Clamp(EClass.rnd(num8 / 10), 1, 100 + EClass.rndHalf(CS$<>8__locals1.CC.LV * 200));
							thing7 = ThingGen.Create("money", -1, -1).SetNum(num8);
							CS$<>8__locals1.TC.ModCurrency(-num8, "money");
						}
					}
					else
					{
						Func<Thing, bool> func = (Thing t) => true;
						if (flag9)
						{
							func = ((Thing t) => t.IsFood);
						}
						List<Thing> list6 = CS$<>8__locals1.TC.things.List(delegate(Thing t)
						{
							Card parentCard = t.parentCard;
							return !(((parentCard != null) ? parentCard.trait : null) is TraitChestMerchant) && !(t.trait is TraitTool) && !t.IsThrownWeapon && (t.trait.CanBeDestroyed && t.things.Count == 0 && t.invY != 1 && t.trait.CanBeStolen && !t.trait.CanOnlyCarry && !t.IsUnique && !t.isEquipped && t.blessedState == BlessedState.Normal) && func(t);
						}, true);
						if (list6.Count > 0)
						{
							thing7 = list6.RandomItem<Thing>();
							if (thing7.Num > 1)
							{
								thing7 = thing7.Split(1);
							}
						}
						CS$<>8__locals1.CC.AddCooldown(6640, 200);
					}
					if (thing7 == null)
					{
						CS$<>8__locals1.CC.Say("abStealNothing", CS$<>8__locals1.CC, CS$<>8__locals1.TC, null, null);
						return;
					}
					thing7.SetInt(116, 1);
					CS$<>8__locals1.TC.PlaySound(thing7.material.GetSoundDrop(thing7.sourceCard), 1f, true);
					CS$<>8__locals1.CC.Pick(thing7, false, true);
					CS$<>8__locals1.CC.Say("abSteal", CS$<>8__locals1.CC, CS$<>8__locals1.TC, thing7.Name, null);
					if (!(CS$<>8__locals1.actRef.n1 == "food"))
					{
						CS$<>8__locals1.CC.Say("abStealEscape", CS$<>8__locals1.CC, null, null);
						CS$<>8__locals1.CC.Teleport(ActEffect.GetTeleportPos(CS$<>8__locals1.tc.pos, 30), true, false);
						return;
					}
					if (CS$<>8__locals1.CC.hunger.value != 0)
					{
						CS$<>8__locals1.CC.InstantEat(thing7, true);
						return;
					}
					return;
				}
				case EffectId.ThrowPotion:
					if (!CS$<>8__locals1.CC.pos.Equals(CS$<>8__locals1.TC.pos))
					{
						Thing t2 = ThingGen.Create(new string[]
						{
							"330",
							"331",
							"334",
							"335",
							"336",
							"1142"
						}.RandomItem<string>(), -1, -1);
						ActThrow.Throw(CS$<>8__locals1.CC, CS$<>8__locals1.TC.pos, t2, ThrowMethod.Punish, 0.7f);
						return;
					}
					return;
				default:
					return;
				}
				break;
			}
			bool flag10 = CS$<>8__locals1.id == EffectId.DamageBody || CS$<>8__locals1.id == EffectId.DamageBodyGreat;
			bool mind2 = CS$<>8__locals1.id == EffectId.DamageMind || CS$<>8__locals1.id == EffectId.DamageMindGreat;
			int num9 = (CS$<>8__locals1.id == EffectId.DamageBody || CS$<>8__locals1.id == EffectId.DamageMind) ? 1 : (4 + EClass.rnd(4));
			if (CS$<>8__locals1.id == EffectId.Weaken)
			{
				flag10 = (EClass.rnd(2) == 0);
				mind2 = !flag10;
				num9 = 1;
			}
			else
			{
				CS$<>8__locals1.TC.PlayEffect("debuff", true, 0f, default(Vector3));
				CS$<>8__locals1.TC.PlaySound("debuff", 1f, true);
			}
			CS$<>8__locals1.TC.Say(flag10 ? "damageBody" : "damageMind", CS$<>8__locals1.TC, null, null);
			for (int m = 0; m < num9; m++)
			{
				CS$<>8__locals1.TC.DamageTempElements(CS$<>8__locals1.power, flag10, mind2);
			}
			if (CS$<>8__locals1.TC.IsPC)
			{
				Tutorial.Play("healer");
				return;
			}
			break;
			IL_2879:
			if (CS$<>8__locals1.id == EffectId.JureHeal)
			{
				EClass.game.religions.Healing.Talk("ability", null, null);
			}
			int num10 = Dice.Create((CS$<>8__locals1.actRef.act != null && EClass.sources.calc.map.ContainsKey(CS$<>8__locals1.actRef.act.ID)) ? CS$<>8__locals1.actRef.act.ID : "SpHealLight", CS$<>8__locals1.power, CS$<>8__locals1.CC, CS$<>8__locals1.actRef.act).Roll();
			if (flag)
			{
				CS$<>8__locals1.TC.DamageHP(num10 / 2, 919, CS$<>8__locals1.power, AttackSource.None, null, true);
				return;
			}
			CS$<>8__locals1.TC.HealHPHost(num10, HealSource.Magic);
			CS$<>8__locals1.TC.CureHost(CureType.Heal, CS$<>8__locals1.power, state);
			CS$<>8__locals1.TC.Say((CS$<>8__locals1.power >= 300) ? "heal_heavy" : "heal_light", CS$<>8__locals1.TC, null, null);
			return;
		}
		}
	}

	public static void Poison(Chara tc, Chara c, int power)
	{
		tc.Say("eat_poison", tc, null, null);
		tc.Talk("scream", null, null, false);
		int num = (int)Mathf.Sqrt((float)(power * 100));
		tc.DamageHP(num * 2 + EClass.rnd(num), 915, power, AttackSource.None, null, true);
		if (!tc.isDead && !tc.IsPC)
		{
			EClass.player.ModKarma(-5);
		}
	}

	public static void LoveMiracle(Chara tc, Chara c, int power)
	{
		if (c == tc)
		{
			tc.Say("love_ground", tc, null, null);
		}
		else
		{
			tc.Say("love_chara", c, tc, null, null);
		}
		tc.ModAffinity(EClass.pc, power / 4, true);
		if (EClass.rnd(2) == 0)
		{
			return;
		}
		if (EClass.rnd(2) == 0)
		{
			tc.MakeMilk(true, 1, true);
			return;
		}
		tc.MakeEgg(true, 1, true);
	}

	public static Point GetTeleportPos(Point org, int radius = 6)
	{
		Point point = new Point();
		for (int i = 0; i < 10000; i++)
		{
			point.Set(org);
			point.x += EClass.rnd(radius) - EClass.rnd(radius);
			point.z += EClass.rnd(radius) - EClass.rnd(radius);
			if (point.IsValid && point.IsInBounds && !point.cell.blocked && point.Distance(org) >= radius / 3 + 1 - i / 50 && !point.cell.HasZoneStairs(true))
			{
				return point;
			}
		}
		return org.GetRandomNeighbor().GetNearestPoint(false, true, true, false);
	}

	public static bool Wish(string s, string name, int power)
	{
		Msg.thirdPerson1.Set(EClass.pc, false);
		string netMsg = GameLang.Parse("wish".langGame(), true, name, s, null, null);
		bool net = EClass.core.config.net.enable && EClass.core.config.net.sendEvent;
		List<ActEffect.WishItem> list = new List<ActEffect.WishItem>();
		int wishLv = 10 + power / 4;
		int wishValue = power * 200;
		Debug.Log(power.ToString() + "/" + wishValue.ToString());
		string _s = s.ToLower();
		using (List<CardRow>.Enumerator enumerator = EClass.sources.cards.rows.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				CardRow r = enumerator.Current;
				if (r.HasTag(CTAG.godArtifact))
				{
					bool flag = false;
					using (List<Religion>.Enumerator enumerator2 = EClass.game.religions.list.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							if (enumerator2.Current.IsValidArtifact(r.id))
							{
								flag = true;
							}
						}
					}
					if (!flag)
					{
						continue;
					}
				}
				else if (r.quality >= 4 || r.HasTag(CTAG.noWish))
				{
					string id = r.id;
					if (!(id == "medal") && !(id == "plat") && !(id == "money") && !(id == "money2"))
					{
						continue;
					}
				}
				if (!r.isChara)
				{
					string text = r.GetName(1).ToLower();
					int score = ActEffect.Compare(_s, text);
					if (score != 0)
					{
						list.Add(new ActEffect.WishItem
						{
							score = score,
							n = text,
							action = delegate()
							{
								Debug.Log(r.id);
								SourceCategory.Row category = EClass.sources.cards.map[r.id].Category;
								if (category.IsChildOf("weapon") || category.IsChildOf("armor") || category.IsChildOf("ranged"))
								{
									CardBlueprint.SetRarity(Rarity.Legendary);
								}
								Thing thing = ThingGen.Create(r.id, -1, wishLv);
								int num = 1;
								string id2 = thing.id;
								if (!(id2 == "rod_wish"))
								{
									if (!(id2 == "money"))
									{
										if (!(id2 == "plat"))
										{
											if (!(id2 == "money2"))
											{
												if (id2 == "medal")
												{
													num = EClass.rndHalf(wishValue / 3000 + 4);
												}
											}
											else
											{
												num = EClass.rndHalf(wishValue / 1000 + 4);
											}
										}
										else
										{
											num = EClass.rndHalf(wishValue / 2000 + 4);
										}
									}
									else
									{
										num = EClass.rndHalf(wishValue);
									}
								}
								else
								{
									thing.c_charges = 0;
								}
								if (num < 1)
								{
									num = 1;
								}
								if (num == 1 && thing.trait.CanStack)
								{
									int num2 = wishValue;
									for (int i = 0; i < 1000; i++)
									{
										int num3 = thing.GetPrice(CurrencyType.Money, false, PriceType.Default, null) + 500 + i * 200;
										if (num2 > num3)
										{
											num++;
											num2 -= num3;
										}
									}
								}
								if (thing.trait is TraitDeed)
								{
									num = 1;
								}
								thing.SetNum(num);
								Debug.Log(string.Concat(new string[]
								{
									_s,
									"/",
									num.ToString(),
									"/",
									score.ToString()
								}));
								if (thing.HasTag(CTAG.godArtifact))
								{
									Religion.Reforge(thing.id, null, true);
								}
								else
								{
									EClass._zone.AddCard(thing, EClass.pc.pos);
								}
								netMsg = netMsg + Lang.space + GameLang.Parse("wishNet".langGame(), Msg.IsThirdPerson(thing), Msg.GetName(thing).ToTitleCase(false), null, null, null);
								if (net)
								{
									Net.SendChat(name, netMsg, ChatCategory.Wish, Lang.langCode);
								}
								Msg.Say("dropReward");
							}
						});
					}
				}
			}
		}
		if (list.Count == 0)
		{
			netMsg = netMsg + Lang.space + "wishFail".langGame();
			if (net)
			{
				Net.SendChat(name, netMsg, ChatCategory.Wish, Lang.langCode);
			}
			Msg.Say("wishFail");
			return false;
		}
		list.Sort((ActEffect.WishItem a, ActEffect.WishItem b) => b.score - a.score);
		foreach (ActEffect.WishItem wishItem in list)
		{
			Debug.Log(string.Concat(new string[]
			{
				wishItem.score.ToString(),
				"/",
				s,
				"/",
				wishItem.n
			}));
		}
		list[0].action();
		return true;
	}

	public static int Compare(string s, string t)
	{
		if (s.IsEmpty())
		{
			return 0;
		}
		int num = 0;
		if (t == s)
		{
			num += 100;
		}
		if (t.Contains(s))
		{
			num += 100;
		}
		return num;
	}

	public static int RapidCount;

	public static float RapidDelay;

	public static int angle = 20;

	private class WishItem
	{
		public string n;

		public int score;

		public Action action;
	}
}
