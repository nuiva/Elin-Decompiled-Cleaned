using UnityEngine;

public class TraitTrap : TraitFloorSwitch
{
	public override byte WeightMod
	{
		get
		{
			if (!owner.isHidden)
			{
				return 10;
			}
			return 0;
		}
	}

	public virtual int DestroyChanceOnActivateTrap => 50;

	public override bool CanBeHeld => EClass._zone.IsPCFaction;

	public override bool StartHidden => true;

	public override bool CanDisarmTrap => !IsJammed;

	public override bool IsNegativeEffect => true;

	public override bool IgnoreWhenLevitating()
	{
		if (owner.sourceCard.vals.Length == 0)
		{
			return false;
		}
		switch (owner.sourceCard.vals[0])
		{
		case "spear":
		case "paralysis":
		case "acid":
		case "blind":
		case "sleep":
		case "mine":
		case "banana":
			return true;
		default:
			return false;
		}
	}

	public override void OnActivateTrap(Chara c)
	{
		if (owner.sourceCard.vals == null || owner.sourceCard.vals.Length == 0)
		{
			Debug.Log(owner.id);
			return;
		}
		string text = owner.sourceCard.vals[0];
		c.PlaySound("trap");
		c.Say("trap", c, owner);
		if (text == "mine")
		{
			Msg.Say("trap_mine", c);
		}
		else
		{
			c.Say("trap_" + text, c);
		}
		switch (text)
		{
		case "sister":
			foreach (Chara chara in EClass._map.charas)
			{
				chara.RemoveCondition<ConSuspend>();
			}
			break;
		case "acid":
			c.PlayEffect("Element/eleAcid");
			ActEffect.Proc(EffectId.Acid, c);
			break;
		case "teleport":
			ActEffect.Proc(EffectId.Teleport, c);
			break;
		case "curse":
			ActEffect.Proc(EffectId.CurseEQ, c);
			break;
		case "sleep":
			ActEffect.Proc(EffectId.Sleep, c);
			break;
		case "spear":
			if (c.IsLevitating)
			{
				c.Say("trap_spear_nullify", c);
			}
			else
			{
				c.DamageHP(base.TrapLv * 2 + 10, AttackSource.Trap);
			}
			break;
		case "blind":
			ActEffect.Proc(EffectId.Blind, c);
			break;
		case "paralysis":
			ActEffect.Proc(EffectId.Paralyze, c);
			break;
		case "mine":
		{
			c.PlayEffect("explosion");
			c.PlaySound("explosion");
			int num = (EClass.debug.enable ? 100000 : 50) + base.TrapLv * 2 + EClass.rnd(150);
			if (IsLaidByDog)
			{
				num = num * 3 / 2;
			}
			c.DamageHP(num, AttackSource.Trap);
			Shaker.ShakeCam();
			break;
		}
		case "banana":
			c.Stumble(150);
			break;
		}
		if (DestroyChanceOnActivateTrap >= EClass.rnd(100))
		{
			owner.Destroy();
		}
	}

	public override void SetName(ref string s)
	{
		if (IsJammed)
		{
			s = "_jammed".lang(s);
		}
	}
}
