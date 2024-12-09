using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttackProcess : EClass
{
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

	public bool IsMartial => weapon == null;

	public bool IsMartialWeapon
	{
		get
		{
			if (weapon != null)
			{
				return weapon.category.skill == 100;
			}
			return false;
		}
	}

	public bool IsRanged
	{
		get
		{
			if (toolRange != null && !isThrow)
			{
				return !toolRange.owner.Thing.isEquipped;
			}
			return false;
		}
	}

	public bool IsCane
	{
		get
		{
			if (IsRanged)
			{
				return toolRange is TraitToolRangeCane;
			}
			return false;
		}
	}

	public string GetText()
	{
		string text = dNum + "d" + dDim;
		text = text + ((dBonus >= 0) ? "+" : "") + dBonus;
		string @ref = (IsMartial ? "evalHand".lang() : "evalWeapon".lang((attackIndex + 1).ToString() ?? ""));
		return "attackEval".lang(@ref, text, dMulti.ToString("F2") ?? "", toHit.ToString() ?? "", penetration.ToString() ?? "");
	}

	public void Prepare(Chara _CC, Thing _weapon, Card _TC = null, Point _TP = null, int _attackIndex = 0, bool _isThrow = false)
	{
		CC = _CC;
		TC = _TC;
		TP = _TP;
		isThrow = _isThrow;
		weapon = _weapon;
		ammo = _weapon?.ammoData;
		hit = (crit = (evadePlus = false));
		toolRange = weapon?.trait as TraitToolRange;
		attackType = AttackType.Slash;
		attackStyle = AttackStyle.Default;
		evasion = 0;
		penetration = 0;
		distMod = 100;
		attackIndex = _attackIndex;
		posRangedAnime = TP;
		ignoreAnime = (ignoreAttackSound = false);
		if (!isThrow)
		{
			if (!IsRanged)
			{
				attackStyle = CC.body.GetAttackStyle();
			}
			else if (TP != null)
			{
				int num = CC.pos.Distance(TP);
				distMod = Mathf.Max(115 - 10 * Mathf.Abs(num - toolRange.BestDist) * 100 / (100 + weapon.Evalue(605 /* mod_precision */) * 10), 80);
			}
		}
		if (isThrow)
		{
			bool isThrowWeapon = weapon.HasTag(CTAG.throwWeapon) || weapon.HasTag(CTAG.throwWeaponEnemy);
			int damageWeightMultiplier = (int)Mathf.Clamp(Mathf.Sqrt(weapon.SelfWeight + weapon.ChildrenWeight) * 3f + 25f + (float)(isThrowWeapon ? 75 : 0), 10f, 400f + Mathf.Sqrt(CC.STR) * 50f);
			int damageHardnessMultiplier = Mathf.Clamp(weapon.material.hardness, isThrowWeapon ? 40 : 20, 200);
			weaponSkill = CC.elements.GetOrCreateElement(108 /* throwing */);
			attackType = AttackType.Blunt;
			dBonus = CC.DMG + (CC.IsPCParty ? 3 : 7);
			dNum = 2;
			dDim = (((!CC.IsPCParty) ? CC.LV : 0) + CC.STR + CC.Evalue(108 /* throwing */)) * damageWeightMultiplier * damageHardnessMultiplier / 10000 / 2;
			dMulti = 1f;
			toHitBase = EClass.curve(CC.DEX / 4 + CC.STR / 2 + weaponSkill.Value, 50, 25) + (CC.IsPCFaction ? 75 : 250);
			toHitFix = CC.HIT + weapon.HIT;
			penetration = 25;
		}
		else if (IsMartial || IsMartialWeapon)
		{
			weaponSkill = CC.elements.GetOrCreateElement(100 /* martial */);
			bool isForceWeapon = weapon != null && weapon.Evalue(482 /* force_weapon */) > 0;
			if (isForceWeapon)
			{
				weaponSkill = CC.elements.GetOrCreateElement(305 /* magicDevice */);
			}
			attackType = ((!CC.race.meleeStyle.IsEmpty()) ? CC.race.meleeStyle.ToEnum<AttackType>() : ((EClass.rnd(2) == 0) ? AttackType.Kick : AttackType.Punch));
			dBonus = CC.DMG + CC.encLV + (int)Mathf.Sqrt(Mathf.Max(0, CC.STR / 5 + weaponSkill.Value / 4));
			dNum = 2 + Mathf.Min(weaponSkill.Value / 10, 4);
			dDim = 5 + (int)Mathf.Sqrt(Mathf.Max(0, weaponSkill.Value / 3));
			dMulti = 0.6f + (float)(CC.STR / 2 + weaponSkill.Value / 2 + CC.Evalue(isForceWeapon ? 304 /* casting */ : 132 /* tactics */) / 2) / 50f;
			dMulti += 0.05f * (float)CC.Evalue(1400 /* featWarrior */);
			toHitBase = EClass.curve(CC.DEX / 3 + CC.STR / 3 + weaponSkill.Value, 50, 25) + 50;
			toHitFix = CC.HIT;
			if (attackStyle == AttackStyle.Shield)
			{
				toHitBase = toHitBase * 75 / 100;
			}
			penetration = Mathf.Clamp(weaponSkill.Value / 10 + 5, 5, 20) + CC.Evalue(92 /* penetration */);
			if (IsMartialWeapon)
			{
				dBonus += weapon.DMG;
				dNum += weapon.source.offense[0];
				dDim = Mathf.Max(dDim / 2 + weapon.c_diceDim, 1);
				toHitFix += weapon.HIT;
				penetration += weapon.Penetration;
				if (!weapon.source.attackType.IsEmpty())
				{
					attackType = weapon.source.attackType.ToEnum<AttackType>();
				}
			}
		}
		else
		{
			if (IsRanged)
			{
				weaponSkill = CC.elements.GetOrCreateElement(toolRange.WeaponSkill);
			}
			else
			{
				weaponSkill = CC.elements.GetOrCreateElement(weapon.category.skill);
			}
			if (!weapon.source.attackType.IsEmpty())
			{
				attackType = weapon.source.attackType.ToEnum<AttackType>();
			}
			bool isCaneOrForceWeapon = IsCane || weapon.Evalue(482 /* force_weapon */) > 0;
			if (isCaneOrForceWeapon)
			{
				weaponSkill = CC.elements.GetOrCreateElement(305 /* magicDevice */);
			}
			dBonus = CC.DMG + CC.encLV + weapon.DMG;
			dNum = weapon.source.offense[0];
			dDim = weapon.c_diceDim;
			dMulti = 0.6f + (float)(weaponSkill.GetParent(CC).Value + weaponSkill.Value / 2 + CC.Evalue(isCaneOrForceWeapon ? 304 /* casting */ : (IsRanged ? 133 /* marksman */ : 132 /* tactics */))) / 50f;
			dMulti += 0.05f * (float)CC.Evalue(IsRanged ? 1404 /* featArcher */ : 1400 /* featWarrior */);
			toHitBase = EClass.curve((IsCane ? CC.WIL : CC.DEX) / 4 + weaponSkill.GetParent(CC).Value / 3 + weaponSkill.Value, 50, 25) + 50;
			toHitFix = CC.HIT + weapon.HIT;
			penetration = weapon.Penetration + CC.Evalue(92 /* penetration */);
			if (IsCane)
			{
				toHitBase += 50;
			}
		}
		if (ammo != null && !(ammo.trait is TraitAmmoTalisman))
		{
			dNumAmmo = ((ammo.source.offense.Length != 0) ? ammo.source.offense[0] : 0);
			dDimAmmo = ammo.c_diceDim;
			dBonusAmmo = ammo.DMG + ammo.encLV;
			if (dNumAmmo < 1)
			{
				dNumAmmo = 1;
			}
			if (dDimAmmo < 1)
			{
				dDimAmmo = 1;
			}
			toHitFix += ammo.HIT;
		}
		else
		{
			dNumAmmo = 0;
			dDimAmmo = 0;
		}
		if (dNum < 1)
		{
			dNum = 1;
		}
		if (dDim < 1)
		{
			dDim = 1;
		}
		if (penetration > 100)
		{
			penetration = 100;
		}
		if (attackStyle == AttackStyle.TwoHand)
		{
			dMulti = dMulti * 1.5f + 0.1f * Mathf.Sqrt(Mathf.Max(0, CC.Evalue(130)));
		}
		dMulti = dMulti * (float)distMod / 100f;
		toHit = toHitBase + toHitFix;
		toHit = toHit * distMod / 100;
		if (CC.HasCondition<ConBane>())
		{
			toHit = toHit * 75 / 100;
		}
		if (TC != null && CC.HasHigherGround(TC))
		{
			toHit = toHit * 120 / 100;
		}
		if (CC.ride != null)
		{
			toHit = toHit * 100 / (100 + 500 / Mathf.Max(5, 10 + CC.EvalueRiding()));
		}
		if (CC.parasite != null)
		{
			toHit = toHit * 100 / (100 + 1000 / Mathf.Max(5, 10 + CC.Evalue(227)));
		}
		if (CC.host != null)
		{
			if (CC.host.ride == CC)
			{
				toHit = toHit * 100 / (100 + 1000 / Mathf.Max(5, 10 + CC.STR));
			}
			if (CC.host.parasite == CC)
			{
				toHit = toHit * 100 / (100 + 2000 / Mathf.Max(5, 10 + CC.DEX));
			}
		}
		if (attackStyle == AttackStyle.TwoHand)
		{
			toHit += 25 + (int)Mathf.Sqrt(Mathf.Max(0, CC.Evalue(130)) * 2);
		}
		else if (attackStyle == AttackStyle.TwoWield && toHit > 0)
		{
			toHit = toHit * 100 / (115 + attackIndex * 15 + attackIndex * Mathf.Clamp(2000 / (20 + CC.Evalue(131)), 0, 100));
		}
		if (CC.isBlind)
		{
			toHit /= ((IsRanged || isThrow) ? 10 : 3);
		}
		if (CC.isConfused || CC.HasCondition<ConDim>())
		{
			toHit /= 2;
		}
		if (TC != null)
		{
			evasion = EClass.curve(TC.PER / 3 + TC.Evalue(150 /* evasion */), 50, 10) + TC.DV + 25;
			if (TC.isChara && TC.Chara.isBlind)
			{
				evasion /= 2;
			}
			if (TC.HasCondition<ConDim>())
			{
				evasion /= 2;
			}
			if (TC.isChara && TC.Chara.HasHigherGround(CC))
			{
				evasion = evasion * 120 / 100;
			}
		}
	}

	public void PlayRangedAnime(int numFire)
	{
		bool isGun = toolRange is TraitToolRangeGun;
		bool isCane = toolRange is TraitToolRangeCane;
		GameSetting.EffectData data = EClass.setting.effect.guns.TryGetValue(weapon.id) ?? EClass.setting.effect.guns[isCane ? "cane" : (isGun ? "gun" : "bow")];
		bool isPCC = CC.IsPCC && CC.renderer.hasActor;
		Vector2 firePos = (isPCC ? new Vector2(data.firePos.x * (float)((CC.renderer.actor.currentDir != 0 && CC.renderer.actor.currentDir != 1) ? 1 : (-1)), data.firePos.y) : Vector2.zero);
		Chara _CC = CC;
		Point _TP = posRangedAnime.Copy();
		Thing _weapon = weapon;
		bool ignoreSound = ignoreAttackSound;
		for (int i = 0; i < numFire; i++)
		{
			TweenUtil.Delay((float)i * data.delay, delegate
			{
				if (EClass.core.IsGameStarted && _CC.IsAliveInCurrentZone)
				{
					if (_weapon.id == "gun_rail")
					{
						_CC.PlayEffect("laser").GetComponent<SpriteBasedLaser>().Play(_TP.PositionCenter());
					}
					else
					{
						Effect effect = Effect.Get("ranged_arrow")._Play(_CC.pos, _CC.isSynced ? _CC.renderer.position : _CC.pos.Position(), 0f, _TP, data.sprite);
						if (isCane)
						{
							IEnumerable<Element> enumerable = toolRange.owner.elements.dict.Values.Where((Element e) => e.source.categorySub == "eleAttack");
							if (enumerable.Count() > 0)
							{
								Element element = enumerable.RandomItem();
								effect.sr.color = EClass.Colors.elementColors[element.source.alias];
							}
						}
					}
					if (data.eject)
					{
						if (!ignoreSound)
						{
							_CC.PlaySound("bullet_drop");
						}
						_CC.PlayEffect("bullet").Emit(1);
					}
					if (isGun)
					{
						if (isPCC)
						{
							_weapon.PlayEffect(data.idEffect.IsEmpty("gunfire"), useRenderPos: true, 0f, firePos);
						}
						else
						{
							_CC.PlayEffect(data.idEffect.IsEmpty("gunfire"));
						}
					}
					if (!ignoreSound)
					{
						_CC.PlaySound(data.idSound.IsEmpty("attack_gun"));
					}
				}
			});
		}
	}

	public bool Perform(int count, bool hasHit, float dmgMulti = 1f, bool maxRoll = false)
	{
		bool isSourceOutOfAmmo = CC.HasCondition<ConReload>();
		hit = CalcHit();
		int damageAmount = Dice.Roll(dNum, dDim, dBonus, CC);
		if (ammo != null && !isSourceOutOfAmmo)
		{
			damageAmount += Dice.Roll(dNumAmmo, dDimAmmo, dBonusAmmo, CC);
		}
		if (crit || maxRoll)
		{
			damageAmount = Dice.RollMax(dNum, dDim, dBonus);
			if (ammo != null && !isSourceOutOfAmmo)
			{
				damageAmount += Dice.RollMax(dNumAmmo, dDimAmmo);
			}
			if (crit && (IsMartial || IsMartialWeapon))
			{
				dMulti *= 1.25f;
			}
		}
		if (CC.Evalue(1355 /* featGod_strife1 */) > 0) // Favor of the Strife: Physical attacks enhanced with each enemy defeated
		{
			ConStrife condition = CC.GetCondition<ConStrife>();
			damageAmount = ((condition == null) ? (damageAmount + 1) : (damageAmount + condition.GetDice().Roll()));
		}
		damageAmount = Mathf.Clamp(damageAmount, 0, 9999999);
		damageAmount = (int)(dMulti * (float)damageAmount * dmgMulti);
		damageAmount = Mathf.Clamp(damageAmount, 0, 9999999);
		if (IsRanged && count >= numFireWithoutDamageLoss)
		{
			damageAmount = damageAmount * 100 / (100 + (count - numFireWithoutDamageLoss + 1) * 30);
		}
		if (CC.isRestrained)
		{
			damageAmount /= 2;
		}
		List<Element> attackProcs = new List<Element>();
		int sourceVorpalSkill = CC.Evalue(91 /* vopal */);
		int weaponModAmmoKnockback = 0;
		if (weapon != null)
		{
			attackProcs = weapon.elements.dict.Values.ToList();
			if (ammo != null && !isSourceOutOfAmmo)
			{
				attackProcs = attackProcs.Concat(ammo.elements.dict.Values).ToList();
			}
			if (IsRanged || isThrow)
			{
				sourceVorpalSkill += weapon.Evalue(91 /* vopal */);
			}
			weaponModAmmoKnockback += weapon.Evalue(603 /* mod_ammo_knockback */);
		}
		else if (CC.id == "rabbit_vopal")
		{
			attackProcs.Add(Element.Create(6650 /* ActNeckHunt */, 100));
		}
		int bane;
		if (TC?.Chara != null)
		{
			SourceRace.Row targetRace = TC.Chara.race;
			bane = CC.Evalue(468);
			AddBane(targetRace.IsUndead, 461 /* bane_undead */);
			AddBane(targetRace.IsAnimal, 463 /* bane_animal */);
			AddBane(targetRace.IsHuman, 464 /* bane_man */);
			AddBane(targetRace.IsDragon, 460 /* bane_dragon */);
			AddBane(targetRace.IsGod, 466 /* bane_god */);
			AddBane(targetRace.IsMachine, 465 /* bane_machine */);
			AddBane(targetRace.IsFish, 467 /* bane_fish */);
			AddBane(targetRace.IsFairy, 462 /* bane_fairy */);
			if (bane != 0)
			{
				damageAmount = damageAmount * (100 + bane * 3) / 100;
			}
		}
		if (CC.IsPCFaction)
		{
			foreach (Element value in EClass.pc.faction.charaElements.dict.Values)
			{
				attackProcs.Add(value);
			}
		}
		if (hit && sourceVorpalSkill > EClass.rnd(100))
		{
			CC.Say("vopal");
			penetration = 100;
		}
		if (crit && CC.IsPC)
		{
			CC.Say("critical");
			CC.PlaySound("critical");
		}
		if (CC.isSynced || (TC != null && TC.isSynced))
		{
			if (toolRange != null && (!IsRanged || count == 0) && !isSourceOutOfAmmo && !ignoreAnime)
			{
				PlayRangedAnime((!IsRanged) ? 1 : numFire);
			}
			if (hit && TC != null && !hasHit)
			{
				PlayHitEffect();
			}
		}
		if (TC == null)
		{
			CC.Say(IsRanged ? "attack_air_range" : "attack_air", CC);
			return true;
		}
		if (!hit)
		{
			if (TC != null)
			{
				if (CC.IsPCParty)
				{
					CC.Say(evadePlus ? "evadePlus2" : "evade2", CC, TC);
				}
				else
				{
					TC.Say(evadePlus ? "evadePlus" : "evade", TC, CC);
				}
				ModExpDef(150, 90);
				ModExpDef(151, 90);
			}
			Proc(attackProcs);
			return false;
		}
		if (TC.IsPC)
		{
			Msg.SetColor("attack_pc");
			EClass.pc.Say("attackMeleeToPC", CC, TC, GetAttackText(attackType, 3));
		}
		else
		{
			CC.Say("attackMelee", CC, TC, GetAttackText(attackType, 0));
		}
		bool showEffect = true;
		int attackElementalDomain = 0;
		int attackElementalDomainPower = 0;
		int attackConvertedElementalDamagePercent = 0;
		ConWeapon conWeapon = null;
		if (weapon != null)
		{
			foreach (Element attackProc in weapon.elements.dict.Values)
			{
				if (attackProc.source.categorySub == "eleConvert")
				{
					attackElementalDomain = EClass.sources.elements.alias[attackProc.source.aliasRef].id;
					attackElementalDomainPower = 50 + attackProc.Value * 2;
					attackConvertedElementalDamagePercent = Mathf.Min(attackProc.Value, 100);
					break;
				}
			}
		}
		if (attackElementalDomain == 0)
		{
			if (CC.HasCondition<ConWeapon>())
			{
				conWeapon = CC.GetCondition<ConWeapon>();
				attackElementalDomain = conWeapon.sourceElement.id;
				attackElementalDomainPower = conWeapon.power / 2;
				attackConvertedElementalDamagePercent = 40 + (int)Mathf.Min(MathF.Sqrt(conWeapon.power), 80f);
			}
			if (conWeapon == null && weapon == null && (CC.MainElement != Element.Void || CC.HasElement(1565)))
			{
				attackElementalDomain = (CC.HasElement(1565 /* etherPoisonHand */) ? 915 /* elePoison */ : CC.MainElement.id);
				attackElementalDomainPower = CC.Power / 3 + EClass.rnd(CC.Power / 2);
				if (CC.MainElement != Element.Void)
				{
					attackElementalDomainPower += CC.MainElement.Value;
				}
				showEffect = false;
				attackConvertedElementalDamagePercent = 50;
			}
			if (conWeapon == null && weapon != null && weapon.trait is TraitToolRangeCane)
			{
				IEnumerable<Element> enumerable = weapon.elements.dict.Values.Where((Element e) => e.source.categorySub == "eleAttack");
				if (enumerable.Count() > 0)
				{
					attackElementalDomain = enumerable.RandomItem().id;
					attackElementalDomainPower = attackElementalDomain switch
					{
						920 /* eleChaos */ => 30, 
						914 /* eleMind */ => 50, 
						918 /* eleNerve */ => 50, 
						_ => 100, 
					};
				}
				attackConvertedElementalDamagePercent = 50;
			}
		}
		int damageAmountUnmitigated = damageAmount;
		int damageAmountElemental = damageAmount * attackConvertedElementalDamagePercent / 100;
		damageAmount -= damageAmountElemental;
		int damageAmountArmorPenetrating = damageAmount * penetration / 100;
		damageAmount -= damageAmountArmorPenetrating;
		damageAmount = TC.ApplyProtection(damageAmount) + damageAmountArmorPenetrating + damageAmountElemental;
		TC.DamageHP(damageAmount, attackElementalDomain, attackElementalDomainPower, (!IsRanged && !isThrow) ? AttackSource.Melee : AttackSource.Range, CC, showEffect);
		conWeapon?.Mod(-1);
		bool isCaneOrForceWeapon = IsCane || (weapon != null && weapon.Evalue(482 /* force_weapon */) > 0);
		int attackStyleElement = CC.body.GetAttackStyleElement(attackStyle); // 130 twohand or 131 twowield or 0 _void
		int sourceSkillExpGain = 100 / (count + 1);
		if (!IsRanged || count == 0)
		{
			ModExpAtk(weaponSkill.id, sourceSkillExpGain);
			ModExpAtk(isCaneOrForceWeapon ? 304 /* casting */ : (IsRanged ? 133 /* marksman */ : 132 /* tactics */), sourceSkillExpGain);
		}
		if (crit)
		{
			ModExpAtk(134 /* eyeofmind */, 50);
		}
		if (count == 0 && attackStyleElement != 0) // Grant 2H or DW exp
		{
			ModExpAtk(attackStyleElement, 100);
		}
		if (!CC.IsAliveInCurrentZone || !TC.IsAliveInCurrentZone)
		{
			return true;
		}
		if (EClass.rnd(8) == 0 && TC.isChara && CC.HasElement(1219 /* featElderCrab */)) // Crab Claw Grip: Restrain the target with scissors
		{
			CC.Say("abCrab", CC, TC);
			TC.Chara.AddCondition<ConParalyze>(30 + EClass.rnd(30));
			TC.Chara.AddCondition<ConGravity>();
		}
		if (attackProcs.Count > 0)
		{
			foreach (Element attackProc in attackProcs)
			{
				if (!TC.IsAliveInCurrentZone)
				{
					break;
				}
				if (attackProc.source.categorySub == "eleAttack")
				{
					int procChance = 25;
					int dmg = EClass.rnd(damageAmount * (100 + attackProc.Value * 10) / 500 + 5);
					if (conWeapon == null && weapon != null && weapon.trait is TraitToolRangeCane)
					{
						procChance = 0;
					}
					if (procChance >= EClass.rnd(100))
					{
						TC.DamageHP(dmg, attackProc.id, isThrow ? (100 + attackProc.Value * 5) : (30 + attackProc.Value), AttackSource.WeaponEnchant, CC);
					}
				}
			}
			Proc(attackProcs);
		}
		if (!CC.IsAliveInCurrentZone || !TC.IsAliveInCurrentZone)
		{
			return true;
		}
		if (!IsRanged && attackStyle == AttackStyle.Shield) // Shield bash
		{
			int sourceShieldSkill = CC.Evalue(123 /* shield */);
			if (CC.elements.ValueWithoutLink(123 /* shield */) >= 10 && Mathf.Clamp(Mathf.Sqrt(sourceShieldSkill) - 2f, 8f, 12f) > (float)EClass.rnd(100))
			{
				damageAmount = damageAmountUnmitigated * Mathf.Min(50 + sourceShieldSkill, 200) / 100;
				damageAmount = TC.ApplyProtection(damageAmount);
				Debug.Log("Bash:" + damageAmount + "/" + damageAmountUnmitigated);
				CC.PlaySound("shield_bash");
				CC.Say("shield_bash", CC, TC);
				TC.DamageHP(damageAmount, AttackSource.None, CC);
				if (TC.IsAliveInCurrentZone && TC.isChara)
				{
					if (EClass.rnd(2) == 0)
					{
						TC.Chara.AddCondition<ConDim>(50 + (int)Mathf.Sqrt(sourceShieldSkill) * 10);
					}
					TC.Chara.AddCondition<ConParalyze>(EClass.rnd(2), force: true);
				}
			}
		}
		if (!CC.IsAliveInCurrentZone || !TC.IsAliveInCurrentZone)
		{
			return true;
		}
		if (TC.isChara && weaponModAmmoKnockback > 0 && weaponModAmmoKnockback * 2 + 15 > EClass.rnd(100) && !TC.isRestrained && TC.Chara.TryMoveFrom(CC.pos) == Card.MoveResult.Success)
		{
			TC.pos.PlayEffect("vanish");
			TC.PlaySound("push", 1.5f);
		}
		return true;
		void AddBane(bool valid, int idEle)
		{
			if (valid)
			{
				bane += CC.Evalue(idEle);
			}
		}
		bool IgnoreExp()
		{
			if (!CC.HasEditorTag(EditorTag.Invulnerable) && !CC.HasEditorTag(EditorTag.InvulnerableToMobs) && !TC.HasEditorTag(EditorTag.Invulnerable))
			{
				return TC.HasEditorTag(EditorTag.InvulnerableToMobs);
			}
			return true;
		}
		void ModExpAtk(int ele, int mod)
		{
			if (!IgnoreExp() && (!TC.isCopy || EClass.rnd(10) == 0))
			{
				int a2 = (Mathf.Clamp((TC.LV + 10 - CC.elements.ValueWithoutLink(ele)) / 2, 1, 10) + Mathf.Min(TC.LV / 10, 10)) * mod / 100;
				a2 = Mathf.Min(a2, 200);
				if (TC == CC)
				{
					a2 /= 2;
				}
				if (a2 > 0)
				{
					CC.ModExp(ele, a2 + EClass.rnd(a2 / 2 + 1));
				}
			}
		}
		void ModExpDef(int ele, int mod)
		{
			if (!IgnoreExp() && (!CC.isCopy || EClass.rnd(10) == 0))
			{
				int a = (Mathf.Clamp((CC.LV + 10 - TC.elements.ValueWithoutLink(ele)) / 2, 1, 10) + Mathf.Min(CC.LV / 10, 10)) * mod / 100;
				a = Mathf.Min(a, TC.isRestrained ? 10 : 200);
				if (TC == CC)
				{
					a /= 2;
				}
				if (!TC.IsPC && !TC.isRestrained && !TC.HasHost)
				{
					a *= 3;
				}
				if (a > 0)
				{
					TC.ModExp(ele, a + EClass.rnd(a / 2 + 1));
				}
			}
		}
		void PlayHitEffect()
		{
			string id = "hit_default";
			string id2 = "hit_default";
			switch (attackType)
			{
			case AttackType.Slash:
				id2 = "hit_slash";
				id = "hit_slash";
				break;
			case AttackType.Spore:
				id2 = "hit_spore";
				id = "hit_spore";
				break;
			case AttackType.Claw:
			case AttackType.Bite:
				id2 = "hit_claw";
				id = "hit_claw";
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
			}
			if (TC != null)
			{
				TC.PlayEffect(id2).SetScale(crit ? 1.25f : 0.75f);
			}
			CC.PlaySound(id);
		}
		void Proc(List<Element> attackProcs)
		{
			if (attackProcs == null)
			{
				return;
			}
			foreach (Element attackProc in attackProcs)
			{
				if (attackProc is Ability)
				{
					int procChance = 10 + attackProc.Value / 5;
					int power = EClass.curve((100 + attackProc.Value * 10) * (100 + weaponSkill.Value) / 100, 400, 100);
					if (procChance >= EClass.rnd(100))
					{
						Act obj = attackProc as Act;
						Card card = (obj.TargetType.CanSelectSelf ? CC : TC);
						string text = ((attackProc.source.proc.Length >= 2) ? attackProc.source.proc[1] : "");
						switch (obj.source.abilityType.TryGet(0))
						{
						case "buff":
							if (CC.HasCondition(text))
							{
								continue;
							}
							card = CC;
							break;
						case "debuff":
						case "attack":
						case "dot":
							card = TC;
							break;
						}
						if (card.IsAliveInCurrentZone)
						{
							Card tC = TC;
							ActEffect.ProcAt(attackProc.source.proc[0].ToEnum<EffectId>(), power, BlessedState.Normal, CC, card, card.pos, isNeg: false, new ActRef
							{
								n1 = text,
								aliasEle = attackProc.source.aliasRef,
								noFriendlyFire = true
							});
							TC = tC;
						}
					}
				}
			}
		}
	}

	private bool Crit()
	{
		crit = true;
		return true;
	}

	private bool EvadePlus()
	{
		evadePlus = true;
		return false;
	}

	public bool CalcHit()
	{
		if (TC != null)
		{
			if (TC.HasCondition<ConDim>() && EClass.rnd(4) == 0)
			{
				return Crit();
			}
			if (TC.IsDeadOrSleeping)
			{
				return Crit();
			}
			int targetGreaterEvasion = TC.Evalue(151 /* evasionPlus */);
			if (targetGreaterEvasion != 0 && toHit < targetGreaterEvasion * 10)
			{
				int evasionCoefficient = evasion * 100 / Mathf.Clamp(toHit, 1, toHit);
				if (evasionCoefficient > 300 && EClass.rnd(targetGreaterEvasion + 250) > 100)
				{
					return EvadePlus();
				}
				if (evasionCoefficient > 200 && EClass.rnd(targetGreaterEvasion + 250) > 150)
				{
					return EvadePlus();
				}
				if (evasionCoefficient > 150 && EClass.rnd(targetGreaterEvasion + 250) > 200)
				{
					return EvadePlus();
				}
			}
			if (TC.Evalue(57 /* evasionPerfect */) > EClass.rnd(100))
			{
				return EvadePlus();
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
		if (toHit < 1)
		{
			return false;
		}
		if (evasion < 1)
		{
			return true;
		}
		if (EClass.rnd(toHit) < EClass.rnd(evasion * (IsRanged ? 150 : 125) / 100))
		{
			return false;
		}
		if (EClass.rnd(5000) < CC.Evalue(73 /* PER */) + 50)
		{
			return Crit();
		}
		if ((float)CC.Evalue(90 /* critical */) + Mathf.Sqrt(CC.Evalue(134 /* eyeofmind */)) > (float)EClass.rnd(200))
		{
			return Crit();
		}
		if (CC.Evalue(1420 /* featExecutioner */) > 0)
		{
			int sourceMissingHealthPercent = Mathf.Min(100, 100 - CC.hp * 100 / CC.MaxHP);
			if (sourceMissingHealthPercent >= 50 && sourceMissingHealthPercent * sourceMissingHealthPercent * sourceMissingHealthPercent * sourceMissingHealthPercent / 3 > EClass.rnd(1_00_00_00_00)) // In other words, sourceMissingHealthPercent**4 / 3 > rnd(100**4) -> at 0% health gives 33% crit chance
			{
				return Crit();
			}
		}
		return true;
	}

	public string GetAttackText(AttackType type, int id)
	{
		return Lang.GetList("attack" + type)[id];
	}
}
