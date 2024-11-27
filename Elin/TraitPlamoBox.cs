using System;

public class TraitPlamoBox : TraitItem
{
	public override string LangUse
	{
		get
		{
			return "actOpen";
		}
	}

	public override bool OnUse(Chara c)
	{
		if (EClass._zone.IsRegion)
		{
			Msg.SayCannotUseHere();
			return false;
		}
		if (this.owner.isNPCProperty)
		{
			Msg.Say("notGood");
			return false;
		}
		EClass.pc.Say("openDoor", EClass.pc, this.owner, null, null);
		Thing thing = ThingGen.Create("plamo", -1, -1);
		thing.DyeRandom();
		EClass.pc.Pick(thing, true, true);
		this.owner.ModNum(-1, true);
		return base.OnUse(c);
	}
}
