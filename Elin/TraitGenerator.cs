using System;
using UnityEngine;

public class TraitGenerator : Trait
{
	public virtual bool Waterproof
	{
		get
		{
			return false;
		}
	}

	public override bool IsOn
	{
		get
		{
			return !this.owner.isBroken && !EClass._map.isBreakerDown;
		}
	}

	public override int Electricity
	{
		get
		{
			if (this.IsOn)
			{
				return base.Electricity;
			}
			return 0;
		}
	}

	public override bool UseAltTiles
	{
		get
		{
			return this.Electricity > 0;
		}
	}

	public override void OnSimulateHour(VirtualDate date)
	{
		if (!date.IsRealTime || !this.owner.IsInstalled)
		{
			return;
		}
		if (!this.Waterproof && (this.owner.Cell.IsTopWater || (!this.owner.Cell.HasRoof && !EClass._map.IsIndoor && EClass.world.weather.IsHazard)))
		{
			this.ModHP(-10);
		}
		else
		{
			this.ModHP(10);
		}
		if (!this.owner.isBroken && EClass._zone.electricity < 0 && 100 >= EClass.rnd(150))
		{
			this.ShortOut();
		}
	}

	public void ModHP(int a)
	{
		this.owner.hp = Mathf.Clamp(this.owner.hp + a, 1, 100);
		if (!this.owner.isBroken && this.owner.hp < 50)
		{
			this.ShortOut();
			return;
		}
		if (this.owner.isBroken && this.owner.hp >= 50)
		{
			this.Recover();
		}
	}

	public void Recover()
	{
		this.owner.hp = 80;
		this.owner.isBroken = false;
		this.owner.PlaySound("electricity_on", 1f, true);
		this.owner.Say("electricity_recover", this.owner, null, null);
		EClass._zone.RefreshElectricity();
	}

	public void ShortOut()
	{
		this.owner.hp -= 20 + EClass.rnd(30);
		this.owner.isBroken = true;
		this.owner.PlaySound("electricity_off", 1f, true);
		this.owner.Say("electricity_short", this.owner, null, null);
		EClass._zone.RefreshElectricity();
	}

	public override void SetName(ref string s)
	{
		if (this.owner.isBroken)
		{
			s = "gen_broken".lang(s, null, null, null, null);
		}
	}
}
