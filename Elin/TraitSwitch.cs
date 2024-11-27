using System;

public class TraitSwitch : Trait
{
	public int TrapLv
	{
		get
		{
			return EClass._zone.DangerLv;
		}
	}

	public virtual bool CanDisarmTrap
	{
		get
		{
			return false;
		}
	}

	public virtual bool CanManucalActivate
	{
		get
		{
			return false;
		}
	}

	public virtual bool StartHidden
	{
		get
		{
			return false;
		}
	}

	public virtual bool IgnoreWhenLevitating()
	{
		return false;
	}

	public virtual bool IsNegativeEffect
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsJammed
	{
		get
		{
			return this.owner.GetInt(60, null) >= 3;
		}
	}

	public virtual bool IsLaidByDog
	{
		get
		{
			return this.owner.c_idRefCard == "dog_mine";
		}
	}

	public override void OnInstall(bool byPlayer)
	{
		if (byPlayer && this.StartHidden)
		{
			this.owner.SetHidden(true);
		}
	}

	public override void TrySetAct(ActPlan p)
	{
		if (!EClass._zone.IsPCFaction && this.CanDisarmTrap)
		{
			p.TrySetAct("actDisarm", delegate()
			{
				if (!this.TryDisarmTrap(EClass.pc) && EClass.pc.Evalue(1656) < 3 && EClass.rnd(2) == 0)
				{
					this.ActivateTrap(EClass.pc);
				}
				return true;
			}, this.owner, null, 1, false, true, false);
			return;
		}
		if (this.CanManucalActivate)
		{
			p.TrySetAct("actUse", delegate()
			{
				this.ActivateTrap(EClass.pc);
				return true;
			}, this.owner, null, 1, false, true, false);
		}
	}

	public bool TryDisarmTrap(Chara c)
	{
		if (EClass.rnd(c.Evalue(293) * 15 + 20 + c.DEX) > EClass.rnd(this.TrapLv * 12 + 100))
		{
			c.Say("disarm_success", c, this.owner, null, null);
			this.owner.PlaySound("disarm", 1f, true);
			c.ModExp(293, 50 + this.TrapLv);
			int num = EClass.debug.enable ? 10 : EClass.pc.Evalue(1656);
			if (!c.IsPCParty)
			{
				num = 0;
			}
			if (!this.IsLaidByDog && num > 0 && num + 2 > EClass.rnd(10))
			{
				string id = "scrap";
				int idMat = -1;
				int lv = num * 5 + EClass.pc.Evalue(293) / 4 + EClass.pc.Evalue(210) / 4;
				if (EClass.rnd(3) == 0)
				{
					id = "junk";
				}
				if (EClass.rnd(4) == 0)
				{
					id = "microchip";
				}
				if (EClass.rnd(20) == 0)
				{
					id = "medal";
				}
				if (EClass.rnd(30) == 0)
				{
					id = "ic";
				}
				if (EClass.rnd(4) == 0)
				{
					id = "ingot";
					idMat = MATERIAL.GetRandomMaterial(lv, "metal", false).id;
				}
				if (EClass.rnd(4) == 0)
				{
					id = "texture";
					idMat = MATERIAL.GetRandomMaterial(lv, "leather", false).id;
				}
				Thing thing = ThingGen.Create(id, idMat, lv);
				thing.isHidden = false;
				EClass.pc.Say("scavenge", this.owner, thing, null, null);
				EClass._map.TrySmoothPick(this.owner.pos, thing, EClass.pc);
			}
			this.owner.Destroy();
			return true;
		}
		c.Say("disarm_fail", c, this.owner, null, null);
		c.PlaySound("disarm_fail", 1f, true);
		c.ModExp(293, 20 + this.TrapLv / 3);
		if (c.IsPCFaction)
		{
			int @int = this.owner.GetInt(60, null);
			this.owner.SetInt(60, @int + 1);
		}
		if (this.IsJammed)
		{
			c.Say("trapJammed", this.owner, null, null);
			c.PlaySound("electricity_insufficient", 1f, true);
		}
		return false;
	}

	public void ActivateTrap(Chara c)
	{
		if (this.IsJammed)
		{
			c.Say("trapJammed2", this.owner, null, null);
			return;
		}
		TraitSwitch.haltMove = true;
		this.OnActivateTrap(c);
		if (c.IsPC && TraitSwitch.haltMove)
		{
			EClass.player.haltMove = true;
		}
	}

	public virtual void OnActivateTrap(Chara c)
	{
	}

	public static bool haltMove;
}
