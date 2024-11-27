using System;
using UnityEngine;

public class ActThrow : ActBaseAttack
{
	public override bool CanPressRepeat
	{
		get
		{
			return true;
		}
	}

	public override TargetType TargetType
	{
		get
		{
			return TargetType.Ground;
		}
	}

	public override int PerformDistance
	{
		get
		{
			return 99;
		}
	}

	public override string GetText(string str = "")
	{
		string str2 = "";
		if (this.target != null && this.pcTarget != null && this.target.trait is TraitMonsterBall && this.pcTarget.LV > this.target.LV)
		{
			str2 = " " + "mb_invalidLV".lang();
		}
		return base.GetText(str) + str2;
	}

	public override bool CanPerform()
	{
		if (this.pcTarget != null && this.pcTarget.ExistsOnMap)
		{
			Act.TP.Set(this.pcTarget.pos);
		}
		return !Act.TP.IsHidden && Act.CC.CanSeeLos(Act.TP, -1);
	}

	public override bool Perform()
	{
		if (this.target == null)
		{
			this.target = (Act.TC as Thing);
		}
		if (this.target == null || this.target.isDestroyed || this.target.GetRootCard() != Act.CC)
		{
			return false;
		}
		if (this.pcTarget != null)
		{
			if (!this.pcTarget.ExistsOnMap)
			{
				return false;
			}
			Act.TP.Set(this.pcTarget.pos);
		}
		ActThrow.Throw(Act.CC, Act.TP, this.target.HasElement(410, 1) ? this.target : this.target.Split(1), ThrowMethod.Default, 0f);
		return true;
	}

	public static bool CanThrow(Chara c, Thing t, Card target, Point p = null)
	{
		if (t == null)
		{
			return false;
		}
		if (t.c_isImportant && !t.HasElement(410, 1))
		{
			return false;
		}
		if (p == null && target != null && target.ExistsOnMap)
		{
			p = target.pos;
		}
		if (p == null)
		{
			return false;
		}
		if ((p.HasBlock && !p.cell.hasDoor && p.sourceBlock.tileType.IsBlockPass) || p.Equals(EClass.pc.pos) || t.trait.CanExtendBuild || t.trait.CanOnlyCarry)
		{
			return false;
		}
		if (c.IsPC && EClass.core.config.game.shiftToUseNegativeAbilityOnSelf && !EInput.isShiftDown && !(t.trait is TraitDrink))
		{
			Card card = target ?? p.FindAttackTarget();
			if (card != null && card.IsPCFactionOrMinion)
			{
				return false;
			}
		}
		return true;
	}

	public static EffectIRenderer Throw(Card c, Point p, Thing t, ThrowMethod method = ThrowMethod.Default, float failChance = 0f)
	{
		if (failChance > EClass.rndf(1f))
		{
			Point randomPoint = p.GetRandomPoint(1, true, true, false, 100);
			if (randomPoint != null && !randomPoint.Equals(c.pos))
			{
				p = randomPoint;
			}
		}
		return ActThrow.Throw(c, p, p.FindAttackTarget(), t, method);
	}

	public static EffectIRenderer Throw(Card c, Point p, Card target, Thing t, ThrowMethod method = ThrowMethod.Default)
	{
		if (t.parent != EClass._zone && !t.HasElement(410, 1))
		{
			EClass._zone.AddCard(t, c.pos).KillAnime();
		}
		Act.TP.Set(p);
		Act.TC = target;
		if (t.trait.ThrowType == ThrowType.Snow)
		{
			t.dir = EClass.rnd(2);
			c.Talk("snow_throw", null, null, false);
			if (EClass.rnd(2) == 0)
			{
				Act.TC = null;
			}
		}
		c.Say("throw", c, t.GetName(NameStyle.Full, 1), null);
		c.LookAt(p);
		c.renderer.NextFrame();
		c.PlaySound("throw", 1f, true);
		EffectIRenderer result = null;
		if (c.isSynced || p.IsSync)
		{
			result = Effect.Get<EffectIRenderer>((t.trait is TraitBall) ? "throw_ball" : "throw").Play(t, c.pos, p, 0.2f);
			t.renderer.SetFirst(false, c.renderer.position);
		}
		if (!t.HasElement(410, 1))
		{
			t._Move(p, Card.MoveType.Walk);
		}
		if (!t.trait.CanBeDestroyed)
		{
			c.PlaySound("miss", 1f, true);
			return result;
		}
		ThrowType throwType = t.trait.ThrowType;
		if (throwType - ThrowType.Potion <= 1)
		{
			Msg.Say("shatter");
		}
		bool flag = method == ThrowMethod.Reward;
		bool flag2 = method == ThrowMethod.Default;
		switch (t.trait.ThrowType)
		{
		case ThrowType.Potion:
			flag = true;
			if (Act.TC != null)
			{
				Act.TC.Say("throw_hit", t, Act.TC, null, null);
			}
			Act.TP.ModFire(-50);
			if (Act.TC != null && Act.TC.isChara)
			{
				if (t.trait.CanDrink(Act.TC.Chara))
				{
					t.trait.OnDrink(Act.TC.Chara);
				}
				flag2 = t.IsNegativeGift;
				Act.TC.Chara.AddCondition<ConWet>(100, false);
			}
			else
			{
				t.trait.OnThrowGround(c.Chara, Act.TP);
			}
			t.Die(null, null, AttackSource.Throw);
			c.ModExp(108, 50);
			break;
		case ThrowType.Vase:
			t.Die(null, null, AttackSource.Throw);
			break;
		case ThrowType.Snow:
			flag = true;
			flag2 = false;
			if (Act.TC != null && Act.TC.isChara)
			{
				Act.TC.Say("throw_hit", t, Act.TC, null, null);
				if (EClass.rnd(2) == 0)
				{
					c.Talk("snow_hit", null, null, false);
				}
				Act.TC.Chara.AddCondition<ConWet>(50, false);
				t.Die(null, null, AttackSource.Throw);
				c.ModExp(108, 50);
			}
			break;
		case ThrowType.Flyer:
			flag = true;
			flag2 = false;
			if (Act.TC != null && Act.TC.isChara && c.isChara)
			{
				Act.TC.Say("throw_hit", t, Act.TC, null, null);
				c.Chara.GiveGift(Act.TC.Chara, t);
				c.ModExp(108, 50);
			}
			break;
		case ThrowType.MonsterBall:
		{
			flag = true;
			flag2 = false;
			TraitMonsterBall traitMonsterBall = t.trait as TraitMonsterBall;
			if (traitMonsterBall.chara != null)
			{
				if (!traitMonsterBall.IsLittleBall || EClass._zone is Zone_LittleGarden)
				{
					Chara _c = EClass._zone.AddCard(traitMonsterBall.chara, p).Chara;
					_c.PlayEffect("identify", true, 0f, default(Vector3));
					t.Die(null, null, AttackSource.None);
					if (traitMonsterBall.IsLittleBall && _c.id == "littleOne")
					{
						_c.orgPos = c.pos.Copy();
						_c.c_originalHostility = (_c.hostility = Hostility.Neutral);
						EClass._zone.ModInfluence(5);
						_c.PlaySound("chime_angel", 1f, true);
						EClass.core.actionsNextFrame.Add(delegate
						{
							_c.Talk("little_saved", null, null, false);
						});
						EClass.player.flags.little_saved = true;
						EClass.player.little_saved++;
					}
					else
					{
						_c.MakeAlly(true);
					}
				}
			}
			else if (Act.TC != null && Act.TC.isChara)
			{
				Act.TC.Say("throw_hit", t, Act.TC, null, null);
				Chara chara = Act.TC.Chara;
				if (traitMonsterBall.IsLittleBall)
				{
					if (chara.id != "littleOne" || EClass._zone is Zone_LittleGarden || EClass._zone.IsUserZone)
					{
						Msg.Say("monsterball_invalid");
						break;
					}
				}
				else
				{
					if (!chara.trait.CanBeTamed || EClass._zone.IsUserZone)
					{
						Msg.Say("monsterball_invalid");
						break;
					}
					if (chara.LV > traitMonsterBall.owner.LV)
					{
						Msg.Say("monsterball_lv");
						break;
					}
					if (!EClass.debug.enable && chara.hp > chara.MaxHP / 10)
					{
						Msg.Say("monsterball_hp");
						break;
					}
				}
				Msg.Say("monsterball_capture", c, chara, null, null);
				chara.PlaySound("identify", 1f, true);
				chara.PlayEffect("identify", true, 0f, default(Vector3));
				t.ChangeMaterial("copper");
				if (chara.IsLocalChara)
				{
					string str = "Creating Replacement NPC for:";
					Chara chara2 = chara;
					Debug.Log(str + ((chara2 != null) ? chara2.ToString() : null));
					EClass._map.deadCharas.Add(chara.CreateReplacement());
				}
				traitMonsterBall.chara = chara;
				EClass._zone.RemoveCard(chara);
				chara.homeZone = null;
				c.ModExp(108, 100);
			}
			break;
		}
		case ThrowType.Explosive:
			flag = true;
			t.c_uidRefCard = c.uid;
			t.Die(null, c, AttackSource.Throw);
			break;
		case ThrowType.Ball:
			flag = true;
			flag2 = false;
			if (Act.TC != null && Act.TC.isChara)
			{
				Act.TC.Say("throw_hit", t, Act.TC, null, null);
				if (EClass.rnd(2) == 0)
				{
					c.Talk("snow_hit", null, null, false);
				}
				Act.TC.Say("ball_hit", null, null);
				Chara chara3 = Act.TC.Chara;
				if (chara3 != null)
				{
					chara3.Pick(t, true, true);
				}
				c.ModExp(108, 50);
			}
			break;
		}
		if (t.trait is TraitDye)
		{
			t.trait.OnThrowGround(c.Chara, Act.TP);
		}
		if (!flag && Act.TC != null)
		{
			AttackProcess.Current.Prepare(c.Chara, t, Act.TC, Act.TP, 0, true);
			if (AttackProcess.Current.Perform(0, false, 1f, false))
			{
				if (Act.TC.IsAliveInCurrentZone && t.trait is TraitErohon && Act.TC.id == t.c_idRefName)
				{
					Act.TC.Chara.OnGiveErohon(t);
				}
				if (!t.isDestroyed && t.trait.CanBeDestroyed && !t.IsFurniture && !t.category.IsChildOf("instrument") && !t.IsUnique && !t.HasElement(410, 1))
				{
					t.Destroy();
				}
			}
			else
			{
				c.PlaySound("miss", 1f, true);
			}
		}
		if (EClass.rnd(2) == 0)
		{
			c.Chara.RemoveCondition<ConInvisibility>();
		}
		if (Act.TC != null)
		{
			if (flag2)
			{
				c.Chara.DoHostileAction(Act.TC, false);
			}
			if ((Act.TC.trait.CanBeAttacked || Act.TC.IsRestrainedResident) && EClass.rnd(2) == 0)
			{
				c.Chara.stamina.Mod(-1);
			}
		}
		if (t.HasElement(410, 1) && Act.TC != null && Act.CC == EClass.pc && !(Act.CC.ai is AI_PracticeDummy) && (Act.TC.trait is TraitTrainingDummy || Act.TC.IsRestrainedResident) && Act.CC.stamina.value > 0)
		{
			Act.CC.SetAI(new AI_PracticeDummy
			{
				target = Act.TC,
				throwItem = t
			});
		}
		return result;
	}

	public Thing target;

	public Chara pcTarget;
}
