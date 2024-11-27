using System;

public class TraitToolRange : TraitTool
{
	public override bool CanAutofire
	{
		get
		{
			return true;
		}
	}

	public virtual bool NeedAmmo
	{
		get
		{
			return true;
		}
	}

	public int MaxAmmo
	{
		get
		{
			return (base.GetParam(1, null) ?? "12").ToInt() * (100 + this.owner.Evalue(600) * 5) / 100;
		}
	}

	public int ReloadTurn
	{
		get
		{
			return (base.GetParam(2, null) ?? "4").ToInt() * 100 / (100 + this.owner.Evalue(601) * 10);
		}
	}

	public virtual Element WeaponSkill
	{
		get
		{
			return null;
		}
	}

	public virtual bool NeedReload
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsAmmo(Thing t)
	{
		return false;
	}

	public int BestDist
	{
		get
		{
			Thing thing = this.owner.Thing;
			if (thing == null)
			{
				return 3;
			}
			return thing.source.range;
		}
	}

	public override void OnCreate(int lv)
	{
		this.owner.c_ammo = this.MaxAmmo;
	}

	public override void SetMainText(UIText t, bool hotitem)
	{
		if (this.NeedAmmo)
		{
			string text = this.owner.c_ammo.ToString() + "/" + this.MaxAmmo.ToString();
			t.SetText(text ?? "", FontColor.Charge);
			t.SetActive(true);
			return;
		}
		base.SetMainText(t, hotitem);
	}
}
