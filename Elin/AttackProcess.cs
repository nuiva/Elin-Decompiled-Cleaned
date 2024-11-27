using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class AttackProcess : EClass
{
	public bool IsMartial
	{
		get
		{
			return this.weapon == null;
		}
	}

	public bool IsMartialWeapon
	{
		get
		{
			return this.weapon != null && this.weapon.category.skill == 100;
		}
	}

	public bool IsRanged
	{
		get
		{
			return this.toolRange != null && !this.isThrow && !this.toolRange.owner.Thing.isEquipped;
		}
	}

	public bool IsCane
	{
		get
		{
			return this.IsRanged && this.toolRange is TraitToolRangeCane;
		}
	}

	public string GetText()
	{
		string text = this.dNum.ToString() + "d" + this.dDim.ToString();
		text = text + ((this.dBonus >= 0) ? "+" : "") + this.dBonus.ToString();
		string @ref = this.IsMartial ? "evalHand".lang() : "evalWeapon".lang((this.attackIndex + 1).ToString() ?? "", null, null, null, null);
		return "attackEval".lang(@ref, text, this.dMulti.ToString("F2") ?? "", this.toHit.ToString() ?? "", this.penetration.ToString() ?? "");
	}

	public void Prepare(Chara _CC, Thing _weapon, Card _TC = null, Point _TP = null, int _attackIndex = 0, bool _isThrow = false)
	{
		this.CC = _CC;
		this.TC = _TC;
		this.TP = _TP;
		this.isThrow = _isThrow;
		this.weapon = _weapon;
		this.ammo = ((_weapon == null) ? null : _weapon.ammoData);
		this.hit = (this.crit = (this.evadePlus = false));
		Thing thing = this.weapon;
		this.toolRange = (((thing != null) ? thing.trait : null) as TraitToolRange);
		this.attackType = AttackType.Slash;
		this.attackStyle = AttackStyle.Default;
		this.evasion = 0;
		this.penetration = 0;
		this.distMod = 100;
		this.attackIndex = _attackIndex;
		this.posRangedAnime = this.TP;
		this.ignoreAnime = (this.ignoreAttackSound = false);
		if (!this.isThrow)
		{
			if (!this.IsRanged)
			{
				this.attackStyle = this.CC.body.GetAttackStyle();
			}
			else if (this.TP != null)
			{
				int num = this.CC.pos.Distance(this.TP);
				this.distMod = Mathf.Max(115 - 10 * Mathf.Abs(num - this.toolRange.BestDist) * 100 / (100 + this.weapon.Evalue(605) * 10), 80);
			}
		}
		if (this.isThrow)
		{
			bool flag = this.weapon.HasTag(CTAG.throwWeapon) || this.weapon.HasTag(CTAG.throwWeaponEnemy);
			int num2 = (int)Mathf.Clamp(Mathf.Sqrt((float)(this.weapon.SelfWeight + this.weapon.ChildrenWeight)) * 3f + 25f + (float)(flag ? 75 : 0), 10f, 400f + Mathf.Sqrt((float)this.CC.STR) * 50f);
			int num3 = Mathf.Clamp(this.weapon.material.hardness, flag ? 40 : 20, 200);
			this.weaponSkill = this.CC.elements.GetOrCreateElement(108);
			this.attackType = AttackType.Blunt;
			this.dBonus = this.CC.DMG + (this.CC.IsPCParty ? 3 : 7);
			this.dNum = 2;
			this.dDim = ((this.CC.IsPCParty ? 0 : this.CC.LV) + this.CC.STR + this.CC.Evalue(108)) * num2 * num3 / 10000 / 2;
			this.dMulti = 1f;
			this.toHitBase = EClass.curve(this.CC.DEX / 4 + this.CC.STR / 2 + this.weaponSkill.Value, 50, 25, 75) + (this.CC.IsPCFaction ? 75 : 250);
			this.toHitFix = this.CC.HIT + this.weapon.HIT;
			this.penetration = 25;
		}
		else if (this.IsMartial || this.IsMartialWeapon)
		{
			this.weaponSkill = this.CC.elements.GetOrCreateElement(100);
			this.attackType = (this.CC.race.meleeStyle.IsEmpty() ? ((EClass.rnd(2) == 0) ? AttackType.Kick : AttackType.Punch) : this.CC.race.meleeStyle.ToEnum(true));
			this.dBonus = this.CC.DMG + this.CC.encLV + (int)Mathf.Sqrt((float)Mathf.Max(0, this.CC.STR / 5 + this.weaponSkill.Value / 4));
			this.dNum = 2 + Mathf.Min(this.weaponSkill.Value / 10, 4);
			this.dDim = 5 + (int)Mathf.Sqrt((float)Mathf.Max(0, this.weaponSkill.Value / 3));
			this.dMulti = 0.6f + (float)(this.CC.STR / 2 + this.weaponSkill.Value / 2 + this.CC.Evalue(132) / 2) / 50f;
			this.dMulti += 0.05f * (float)this.CC.Evalue(1400);
			this.toHitBase = EClass.curve(this.CC.DEX / 3 + this.CC.STR / 3 + this.weaponSkill.Value, 50, 25, 75) + 50;
			this.toHitFix = this.CC.HIT;
			if (this.attackStyle == AttackStyle.Shield)
			{
				this.toHitBase = this.toHitBase * 75 / 100;
			}
			this.penetration = Mathf.Clamp(this.weaponSkill.Value / 10 + 5, 5, 20) + this.CC.Evalue(92);
			if (this.IsMartialWeapon)
			{
				this.dBonus += this.weapon.DMG;
				this.dNum += this.weapon.source.offense[0];
				this.dDim = Mathf.Max(this.dDim / 2 + this.weapon.c_diceDim, 1);
				this.toHitFix += this.weapon.HIT;
				this.penetration += this.weapon.Penetration;
				if (!this.weapon.source.attackType.IsEmpty())
				{
					this.attackType = this.weapon.source.attackType.ToEnum(true);
				}
			}
		}
		else
		{
			if (this.IsRanged)
			{
				this.weaponSkill = this.CC.elements.GetOrCreateElement(this.toolRange.WeaponSkill);
			}
			else
			{
				this.weaponSkill = this.CC.elements.GetOrCreateElement(this.weapon.category.skill);
			}
			if (!this.weapon.source.attackType.IsEmpty())
			{
				this.attackType = this.weapon.source.attackType.ToEnum(true);
			}
			bool flag2 = this.IsCane || this.weapon.Evalue(482) > 0;
			if (flag2)
			{
				this.weaponSkill = this.CC.elements.GetOrCreateElement(305);
			}
			this.dBonus = this.CC.DMG + this.CC.encLV + this.weapon.DMG;
			this.dNum = this.weapon.source.offense[0];
			this.dDim = this.weapon.c_diceDim;
			this.dMulti = 0.6f + (float)(this.weaponSkill.GetParent(this.CC).Value + this.weaponSkill.Value / 2 + this.CC.Evalue(flag2 ? 304 : (this.IsRanged ? 133 : 132))) / 50f;
			this.dMulti += 0.05f * (float)this.CC.Evalue(this.IsRanged ? 1404 : 1400);
			this.toHitBase = EClass.curve((this.IsCane ? this.CC.WIL : this.CC.DEX) / 4 + this.weaponSkill.GetParent(this.CC).Value / 3 + this.weaponSkill.Value, 50, 25, 75) + 50;
			this.toHitFix = this.CC.HIT + this.weapon.HIT;
			this.penetration = this.weapon.Penetration + this.CC.Evalue(92);
			if (this.IsCane)
			{
				this.toHitBase += 50;
			}
		}
		if (this.ammo != null && !(this.ammo.trait is TraitAmmoTalisman))
		{
			this.dNumAmmo = ((this.ammo.source.offense.Length != 0) ? this.ammo.source.offense[0] : 0);
			this.dDimAmmo = this.ammo.c_diceDim;
			this.dBonusAmmo = this.ammo.DMG + this.ammo.encLV;
			if (this.dNumAmmo < 1)
			{
				this.dNumAmmo = 1;
			}
			if (this.dDimAmmo < 1)
			{
				this.dDimAmmo = 1;
			}
			this.toHitFix += this.ammo.HIT;
		}
		else
		{
			this.dNumAmmo = 0;
			this.dDimAmmo = 0;
		}
		if (this.dNum < 1)
		{
			this.dNum = 1;
		}
		if (this.dDim < 1)
		{
			this.dDim = 1;
		}
		if (this.penetration > 100)
		{
			this.penetration = 100;
		}
		if (this.attackStyle == AttackStyle.TwoHand)
		{
			this.dMulti = this.dMulti * 1.5f + 0.1f * Mathf.Sqrt((float)Mathf.Max(0, this.CC.Evalue(130)));
		}
		this.dMulti = this.dMulti * (float)this.distMod / 100f;
		this.toHit = this.toHitBase + this.toHitFix;
		this.toHit = this.toHit * this.distMod / 100;
		if (this.CC.HasCondition<ConBane>())
		{
			this.toHit = this.toHit * 75 / 100;
		}
		if (this.TC != null && this.CC.HasHigherGround(this.TC))
		{
			this.toHit = this.toHit * 120 / 100;
		}
		if (this.CC.ride != null)
		{
			this.toHit = this.toHit * 100 / (100 + 500 / Mathf.Max(5, 10 + this.CC.EvalueRiding()));
		}
		if (this.CC.parasite != null)
		{
			this.toHit = this.toHit * 100 / (100 + 1000 / Mathf.Max(5, 10 + this.CC.Evalue(227)));
		}
		if (this.CC.host != null)
		{
			if (this.CC.host.ride == this.CC)
			{
				this.toHit = this.toHit * 100 / (100 + 1000 / Mathf.Max(5, 10 + this.CC.STR));
			}
			if (this.CC.host.parasite == this.CC)
			{
				this.toHit = this.toHit * 100 / (100 + 2000 / Mathf.Max(5, 10 + this.CC.DEX));
			}
		}
		if (this.attackStyle == AttackStyle.TwoHand)
		{
			this.toHit += 25 + (int)Mathf.Sqrt((float)(Mathf.Max(0, this.CC.Evalue(130)) * 2));
		}
		else if (this.attackStyle == AttackStyle.TwoWield && this.toHit > 0)
		{
			this.toHit = this.toHit * 100 / (115 + this.attackIndex * 15 + this.attackIndex * Mathf.Clamp(2000 / (20 + this.CC.Evalue(131)), 0, 100));
		}
		if (this.CC.isBlind)
		{
			this.toHit /= ((this.IsRanged || this.isThrow) ? 10 : 3);
		}
		if (this.CC.isConfused || this.CC.HasCondition<ConDim>())
		{
			this.toHit /= 2;
		}
		if (this.TC == null)
		{
			return;
		}
		this.evasion = EClass.curve(this.TC.PER / 3 + this.TC.Evalue(150), 50, 10, 75) + this.TC.DV + 25;
		if (this.TC.isChara && this.TC.Chara.isBlind)
		{
			this.evasion /= 2;
		}
		if (this.TC.HasCondition<ConDim>())
		{
			this.evasion /= 2;
		}
		if (this.TC.isChara && this.TC.Chara.HasHigherGround(this.CC))
		{
			this.evasion = this.evasion * 120 / 100;
		}
	}

	public unsafe void PlayRangedAnime(int numFire)
	{
		AttackProcess.<>c__DisplayClass43_0 CS$<>8__locals1 = new AttackProcess.<>c__DisplayClass43_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.isGun = (this.toolRange is TraitToolRangeGun);
		CS$<>8__locals1.isCane = (this.toolRange is TraitToolRangeCane);
		AttackProcess.<>c__DisplayClass43_0 CS$<>8__locals2 = CS$<>8__locals1;
		GameSetting.EffectData data;
		if ((data = EClass.setting.effect.guns.TryGetValue(this.weapon.id, null)) == null)
		{
			data = EClass.setting.effect.guns[CS$<>8__locals1.isCane ? "cane" : (CS$<>8__locals1.isGun ? "gun" : "bow")];
		}
		CS$<>8__locals2.data = data;
		CS$<>8__locals1.isPCC = (this.CC.IsPCC && this.CC.renderer.hasActor);
		CS$<>8__locals1.firePos = (CS$<>8__locals1.isPCC ? new Vector2(CS$<>8__locals1.data.firePos.x * (float)((this.CC.renderer.actor.currentDir == 0 || this.CC.renderer.actor.currentDir == 1) ? -1 : 1), CS$<>8__locals1.data.firePos.y) : Vector2.zero);
		CS$<>8__locals1._CC = this.CC;
		CS$<>8__locals1._TP = this.posRangedAnime.Copy();
		CS$<>8__locals1._weapon = this.weapon;
		CS$<>8__locals1.ignoreSound = this.ignoreAttackSound;
		for (int i = 0; i < numFire; i++)
		{
			float delay = (float)i * CS$<>8__locals1.data.delay;
			Action action;
			if ((action = CS$<>8__locals1.<>9__0) == null)
			{
				action = (CS$<>8__locals1.<>9__0 = delegate()
				{
					if (!EClass.core.IsGameStarted || !CS$<>8__locals1._CC.IsAliveInCurrentZone)
					{
						return;
					}
					if (CS$<>8__locals1._weapon.id == "gun_rail")
					{
						CS$<>8__locals1._CC.PlayEffect("laser", true, 0f, default(Vector3)).GetComponent<SpriteBasedLaser>().Play(*CS$<>8__locals1._TP.PositionCenter());
					}
					else
					{
						Effect effect = Effect.Get("ranged_arrow")._Play(CS$<>8__locals1._CC.pos, CS$<>8__locals1._CC.isSynced ? CS$<>8__locals1._CC.renderer.position : (*CS$<>8__locals1._CC.pos.Position()), 0f, CS$<>8__locals1._TP, CS$<>8__locals1.data.sprite);
						if (CS$<>8__locals1.isCane)
						{
							IEnumerable<Element> enumerable = from e in CS$<>8__locals1.<>4__this.toolRange.owner.elements.dict.Values
							where e.source.categorySub == "eleAttack"
							select e;
							if (enumerable.Count<Element>() > 0)
							{
								Element element = enumerable.RandomItem<Element>();
								effect.sr.color = EClass.Colors.elementColors[element.source.alias];
							}
						}
					}
					if (CS$<>8__locals1.data.eject)
					{
						if (!CS$<>8__locals1.ignoreSound)
						{
							CS$<>8__locals1._CC.PlaySound("bullet_drop", 1f, true);
						}
						CS$<>8__locals1._CC.PlayEffect("bullet", true, 0f, default(Vector3)).Emit(1);
					}
					if (CS$<>8__locals1.isGun)
					{
						if (CS$<>8__locals1.isPCC)
						{
							CS$<>8__locals1._weapon.PlayEffect(CS$<>8__locals1.data.idEffect.IsEmpty("gunfire"), true, 0f, CS$<>8__locals1.firePos);
						}
						else
						{
							CS$<>8__locals1._CC.PlayEffect(CS$<>8__locals1.data.idEffect.IsEmpty("gunfire"), true, 0f, default(Vector3));
						}
					}
					if (!CS$<>8__locals1.ignoreSound)
					{
						CS$<>8__locals1._CC.PlaySound(CS$<>8__locals1.data.idSound.IsEmpty("attack_gun"), 1f, true);
					}
				});
			}
			TweenUtil.Delay(delay, action);
		}
	}

	public bool Perform(int count, bool hasHit, float dmgMulti = 1f, bool maxRoll = false)
	{
		bool flag = this.CC.HasCondition<ConReload>();
		this.hit = this.CalcHit();
		int num = Dice.Roll(this.dNum, this.dDim, this.dBonus, this.CC);
		if (this.ammo != null && !flag)
		{
			num += Dice.Roll(this.dNumAmmo, this.dDimAmmo, this.dBonusAmmo, this.CC);
		}
		if (this.crit || maxRoll)
		{
			num = Dice.RollMax(this.dNum, this.dDim, this.dBonus);
			if (this.ammo != null && !flag)
			{
				num += Dice.RollMax(this.dNumAmmo, this.dDimAmmo, 0);
			}
			if (this.crit && (this.IsMartial || this.IsMartialWeapon))
			{
				this.dMulti *= 1.25f;
			}
		}
		if (this.CC.Evalue(1355) > 0)
		{
			ConStrife condition = this.CC.GetCondition<ConStrife>();
			if (condition != null)
			{
				num += condition.GetDice().Roll();
			}
			else
			{
				num++;
			}
		}
		num = Mathf.Clamp(num, 0, 9999999);
		num = (int)(this.dMulti * (float)num * dmgMulti);
		num = Mathf.Clamp(num, 0, 9999999);
		if (this.IsRanged && count >= this.numFireWithoutDamageLoss)
		{
			num = num * 100 / (100 + (count - this.numFireWithoutDamageLoss + 1) * 30);
		}
		if (this.CC.isRestrained)
		{
			num /= 2;
		}
		List<Element> list = new List<Element>();
		int num2 = this.CC.Evalue(91);
		int num3 = 0;
		if (this.weapon != null)
		{
			list = this.weapon.elements.dict.Values.ToList<Element>();
			if (this.ammo != null && !flag)
			{
				list = list.Concat(this.ammo.elements.dict.Values).ToList<Element>();
			}
			if (this.IsRanged || this.isThrow)
			{
				num2 += this.weapon.Evalue(91);
			}
			num3 += this.weapon.Evalue(603);
		}
		else if (this.CC.id == "rabbit_vopal")
		{
			list.Add(Element.Create(6650, 100));
		}
		Card tc = this.TC;
		if (((tc != null) ? tc.Chara : null) != null)
		{
			SourceRace.Row race = this.TC.Chara.race;
			AttackProcess.<>c__DisplayClass44_0 CS$<>8__locals1;
			CS$<>8__locals1.bane = this.CC.Evalue(468);
			this.<Perform>g__AddBane|44_5(race.IsUndead, 461, ref CS$<>8__locals1);
			this.<Perform>g__AddBane|44_5(race.IsAnimal, 463, ref CS$<>8__locals1);
			this.<Perform>g__AddBane|44_5(race.IsHuman, 464, ref CS$<>8__locals1);
			this.<Perform>g__AddBane|44_5(race.IsDragon, 460, ref CS$<>8__locals1);
			this.<Perform>g__AddBane|44_5(race.IsGod, 466, ref CS$<>8__locals1);
			this.<Perform>g__AddBane|44_5(race.IsMachine, 465, ref CS$<>8__locals1);
			this.<Perform>g__AddBane|44_5(race.IsFish, 467, ref CS$<>8__locals1);
			this.<Perform>g__AddBane|44_5(race.IsFairy, 462, ref CS$<>8__locals1);
			if (CS$<>8__locals1.bane != 0)
			{
				num = num * (100 + CS$<>8__locals1.bane * 3) / 100;
			}
		}
		if (this.CC.IsPCFaction)
		{
			foreach (Element item in EClass.pc.faction.charaElements.dict.Values)
			{
				list.Add(item);
			}
		}
		if (this.hit && num2 > EClass.rnd(100))
		{
			this.CC.Say("vopal", null, null);
			this.penetration = 100;
		}
		if (this.crit && this.CC.IsPC)
		{
			this.CC.Say("critical", null, null);
			this.CC.PlaySound("critical", 1f, true);
		}
		if (this.CC.isSynced || (this.TC != null && this.TC.isSynced))
		{
			if (this.toolRange != null && (!this.IsRanged || count == 0) && !flag && !this.ignoreAnime)
			{
				this.PlayRangedAnime(this.IsRanged ? this.numFire : 1);
			}
			if (this.hit && this.TC != null && !hasHit)
			{
				this.<Perform>g__PlayHitEffect|44_4();
			}
		}
		if (this.TC == null)
		{
			this.CC.Say(this.IsRanged ? "attack_air_range" : "attack_air", this.CC, null, null);
			return true;
		}
		if (!this.hit)
		{
			if (this.TC != null)
			{
				if (this.CC.IsPCParty)
				{
					this.CC.Say(this.evadePlus ? "evadePlus2" : "evade2", this.CC, this.TC, null, null);
				}
				else
				{
					this.TC.Say(this.evadePlus ? "evadePlus" : "evade", this.TC, this.CC, null, null);
				}
				this.<Perform>g__ModExpDef|44_2(150, 90);
				this.<Perform>g__ModExpDef|44_2(151, 90);
			}
			this.<Perform>g__Proc|44_0(list);
			return false;
		}
		if (this.TC.IsPC)
		{
			Msg.SetColor("attack_pc");
			EClass.pc.Say("attackMeleeToPC", this.CC, this.TC, this.GetAttackText(this.attackType, 3), null);
		}
		else
		{
			this.CC.Say("attackMelee", this.CC, this.TC, this.GetAttackText(this.attackType, 0), null);
		}
		bool showEffect = true;
		int num4 = 0;
		int num5 = 0;
		int num6 = 0;
		ConWeapon conWeapon = null;
		if (this.weapon != null)
		{
			foreach (Element element in this.weapon.elements.dict.Values)
			{
				if (element.source.categorySub == "eleConvert")
				{
					num4 = EClass.sources.elements.alias[element.source.aliasRef].id;
					num5 = 50 + element.Value * 2;
					num6 = Mathf.Min(element.Value, 100);
					break;
				}
			}
		}
		if (num4 == 0)
		{
			if (this.CC.HasCondition<ConWeapon>())
			{
				conWeapon = this.CC.GetCondition<ConWeapon>();
				num4 = conWeapon.sourceElement.id;
				num5 = conWeapon.power / 2;
				num6 = 40 + (int)Mathf.Min(MathF.Sqrt((float)conWeapon.power), 80f);
			}
			if (conWeapon == null && this.weapon == null && (this.CC.MainElement != Element.Void || this.CC.HasElement(1565, 1)))
			{
				num4 = (this.CC.HasElement(1565, 1) ? 915 : this.CC.MainElement.id);
				num5 = this.CC.Power / 3 + EClass.rnd(this.CC.Power / 2);
				if (this.CC.MainElement != Element.Void)
				{
					num5 += this.CC.MainElement.Value;
				}
				showEffect = false;
				num6 = 50;
			}
			if (conWeapon == null && this.weapon != null && this.weapon.trait is TraitToolRangeCane)
			{
				IEnumerable<Element> enumerable = from e in this.weapon.elements.dict.Values
				where e.source.categorySub == "eleAttack"
				select e;
				if (enumerable.Count<Element>() > 0)
				{
					num4 = enumerable.RandomItem<Element>().id;
					if (num4 != 914)
					{
						if (num4 != 918)
						{
							if (num4 == 920)
							{
								num5 = 30;
							}
							else
							{
								num5 = 100;
							}
						}
						else
						{
							num5 = 50;
						}
					}
					else
					{
						num5 = 50;
					}
				}
				num6 = 50;
			}
		}
		int num7 = num;
		int num8 = num * num6 / 100;
		num -= num8;
		int num9 = num * this.penetration / 100;
		num -= num9;
		num = this.TC.ApplyProtection(num, 100) + num9 + num8;
		this.TC.DamageHP(num, num4, num5, (this.IsRanged || this.isThrow) ? AttackSource.Range : AttackSource.Melee, this.CC, showEffect);
		if (conWeapon != null)
		{
			conWeapon.Mod(-1, false);
		}
		bool flag2 = this.IsCane || (this.weapon != null && this.weapon.Evalue(482) > 0);
		int attackStyleElement = this.CC.body.GetAttackStyleElement(this.attackStyle);
		int mod = 100 / (count + 1);
		if (!this.IsRanged || count == 0)
		{
			this.<Perform>g__ModExpAtk|44_3(this.weaponSkill.id, mod);
			this.<Perform>g__ModExpAtk|44_3(flag2 ? 304 : (this.IsRanged ? 133 : 132), mod);
		}
		if (this.crit)
		{
			this.<Perform>g__ModExpAtk|44_3(134, 50);
		}
		if (count == 0 && attackStyleElement != 0)
		{
			this.<Perform>g__ModExpAtk|44_3(attackStyleElement, 100);
		}
		if (!this.CC.IsAliveInCurrentZone || !this.TC.IsAliveInCurrentZone)
		{
			return true;
		}
		if (EClass.rnd(8) == 0 && this.TC.isChara && this.CC.HasElement(1219, 1))
		{
			this.CC.Say("abCrab", this.CC, this.TC, null, null);
			this.TC.Chara.AddCondition<ConParalyze>(30 + EClass.rnd(30), false);
			this.TC.Chara.AddCondition<ConGravity>(100, false);
		}
		if (list.Count > 0)
		{
			foreach (Element element2 in list)
			{
				if (!this.TC.IsAliveInCurrentZone)
				{
					break;
				}
				if (element2.source.categorySub == "eleAttack")
				{
					int num10 = 25;
					int dmg = EClass.rnd(num * (100 + element2.Value * 10) / 500 + 5);
					if (conWeapon == null && this.weapon != null && this.weapon.trait is TraitToolRangeCane)
					{
						num10 = 0;
					}
					if (num10 >= EClass.rnd(100))
					{
						this.TC.DamageHP(dmg, element2.id, this.isThrow ? (100 + element2.Value * 5) : (30 + element2.Value), AttackSource.WeaponEnchant, this.CC, true);
					}
				}
			}
			this.<Perform>g__Proc|44_0(list);
		}
		if (!this.CC.IsAliveInCurrentZone || !this.TC.IsAliveInCurrentZone)
		{
			return true;
		}
		if (!this.IsRanged && this.attackStyle == AttackStyle.Shield)
		{
			int num11 = this.CC.Evalue(123);
			if (this.CC.elements.ValueWithoutLink(123) >= 10 && Mathf.Clamp(Mathf.Sqrt((float)num11) - 2f, 8f, 12f) > (float)EClass.rnd(100))
			{
				num = num7 * Mathf.Min(50 + num11, 200) / 100;
				num = this.TC.ApplyProtection(num, 100);
				Debug.Log("Bash:" + num.ToString() + "/" + num7.ToString());
				this.CC.PlaySound("shield_bash", 1f, true);
				this.CC.Say("shield_bash", this.CC, this.TC, null, null);
				this.TC.DamageHP(num, AttackSource.None, this.CC);
				if (this.TC.IsAliveInCurrentZone && this.TC.isChara)
				{
					if (EClass.rnd(2) == 0)
					{
						this.TC.Chara.AddCondition<ConDim>(50 + (int)Mathf.Sqrt((float)num11) * 10, false);
					}
					this.TC.Chara.AddCondition<ConParalyze>(EClass.rnd(2), true);
				}
			}
		}
		if (!this.CC.IsAliveInCurrentZone || !this.TC.IsAliveInCurrentZone)
		{
			return true;
		}
		if (this.TC.isChara && num3 > 0 && num3 * 2 + 15 > EClass.rnd(100) && !this.TC.isRestrained && this.TC.Chara.TryMoveFrom(this.CC.pos) == Card.MoveResult.Success)
		{
			this.TC.pos.PlayEffect("vanish");
			this.TC.PlaySound("push", 1.5f, true);
		}
		return true;
	}

	private bool Crit()
	{
		this.crit = true;
		return true;
	}

	private bool EvadePlus()
	{
		this.evadePlus = true;
		return false;
	}

	public bool CalcHit()
	{
		if (this.TC != null)
		{
			if (this.TC.HasCondition<ConDim>() && EClass.rnd(4) == 0)
			{
				return this.Crit();
			}
			if (this.TC.IsDeadOrSleeping)
			{
				return this.Crit();
			}
			int num = this.TC.Evalue(151);
			if (num != 0 && this.toHit < num * 10)
			{
				int num2 = this.evasion * 100 / Mathf.Clamp(this.toHit, 1, this.toHit);
				if (num2 > 300 && EClass.rnd(num + 250) > 100)
				{
					return this.EvadePlus();
				}
				if (num2 > 200 && EClass.rnd(num + 250) > 150)
				{
					return this.EvadePlus();
				}
				if (num2 > 150 && EClass.rnd(num + 250) > 200)
				{
					return this.EvadePlus();
				}
			}
			if (this.TC.Evalue(57) > EClass.rnd(100))
			{
				return this.EvadePlus();
			}
		}
		if (EClass.rnd(20) == 0)
		{
			return true;
		}
		if (EClass.rnd(20) == 0)
		{
			return false;
		}
		if (this.toHit < 1)
		{
			return false;
		}
		if (this.evasion < 1)
		{
			return true;
		}
		if (EClass.rnd(this.toHit) < EClass.rnd(this.evasion * (this.IsRanged ? 150 : 125) / 100))
		{
			return false;
		}
		if (EClass.rnd(5000) < this.CC.Evalue(73) + 50)
		{
			return this.Crit();
		}
		if ((float)this.CC.Evalue(90) + Mathf.Sqrt((float)this.CC.Evalue(134)) > (float)EClass.rnd(200))
		{
			return this.Crit();
		}
		if (this.CC.Evalue(1420) > 0)
		{
			int num3 = Mathf.Min(100, 100 - this.CC.hp * 100 / this.CC.MaxHP);
			if (num3 >= 50 && num3 * num3 * num3 * num3 / 3 > EClass.rnd(100000000))
			{
				return this.Crit();
			}
		}
		return true;
	}

	public string GetAttackText(AttackType type, int id)
	{
		return Lang.GetList("attack" + type.ToString())[id];
	}

	[CompilerGenerated]
	private void <Perform>g__Proc|44_0(List<Element> list)
	{
		if (list == null)
		{
			return;
		}
		foreach (Element element in list)
		{
			if (element is Ability)
			{
				int num = 10 + element.Value / 5;
				int power = EClass.curve((100 + element.Value * 10) * (100 + this.weaponSkill.Value) / 100, 400, 100, 75);
				if (num >= EClass.rnd(100))
				{
					Act act = element as Act;
					Card card = act.TargetType.CanSelectSelf ? this.CC : this.TC;
					string text = (element.source.proc.Length >= 2) ? element.source.proc[1] : "";
					string a = act.source.abilityType.TryGet(0, -1);
					if (!(a == "buff"))
					{
						if (a == "debuff" || a == "attack" || a == "dot")
						{
							card = this.TC;
						}
					}
					else
					{
						if (this.CC.HasCondition(text))
						{
							continue;
						}
						card = this.CC;
					}
					if (card.IsAliveInCurrentZone)
					{
						Card tc = this.TC;
						ActEffect.ProcAt(element.source.proc[0].ToEnum(true), power, BlessedState.Normal, this.CC, card, card.pos, false, new ActRef
						{
							n1 = text,
							aliasEle = element.source.aliasRef,
							noFriendlyFire = true
						});
						this.TC = tc;
					}
				}
			}
		}
	}

	[CompilerGenerated]
	private bool <Perform>g__IgnoreExp|44_1()
	{
		return this.CC.HasEditorTag(EditorTag.Invulnerable) || this.CC.HasEditorTag(EditorTag.InvulnerableToMobs) || this.TC.HasEditorTag(EditorTag.Invulnerable) || this.TC.HasEditorTag(EditorTag.InvulnerableToMobs);
	}

	[CompilerGenerated]
	private void <Perform>g__ModExpDef|44_2(int ele, int mod)
	{
		if (this.<Perform>g__IgnoreExp|44_1())
		{
			return;
		}
		if (this.CC.isCopy && EClass.rnd(10) != 0)
		{
			return;
		}
		int num = (Mathf.Clamp((this.CC.LV + 10 - this.TC.elements.ValueWithoutLink(ele)) / 2, 1, 10) + Mathf.Min(this.CC.LV / 10, 10)) * mod / 100;
		num = Mathf.Min(num, this.TC.isRestrained ? 10 : 200);
		if (this.TC == this.CC)
		{
			num /= 2;
		}
		if (!this.TC.IsPC && !this.TC.isRestrained && !this.TC.HasHost)
		{
			num *= 3;
		}
		if (num > 0)
		{
			this.TC.ModExp(ele, num + EClass.rnd(num / 2 + 1));
		}
	}

	[CompilerGenerated]
	private void <Perform>g__ModExpAtk|44_3(int ele, int mod)
	{
		if (this.<Perform>g__IgnoreExp|44_1())
		{
			return;
		}
		if (this.TC.isCopy && EClass.rnd(10) != 0)
		{
			return;
		}
		int num = (Mathf.Clamp((this.TC.LV + 10 - this.CC.elements.ValueWithoutLink(ele)) / 2, 1, 10) + Mathf.Min(this.TC.LV / 10, 10)) * mod / 100;
		num = Mathf.Min(num, 200);
		if (this.TC == this.CC)
		{
			num /= 2;
		}
		if (num > 0)
		{
			this.CC.ModExp(ele, num + EClass.rnd(num / 2 + 1));
		}
	}

	[CompilerGenerated]
	private void <Perform>g__AddBane|44_5(bool valid, int idEle, ref AttackProcess.<>c__DisplayClass44_0 A_3)
	{
		if (!valid)
		{
			return;
		}
		A_3.bane += this.CC.Evalue(idEle);
	}

	[CompilerGenerated]
	private void <Perform>g__PlayHitEffect|44_4()
	{
		string id = "hit_default";
		string id2 = "hit_default";
		switch (this.attackType)
		{
		case AttackType.Slash:
			id2 = "hit_slash";
			id = "hit_slash";
			break;
		case AttackType.Blunt:
		case AttackType.Punch:
		case AttackType.Kick:
		case AttackType.Bow:
		case AttackType.Gun:
		case AttackType.Cane:
			id2 = "hit_blunt";
			id = "hit_blunt";
			break;
		case AttackType.Claw:
		case AttackType.Bite:
			id2 = "hit_claw";
			id = "hit_claw";
			break;
		case AttackType.Spore:
			id2 = "hit_spore";
			id = "hit_spore";
			break;
		}
		if (this.TC != null)
		{
			this.TC.PlayEffect(id2, true, 0f, default(Vector3)).SetScale(this.crit ? 1.25f : 0.75f);
		}
		this.CC.PlaySound(id, 1f, true);
	}

	public static AttackProcess Current = new AttackProcess();

	public int dNum;

	public int dDim;

	public int dBonus;

	public int toHit;

	public int toHitBase;

	public int toHitFix;

	public int evasion;

	public int penetration;

	public int distMod;

	public int attackIndex;

	public int dNumAmmo;

	public int dDimAmmo;

	public int dBonusAmmo;

	public int numFire;

	public int numFireWithoutDamageLoss;

	public float dMulti;

	public bool crit;

	public bool hit;

	public bool evadePlus;

	public bool isThrow;

	public bool ignoreAnime;

	public bool ignoreAttackSound;

	public Chara CC;

	public Card TC;

	public Point TP;

	public Point posRangedAnime;

	public Element weaponSkill;

	public Thing weapon;

	public Thing ammo;

	public TraitToolRange toolRange;

	public AttackType attackType;

	public AttackStyle attackStyle;
}
