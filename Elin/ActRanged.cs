using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActRanged : ActThrow
{
	public bool IsReload
	{
		get
		{
			return Act.CC.IsPC && Act.TC == Act.CC && !Act.CC.HasCondition<ConReload>() && EClass.player.currentHotItem.Thing != null && !(EClass.player.currentHotItem.Thing.trait is TraitToolRangeCane);
		}
	}

	public override CursorInfo CursorIcon
	{
		get
		{
			return CursorSystem.IconRange;
		}
	}

	public override int PerformDistance
	{
		get
		{
			return 99;
		}
	}

	public override string Name
	{
		get
		{
			if (!this.IsReload)
			{
				return base.Name;
			}
			return "ActReload".lang();
		}
	}

	public override string GetText(string str = "")
	{
		if (!this.IsReload)
		{
			return base.GetText(str);
		}
		return this.Name;
	}

	public override bool CanPerform()
	{
		if (Act.CC.isRestrained)
		{
			return false;
		}
		if (Act.CC.HasCondition<ConReload>())
		{
			return false;
		}
		if (Act.TC == null)
		{
			return Act.CC.CanSeeLos(Act.TP, -1);
		}
		return Act.TC.IsAliveInCurrentZone && Act.CC.CanSeeLos(Act.TC.pos, -1);
	}

	public override bool Perform()
	{
		ActRanged.<>c__DisplayClass10_0 CS$<>8__locals1 = new ActRanged.<>c__DisplayClass10_0();
		CS$<>8__locals1.<>4__this = this;
		if (Act.TC == null)
		{
			Act.TC = Act.TP.FindAttackTarget();
		}
		if (Act.TC != null)
		{
			Act.TP.Set(Act.TC.pos);
		}
		if (Act.TC != null && Act.CC == EClass.pc && !(Act.CC.ai is AI_PracticeDummy) && (Act.TC.trait is TraitTrainingDummy || Act.TC.IsRestrainedResident))
		{
			Act.CC.SetAI(new AI_PracticeDummy
			{
				target = Act.TC,
				range = true
			});
			return true;
		}
		Act.CC.renderer.NextFrame();
		if (!Act.CC.IsPC)
		{
			Act.CC.TryEquipRanged();
		}
		CS$<>8__locals1.weapon = Act.CC.ranged;
		if (CS$<>8__locals1.weapon != null && Act.CC.IsPC && Act.CC.body.IsTooHeavyToEquip(CS$<>8__locals1.weapon))
		{
			Msg.Say("tooHeavyToEquip", CS$<>8__locals1.weapon, null, null, null);
			return false;
		}
		if (CS$<>8__locals1.weapon == null || !CS$<>8__locals1.weapon.CanAutoFire(Act.CC, Act.TC, false))
		{
			return false;
		}
		bool flag = CS$<>8__locals1.weapon.trait is TraitToolRangeGun;
		bool flag2 = CS$<>8__locals1.weapon.trait is TraitToolRangeCane;
		GameSetting.EffectData effectData;
		if ((effectData = EClass.setting.effect.guns.TryGetValue(CS$<>8__locals1.weapon.id, null)) == null)
		{
			effectData = EClass.setting.effect.guns[flag2 ? "cane" : (flag ? "gun" : "bow")];
		}
		GameSetting.EffectData effectData2 = effectData;
		CS$<>8__locals1.hasHit = false;
		CS$<>8__locals1.numFire = effectData2.num;
		CS$<>8__locals1.numFireWithoutDamageLoss = CS$<>8__locals1.numFire;
		int num = CS$<>8__locals1.weapon.Evalue(602);
		if (num > 0)
		{
			CS$<>8__locals1.numFire += num / 10 + ((num % 10 > EClass.rnd(10)) ? 1 : 0);
		}
		CS$<>8__locals1.numFire += Act.CC.Evalue(1652);
		int num2 = CS$<>8__locals1.numFire;
		int num3 = 1 + CS$<>8__locals1.weapon.material.hardness / 30 + EClass.rnd(3);
		CS$<>8__locals1.drill = CS$<>8__locals1.weapon.Evalue(606);
		CS$<>8__locals1.scatter = CS$<>8__locals1.weapon.Evalue(607);
		int num4 = CS$<>8__locals1.weapon.Evalue(604);
		if (num4 > 0)
		{
			for (int i = 0; i < CS$<>8__locals1.numFire; i++)
			{
				if (Mathf.Sqrt((float)num4) * 5f + 10f > (float)EClass.rnd(100))
				{
					num2--;
				}
			}
			num3 = Mathf.Max(1, num3 * 100 / (100 + num4 * 5));
		}
		CS$<>8__locals1.missSound = ((CS$<>8__locals1.weapon.trait is TraitToolRangeGun) ? "miss_bullet" : "miss_arrow");
		if (CS$<>8__locals1.weapon.trait is TraitToolRangeCane)
		{
			foreach (Element element in from e in CS$<>8__locals1.weapon.elements.dict.Values
			where e.source.categorySub == "eleAttack"
			select e)
			{
				num3 += element.source.LV / 15;
			}
			if (Act.CC.IsPC)
			{
				if (Act.CC.mana.value < num3)
				{
					if (!Act.CC.ai.IsNoGoal)
					{
						return false;
					}
					if (!Dialog.warned && !EClass.debug.godMode)
					{
						ActPlan.warning = true;
						Dialog.TryWarnMana(delegate
						{
							if (CS$<>8__locals1.<>4__this.Perform())
							{
								EClass.player.EndTurn(true);
							}
						});
						return false;
					}
				}
				EClass.ui.CloseLayers();
			}
		}
		else
		{
			if (this.IsReload)
			{
				return ActRanged.TryReload(CS$<>8__locals1.weapon, null);
			}
			if (CS$<>8__locals1.weapon.c_ammo <= 0)
			{
				if (!ActRanged.TryReload(CS$<>8__locals1.weapon, null))
				{
					if (Act.CC.IsPC)
					{
						EInput.Consume(false, 1);
					}
					return false;
				}
				if (Act.CC.HasCondition<ConReload>())
				{
					return true;
				}
			}
			if (Act.CC.HasCondition<ConFear>())
			{
				Act.CC.Say("fear", Act.CC, Act.TC, null, null);
				if (Act.CC.IsPC)
				{
					EInput.Consume(true, 1);
				}
				return true;
			}
		}
		Act.CC.LookAt(Act.TP);
		CS$<>8__locals1.index = 0;
		CS$<>8__locals1.orgTP = Act.TP.Copy();
		CS$<>8__locals1.points = new List<Point>();
		if (CS$<>8__locals1.drill > 0)
		{
			CS$<>8__locals1.points = EClass._map.ListPointsInLine(Act.CC.pos, Act.TP, CS$<>8__locals1.drill / 10 + ((CS$<>8__locals1.drill % 10 > EClass.rnd(10)) ? 1 : 0) + 1);
		}
		else if (CS$<>8__locals1.scatter > 0)
		{
			Act.TP.ForeachNeighbor(delegate(Point _p)
			{
				CS$<>8__locals1.points.Add(_p.Copy());
			}, true);
		}
		if (EClass.core.config.game.waitOnRange)
		{
			EClass.Wait(0.25f, Act.CC);
		}
		CS$<>8__locals1.<Perform>g__Shoot|1(Act.TC, Act.TP);
		if (CS$<>8__locals1.points.Count > 0)
		{
			Point obj = Act.TP.Copy();
			foreach (Point point in CS$<>8__locals1.points)
			{
				if (!point.Equals(obj))
				{
					Chara firstChara = point.FirstChara;
					if ((firstChara == null || firstChara.IsHostile(Act.CC)) && (firstChara != null || CS$<>8__locals1.scatter != 0))
					{
						CS$<>8__locals1.<Perform>g__Shoot|1(point.FirstChara, point);
					}
				}
			}
		}
		if (!(CS$<>8__locals1.weapon.trait is TraitToolRangeCane))
		{
			CS$<>8__locals1.weapon.c_ammo -= num2;
			if (CS$<>8__locals1.weapon.ammoData != null)
			{
				CS$<>8__locals1.weapon.ammoData.Num = CS$<>8__locals1.weapon.c_ammo;
			}
			if (CS$<>8__locals1.weapon.c_ammo <= 0)
			{
				CS$<>8__locals1.weapon.c_ammo = 0;
				CS$<>8__locals1.weapon.ammoData = null;
			}
		}
		if (Act.CC.IsPC)
		{
			LayerInventory.SetDirty(CS$<>8__locals1.weapon);
		}
		if (Act.TC != null && !CS$<>8__locals1.hasHit)
		{
			Act.CC.PlaySound(CS$<>8__locals1.missSound, 1f, true);
		}
		if (!Act.CC.isDead)
		{
			if (EClass.rnd(2) == 0)
			{
				Act.CC.RemoveCondition<ConInvisibility>();
			}
			if (CS$<>8__locals1.weapon.trait is TraitToolRangeCane)
			{
				Act.CC.mana.Mod(-num3 * CS$<>8__locals1.numFire);
			}
		}
		return true;
	}

	public static bool TryReload(Thing weapon, Thing ammo = null)
	{
		TraitToolRange traitToolRange = weapon.trait as TraitToolRange;
		if (Act.CC.IsPC)
		{
			LayerInventory.SetDirty(weapon);
		}
		if (weapon.ammoData != null)
		{
			if (weapon.ammoData.Num > 0)
			{
				Act.CC.Pick(weapon.ammoData, true, true);
			}
			weapon.ammoData = null;
		}
		if (ammo == null)
		{
			ammo = Act.CC.FindAmmo(weapon);
		}
		int num;
		if (ammo == null)
		{
			if (Act.CC.IsPC)
			{
				if (!weapon.IsMeleeWithAmmo)
				{
					Msg.Say("noAmmo", weapon, null, null, null);
				}
				return false;
			}
			num = traitToolRange.MaxAmmo;
		}
		else
		{
			num = Mathf.Min(ammo.Num, traitToolRange.MaxAmmo);
			Thing thing = ammo.Split(num);
			Act.CC.Say("takeAmmo", thing, null, null);
			if (thing.GetRootCard() == Act.CC)
			{
				thing.parent.RemoveCard(thing);
			}
			weapon.ammoData = thing;
		}
		weapon.c_ammo = num;
		int num2 = traitToolRange.ReloadTurn;
		num2 = num2 * 100 / (100 + Act.CC.Evalue(1652) * 100);
		if (traitToolRange.NeedReload && num2 > 0)
		{
			Act.CC.AddCondition<ConReload>(num2 * 10, false);
		}
		return true;
	}
}
