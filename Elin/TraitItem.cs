using System;

public class TraitItem : Trait
{
	public virtual bool CanUseFromInventory
	{
		get
		{
			return true;
		}
	}

	public virtual bool CanUseInUserZone
	{
		get
		{
			return !this.owner.isNPCProperty;
		}
	}

	public override bool CanUse(Chara c)
	{
		return (this.CanUseFromInventory || this.owner.IsInstalled) && (this.CanUseInUserZone || !EClass._zone.IsUserZone || !this.owner.isNPCProperty) && (this.Electricity >= 0 || this.owner.isOn);
	}

	public override void WriteNote(UINote n, bool identified)
	{
		if (!this.langNote.IsEmpty())
		{
			n.Space(20, 1);
			n.AddText(this.langNote.lang(), FontColor.Good);
		}
	}
}
