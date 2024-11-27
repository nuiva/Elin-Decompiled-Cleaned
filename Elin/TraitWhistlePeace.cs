using System;

public class TraitWhistlePeace : TraitItem
{
	public override bool IsTool
	{
		get
		{
			return true;
		}
	}

	public override bool ShowAsTool
	{
		get
		{
			return true;
		}
	}

	public override bool OnUse(Chara c)
	{
		EClass._zone.isPeace = !EClass._zone.isPeace;
		EClass.pc.Say("whistle", EClass.pc, this.owner, null, null);
		EClass.pc.Say("whistle_" + (EClass._zone.isPeace ? "peace" : "peace_end"), null, null);
		EClass.pc.PlaySound("whistle" + (EClass._zone.isPeace ? "" : "_end"), 1f, true);
		return false;
	}
}
