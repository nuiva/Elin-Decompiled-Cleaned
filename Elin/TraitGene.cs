public class TraitGene : Trait
{
	public override bool CanStack => false;

	public override bool CanBeStolen => false;

	public override bool CanBeDestroyed => false;

	public override float DropChance => 1f;

	public override string GetName()
	{
		if (owner.c_DNA == null || owner.c_DNA.type == DNA.Type.Default || owner.c_DNA.type == DNA.Type.Brain)
		{
			return base.GetName();
		}
		return ("dna_" + owner.c_DNA.type).lang() + Lang.space + owner.sourceCard.GetText();
	}

	public override void WriteNote(UINote n, bool identified)
	{
		if (owner.c_DNA != null)
		{
			if (owner.c_DNA.cost > 0)
			{
				n.AddText("isCostFeatPoint".lang(owner.c_DNA.cost.ToString() ?? ""));
			}
			if (EClass.debug.showExtra)
			{
				n.AddText("duration:" + owner.c_DNA.GetDurationHour());
			}
			owner.c_DNA.WriteNote(n);
		}
	}

	public override int GetValue()
	{
		return base.GetValue() * ((owner.c_DNA == null) ? 100 : (100 + owner.c_DNA.cost * 10)) / 100;
	}
}
