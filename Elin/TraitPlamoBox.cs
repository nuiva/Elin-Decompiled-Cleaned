public class TraitPlamoBox : TraitItem
{
	public override string LangUse => "actOpen";

	public override bool OnUse(Chara c)
	{
		if (EClass._zone.IsRegion)
		{
			Msg.SayCannotUseHere();
			return false;
		}
		if (owner.isNPCProperty)
		{
			Msg.Say("notGood");
			return false;
		}
		EClass.pc.Say("openDoor", EClass.pc, owner);
		Thing thing = ThingGen.Create("plamo");
		thing.DyeRandom();
		EClass.pc.Pick(thing);
		owner.ModNum(-1);
		return base.OnUse(c);
	}
}
