using System;

public class TraitGene : Trait
{
	public override bool CanStack
	{
		get
		{
			return false;
		}
	}

	public override bool CanBeStolen
	{
		get
		{
			return false;
		}
	}

	public override bool CanBeDestroyed
	{
		get
		{
			return false;
		}
	}

	public override float DropChance
	{
		get
		{
			return 1f;
		}
	}

	public override string GetName()
	{
		if (this.owner.c_DNA == null || this.owner.c_DNA.type == DNA.Type.Default || this.owner.c_DNA.type == DNA.Type.Brain)
		{
			return base.GetName();
		}
		return ("dna_" + this.owner.c_DNA.type.ToString()).lang() + Lang.space + this.owner.sourceCard.GetText("name", false);
	}

	public override void WriteNote(UINote n, bool identified)
	{
		if (this.owner.c_DNA != null)
		{
			if (this.owner.c_DNA.cost > 0)
			{
				n.AddText("isCostFeatPoint".lang(this.owner.c_DNA.cost.ToString() ?? "", null, null, null, null), FontColor.DontChange);
			}
			if (EClass.debug.showExtra)
			{
				n.AddText("duration:" + this.owner.c_DNA.GetDurationHour().ToString(), FontColor.DontChange);
			}
			this.owner.c_DNA.WriteNote(n);
		}
	}

	public override int GetValue()
	{
		return base.GetValue() * ((this.owner.c_DNA == null) ? 100 : (100 + this.owner.c_DNA.cost * 10)) / 100;
	}
}
