using UnityEngine;

public class TraitGenerator : Trait
{
	public virtual bool Waterproof => false;

	public override bool IsOn
	{
		get
		{
			if (!owner.isBroken)
			{
				return !EClass._map.isBreakerDown;
			}
			return false;
		}
	}

	public override int Electricity
	{
		get
		{
			if (IsOn)
			{
				return base.Electricity;
			}
			return 0;
		}
	}

	public override bool UseAltTiles => Electricity > 0;

	public override void OnSimulateHour(VirtualDate date)
	{
		if (date.IsRealTime && owner.IsInstalled)
		{
			if (!Waterproof && (owner.Cell.IsTopWater || (!owner.Cell.HasRoof && !EClass._map.IsIndoor && EClass.world.weather.IsHazard)))
			{
				ModHP(-10);
			}
			else
			{
				ModHP(10);
			}
			if (!owner.isBroken && EClass._zone.electricity < 0 && 100 >= EClass.rnd(150))
			{
				ShortOut();
			}
		}
	}

	public void ModHP(int a)
	{
		owner.hp = Mathf.Clamp(owner.hp + a, 1, 100);
		if (!owner.isBroken && owner.hp < 50)
		{
			ShortOut();
		}
		else if (owner.isBroken && owner.hp >= 50)
		{
			Recover();
		}
	}

	public void Recover()
	{
		owner.hp = 80;
		owner.isBroken = false;
		owner.PlaySound("electricity_on");
		owner.Say("electricity_recover", owner);
		EClass._zone.RefreshElectricity();
	}

	public void ShortOut()
	{
		owner.hp -= 20 + EClass.rnd(30);
		owner.isBroken = true;
		owner.PlaySound("electricity_off");
		owner.Say("electricity_short", owner);
		EClass._zone.RefreshElectricity();
	}

	public override void SetName(ref string s)
	{
		if (owner.isBroken)
		{
			s = "gen_broken".lang(s);
		}
	}
}
