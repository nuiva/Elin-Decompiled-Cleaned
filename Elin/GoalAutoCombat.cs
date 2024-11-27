using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GoalAutoCombat : GoalCombat
{
	public override Color GetActPlanColor()
	{
		if (EClass.pc.IsCriticallyWounded(false))
		{
			return EClass.Colors.colorActCriticalWarning;
		}
		if (!EClass.pc.party.IsCriticallyWounded(false))
		{
			return EClass.Colors.colorAct;
		}
		return EClass.Colors.colorActWarnning;
	}

	public override bool UseTurbo
	{
		get
		{
			return EClass.game.config.autoCombat.turbo;
		}
	}

	public override Thing RenderThing
	{
		get
		{
			return this.renderThing;
		}
	}

	public ConfigAutoCombat config
	{
		get
		{
			return EClass.game.config.autoCombat;
		}
	}

	public GoalAutoCombat(Chara e)
	{
		this.destEnemy = e;
		EClass.player.autoCombatStartHP = ((EClass.pc.Evalue(1421) > 0) ? (EClass.pc.hp + EClass.pc.mana.value) : EClass.pc.hp);
		foreach (Chara chara in EClass.pc.party.members)
		{
			if (!chara.IsCriticallyWounded(false))
			{
				this.listHealthy.Add(chara);
			}
		}
	}

	public override bool TryUseRanged(int dist)
	{
		Thing ranged = this.owner.ranged;
		this.owner.ranged = null;
		if (this.config.bUseHotBar)
		{
			this.<TryUseRanged>g__FindRanged|10_0(true);
		}
		if (this.owner.ranged == null || !ACT.Ranged.Perform(this.owner, this.tc, null))
		{
			return false;
		}
		if (this.owner == null)
		{
			return true;
		}
		this.renderThing = this.owner.ranged;
		if (this.owner.ranged != ranged)
		{
			this.owner.ranged = ranged;
		}
		return true;
	}

	public void GetAbilities(Func<Element, bool> func)
	{
		if (this.config.bUseHotBar)
		{
			foreach (Thing thing in from t in EClass.pc.things
			where t.IsHotItem && t.trait is TraitAbility
			select t)
			{
				Element element = this.owner.elements.GetElement(thing.c_idAbility);
				if (element != null && func(element))
				{
					base.AddAbility(element.act, 15, 100, false);
				}
			}
		}
		if (this.config.bUseInventory)
		{
			foreach (Element element2 in this.owner.elements.dict.Values)
			{
				if ((!this.config.bUseFav || EClass.player.favAbility.Contains(element2.id)) && func(element2))
				{
					base.AddAbility(element2.act, 0, 100, false);
				}
			}
		}
	}

	public override void BuildAbilityList()
	{
		this.GetAbilities((Element e) => e.source.abilityType.Length != 0);
		base.AddAbility(ACT.Ranged, 0, 100, false);
		base.AddAbility(ACT.Melee, 0, 100, false);
		base.AddAbility(ACT.Item, 0, 100, false);
	}

	public override bool TryAbortCombat()
	{
		if (this.idleCount >= 2)
		{
			Msg.Say("abort_idle");
			return true;
		}
		return false;
	}

	[CompilerGenerated]
	private void <TryUseRanged>g__FindRanged|10_0(bool hotbar)
	{
		this.owner.things.Foreach(delegate(Thing t)
		{
			if (t.IsHotItem)
			{
				if (!hotbar)
				{
					return false;
				}
			}
			else if (hotbar)
			{
				return false;
			}
			if (t.IsRangedWeapon && (t.trait is TraitToolRangeCane || t.c_ammo > 0 || EClass.pc.FindAmmo(t) != null) && this.owner.CanEquipRanged(t))
			{
				this.owner.ranged = t;
				return true;
			}
			return false;
		}, true);
	}

	public Thing renderThing;

	public List<Chara> listHealthy = new List<Chara>();
}
