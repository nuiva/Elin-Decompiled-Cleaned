using System;

public class TraitGiftPack : TraitItem
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
		EClass.pc.Say("openDoor", EClass.pc, this.owner, null, null);
		SE.Play("dropReward");
		EClass.pc.Pick(ThingGen.Create("sketch_special", -1, -1), true, true);
		EClass.pc.Pick(ThingGen.Create("letter_will", -1, -1), true, true);
		EClass.pc.Pick(ThingGen.Create("crimAle", -1, -1), true, true);
		this.owner.ModNum(-1, true);
		return base.OnUse(c);
	}
}
