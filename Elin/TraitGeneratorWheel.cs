using System;

public class TraitGeneratorWheel : TraitGenerator
{
	public override bool HaveUpdate
	{
		get
		{
			return true;
		}
	}

	public override bool Waterproof
	{
		get
		{
			return true;
		}
	}

	public override ToggleType ToggleType
	{
		get
		{
			return ToggleType.Custom;
		}
	}

	public override bool IsOn
	{
		get
		{
			return this.owner.isOn || !EClass._zone.IsPCFaction;
		}
	}

	public override bool CanUse(Chara c)
	{
		return this.owner.IsInstalled && EClass._zone.IsPCFaction && !this.owner.pos.HasChara;
	}

	public override bool OnUse(Chara c)
	{
		LayerPeople.CreateSelect("", "", delegate(UIList l)
		{
			foreach (Chara chara in EClass.Branch.members)
			{
				if (chara.IsAliveInCurrentZone && !chara.IsPCParty && chara.memberType == FactionMemberType.Default)
				{
					l.Add(chara);
				}
			}
		}, delegate(Chara c)
		{
			c.Teleport(this.owner.pos, false, true);
			c.RemoveCondition<ConSleep>();
			c.noMove = true;
			c.orgPos = new Point(this.owner.pos);
			c.PlaySound("ride", 1f, true);
		}, null);
		return true;
	}

	public override void OnStepped(Chara c)
	{
		if (this.IsOn)
		{
			return;
		}
		if (this.ShouldWork())
		{
			this.owner.isOn = true;
			EClass._zone.dirtyElectricity = true;
			this.owner.Say("generator_start", this.owner, null, null);
			this.owner.PlaySound("electricity_on", 1f, true);
			this.Refresh(c);
		}
	}

	public override void OnSteppedOut(Chara c)
	{
		this.Refresh(c);
	}

	public override void Update()
	{
		if (!this.owner.isOn || !EClass._zone.IsPCFaction)
		{
			return;
		}
		if (!this.ShouldWork())
		{
			this.owner.isOn = false;
			EClass._zone.dirtyElectricity = true;
			this.owner.Say("generator_stop", this.owner, null, null);
			this.owner.PlaySound("electricity_off", 1f, true);
		}
	}

	public void Refresh(Chara c)
	{
		if (c.IsPCFaction && c.homeBranch == EClass._zone.branch)
		{
			c.RefreshWorkElements(EClass._zone.elements);
		}
	}

	public bool ShouldWork()
	{
		if (!this.owner.pos.HasChara)
		{
			return false;
		}
		int num = 0;
		foreach (Thing thing in this.owner.pos.Things)
		{
			if (thing.IsInstalled && thing.trait is TraitGeneratorWheel)
			{
				num++;
			}
		}
		if (num != 1)
		{
			return false;
		}
		if (!EClass._zone.IsPCFaction)
		{
			return true;
		}
		foreach (Chara chara in this.owner.pos.Charas)
		{
			if (chara.IsPCFaction && chara.memberType == FactionMemberType.Default)
			{
				return true;
			}
		}
		return false;
	}
}
