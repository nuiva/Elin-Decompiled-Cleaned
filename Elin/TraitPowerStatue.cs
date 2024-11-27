using System;
using UnityEngine;

public class TraitPowerStatue : TraitItem
{
	public override bool CanUseFromInventory
	{
		get
		{
			return false;
		}
	}

	public override bool UseExtra
	{
		get
		{
			return this.owner.isOn;
		}
	}

	public override void OnCreate(int lv)
	{
		this.owner.isOn = true;
		this.owner.ChangeMaterial(12);
		this.owner.c_seed = EClass.rnd(20000);
		this.owner.SetLv(lv);
	}

	public override bool CanUse(Chara c)
	{
		return base.CanUse(c) && this.owner.isOn && !EClass._zone.IsUserZone;
	}

	public override bool OnUse(Chara c)
	{
		bool flag = this is TraitGodStatue;
		if (!this.IsImplemented())
		{
			Msg.SayNothingHappen();
			return true;
		}
		Msg.Say("shrine_power", this.owner, null, null, null);
		if (flag)
		{
			SE.Play("godbless");
			this.owner.PlayEffect("aura_heaven", true, 0f, default(Vector3));
		}
		else
		{
			SE.Play("shrine");
			this.owner.PlayEffect("buff", true, 0f, default(Vector3));
		}
		this._OnUse(c);
		this.owner.isOn = false;
		if (flag)
		{
			this.owner.ChangeMaterial("onyx");
			this.owner.rarity = Rarity.Normal;
		}
		this.owner.renderer.RefreshExtra();
		return true;
	}

	public virtual void _OnUse(Chara c)
	{
	}

	public virtual bool IsImplemented()
	{
		return true;
	}

	public override bool CanStack
	{
		get
		{
			return false;
		}
	}
}
