using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class ActMelee : ActBaseAttack
{
	public override int PerformDistance
	{
		get
		{
			if (Act.CC != null)
			{
				return Act.CC.body.GetMeleeDistance();
			}
			return 1;
		}
	}

	public override bool ResetAxis
	{
		get
		{
			return true;
		}
	}

	public bool HideHint(Card c)
	{
		return EClass.pc.isBlind || c == null || (c.isChara && !EClass.pc.CanSee(c));
	}

	public override string GetHintText(string str = "")
	{
		return "";
	}

	public override string GetTextSmall(Card c)
	{
		if (!this.HideHint(c))
		{
			return base.GetTextSmall(c);
		}
		return null;
	}

	public override CursorInfo GetCursorIcon(Card c)
	{
		if (!this.HideHint(c))
		{
			return CursorSystem.IconMelee;
		}
		return null;
	}

	public override bool ShowMouseHint(Card c)
	{
		return !this.HideHint(c);
	}

	public override bool CanPressRepeat
	{
		get
		{
			return true;
		}
	}

	public override bool CanPerform()
	{
		if (Act.TC == null || !Act.TC.IsAliveInCurrentZone)
		{
			return false;
		}
		if (Act.CC.Dist(Act.TC) > this.PerformDistance)
		{
			return false;
		}
		if (this.PerformDistance == 1)
		{
			if (!Act.CC.CanInteractTo(Act.TC))
			{
				return false;
			}
		}
		else if (!Act.CC.CanSeeLos(Act.TC, -1, false))
		{
			return false;
		}
		return base.CanPerform();
	}

	public override bool Perform()
	{
		return this.Attack(1f, false);
	}

	public bool Attack(float dmgMulti = 1f, bool maxRoll = false)
	{
		ActMelee.<>c__DisplayClass13_0 CS$<>8__locals1;
		CS$<>8__locals1.dmgMulti = dmgMulti;
		CS$<>8__locals1.maxRoll = maxRoll;
		Act.CC.combatCount = 10;
		Act.CC.LookAt(Act.TC);
		Act.CC.renderer.NextFrame();
		if (Act.CC.HasCondition<ConFear>())
		{
			Act.CC.Say("fear", Act.CC, Act.TC, null, null);
			if (Act.CC.IsPC)
			{
				EInput.Consume(true, 1);
			}
			return true;
		}
		Act.CC.renderer.PlayAnime(AnimeID.Attack, Act.TC);
		Card tc = Act.TC;
		if (tc != null)
		{
			Chara chara = tc.Chara;
			if (chara != null)
			{
				chara.RequestProtection(Act.CC, delegate(Chara c)
				{
					Act.TC = c;
				});
			}
		}
		CellEffect effect = Act.TP.cell.effect;
		if (effect != null && effect.id == 6 && EClass.rnd(2) == 0)
		{
			Act.CC.PlaySound("miss", 1f, true);
			Act.CC.Say("abMistOfDarkness_miss", Act.CC, null, null);
			return true;
		}
		CS$<>8__locals1.hasHit = false;
		bool flag = false;
		CS$<>8__locals1.usedTalisman = false;
		CS$<>8__locals1.count = 0;
		int num = Act.CC.Dist(Act.TC);
		foreach (BodySlot bodySlot in Act.CC.body.slots)
		{
			if (Act.TC == null || !Act.TC.IsAliveInCurrentZone)
			{
				return true;
			}
			if (bodySlot.thing != null && bodySlot.elementId == 35 && bodySlot.thing.source.offense.Length >= 2)
			{
				ActMelee.<>c__DisplayClass13_1 CS$<>8__locals2;
				CS$<>8__locals2.w = bodySlot.thing;
				if (num <= 1 || num <= CS$<>8__locals2.w.Evalue(666) + 1)
				{
					flag = true;
					if (CS$<>8__locals2.w.IsMeleeWithAmmo && Act.CC.IsPC && CS$<>8__locals2.w.c_ammo <= 0 && !Act.CC.HasCondition<ConReload>())
					{
						ActRanged.TryReload(CS$<>8__locals2.w, null);
					}
					int num2 = (CS$<>8__locals2.w == null) ? 0 : CS$<>8__locals2.w.Evalue(606);
					if (CS$<>8__locals2.w != null)
					{
						CS$<>8__locals2.w.Evalue(607);
					}
					List<Point> list = EClass._map.ListPointsInLine(Act.CC.pos, Act.TC.pos, num2 / 10 + ((num2 % 10 > EClass.rnd(10)) ? 1 : 0) + 1);
					ActMelee.<Attack>g__Attack|13_1(Act.TC, Act.TP, ref CS$<>8__locals1, ref CS$<>8__locals2);
					if (num2 > 0)
					{
						Point obj = Act.TC.pos.Copy();
						foreach (Point point in list)
						{
							if (!point.Equals(obj))
							{
								Chara firstChara = point.FirstChara;
								if (firstChara != null && firstChara.IsHostile(Act.CC))
								{
									ActMelee.<Attack>g__Attack|13_1(point.FirstChara, point, ref CS$<>8__locals1, ref CS$<>8__locals2);
								}
							}
						}
					}
					int count = CS$<>8__locals1.count;
					CS$<>8__locals1.count = count + 1;
				}
			}
		}
		if (!flag)
		{
			AttackProcess.Current.Prepare(Act.CC, null, Act.TC, Act.TP, 0, false);
			if (AttackProcess.Current.Perform(CS$<>8__locals1.count, CS$<>8__locals1.hasHit, CS$<>8__locals1.dmgMulti, CS$<>8__locals1.maxRoll))
			{
				CS$<>8__locals1.hasHit = true;
			}
			Act.CC.DoHostileAction(Act.TC, false);
		}
		if (EClass.core.config.game.waitOnMelee)
		{
			EClass.Wait(0.25f, Act.CC);
		}
		if (!CS$<>8__locals1.hasHit)
		{
			Act.CC.PlaySound("miss", 1f, true);
		}
		if (EClass.rnd(2) == 0)
		{
			Act.CC.RemoveCondition<ConInvisibility>();
		}
		return true;
	}

	[CompilerGenerated]
	internal static void <Attack>g__Attack|13_1(Card _tc, Point _tp, ref ActMelee.<>c__DisplayClass13_0 A_2, ref ActMelee.<>c__DisplayClass13_1 A_3)
	{
		Act.TC = _tc;
		Act.TP = _tp;
		AttackProcess.Current.Prepare(Act.CC, A_3.w, Act.TC, Act.TP, A_2.count, false);
		bool flag = AttackProcess.Current.Perform(A_2.count, A_2.hasHit, A_2.dmgMulti, A_2.maxRoll);
		if (flag)
		{
			A_2.hasHit = true;
		}
		if (A_3.w.c_ammo > 0 && !Act.CC.HasCondition<ConReload>())
		{
			bool flag2 = true;
			TraitAmmo traitAmmo = (A_3.w.ammoData == null) ? null : (A_3.w.ammoData.trait as TraitAmmo);
			if (traitAmmo != null)
			{
				TraitAmmoTalisman traitAmmoTalisman = traitAmmo as TraitAmmoTalisman;
				if (traitAmmoTalisman != null)
				{
					flag2 = false;
					if (flag && !A_2.usedTalisman && Act.TC != null && Act.TC.IsAliveInCurrentZone)
					{
						Element element = Act.CC.elements.GetElement(traitAmmoTalisman.owner.refVal);
						Act act = ((element != null) ? element.act : null) ?? ACT.Create(traitAmmoTalisman.owner.refVal);
						Act.powerMod = traitAmmo.owner.encLV;
						Card tc = Act.TC;
						if (act.Perform(Act.CC, Act.TC, Act.TP))
						{
							A_2.usedTalisman = true;
							flag2 = true;
							int spellExp = Act.CC.elements.GetSpellExp(Act.CC, act, 200);
							Act.CC.ModExp(act.id, spellExp);
						}
						Act.TC = tc;
						Act.powerMod = 100;
					}
				}
			}
			if (flag2)
			{
				Thing w = A_3.w;
				int c_ammo = w.c_ammo;
				w.c_ammo = c_ammo - 1;
				if (A_3.w.ammoData != null)
				{
					A_3.w.ammoData.Num = A_3.w.c_ammo;
				}
				if (A_3.w.c_ammo <= 0)
				{
					A_3.w.c_ammo = 0;
					A_3.w.ammoData = null;
				}
				LayerInventory.SetDirty(A_3.w);
			}
		}
		Act.CC.DoHostileAction(Act.TC, false);
	}
}
