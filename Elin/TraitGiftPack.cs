public class TraitGiftPack : TraitItem
{
	public override string LangUse => "actOpen";

	public override bool OnUse(Chara c)
	{
		if (EClass._zone.IsRegion)
		{
			Msg.SayCannotUseHere();
			return false;
		}
		EClass.pc.Say("openDoor", EClass.pc, owner);
		SE.Play("dropReward");
		EClass.pc.Pick(ThingGen.Create("sketch_special"));
		EClass.pc.Pick(ThingGen.Create("letter_will"));
		EClass.pc.Pick(ThingGen.Create("crimAle"));
		owner.ModNum(-1);
		return base.OnUse(c);
	}
}
